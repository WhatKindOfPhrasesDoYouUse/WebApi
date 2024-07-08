﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    [Table("shoe")]
    public class Shoe
    {
        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("price")]
        public int Price { get; set; }

        [Column("size")]
        public int Size { get; set; }

        [Column("color")]
        public string Color { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }
    }
}
