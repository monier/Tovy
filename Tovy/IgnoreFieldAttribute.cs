using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tovy
{
    /// <summary>
    /// Describes that the specified property is to be ignored when mapping the whole entity/object with any [external] data source.
    /// </summary>
    public class IgnoreFieldAttribute : FieldAttribute
    {
        /// <summary>
        /// Instantiate an attribute that describes that the specified property is to be ignored when mapping the whole entity/object with any [external] data source.
        /// </summary>
        public IgnoreFieldAttribute()
        {
            Ignore = true;
        }
    }
}
