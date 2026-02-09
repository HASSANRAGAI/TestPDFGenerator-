namespace TestPDFGenerator.Api.Models;

public class SampleInvoice
{
    public Guid Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public List<SampleInvoiceItem> Items { get; set; } = new();
}

public class SampleInvoiceItem
{
    public Guid Id { get; set; }
    public Guid SampleInvoiceId { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public SampleInvoice? SampleInvoice { get; set; }
}
