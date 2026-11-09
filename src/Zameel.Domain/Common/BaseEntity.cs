namespace Zameel.Domain.Common;

public abstract class BaseEntity
{
    public Guid id { get; set; } = Guid.NewGuid();
    public DateTime createdAt { get; set; } = DateTime.UtcNow;
    public DateTime? updatedAt { get; set; }
    public bool isDeleted { get; set; } = false;
}
