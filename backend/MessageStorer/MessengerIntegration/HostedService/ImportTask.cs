using MessengerIntegration.Config;
using MessengerIntegration.Infrastructure;
using MessengerIntegration.Persistance.Entity;
using MessengerIntegration.Persistance.Repository;
using MessengerIntegration.Service;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MessengerIntegration.HostedService
{
    public class ImportTask
    {
        public bool Completed { get; set; }      
        private CancellationTokenSource _cancellationTokenSource;
        private readonly object _syncObject;
        private readonly IImportConfig _config;
        private readonly IImportRepository _importRepository;
        private readonly IImportService _importService;
        private readonly IZipFile _zipFile;
        private readonly IApiClient _apiClient;
        private readonly ILogger<ImportTask> _logger;
        private Imports _import;

        public ImportTask(object syncObject, IImportConfig config, IImportRepository importRepository, IImportService importService, IZipFile zipFile, IApiClient apiClient, ILogger<ImportTask> logger)
        {
            Completed = true;
            _syncObject = syncObject;
            _config = config;
            _importRepository = importRepository;
            _importService = importService;
            _zipFile = zipFile;
            _apiClient = apiClient;
            _logger = logger;
        }

        public void StartImport(Imports import)
        {
            Completed = false;
            _import = import;
            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;
            Task.Run(async () => await ImportFile(token), token);
            
        }
        public async Task Cancel()
        {
            _cancellationTokenSource.Cancel();
            await SetStatus(Statuses.Queued);
        }
        private async Task ImportFile(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Start import {_import.Id}");
            try
            {
                await SetStatus(Statuses.Processing);
                cancellationToken.ThrowIfCancellationRequested();

                await SetStatus(Statuses.Completed);
                _logger.LogInformation($"Finished import {_import.Id}");
            }
            catch(Exception e)
            {
                _logger.LogError($"Exception occured while importing file for import: {_import.Id}: {e.Message}\n{e.StackTrace}");
                await SetStatus(Statuses.ErrorUnknownError);
            }
            finally
            {
                lock(_syncObject)
                {
                    Completed = true;
                }
            }
            
        }
        private async Task SetStatus(string statusName)
        {
            if (!Completed && _import != null)
            {
                await _importService.SetStatus(_import, statusName);
            }
        }
    }
}
