using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tovy
{
    /// <summary>
    /// Specifies a prefix to append to all <see cref="FieldAttribute"/> in the entity.
    /// </summary>
    /// <example>
    /// <code>
    /// [FieldPrefix("my")]
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
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class FieldPrefixAttribute : Attribute
    {
        /// <summary>
        /// Prefix to append to all <see cref="FieldAttribute"/> in the entity
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Instantiate an attribute that specifies a prefix to append to all <see cref="FieldAttribute"/> in the entity.
        /// </summary>
        /// <param name="name">prefix to append</param>
        public FieldPrefixAttribute(string name)
        {
            Name = name;
        }
    }
}
