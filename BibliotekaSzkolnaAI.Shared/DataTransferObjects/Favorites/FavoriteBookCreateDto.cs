using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotekaSzkolnaAI.Shared.DataTransferObjects.Favorites
{
    public class FavoriteBookCreateDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int BookId { get; set; }
    }
}
