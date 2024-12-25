using Azure.Data.Tables;

namespace Azure.Storage.Samples.API.Models.Table;
internal record Product : ITableEntity
{
    public string? Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public bool IsActive { get; set; }
    public string PartitionKey { get; set; } = "phones";
    public string RowKey { get; set; } = Guid.NewGuid().ToString();
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }


    public Product() { }

    public Product(string name, decimal price, int quantity, bool isActive) : this()
    {
        Name = name;
        Price = price;
        Quantity = quantity;
        IsActive = isActive;
    }
}
