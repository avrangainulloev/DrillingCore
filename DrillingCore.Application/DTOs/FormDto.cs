using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.DTOs
{
    public class FormDto
    {
        public int Id { get; set; }
        public string FormTypeName { get; set; } = default!;
        public string CrewName { get; set; } = default!;
        public string UnitNumber { get; set; } = default!;
        public DateTime DateFilled { get; set; }
        public string? OtherComments { get; set; }
    }
}
