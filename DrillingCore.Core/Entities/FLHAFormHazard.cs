using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Core.Entities
{
    public class FLHAFormHazard
    {
        public int Id { get; set; }

        public int FLHAFormId { get; set; }
        public FLHAForm FLHAForm { get; set; }

        public string HazardText { get; set; }        // либо из шаблона, либо своё
        public string ControlMeasures { get; set; }   // можно изменить шаблонный ответ или свой

        public int? HazardTemplateId { get; set; }    // если из шаблона
        public FLHAHazard? HazardTemplate { get; set; }
    }

}
