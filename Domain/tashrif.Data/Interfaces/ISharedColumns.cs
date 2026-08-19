namespace tashrif.Data.Interfaces;

public interface ISharedColumns
{
    bool IsDeleted { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
    DateTime? DeletedTime { get; set; }
}
