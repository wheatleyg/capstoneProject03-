using System.Runtime.InteropServices;
using Microsoft.AspNetCore;

namespace CapstoneBackend.Core;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    private static IWebHostBuilder CreateHostBuilder(string[] args)
    {
        var builder = WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>();
        return builder;
    }
}