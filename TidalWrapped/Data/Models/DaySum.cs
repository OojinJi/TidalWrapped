using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TidalWrapped.Data.Models
{
    [Table("DaySum")]
    public class DaySum
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }

        [Column("Date")]
        public DateTime Date { get; set; }

        [Column("MostActive")]
        public string MostActive { get; set; }

        [Column("TotalListens")]
        public int TotalListens { get; set; }

        public ICollection<Track> Tracks { get; set; }

    }
}



