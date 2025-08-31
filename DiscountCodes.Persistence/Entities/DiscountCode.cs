namespace DiscountCodes.Persistence.Entities;

public class DiscountCode : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public bool IsUsed { get; set; }                   // one-time usage flag
    public DateTimeOffset? UsedAt { get; set; }
}  
