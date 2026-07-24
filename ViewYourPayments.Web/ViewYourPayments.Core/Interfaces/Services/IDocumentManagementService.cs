using System;
using System.Collections.Generic;
using System.Text;

namespace ViewYourPayments.Core.Interfaces.Services
{
    /// <summary>
    /// Api for manipulating and getting information from spreadsheets.
    /// </summary>
    public interface IDocumentManagementService
    {
        /// <summary>
        /// Transform records into byte array to create csv file
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        byte[] TransformRecordsToCsvBytes(IEnumerable<object> records);
    }
}
