using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TidalWrapped.Data.Models
{
    [Table("listeningData")]
    public class Track
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("Song")]
        public string song { get; set; }
        [Column("Artist")]
        public string artist { get; set; }
        [Column("Album")]
        public string album { get; set; }
        [Column("songDuration")]
        public long songDuration { get; set; }
        [Column("albumArt")]
        public string albumArt { get; set; }
        [Column("whenPlayed")]
        public DateTime whenPlayed { get; set; }
    }
}
