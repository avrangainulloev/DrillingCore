using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.DTOs
{
    public class FormListItemDto
    {
        public int Id { get; set; }
        public string FormType { get; set; } = null!;
        public string CreatorName { get; set; } = null!;
        public string CrewName { get; set; } = null!;
        public DateTime DateFilled { get; set; }
        public string Status { get; set; }
        public int UserId { get; set; }
    }
}
