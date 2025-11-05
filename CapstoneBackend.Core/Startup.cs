using CapstoneBackend.Auth;
using CapstoneBackend.Utilities;

namespace CapstoneBackend.Core;

public class Startup
{
    private readonly IConfiguration _configuration;
    
    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    //This method gets called by the runtime. Use this method to add services.
    public void ConfigureServices(IServiceCollection services)
    {
        /* //Check what value providers are registered
        foreach (var provider in ((IConfigurationRoot)_configuration).Providers.ToList())
        {
            Console.WriteLine(provider.ToString());
        }
        */
        
        services.AddControllers();
        
        services.AddScoped<DbConnectionTest>();
        AuthSetup.AddAuth(services, _configuration);
    }

    //This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            
        }
        else //we're not doing anything fancy, so we can assume dev or 'prod' are the only choices here
        {
            app.UseHsts(); //OWASP says to use this
            app.UseHttpsRedirection();
        }

        app.UseRouting();
        
        //oversimplified CORS policy to prevent headaches
        app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        
        app.UseAuthentication();
        
        //enables access to request bodies when logging
        app.Use((context, next) =>
        {
            context.Request.EnableBuffering();
            return next();
        });
        
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}