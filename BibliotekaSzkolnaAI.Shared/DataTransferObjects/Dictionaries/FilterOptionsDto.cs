using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotekaSzkolnaAI.Shared.DataTransferObjects.Dictionaries
{
    public class FilterOptionsDto
    {
        public List<string> Authors { get; set; } = new();
        public List<string> Categories { get; set; } = new();
        public List<string> Genres { get; set; } = new();
        public List<string> Publishers { get; set; } = new();
        public List<string> Series { get; set; } = new();
        public List<string> Types { get; set; } = new();
        public List<string> Years { get; set; } = new();
    }
}
