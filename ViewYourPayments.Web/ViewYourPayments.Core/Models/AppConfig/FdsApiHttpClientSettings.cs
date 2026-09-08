using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewYourPayments.Core.Models.AppConfig
{
    public class FdsApiHttpClientSettings
    {
        /// <summary>
        /// The base address of the FDS API.
        /// </summary>
        public string BaseAddress { get; set; }

        /// <summary>
        /// The subscription key for the FDS API.
        /// </summary>
        public string ApimSubscriptionKey { get; set; }
    }
}
