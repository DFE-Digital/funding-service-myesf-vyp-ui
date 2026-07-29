using CsvHelper;
using System.Globalization;
using ViewYourPayments.Core.Interfaces.Services;

namespace ViewYourPayments.Core.Services
{
    public class DocumentManagementService : IDocumentManagementService
    {
        public byte[] TransformRecordsToCsvBytes(IEnumerable<object> records)
        {
            if (records == null) return Array.Empty<byte>();
            using var memoryStream = new MemoryStream();
            using var streamWriter = new StreamWriter(memoryStream);
            using var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);
            csvWriter.WriteRecords(records);
            streamWriter.Flush();
            return memoryStream.ToArray();
        }
    }
}
