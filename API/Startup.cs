using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using API.Models;
using API.Services;


namespace API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<UserDbSettings>(Configuration.GetSection(nameof(UserDbSettings)));
            services.AddSingleton<IUserDbSettings>(sp => sp.GetRequiredService<IOptions<UserDbSettings>>().Value);
            services.AddSingleton<UserService>();
            services.Configure<ChatDbSettings>(Configuration.GetSection(nameof(ChatDbSettings)));
            services.AddSingleton<IChatDbSettings>(sp => sp.GetRequiredService<IOptions<ChatDbSettings>>().Value);
            services.AddSingleton<ChatService>();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
