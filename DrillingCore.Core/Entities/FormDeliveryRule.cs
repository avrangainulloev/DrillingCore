using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Core.Entities
{
    public class FormDeliveryRule
    {
        public int Id { get; set; }

        public int ProjectId { get; set; }
        public int FormTypeId { get; set; }

        public FormDeliveryCondition Condition { get; set; }

        public Project Project { get; set; } = null!;
        public FormType FormType { get; set; } = null!;
        public ICollection<FormDeliveryRecipient> Recipients { get; set; } = new List<FormDeliveryRecipient>();
    }

}
