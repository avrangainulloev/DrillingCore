using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.DTOs
{
    public class ChecklistItemDto
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public string? GroupName { get; set; }
    }
}
