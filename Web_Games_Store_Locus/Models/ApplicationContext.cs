using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web_Games_Store_Locus.Models.Entities;

namespace Web_Games_Store_Locus.Models
{
    public class ApplicationContext : IdentityDbContext<User>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Tag>()
                .HasKey(el => el.Id);
            builder.Entity<Category>()
                .HasKey(el => el.Id);
            builder.Entity<UserInfo>()
                .HasKey(el => el.Id);

            builder.Entity<User>()
                .HasOne(el => el.UserInfo)
                .WithOne(el => el.User)
                .HasForeignKey<UserInfo>(el => el.Id);

            //builder.Entity<UserInfo>()
            //    .HasMany(el => el.MyFriends)
            //    .WithMany(el => el.MeAsFriend)
            //    .UsingEntity(el=>el.ToTable("Friend"));
            //builder.Entity<User>()
            //    .HasOne(el => el.UserInfo)
            //    .WithOne(el => el.User)
            //    .HasForeignKey<UserInfo>(el => el.Id);

            builder.Entity<Tag>()
                .HasMany(el => el.Products)
                .WithMany(el => el.Tags)
                .UsingEntity(j => j.ToTable("ProductsTags"));
            //builder.Entity<PostUser>()
            //    .HasOne(el => el.Post)
            //    .WithMany(el => el.Likes)
            //    .HasForeignKey(el => el.Id);

            base.OnModelCreating(builder);
        }
        public DbSet<UserInfo> UserInfos { get; set; }
        public DbSet<Category> Categories { get; set; }
        //public DbSet<Product> Products { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Invite> Invites { get; set; }
        public DbSet<Friend> Friends { get; set; }
        public DbSet<Post> Posts { get; set; }
    }
}
