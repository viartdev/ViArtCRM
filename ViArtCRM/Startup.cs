﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ViArtCRM {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddMvc();            
            services.AddDistributedMemoryCache();
            services.AddSession(s => s.IdleTimeout = TimeSpan.FromSeconds(10));

            services.Add(new ServiceDescriptor(typeof(Models.TasksContext), new Models.TasksContext(Configuration.GetConnectionString("TasksConnection"))));
            services.Add(new ServiceDescriptor(typeof(Models.TaskModuleContext), new Models.TaskModuleContext(Configuration.GetConnectionString("TasksConnection"))));
            services.Add(new ServiceDescriptor(typeof(Models.UserContext), new Models.UserContext(Configuration.GetConnectionString("TasksConnection"))));
            services.Add(new ServiceDescriptor(typeof(Models.ChatContext), new Models.ChatContext(Configuration.GetConnectionString("TasksConnection"))));
            services.Add(new ServiceDescriptor(typeof(Models.SessionContext), new Models.SessionContext(Configuration.GetConnectionString("TasksConnection"))));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseSession();

            app.UseMvc(routes => {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Scheduler}/{action=Index}/{id?}");
            });

        }
    }
}
