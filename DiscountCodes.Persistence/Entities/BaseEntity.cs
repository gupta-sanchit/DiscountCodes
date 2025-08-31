namespace DiscountCodes.Persistence.Entities;

public abstract class BaseEntity
{
    public long Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

}
