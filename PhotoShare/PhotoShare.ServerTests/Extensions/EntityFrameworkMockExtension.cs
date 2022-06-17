using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace PhotoShare.ServerTests.Extensions
{
    public static class EntityFrameworkMockExtension
    {
        public static Mock<DbSet<TEntity>> SetupDbSet<TEntity>( this Mock<DbSet<TEntity>> mock, List<TEntity> values) where TEntity : class
        {
            mock.Setup(x => x.Add(It.IsAny<TEntity>())).Returns((TEntity entity) =>
            {
                values.Add(entity);
                return null;
            });
            //https://stackoverflow.com/questions/6645645/moq-testing-linq-where-queries
            var queryable = values.AsQueryable();
            mock.As<IQueryable<TEntity>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mock.As<IQueryable<TEntity>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mock.As<IQueryable<TEntity>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mock.As<IQueryable<TEntity>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
            
            return mock;

        }
    }
}
