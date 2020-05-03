using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Entities.Models;

namespace Entities.Context
{
    public class ApplicationUserContext: IdentityDbContext<User>
    {
        public ApplicationUserContext(DbContextOptions<ApplicationUserContext>options): base(options)
        {
            //Database.Migrate();
        }
    }
}
