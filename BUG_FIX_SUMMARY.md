# Bug Fix Summary: Stack Overflow and EF Core Warnings

## Issue Description

The application was experiencing two critical issues:

1. **Stack Overflow Error** - The application would crash with a stack overflow when attempting to preview PDFs
2. **Entity Framework Core Warnings** - Multiple warnings about collections lacking value comparers

## Root Causes

### 1. Stack Overflow in TemplateEngineService.ShapeData

The `ShapeData` method in `TemplateEngineService` was causing infinite recursion due to circular references:

```
SampleInvoice → Items (collection) → SampleInvoiceItem → SampleInvoice (navigation property) → Items → ...
```

The method would recursively process:
1. `SampleInvoice` object with its `Items` collection
2. Each `SampleInvoiceItem` in the collection
3. The `SampleInvoice` navigation property on each item
4. Back to the `Items` collection (infinite loop)

### 2. Missing Value Comparers

Entity Framework Core requires value comparers for collection properties that have value converters. The following properties were missing comparers:

- `ContextProfile.AllowedFields` (List<string>)
- `ContextProfile.IncludePaths` (List<string>)
- `ContextProfile.CustomJoins` (List<CustomJoin>)
- `ContextProfile.Labels` (Dictionary<string, string>)

## Solutions Implemented

### Fix 1: Object Tracking in ShapeData

**File**: `src/PdfTemplateSystem.Api/Services/TemplateEngineService.cs`

Added a `HashSet<object>` to track visited objects and prevent circular references:

```csharp
private object ShapeData(object entity, ContextProfile profile, string pathPrefix)
{
    return ShapeData(entity, profile, pathPrefix, new HashSet<object>());
}

private object ShapeData(object entity, ContextProfile profile, string pathPrefix, HashSet<object> visited)
{
    if (entity == null) return new { };
    
    // Prevent circular references
    if (visited.Contains(entity))
    {
        return new { };
    }
    
    visited.Add(entity);
    // ... rest of the method
}
```

**Benefits**:
- Prevents infinite recursion
- Safely handles circular references
- Maintains proper object graph traversal
- No performance impact on normal operation

### Fix 2: Added Value Comparers

**File**: `src/PdfTemplateSystem.Api/Data/ApplicationDbContext.cs`

Added value comparers for all collection properties:

```csharp
entity.Property(e => e.IncludePaths)
    .HasConversion(...)
    .Metadata.SetValueComparer(new ValueComparer<List<string>>(
        (c1, c2) => c1!.SequenceEqual(c2!),
        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
        c => c.ToList()));
```

**File**: `src/PdfTemplateSystem.Api/Models/CustomJoin.cs`

Implemented proper equality comparison for `CustomJoin` and `JoinCondition`:

```csharp
public override bool Equals(object? obj)
{
    if (obj is not CustomJoin other) return false;
    return Alias == other.Alias &&
           TargetEntity == other.TargetEntity &&
           JoinType == other.JoinType &&
           Condition.Equals(other.Condition) &&
           NestedJoins.SequenceEqual(other.NestedJoins);
}

public override int GetHashCode()
{
    var hash = HashCode.Combine(Alias, TargetEntity, JoinType, Condition);
    foreach (var nested in NestedJoins)
    {
        hash = HashCode.Combine(hash, nested);
    }
    return hash;
}
```

## Testing Results

### Before Fix
```
warn: Microsoft.EntityFrameworkCore.Model.Validation[10620]
      The property 'ContextProfile.AllowedFields' is a collection or enumeration type with a value converter but with no value comparer...
warn: Microsoft.EntityFrameworkCore.Model.Validation[10620]
      The property 'ContextProfile.CustomJoins' is a collection or enumeration type with a value converter but with no value comparer...
...
Stack overflow.
   at System.String.Concat(System.String, System.String, System.String)
   at PdfTemplateSystem.Services.TemplateEngineService.ShapeData(...)
```

### After Fix
```
info: Microsoft.EntityFrameworkCore.Update[30100]
      Saved 5 entities to in-memory store.
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

### Verification Tests

✅ **Build**: Compiled successfully with 0 warnings, 0 errors
✅ **Startup**: No EF Core warnings about value comparers
✅ **Templates API**: `/api/Templates` returns 1 template
✅ **PDF Preview Invoice 1**: `/api/Pdf/preview/invoice/11111111-1111-1111-1111-111111111111` - Returns HTML successfully
✅ **PDF Preview Invoice 2**: `/api/Pdf/preview/invoice/22222222-2222-2222-2222-222222222222` - Returns HTML successfully
✅ **No Stack Overflow**: Confirmed via log analysis
✅ **No EF Warnings**: Confirmed via log analysis

## Impact

### What Was Fixed
- ✅ Stack overflow eliminated - application no longer crashes
- ✅ All EF Core warnings removed
- ✅ PDF preview functionality fully operational
- ✅ Template rendering works correctly
- ✅ Proper change tracking for collection properties

### What Remains Unchanged
- ✅ API endpoints remain the same
- ✅ Database schema unchanged
- ✅ Template syntax unchanged
- ✅ Frontend integration unaffected
- ✅ All existing functionality preserved

## Files Modified

1. `src/PdfTemplateSystem.Api/Services/TemplateEngineService.cs`
   - Added overloaded `ShapeData` method with visited object tracking
   - Prevents circular references in object graph traversal

2. `src/PdfTemplateSystem.Api/Data/ApplicationDbContext.cs`
   - Added value comparers for `IncludePaths`, `AllowedFields`, `Labels`, and `CustomJoins`
   - Imported `Microsoft.EntityFrameworkCore.ChangeTracking` namespace

3. `src/PdfTemplateSystem.Api/Models/CustomJoin.cs`
   - Implemented `Equals()` and `GetHashCode()` for `CustomJoin` class
   - Implemented `Equals()` and `GetHashCode()` for `JoinCondition` class

## Conclusion

Both critical issues have been resolved:
1. The stack overflow is fixed through proper circular reference prevention
2. All EF Core warnings are eliminated through proper value comparers

The application now runs without errors or warnings, and all functionality works as expected.
