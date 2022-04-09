namespace UCSBot.Configurations;

public sealed class ServiceBusConfiguration
{
    public string ConnectionString { get; set; }
    public string QueueName { get; set; }
}