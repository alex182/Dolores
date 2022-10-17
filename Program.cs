using Dolores.Clients.Discord;
using Dolores.Startup;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;


public class Program
{
    public static void Main(string[] args)
    {
         CreateWHostBuilder(args)
        .Build()
        .Run();
    }

    public static IWebHostBuilder CreateWHostBuilder(string[] args) =>
          WebHost.CreateDefaultBuilder(args)
              .UseStartup<Dolores.Startup.Startup>();
}
