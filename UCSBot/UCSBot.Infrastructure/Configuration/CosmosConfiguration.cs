namespace UCSBot.Infrastructure.Configuration;

public sealed class CosmosConfiguration
{
    public string AccountEndpoint { get; set; }
    public string AccountKey { get; set; }
    public string DatabaseName { get; set; }
    public string ChannelContainerName { get; set; }
    public string ChartContainerName { get; set; }
    public string ChartMessageContainerName { get; set; }
    public string UserMessageCategoryContainerName { get; set; }
}