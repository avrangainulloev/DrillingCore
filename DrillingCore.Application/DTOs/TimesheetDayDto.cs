using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.DTOs
{
    public class TimesheetDayDto
    {
        public DateOnly Date { get; set; }
        public bool Worked { get; set; }
        public decimal DayRate { get; set; }
        public decimal MeterRate { get; set; }
        public double Meters { get; set; }
        public string Narrative { get; set; } = "";
    }
}
