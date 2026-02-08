using Microsoft.EntityFrameworkCore;
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
                    v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>());

            entity.Property(e => e.AllowedFields)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>());

            entity.Property(e => e.Labels)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, string>());
        });

        // Configure relationships
        modelBuilder.Entity<SampleInvoiceItem>()
            .HasOne(i => i.SampleInvoice)
            .WithMany(s => s.Items)
            .HasForeignKey(i => i.SampleInvoiceId);
    }
}
