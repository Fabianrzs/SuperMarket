using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMarket.Aplication
{
    public class EntityResponse<TEntity>
    {
        public bool Error { get; set; }
        public ICollection<TEntity> Entities { get; set; }
        public TEntity Entity { get; set; }
        public string Message { get; set; }
        public EntityResponse(string message)
        {
            Message = message;
            Error = true;
        }
        public EntityResponse(string message, bool error)
        {
            Message = message;
            Error = error;
        }
        public EntityResponse(TEntity entity)
        {
            Entity = entity;
            Error = false;
        }
        public EntityResponse(ICollection<TEntity> entities)
        {
            Entities = entities;
            Error = false;
        }

        public void ConsolePrint(TEntity value)
        {
            Console.ForegroundColor = ConsoleColor.White;
            if (value != null)
            {
                var entityType = typeof(TEntity);
                var properties = entityType.GetProperties();

                foreach (var property in properties)
                {
                    var propertyName = property.Name;
                    var propertyValue = property.GetValue(value);

                    if (propertyValue != null && (property.PropertyType.IsPrimitive 
                        || property.PropertyType == typeof(string) || property.PropertyType == typeof(DateTime)))
                    {
                        Console.WriteLine($"{propertyName} : {propertyValue}");
                        Console.WriteLine($"{string.Join("",Enumerable.Repeat("-", 5 + propertyName.Length + propertyValue.ToString().Length))}\n");
                    }
                }
            }
        }
    }
}
