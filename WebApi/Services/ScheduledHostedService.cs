using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebApi.Entities;
using WebApi.Helpers;

namespace WebApi.Services
{
    public class ScheduledHostedService : IHostedService, IDisposable
    {
        private DataContext _context;
        private readonly ILogger _logger;
        private Timer _timer;

        public ScheduledHostedService(
            DataContext context,
            ILogger<ScheduledHostedService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Background Service for scheduled jobs is starting.");

            _timer = new Timer(CheckApplicationExpiration, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        private void CheckApplicationExpiration(object state)
        {
            var applications = _context.Applications
                .Where(a => (a.Status == InterviuStatus.NotStarted.ToString()) &&
                            (a.Expiration <= DateTime.Now));

            if (applications != null)
            {
                //Change status of the expired applications to Expired                
                applications.ToList().ForEach(a => {
                    a.Status = InterviuStatus.Expired.ToString();
                    _logger.LogInformation("Background Service detected expired application: " + a.Id + " , changing status to Expired");
                });
                _context.SaveChanges();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Background Service for scheduled jobs is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
