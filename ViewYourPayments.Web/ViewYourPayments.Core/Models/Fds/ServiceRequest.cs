using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewYourPayments.Core.Models.Fds
{
    internal class ServiceRequest
    {
        /// <summary>
        /// Gets or sets the search criteria.
        /// </summary>
        public List<List<SearchCriteria>> SearchCriteria { get; set; } = new List<List<SearchCriteria>>();

        /// <summary>
        /// Gets or sets the page number.
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Gets or sets the page size.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the sort column.
        /// </summary>
        public string SortColumn { get; set; } = "ukprn";

        /// <summary>
        /// Gets or sets a value indicating whether the response data is on sort order descending.
        /// </summary>
        public bool SortDescending { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the response data to skip paging.
        /// </summary>
        public bool SkipPaging { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the response data is spi data format.
        /// </summary>
        public bool SpiData { get; set; } = true;
    }
}
