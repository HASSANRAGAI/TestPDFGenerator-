using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PdfTemplateSystem.Models;

namespace PdfTemplateSystem.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<PdfTemplate> PdfTemplates { get; set; }
    public DbSet<ContextProfile> ContextProfiles { get; set; }
    public DbSet<SampleInvoice> SampleInvoices { get; set; }
    public DbSet<SampleInvoiceItem> SampleInvoiceItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure ContextProfile to store lists as JSON
        modelBuilder.Entity<ContextProfile>(entity =>
        {
            entity.Property(e => e.IncludePaths)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
                .Metadata.SetValueComparer(new ValueComparer<List<string>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()));

            entity.Property(e => e.AllowedFields)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
                .Metadata.SetValueComparer(new ValueComparer<List<string>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()));

            entity.Property(e => e.Labels)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, string>())
                .Metadata.SetValueComparer(new ValueComparer<Dictionary<string, string>>(
                    (c1, c2) => c1!.Count == c2!.Count && !c1.Except(c2).Any(),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.Key.GetHashCode(), v.Value.GetHashCode())),
                    c => c.ToDictionary(entry => entry.Key, entry => entry.Value)));

            entity.Property(e => e.CustomJoins)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<List<CustomJoin>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<CustomJoin>())
                .Metadata.SetValueComparer(new ValueComparer<List<CustomJoin>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()));
        });

        // Configure relationships
        modelBuilder.Entity<SampleInvoiceItem>()
            .HasOne(i => i.SampleInvoice)
            .WithMany(s => s.Items)
            .HasForeignKey(i => i.SampleInvoiceId);
    }
}
