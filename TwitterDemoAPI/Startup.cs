using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models; 
using TwitterModels;
using TwitterServices;
using TwitterServices.Interfaces;

namespace TwitterDemoAPI
{
    public class Startup
    {  
        private readonly IConfiguration _configuration; 

        public Startup(IConfiguration config)
        {
            _configuration = config; 
        }

        public IConfiguration Configuration { get; }
          
        public void ConfigureServices(IServiceCollection services)
        {  
            services.AddCors(options =>
            {
                options.AddPolicy(name: "AppPolicy",
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:44314")
                        .WithMethods("GET", "POST", "DELETE");
                    });
            });
             
            services.AddControllers();
            services.Configure<AppSettings>(_configuration.GetSection("AppSettings"));
            services.AddTransient<ITwitterService, TwitterService>();
            services.AddMemoryCache();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TwitterDemoAPI", Version = "v1" });
                var xmlPath = "TwitterDemoAPI.xml";
                c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
            }); 
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TwitterDemoAPI v1"));
            }

            app.UseHttpsRedirection(); 
            app.UseRouting(); 
            app.UseCors("AppPolicy"); 
            app.UseAuthorization(); 
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
