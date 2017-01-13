namespace NBasis.Models
{
    public interface IEntity<TKey>
    {
        TKey Id { get; }
    }
}
