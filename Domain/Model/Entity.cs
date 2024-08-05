using Domain.Interface;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Model
{
    public abstract class Entity<TKey> : IHaveIdentifier<TKey>, ISoftDelete
    {
        [Column(Order = 1)]
        public TKey Id { get; set; }
        [Column(Order = 4)]
        public bool IsDeleted { get; set; }
    }

    public abstract class Entity : ISoftDelete
    {
        [Column(Order = 4)]
        public bool IsDeleted { get; set; }
    }
}
