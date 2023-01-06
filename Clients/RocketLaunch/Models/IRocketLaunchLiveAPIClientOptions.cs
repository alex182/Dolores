namespace Dolores.Clients.RocketLaunch
{
    public interface IRocketLaunchLiveAPIClientOptions
    {
        string BaseUrl { get; set; }
        string ApiKey { get; set; }
    }
}