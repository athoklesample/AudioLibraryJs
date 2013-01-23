using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using AudioLibrary.DataInterface;

namespace AudioLibrary.DataContext.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity>, IEfRepository where TEntity : class
    {
        public Repository(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null) throw new ArgumentNullException("unitOfWork");
            UnitOfWork = unitOfWork;
        }

        protected DataContext DataContext
        {
            get { return (DataContext) UnitOfWork; }
        }

        public IUnitOfWork UnitOfWork { get; private set; }

        /// <summary>
        ///     Add item to dbset
        /// </summary>
        /// <param name="item"></param>
        public virtual void Add(TEntity item)
        {
            GetDbSet().Add(item);
            UnitOfWork.Commit();
        }

        public void AddAll(IEnumerable<TEntity> items)
        {
            DbSet<TEntity> dbSet = GetDbSet();
            foreach (TEntity entity in items)
            {
                dbSet.Add(entity);
            }
            UnitOfWork.Commit();
        }

        /// <summary>
        ///     remove item from dbset
        /// </summary>
        /// <param name="item"></param>
        public virtual void Remove(TEntity item)
        {
            //attach item if it does not exist
            GetDbSet().Attach(item);

            //set as "removed"
            GetDbSet().Remove(item);
            UnitOfWork.Commit();
        }

        public void RemoveAll(IEnumerable<TEntity> items)
        {
            DbSet<TEntity> dbSet = GetDbSet();
            foreach (TEntity entity in items)
            {
                dbSet.Attach(entity);
                dbSet.Remove(entity);
            }
            UnitOfWork.Commit();
        }

        public void RemoveAll(IEnumerable<long> ids)
        {
            DbSet<TEntity> dbSet = GetDbSet();
            foreach (TEntity entity in ids.Select(id => dbSet.Find(id)))
            {
                dbSet.Attach(entity);
                dbSet.Remove(entity);
            }
            UnitOfWork.Commit();
        }


        /// <summary>
        ///     Retrieves an entity by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual TEntity Get(long id)
        {
            return GetDbSet().Find(id);
        }


        /// <summary>
        ///     Updates the entity in the context
        /// </summary>
        /// <param name="item"></param>
        public void Update(TEntity item)
        {
            GetDbSet().Attach(item);
            SetEntityState(item, EntityState.Modified);
            UnitOfWork.Commit();
        }


        /// <summary>
        ///     Retrieves a paged list
        /// </summary>
        /// <typeparam name="TKProperty"></typeparam>
        /// <param name="pageIndex"></param>
        /// <param name="pageCount"></param>
        /// <param name="orderByExpression"></param>
        /// <param name="ascending"></param>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> GetPaged<TKProperty>(int pageIndex, int pageCount,
                                                                 Expression<Func<TEntity, TKProperty>> orderByExpression,
                                                                 bool ascending)
        {
            DbSet<TEntity> set = GetDbSet();

            if (ascending)
            {
                return set.OrderBy(orderByExpression)
                          .Skip(pageCount*pageIndex)
                          .Take(pageCount);
            }
            return set.OrderByDescending(orderByExpression)
                      .Skip(pageCount*pageIndex)
                      .Take(pageCount);
        }

        /// <summary>
        ///     Retrieves list of entities filtered by whereExpression
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> GetFiltered(Expression<Func<TEntity, bool>> whereExpression)
        {
            return GetDbSet().Where(whereExpression);
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (UnitOfWork != null)
                UnitOfWork.Dispose();
        }

        /// <summary>
        ///     returns the dbset
        /// </summary>
        /// <returns></returns>
        public virtual DbSet<TEntity> GetDbSet()
        {
            return DataContext.Set<TEntity>();
        }

        /// <summary>
        ///     sets the state for an entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="entityState"></param>
        protected virtual void SetEntityState(object entity, EntityState entityState)
        {
            DataContext.Entry(entity).State = entityState;
        }
    }
}