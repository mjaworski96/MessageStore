using MessengerIntegration.Config;
using MessengerIntegration.Infrastructure;
using MessengerIntegration.Persistance.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MessengerIntegration.HostedService
{
    public class HostedImportService : IHostedService, IDisposable
    {
        private readonly IImportConfig _config;
        private readonly IImportRepository _importRepository;
        private readonly IStatusRepository _statusRepository;
        private readonly IZipFile _zipFile;
        private readonly IApiClient _apiClient;

        private Timer _timer;

        public HostedImportService(IImportConfig config, IServiceScopeFactory serviceScopeProvider, IZipFile zipFile, IApiClient apiClient)
        {
            _config = config;
            using (var scope = serviceScopeProvider.CreateScope())
            {
                _importRepository = scope.ServiceProvider.GetService<IImportRepository>();
                _statusRepository = scope.ServiceProvider.GetService<IStatusRepository>();
            }
            _zipFile = zipFile;
            _apiClient = apiClient;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            Console.WriteLine("TODO");
        }
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _timer.DisposeAsync();
        }

        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}
