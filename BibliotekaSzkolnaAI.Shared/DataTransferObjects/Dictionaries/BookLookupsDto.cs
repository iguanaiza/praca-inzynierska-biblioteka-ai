using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotekaSzkolnaAI.Shared.DataTransferObjects.Dictionaries
{
    public class LookupDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
    public class BookLookupsDto
    {
        public List<LookupDto> Authors { get; set; } = new();
        public List<LookupDto> Publishers { get; set; } = new();
        public List<LookupDto> Series { get; set; } = new();
        public List<LookupDto> Types { get; set; } = new();
        public List<LookupDto> Categories { get; set; } = new();
        public List<LookupDto> Genres { get; set; } = new();
        public List<LookupDto> SpecialTags { get; set; } = new();
    }
}
