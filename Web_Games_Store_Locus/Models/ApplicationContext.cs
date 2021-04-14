using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web_Games_Store_Locus.Models.Entities;

namespace Web_Games_Store_Locus.Models
{
    public class ApplicationContext: IdentityDbContext<User>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>()
                .HasOne(el => el.UserInfo)
                .WithOne(el => el.User)
                .HasForeignKey<UserInfo>(el => el.Id);

            builder.Entity<User>()
                .HasOne(el => el.UserInfo)
                .WithOne(el => el.User)
                .HasForeignKey<UserInfo>(el => el.Id);

            base.OnModelCreating(builder);
        }
        public DbSet<UserInfo> UserInfos { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Category> Category { get; set; }
    }
}
