using Microsoft.Extensions.Logging;
using Microsoft.SCIM;
using System;

namespace MvcMovie.Services
{
    public class LoggerMonitor : IMonitor
    {
        private readonly ILogger<LoggerMonitor> _logger;

        public LoggerMonitor(ILogger<LoggerMonitor> logger)
        {
            _logger = logger;
        }

        public void Inform(IInformationNotification notification)
        {
            _logger.LogInformation(notification.Message);
        }

        public void Report(IExceptionNotification notification)
        {
            _logger.LogInformation(notification.Message, notification.Message.Message);
        }

        public void Warn(Notification<Exception> notification)
        {
            _logger.LogWarning(notification.Message, notification.Message.Message);
        }

        public void Warn(Notification<string> notification)
        {
            _logger.LogWarning(notification.Message);
        }
    }
}
