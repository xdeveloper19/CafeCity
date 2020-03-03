using Entities.Context;
using Entities.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;

namespace Entities.Repositorii
{
    public class SeedSlivers
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            
            using (var context = new TeamContext(serviceProvider.GetRequiredService<DbContextOptions<TeamContext>>()))
            {
                //Look for any movies
                if (context.Slivers.Any())
                {
                    return; //DB has been seeded
                }
                var appEnvironment = serviceProvider.GetService<IHostingEnvironment>();

                string starPath = Path.Combine(appEnvironment.ContentRootPath, "wwwroot/img/slivers/star.jpg");
                string firePath = Path.Combine(appEnvironment.ContentRootPath, "wwwroot/img/slivers/fire.jpg");
                string hammerPath = Path.Combine(appEnvironment.ContentRootPath, "wwwroot/img/slivers/hammer.jpg");
                string commonPath = Path.Combine(appEnvironment.ContentRootPath, "wwwroot/img/slivers/lso.jpg");

                context.Slivers.AddRange(
                        new Sliver
                        {
                            Title = "Звезда",
                            Rang = "Командир",
                            FileContent = File.ReadAllBytes(starPath),
                            FileType = "png/jpeg"
                        },

                        new Sliver
                        {
                            Title = "Факел",
                            Rang = "Комиссар",
                            FileContent = File.ReadAllBytes(firePath),
                            FileType = "png/jpeg"
                        },

                        new Sliver
                        {
                            Title = "ЛСО",
                            Rang = "Боец",
                            FileContent = File.ReadAllBytes(commonPath),
                            FileType = "png/jpeg"
                        },

                        new Sliver
                        {
                            Title = "Молоточек",
                            Rang = "Мастер",
                            FileContent = File.ReadAllBytes(hammerPath),
                            FileType = "png/jpeg"
                        }
                        );
                context.SaveChanges();
            }
        }
    }
}

