using System;
using System.Collections.Generic;
using Core_Discord.CoreDatabase.Models;
using Microsoft.EntityFrameworkCore;

namespace Core_Discord.CoreDatabase.Repository
{
    /// <summary>
    /// Interface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T> where T : DbEntity
    {
        //SELECT
        T Get(int id);
        IEnumerable<T> GetAll();
        
        //INSERT
        void Add(T obj);
        void AddRange(params T[] objs);

        //DELETE
        void Remove(int id);
        void Remove(T obj);
        void RemoveRange(params T[] objs);

        //UPDATE
        void Update(T obj);
        void UpdateRange(params T[] objs);
    }
}
