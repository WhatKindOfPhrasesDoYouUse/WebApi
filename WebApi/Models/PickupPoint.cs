using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models
{
    [Table("pickuppoint")]
    public class PickupPoint
    {
        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Column("city")]
        public string City { get; set; }

        [Column("address")]
        public string Address { get; set; }
    }
}
