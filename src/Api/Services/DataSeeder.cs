using Microsoft.EntityFrameworkCore;
using TestPDFGenerator.Api.Data;
using TestPDFGenerator.Api.Models;

namespace TestPDFGenerator.Api.Services;

public static class DataSeeder
{
    public static async Task SeedDataAsync(ApplicationDbContext context)
    {
        // Seed Sample Invoices
        if (!await context.SampleInvoices.AnyAsync())
        {
            var invoice1 = new SampleInvoice
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Number = "INV-2024-001",
                Date = new DateTime(2024, 1, 15),
                CustomerName = "شركة التقنية المتقدمة",
                Items = new List<SampleInvoiceItem>
                {
                    new SampleInvoiceItem
                    {
                        Id = Guid.NewGuid(),
                        Description = "خدمات استشارية",
                        Quantity = 10,
                        UnitPrice = 500.00m
                    },
                    new SampleInvoiceItem
                    {
                        Id = Guid.NewGuid(),
                        Description = "دعم فني",
                        Quantity = 5,
                        UnitPrice = 300.00m
                    }
                }
            };

            var invoice2 = new SampleInvoice
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Number = "INV-2024-002",
                Date = new DateTime(2024, 2, 20),
                CustomerName = "مؤسسة البرمجيات الحديثة",
                Items = new List<SampleInvoiceItem>
                {
                    new SampleInvoiceItem
                    {
                        Id = Guid.NewGuid(),
                        Description = "تطوير برمجيات مخصصة",
                        Quantity = 20,
                        UnitPrice = 1000.00m
                    }
                }
            };

            context.SampleInvoices.AddRange(invoice1, invoice2);
            await context.SaveChangesAsync();
        }

        // Seed Context Profiles
        if (!await context.ContextProfiles.AnyAsync())
        {
            var invoiceProfile = new ContextProfile
            {
                Id = Guid.NewGuid(),
                ContextName = "invoice",
                RootEntity = "SampleInvoice",
                IncludePaths = new List<string> { "Items" },
                AllowedFields = new List<string>
                {
                    "Number", "Date", "CustomerName",
                    "Items.Description", "Items.Quantity", "Items.UnitPrice"
                },
                Labels = new Dictionary<string, string>
                {
                    ["Number"] = "رقم الفاتورة",
                    ["Date"] = "التاريخ",
                    ["CustomerName"] = "اسم العميل",
                    ["Items"] = "العناصر",
                    ["Items.Description"] = "الوصف",
                    ["Items.Quantity"] = "الكمية",
                    ["Items.UnitPrice"] = "سعر الوحدة"
                }
            };

            context.ContextProfiles.Add(invoiceProfile);
            await context.SaveChangesAsync();
        }

        // Seed PDF Templates
        if (!await context.PdfTemplates.AnyAsync())
        {
            var invoiceTemplate = new PdfTemplate
            {
                Id = Guid.NewGuid(),
                Name = "Arabic Invoice Template",
                Context = "invoice",
                DefaultSampleEntityId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                HtmlContent = @"<!DOCTYPE html>
<html dir=""rtl"" lang=""ar"">
<head>
    <meta charset=""UTF-8"">
    <style>
        body { 
            font-family: 'Arial', sans-serif; 
            direction: rtl; 
            text-align: right;
            padding: 20px;
        }
        h1 { color: #2c3e50; border-bottom: 2px solid #3498db; padding-bottom: 10px; }
        table { 
            width: 100%; 
            border-collapse: collapse; 
            margin-top: 20px; 
        }
        th, td { 
            border: 1px solid #ddd; 
            padding: 12px; 
            text-align: right; 
        }
        th { 
            background-color: #3498db; 
            color: white; 
        }
        tr:nth-child(even) { background-color: #f2f2f2; }
        .header-info { margin: 20px 0; }
        .info-row { margin: 10px 0; }
    </style>
</head>
<body>
    <h1>فاتورة رقم {{Number}}</h1>
    <div class=""header-info"">
        <div class=""info-row""><strong>العميل:</strong> {{CustomerName}}</div>
        <div class=""info-row""><strong>التاريخ:</strong> {{Date}}</div>
    </div>
    <table>
        <thead>
            <tr>
                <th>الوصف</th>
                <th>الكمية</th>
                <th>السعر</th>
                <th>المجموع</th>
            </tr>
        </thead>
        <tbody>
            {{#each Items}}
            <tr>
                <td>{{Description}}</td>
                <td>{{Quantity}}</td>
                <td>{{UnitPrice}}</td>
                <td>{{Quantity}} × {{UnitPrice}}</td>
            </tr>
            {{/each}}
        </tbody>
    </table>
</body>
</html>"
            };

            context.PdfTemplates.Add(invoiceTemplate);
            await context.SaveChangesAsync();
        }
    }
}
