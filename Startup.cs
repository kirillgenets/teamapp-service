
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TeamAppService.Models;
using System.Diagnostics;

namespace TeamAppService
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
            services.AddMvc();
            services.AddDbContext<TaskContext>(opt =>
                opt.UseInMemoryDatabase("TaskList"));
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            string runDbFilePath = System.IO.Directory.GetCurrentDirectory() + "\\run-db.bat";

            // starting MongoDB server
            Process process = new Process();
            process.StartInfo.FileName = @"C:\\Windows\\System32\cmd.exe";
            process.StartInfo.Arguments = runDbFilePath;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = false;

            process.Start();

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
