using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DDDCqrsEs.Common;
using DDDCqrsEs.Domain.Entities;
using DDDCqrsEs.Domain.Projections;
using Quotation.Domain.Entities.Authentication;

namespace DDDCqrsEs.Persistance
{
    [MapServiceDependency(nameof(ToDoDbContext))]
    public class ToDoDbContext : IdentityDbContext<DDDCqrsEs.Domain.Entities.User>, IToDoDbContext
    {
        public DbSet<ToDo> ToDos { get; set; }
        public DbSet<StockProjection> Stocks { get; set; }
        public DbSet<ApplicationRole> ApplicationRoles { get; set; }

        public ToDoDbContext(DbContextOptions<ToDoDbContext> options)
           : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ToDoDbContext).Assembly);
        }
    }
}
