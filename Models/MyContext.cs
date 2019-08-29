using Microsoft.EntityFrameworkCore;

namespace GroupLearn.Models
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions options) : base(options){}
        public DbSet<User> Users{get;set;}
        public DbSet<Group> Groups{get;set;}
        public DbSet<School> Schools{get;set;}
        public DbSet<UserGroup> UserGroups{get;set;}
        public DbSet<UserRates> UserRates {get; set;}
    }
}