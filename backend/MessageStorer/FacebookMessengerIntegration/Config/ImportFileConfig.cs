﻿using Microsoft.Extensions.Configuration;

namespace FacebookMessengerIntegration.Config
{
    public interface IImportFileConfig
    {
        public string Directory { get; }
    }
    public class ImportFileConfig: IImportFileConfig
    {
        public ImportFileConfig(IConfiguration configuration)
        {
            configuration.GetSection("FileConfiguration").Bind(this);
        }
        public string Directory { get; set; }
    }
}
