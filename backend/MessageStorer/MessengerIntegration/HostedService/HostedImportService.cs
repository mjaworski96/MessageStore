using MessengerIntegration.Config;
using MessengerIntegration.Infrastructure;
using MessengerIntegration.Persistance.Repository;
using MessengerIntegration.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MessengerIntegration.HostedService
{
    public class HostedImportService : IHostedService, IDisposable
    {
        private readonly IImportConfig _config;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IFileUtils _fileUtils;
        private readonly IZipFile _zipFile;
        private readonly IAttachmentResolve _attachmentResolve;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IApiConfig _apiConfig;
        private readonly ILogger<ImportTask> _helperLogger;

        private Timer _timer;
        private List<ImportTask> _importTasks;
        private object _syncObject;
       

        public HostedImportService(IImportConfig config, IServiceScopeFactory serviceScopeFactory,
            IFileUtils fileUtils, IZipFile zipFile, IAttachmentResolve attachmentResolve,
            IHttpClientFactory httpClientFactory, IApiConfig apiConfig, ILogger<ImportTask> helperLogger)
        {
            _config = config;
            _serviceScopeFactory = serviceScopeFactory;

            _fileUtils = fileUtils;
            _zipFile = zipFile;
            _attachmentResolve = attachmentResolve;
            _apiConfig = apiConfig;
            _syncObject = new object();
            _helperLogger = helperLogger;
            _httpClientFactory = httpClientFactory;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _importTasks = new List<ImportTask>(_config.ParallelImportsCount);
            for (int i = 0; i < _config.ParallelImportsCount; i++)
            {
                _importTasks.Add(new ImportTask(_syncObject, _config, _serviceScopeFactory, _fileUtils,
                    _zipFile, _attachmentResolve, _httpClientFactory, _apiConfig, _helperLogger));
            }

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(_config.SecondsToWaitBeforeCheckForNewImports));

            return Task.CompletedTask;
        }

        public void DoWork(object state)
        {
            List<ImportTask> freeTasks;
            lock (_syncObject)
            {
                freeTasks = _importTasks.Where(x => x.Completed).ToList();

                if (freeTasks.Any())
                {
                    using(var scope = _serviceScopeFactory.CreateScope())
                    {
                        var importRepository = scope.ServiceProvider.GetService<IImportRepository>();
                        var queuedImportsTask = importRepository.GetQueued(_config.ParallelImportsCount);
                        queuedImportsTask.Wait();
                        var queuedImports = queuedImportsTask.Result;
                        for (int i = 0; i < queuedImports.Count; i++)
                        {
                            freeTasks[i].StartImport(queuedImports[i].Id).Wait();
                        }
                    }   
                }
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _timer.DisposeAsync();
            _importTasks.ForEach(async item => await item.Cancel());
        }

        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}
