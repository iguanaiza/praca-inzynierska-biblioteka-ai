using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotekaSzkolnaAI.Shared.DataTransferObjects.Loans
{
    public class LoanCreateDto
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int BookCopyId { get; set; }

        [Required]
        public DateTime DueDate { get; set; }
    }
}
