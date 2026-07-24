using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ViewYourPayments.Web.Models
{
    /// <summary>
    /// Object to hold description filter item to populate through FastSelect.
    /// </summary>
    public class DescriptionFilterItem
    {
        /// <summary>
        /// Descripiton as Text.
        /// </summary>
        public string Text { get; set; }
        
        /// <summary>
        /// Description as Value.
        /// </summary>
        public string Value { get; set; }
    }
}
