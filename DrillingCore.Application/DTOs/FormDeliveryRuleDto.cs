using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.DTOs
{
    public class FormDeliveryRuleDto
    {
        public int Id { get; set; }
        public int FormTypeId { get; set; }
        public string FormTypeName { get; set; } = null!;
        public string Condition { get; set; } = null!;
        public List<FormDeliveryRecipientDto> Recipients { get; set; } = new();
    }
}
