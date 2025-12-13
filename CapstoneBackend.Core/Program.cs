using System.Runtime.InteropServices;
using CapstoneBackend.Core.Models;
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
        // Explicitly set URLs from environment variable or use defaults for Docker
        // This must be set before UseStartup to ensure it's not overridden
        var urls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS");
        var builder = WebHost.CreateDefaultBuilder(args);
        
        if (!string.IsNullOrEmpty(urls))
        {
            builder.UseUrls(urls.Split(';'));
        }
        else
        {
            // Default for Docker if no environment variable is set
            builder.UseUrls("http://0.0.0.0:8080");
        }
        
        builder.UseStartup<Startup>();
        return builder;
    }
}