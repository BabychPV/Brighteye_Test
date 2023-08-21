using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Brighteye_Test
{
    static class DbContextExtensions
    {
        public static IQueryable<TEntity> SortTable<TEntity>(
     this DbSet<TEntity> inputTable, string columnName, bool descending = false)
     where TEntity : class
        {
            try
            {
                var entityType = typeof(TEntity);
                var property = entityType.GetProperty(columnName);

                if (property != null)
                {

                    var parameter = Expression.Parameter(entityType, "entity");
                    var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                    var orderByExp = Expression.Lambda(propertyAccess, parameter);

                    var queryableTable = (IQueryable<TEntity>)inputTable;
                    var orderByMethod = descending
                        ? typeof(Queryable).GetMethods()
                            .Where(m => m.Name == "OrderByDescending" && m.GetParameters().Length == 2)
                            .Single()
                            .MakeGenericMethod(entityType, property.PropertyType)
                        : typeof(Queryable).GetMethods()
                            .Where(m => m.Name == "OrderBy" && m.GetParameters().Length == 2)
                            .Single()
                            .MakeGenericMethod(entityType, property.PropertyType);

                    return (IQueryable<TEntity>)(orderByMethod?.Invoke(null, new object[] { queryableTable, orderByExp }) ?? queryableTable);

                }
                else
                {
                    throw new ArgumentException($"Property '{columnName}' not found in entity type '{entityType.FullName}'.");
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException("sorting table", ex);
                return inputTable;
            }
        }
    }
}
