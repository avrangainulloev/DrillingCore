using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.DTOs
{
    public class ActiveProjectDto
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = default!;
    }
}
