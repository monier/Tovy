using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tovy
{
    /// <summary>
    /// Describes the mapping between an entity's property and a data value from any [external] source.
    /// </summary>
    /// <example>
    /// <code>
    /// public class MyEntity
    /// {
    ///     [Field("id")}
    ///     public int Id {get; set;}
    ///     [Field("name")]
    ///     public string Name {get; set;}
    ///     [Field(Ignore=true)]
    ///     public string TemporaryValue {get; set;}
    /// }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class FieldAttribute : Attribute
    {
        /// <summary>
        /// Data field name in the data source
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// True if the specified property should be ignored when mapping fields from data source
        /// </summary>
        public bool Ignore { get; set; }
        /// <summary>
        /// Default value for the entity's property if data field is null
        /// </summary>
        public object DefaultValueIfNull { get; set; }
        /// <summary>
        /// Instantiate an attribute that provides a way to describe mapping between an entity and a data source
        /// </summary>
        /// <param name="name">name of the data field in the data source</param>
        public FieldAttribute(string name)
        {
            Name = name;
        }
        /// <summary>
        /// Instantiate an attribute that provides a way to describe mapping between an entity and a data source
        /// </summary>
        public FieldAttribute() { }
    }
}
