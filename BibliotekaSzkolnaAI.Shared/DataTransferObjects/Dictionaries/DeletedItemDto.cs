using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotekaSzkolnaAI.Shared.DataTransferObjects.Bin
{
    public class DeletedItemDto
    {
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;

        public string DisplayName { get; set; } = string.Empty;

        public string Details { get; set; } = string.Empty;

        public DateTime? DeletedAt { get; set; }
    }

    public class RestoreItemDto
    {
        public string Id { get; set; }
        public string Type { get; set; }
    }
}
