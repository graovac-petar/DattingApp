using System.ComponentModel.DataAnnotations;

namespace AppDating.API.Model.Domain
{
    public class Group
    {
        [Key]
        public required string Name { get; set; }
        public ICollection<Connection> Connections { get; set; } = [];
    }
}
