using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Entities.Context;
using Entities.Models;
using MvcUser.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.FileProviders;
using System.IO;

namespace MvcUser
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
            //string conString = "DataSource=HOME-DASHA\\SQLEXPRESS;Initial Catalog=usersstoredb;UserId=sa;Password=qaftfh6h;MultipleActiveResultSets=True;App=EntityFramework";
            services.AddDbContext<ApplicationUserContext>(options =>
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.UseSqlServer(Configuration.GetConnectionString("DEBContext"), x => x.MigrationsAssembly("Entities")));

            services.AddDbContext<TeamContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DEBContext"), x => x.MigrationsAssembly("Entities")));

            services.AddDbContext<CafeContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DEBContext"), x => x.MigrationsAssembly("Entities")));

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationUserContext>()
                .AddDefaultTokenProviders();

            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.Configure<SMSoptions>(Configuration);
            //services.Configure<Emailoptions>(Configuration);
            services.Configure<IdentityOptions>(options =>
            {
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
                // Default Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 1;
            }
           );
           

            /*var optionsBuilder = new DbContextOptionsBuilder<ApplicationUserContext>();
            optionsBuilder.UseSqlServer(Configuration.GetConnectionString("DEBContext"));

            using (var context = new ApplicationUserContext(optionsBuilder.Options))
            {
                context.Database.Migrate();
            }

            var optionBuilder = new DbContextOptionsBuilder<TeamContext>();
            optionBuilder.UseSqlServer(Configuration.GetConnectionString("DEBContext"));

            using (var context = new TeamContext(optionBuilder.Options))
            {
                context.Database.Migrate();
            }*/

            services.AddDirectoryBrowser();


            services.AddMvc()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressConsumesConstraintForFormFileParameters = true;
                    options.SuppressInferBindingSourcesForParameters = true;
                    options.SuppressModelStateInvalidFilter = true;
                    options.SuppressMapClientErrors = true;
                    options.SuppressUseValidationProblemDetailsForInvalidModelStateResponses = true;
                    options.ClientErrorMapping[404].Link =
                    "https://httpstatuses.com/404";
                }
                );
        }

        private async Task CreateUserRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<User>>();
            string[] roleNames = { "admin", "leader" };
            IdentityResult roleResult;
            
            //Adding Leader Role
            foreach (var roleName in roleNames)
            {
                var roleCheck = await RoleManager.RoleExistsAsync(roleName);
                if (!roleCheck)
                {
                    //Create Role
                    roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            var powerUser = new User
            {
                UserName = Configuration.GetSection("UserSettings")["UserEmail"],
                Email = Configuration.GetSection("UserSettings")["UserEmail"],
                FirstName = Configuration.GetSection("UserSettings")["UserFirstName"],
                LastName = Configuration.GetSection("UserSettings")["UserLastName"]
            };

            string UserPassword = Configuration.GetSection("UserSettings")["UserPassword"];
            var _user = await UserManager.FindByEmailAsync(Configuration.GetSection("UserSettings")["UserEmail"]);

            if (_user == null)
            {
                var createPowerUser = await UserManager.CreateAsync(powerUser, UserPassword);
                if (createPowerUser.Succeeded)
                {
                    await UserManager.AddToRoleAsync(powerUser, "admin");
                }
            }
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider services)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseHttpsRedirection();


            app.UseStaticFiles(new StaticFileOptions
            {
                ServeUnknownFileTypes = true
                
            });

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                     name: "default",
                     template: "{controller=Home}/{action=Index}/{id?}");

                
                /*routes.MapRoute(
                    name: "Default",
                    template: "{controller}/{action}/{id}",
                    defaults: new { controller = "Home", action = "Index", id = RouteParameter.Optional });*/
            });

            app.UseFileServer(new FileServerOptions
            {
                FileProvider = new PhysicalFileProvider(
            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "download")),
                RequestPath = "/download",
                EnableDirectoryBrowsing = true
            });

            CreateUserRoles(services).Wait();
        }

    }
}
