﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinnworksSales.Data.Data.Models.Entity;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Z.BulkOperations;

namespace LinnworksSales.Data.Data.Repository.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class, IEntity
    {
        /// <summary>
        /// Get entity by ID
        /// </summary>
        /// <param name="id">Entity ID</param>
        /// <returns>Entity with the specified Id, if exists. Else, null.</returns>
        Task<TEntity> GetAsync(int id);

        TEntity Get(int id);

        /// <summary>
        /// Loading all objects of this entity
        /// </summary>
        /// <returns>Unordered list of all objects</returns>
        IQueryable<TEntity> GetAll();

        /// <summary>
        /// Save entity object
        /// </summary>
        /// <param name="entity">Saved object</param>
        Task<bool> SaveAsync(TEntity entity);

        /// <summary>
        /// The EF extension method let you insert a large number of entities in your database.
        /// </summary>
        /// <param name="collection">Entities collection</param>
        Task BulkInsertAsync(IEnumerable<TEntity> collection);

        /// <summary>
        /// The EF extension method let you merge (insert or update/Upsert) a large number of entities in your database.
        /// </summary>
        /// <param name="collection">Entities collection</param>
        Task BulkMergeAsync(IEnumerable<TEntity> collection);

        /// <summary>
        /// The EF extension method let you merge (insert or update/Upsert) a large number of entities in your database.
        /// </summary>
        /// <param name="collection">Entities collection</param>
        Task BulkMergeAsync(IEnumerable<TEntity> collection, Action<BulkOperation<TEntity>> options);

        /// <summary>
        /// Update entity object
        /// </summary>
        /// <param name="entity">Updated object</param>
        bool Update(TEntity entity);

        /// <summary>
        /// Delete entity object
        /// </summary>
        /// <param name="entity">Deleted object</param>
        bool Delete(TEntity entity);
    }
}
