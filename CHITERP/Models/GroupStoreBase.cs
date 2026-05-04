using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Threading.Tasks;


namespace CHITERP.Models
{
    public class GroupStoreBase
    {
        public DbContext Context
        {
            get;
            private set;
        }


        public DbSet<ApplicationGroups> DbEntitySet
        {
            get;
            private set;
        }


        public IQueryable<ApplicationGroups> EntitySet
        {
            get
            {
                return this.DbEntitySet;
            }
        }


        public GroupStoreBase(DbContext context)
        {
            this.Context = context;
            this.DbEntitySet = context.Set<ApplicationGroups>();
        }


        public void Create(ApplicationGroups entity)
        {
            this.DbEntitySet.Add(entity);
        }


        public void Delete(ApplicationGroups entity)
        {
            this.DbEntitySet.Remove(entity);
        }


        public virtual Task<ApplicationGroups> GetByIdAsync(object id)
        {
            return this.DbEntitySet.FindAsync(new object[] { id });
        }


        public virtual ApplicationGroups GetById(object id)
        {
            return this.DbEntitySet.Find(new object[] { id });
        }


        public virtual void Update(ApplicationGroups entity)
        {
            if (entity != null)
            {
                this.Context.Entry<ApplicationGroups>(entity).State = EntityState.Modified;
            }
        }
    }
}