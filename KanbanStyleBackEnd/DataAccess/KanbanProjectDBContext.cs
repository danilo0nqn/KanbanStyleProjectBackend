using KanbanStyleBackEnd.Models.DataModels;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace KanbanStyleBackEnd.DataAccess
{
    public class KanbanProjectDBContext: DbContext
    {
        public KanbanProjectDBContext(DbContextOptions<KanbanProjectDBContext> options): base(options) { }

        //Add DbSets (Tables of our database)
        public DbSet<User>? Users { get; set; }
        public DbSet<Project>? Projects { get; set; }
        public DbSet<Assignment>? Assignments { get; set; }
        public DbSet<UsersLogin>? UsersLogins { get; set; }
        public DbSet<ProjectUser>? ProjectUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProjectUser>()
                .HasKey(pu => new { pu.ProjectId, pu.UserId });
            modelBuilder.Entity<ProjectUser>()
                .HasOne(p => p.Project)
                .WithMany(pu => pu.ProjectUsers)
                .HasForeignKey(p => p.ProjectId);
            modelBuilder.Entity<ProjectUser>()
                .HasOne(u => u.User)
                .WithMany(pu => pu.ProjectUsers)
                .HasForeignKey(u => u.UserId);
        }
    }
}
