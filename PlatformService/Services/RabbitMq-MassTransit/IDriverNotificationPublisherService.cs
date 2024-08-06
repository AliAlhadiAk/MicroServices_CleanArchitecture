namespace PlatformService.Services.RabbitMq_MassTransit
{
	public interface IDriverNotificationPublisherService
	{
		Task SendNotification(Guid driverId, string teamName);
	}
}
