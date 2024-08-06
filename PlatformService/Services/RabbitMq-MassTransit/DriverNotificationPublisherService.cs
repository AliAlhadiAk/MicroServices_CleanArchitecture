using MassTransit;
using PlatformService.Dto_s;

namespace PlatformService.Services.RabbitMq_MassTransit
{
	public class DriverNotificationPublisherService:IDriverNotificationPublisherService
	{
		private readonly ILogger<DriverNotificationPublisherService> _logger;
		private readonly IPublishEndpoint _publishEndpoint;

        public DriverNotificationPublisherService(ILogger<DriverNotificationPublisherService> logger, IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

		public async Task SendNotification(Guid driverId, string teamName)
		{
			_logger.LogInformation("Notification Sending Started .....");
			await _publishEndpoint.Publish(new DriverNotificationRecord(driverId, teamName));

		}
	}
}
