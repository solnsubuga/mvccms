using Microsoft.AspNet.Identity.EntityFramework;
using MvcCMS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcCMS.Data
{
   public class CmsContext: IdentityDbContext<CmsUser>
    {
        public CmsContext()
            : base("CmsContext", throwIfV1Schema: false)
        {
        }
        public DbSet<Post> Posts { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Post>()
                .HasKey(e => e.Id)
                .Property(e => e.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            modelBuilder.Entity<Post>()
                .HasRequired(e => e.Author);
        }
    }
}
