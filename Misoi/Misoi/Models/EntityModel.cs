using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Misoi.Models
{
    public class EntityModel : DbContext
    {
        public EntityModel() : base("EntityModel") { }

        static EntityModel()
        {
           System.Data.Entity.Database.SetInitializer(new DropCreateDatabaseIfModelChanges<EntityModel>()); 
        }

        public virtual DbSet<LoadedImage> Images { get; set; }
    }
}