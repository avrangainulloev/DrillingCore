using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.DTOs
{
    public class FLHAFormListDto
    {
        public int Id { get; set; }
        public DateTime DateFilled { get; set; }
        public string TaskDescription { get; set; } = string.Empty;
        public string? OtherComments { get; set; }
        public string Status { get; set; } = "Pending";
        public string CreatorName { get; set; } = string.Empty;
    }
}
