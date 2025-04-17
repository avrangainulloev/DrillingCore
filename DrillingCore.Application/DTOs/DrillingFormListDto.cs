using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.DTOs
{
    public class DrillingFormListDto
    {
        public int Id { get; set; }
        public string CreatorName { get; set; }
        public string DateFilled { get; set; } = string.Empty;
        public int TotalWells { get; set; }
        public double TotalMeters { get; set; }
        public string? OtherComments { get; set; }
        public string Status { get; set; }
    }
}
