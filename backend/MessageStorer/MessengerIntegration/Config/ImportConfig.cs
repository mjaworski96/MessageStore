using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerIntegration.Config
{
    public interface IImportConfig
    {
        int SecondsToWaitBeforeCheckForNewImports { get; }
        int ParallelImportsCount { get; }
        bool DeleteFileAfterImport { get; }
        string FileEncoding { get; }
    }
    public class ImportConfig: IImportConfig
    {
        public int SecondsToWaitBeforeCheckForNewImports { get; set; }
        public int ParallelImportsCount { get; set; }
        public bool DeleteFileAfterImport { get; set; }
        public string FileEncoding { get; set; }

        public ImportConfig(IConfiguration configuration)
        {
            configuration.GetSection("ImportConfig").Bind(this);
        }
    }
}
