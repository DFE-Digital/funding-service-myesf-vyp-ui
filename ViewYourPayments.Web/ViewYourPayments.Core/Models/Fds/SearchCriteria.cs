using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewYourPayments.Core.Models.Fds
{
    public class SearchCriteria
    {
        /// <summary>
        /// Gets or sets the field name.
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Gets or sets the field comparer operator.
        /// </summary>
        public string Operator { get; set; } = "numericContains";

        /// <summary>
        /// Gets or sets the field comparer value.
        /// </summary>
        public string Value { get; set; }
    }
}
