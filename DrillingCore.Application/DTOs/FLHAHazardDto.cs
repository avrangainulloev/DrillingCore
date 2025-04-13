using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.DTOs
{
    public class FLHAHazardDto
    {
        public int Id { get; set; }
        public string Label { get; set; } = string.Empty;
        public string ControlSuggestion { get; set; } = string.Empty;
        public int GroupId { get; set; }
    }
}
