using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.DTOs
{
    public class FormPhotoDto
    {
        public int Id { get; set; }
        public string PhotoUrl { get; set; } = default!;
        public DateTime CreatedDate { get; set; }
    }
}
