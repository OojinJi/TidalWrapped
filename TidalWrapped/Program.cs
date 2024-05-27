using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Api.Enums;
using IF.Lastfm.Core.Objects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using TidalWrapped;
using TidalWrapped.Models;
using TidalWrapped.Services;

public class Program
{
    private static async Task Main(string[] args)
    {
        var services = CreateService();
        TidalWrappedLogic app = services.GetRequiredService<TidalWrappedLogic>();
        app.mainLogic();


    }
    public static ServiceProvider CreateService()
    {
        var serviceProvider = new ServiceCollection()
            .AddLogging(options=>
            {
                options.ClearProviders();
            })
            .AddSingleton<DataService>()
            .AddSingleton<TidalWrappedLogic>()
            .AddSingleton<EmailService>()
            .BuildServiceProvider();

        return serviceProvider;
    }

}