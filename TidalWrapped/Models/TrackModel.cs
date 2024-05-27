using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TidalWrapped.Models
{
    public class TrackModel
    {
        public string song {  get; set; }
        public string artist { get; set; }
        public string album { get; set; }
        public DateTime whenPlayed { get; set; }
        public DateTime releaseDate { get; set; }
        public string trackPage {  get; set; }
    }
}
