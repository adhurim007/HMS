using HrmsH.Domain.Common;

namespace HrmsH.Domain.Pharmacy;

public class Product : BaseEntity
{
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? GenericName { get; set; }
    public string? Strength { get; set; }
    public string? Unit { get; set; }
    public decimal? DefaultSalePrice { get; set; }

    public bool IsActive { get; set; } = true;
    public ICollection<StockBatch> Batches { get; set; } = new List<StockBatch>();
}

