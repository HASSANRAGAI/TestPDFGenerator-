using System.Dynamic;
using TestPDFGenerator.Api.Models;

namespace TestPDFGenerator.Api.Services;

public class ResultShaper
{
    private readonly ContextProfile _profile;

    public ResultShaper(ContextProfile profile)
    {
        _profile = profile;
    }

    public object ShapeToNestedObject(IEnumerable<dynamic> flatResults)
    {
        if (!flatResults.Any())
            return new { };

        var root = new ExpandoObject() as IDictionary<string, object?>;
        var collections = new Dictionary<string, Dictionary<object, IDictionary<string, object?>>>();

        foreach (var row in flatResults)
        {
            var rowDict = (IDictionary<string, object>)row;

            // Set root Id first
            if (!root.ContainsKey("Id") && rowDict.ContainsKey("root_Id"))
            {
                root["Id"] = rowDict["root_Id"];
            }

            foreach (var field in _profile.AllowedFields)
            {
                var key = string.Join("_", field.Split('.')).Replace("[]", "");
                if (!rowDict.ContainsKey(key)) continue;

                var value = rowDict[key];
                var parts = field.Split('.');

                if (parts.Length == 1)
                {
                    // Root field
                    root[field] = value;
                }
                else if (parts[0].EndsWith("[]"))
                {
                    // Collection field
                    var collectionName = parts[0].TrimEnd('[', ']');
                    if (!collections.ContainsKey(collectionName))
                        collections[collectionName] = new Dictionary<object, IDictionary<string, object?>>();

                    // Add to collection (handle grouping by collection item ID)
                    AddToCollection(collections[collectionName], parts, value, rowDict, collectionName);
                }
                else
                {
                    // Nested object from custom join
                    SetNestedValue(root, parts, value);
                }
            }
        }

        // Convert collections to lists
        foreach (var kvp in collections)
        {
            root[kvp.Key] = kvp.Value.Values.ToList();
        }

        return root;
    }

    private void SetNestedValue(IDictionary<string, object?> root, string[] path, object? value)
    {
        var current = root;

        for (int i = 0; i < path.Length - 1; i++)
        {
            if (!current.ContainsKey(path[i]) || current[path[i]] == null)
            {
                current[path[i]] = new ExpandoObject();
            }
            current = (IDictionary<string, object?>)current[path[i]]!;
        }

        current[path[^1]] = value;
    }

    private void AddToCollection(
        Dictionary<object, IDictionary<string, object?>> collection,
        string[] path,
        object? value,
        IDictionary<string, object> row,
        string collectionName)
    {
        // Group by collection item ID to avoid duplicates
        var itemIdKey = $"{collectionName}_Id";
        if (!row.ContainsKey(itemIdKey)) return;

        var itemId = row[itemIdKey];
        if (itemId == null) return;

        if (!collection.ContainsKey(itemId))
        {
            var newItem = new ExpandoObject() as IDictionary<string, object?>;
            newItem["Id"] = itemId;
            collection[itemId] = newItem;
        }

        var itemDict = collection[itemId];
        
        // Handle nested properties within collection items
        if (path.Length > 2)
        {
            // path[0] is collection name with []
            // path[1..^1] is the nested path
            var nestedPath = path.Skip(1).ToArray();
            SetNestedValue(itemDict, nestedPath, value);
        }
        else
        {
            // Simple property
            itemDict[path[^1]] = value;
        }
    }
}
