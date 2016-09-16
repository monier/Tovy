using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Tovy
{
    /// <summary>
    /// Service that helps to map any compatible data source fields to an entity having properties decorated with <see cref="FieldAttribute"/>.
    /// </summary>
    public static class FieldMapper
    {
        /// <summary>
        /// Object instance used to synchronize access to the mapper in a multithead context
        /// </summary>
        private static object _lockObj = new object();
        /// <summary>
        /// Cache of map infos of entities (POCO) to prevent retrieving in each mapping
        /// </summary>
        private static Dictionary<Type, Dictionary<string, FieldMapInfo>> _mapInfosCache = new Dictionary<Type, Dictionary<string, FieldMapInfo>>();
        /// <summary>
        /// Maps the provided fields data to a list of entity
        /// </summary>
        /// <typeparam name="T">type of the entity</typeparam>
        /// <param name="dataReader">reader of the [external] data source</param>
        /// <returns>list of mapped entities</returns>
        public static IEnumerable<T> Map<T>(IFieldReader dataReader) where T : new()
        {
            T entity = default(T);
            Dictionary<string, FieldMapInfo> mapInfos = null;

            mapInfos = GetEntityMapInfos<T>();
            while (dataReader.Read())
            {
                entity = new T();
                foreach(var mapInfo in mapInfos)
                {
                    var value = dataReader.GetFieldValue(mapInfo.Key, mapInfo.Value.DefaultValueIfNull);
                    mapInfo.Value.PropertyInfo.SetValue(entity, value, null);
                }
                yield return entity;
            }
        }

        /// <summary>
        /// Returns a dictionary that contains the mapping descriptions of the entity's properties and the [external] data source's fields
        /// <para>The key of the dictionary is the field's name</para>
        /// </summary>
        /// <typeparam name="T">type of the entity</typeparam>
        /// <returns>dictionary that contains the mapping descriptions of the entity's properties and the [external] data source's fields</returns>
        private static Dictionary<string, FieldMapInfo> GetEntityMapInfos<T>()
        {
            Dictionary<string, FieldMapInfo> map = null;

            lock (_lockObj)
            {
                if (_mapInfosCache.TryGetValue(typeof(T), out map))
                    return map;
            }

            object[] attributes = null;
            FieldAttribute fieldAttribute = null;
            string fieldPrefix = null;
            string fieldName = null;

            map = new Dictionary<string, FieldMapInfo>();
            var properties = typeof(T).GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance);
            if (properties != null)
            {
                attributes = typeof(T).GetCustomAttributes(typeof(FieldPrefixAttribute), true);
                if (attributes != null && attributes.Length > 0)
                    fieldPrefix = ((FieldPrefixAttribute)attributes.First()).Name;
                foreach (var property in properties)
                {
                    attributes = property.GetCustomAttributes(typeof(FieldAttribute), true);
                    if (attributes != null && attributes.Length > 0)
                    {
                        // only one Field attribute is considered at this time
                        fieldAttribute = attributes.First() as FieldAttribute;
                        if (!fieldAttribute.Ignore)
                        {
                            fieldName = fieldPrefix + fieldAttribute.Name;
                            map.Add(fieldName, new FieldMapInfo() { FieldName = fieldName, PropertyInfo = property, DefaultValueIfNull = fieldAttribute.DefaultValueIfNull });
                        }
                    }
                }
            }
            return map;
        }
        /// <summary>
        /// Exposes members that describe the mapping between the entity's properties and the [external] data source's fields.
        /// </summary>
        private class FieldMapInfo
        {
            /// <summary>
            /// Field name from the [external] data source
            /// </summary>
            public string FieldName { get; set; }
            /// <summary>
            /// Entity's property info
            /// </summary>
            public PropertyInfo PropertyInfo { get; set; }
            /// <summary>
            /// Entity's property's default value if field's value is null
            /// </summary>
            public object DefaultValueIfNull { get; set; }
        }
    }
}
