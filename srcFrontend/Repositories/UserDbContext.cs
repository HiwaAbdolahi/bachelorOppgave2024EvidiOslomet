
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using srcFrontend.Models;
using Microsoft.EntityFrameworkCore;

namespace srcFrontend.Repositories;

public class UserDbContext : IdentityDbContext
{

    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }


    public DbSet<User> Users1 { get; set; }

   

}
