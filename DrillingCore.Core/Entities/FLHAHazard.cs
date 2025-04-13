using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Core.Entities
{
    public class FLHAHazard
    {
        public int Id { get; set; }
        public string Label { get; set; }

        public string? ControlSuggestion { get; set; }

        public int GroupId { get; set; }
        public FLHAHazardGroup Group { get; set; }
    }
}
