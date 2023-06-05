
using Web.Models;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {


        //Datos base de datos
        Config.DBServer = Configuration["DataBase:Host"];
        Config.DBName = Configuration["DataBase:Name"];
        Config.DBUser = Configuration["DataBase:User"];
        Config.DBPass = Configuration["DataBase:Pass"];

        //Seguridad ApiService
        Config.KeysApiControllerUsername = Configuration["KeyApi:KeysApiControllerUsername"];
        Config.KeysApiControllerPassword = Configuration["KeyApi:KeysApiControllerPassword"];

        //Url base de la api
        Config.UrlBase = Configuration["ApiNative:UrlBase"];

        services.AddControllers();

        // Configuración de CORS
        services.AddCors(options =>
        {
            options.AddPolicy("AllowOrigin", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });

        // Otros servicios de la aplicación
        // ...
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            // Configuraciones para el entorno de producción
            // ...
        }

        app.UseRouting();

        // Middleware de CORS
        app.UseCors("AllowOrigin");

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
