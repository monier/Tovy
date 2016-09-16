using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tovy
{
    /// <summary>
    /// Service that reads field data from the [external] data source.
    /// <para>First, call the <see cref="Read"/> to know if there are sets of data to read.</para>
    /// <para>Then, call the <see cref="GetFieldValue"/> to get the value of the provided field name</para>
    /// </summary>
    public interface IFieldReader
    {
        /// <summary>
        /// Reads next set of data fields from the data source
        /// </summary>
        /// <returns>true if there's still data to read in the data source</returns>
        bool Read();
        /// <summary>
        /// Returns field value from the data source
        /// </summary>
        /// <param name="fieldName">field name</param>
        /// <param name="defaultValueIfNull">value to return if the field's value is null or unset</param>
        /// <returns>field value from the data source</returns>
        object GetFieldValue(string fieldName, object defaultValueIfNull = null);
    }
}
