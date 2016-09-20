using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tovy
{
    /// <summary>
    /// Service that helps to map any compatible data source fields to an entity having properties decorated with <see cref="FieldAttribute"/>.
    /// </summary>
    public interface IFieldMapper
    {
        /// <summary>
        /// Maps the provided fields data to a list of entity
        /// </summary>
        /// <typeparam name="T">type of the entity</typeparam>
        /// <param name="dataReader">reader of the [external] data source</param>
        /// <returns>list of mapped entities</returns>
        IEnumerable<T> Map<T>(IFieldReader dataReader) where T : new();
    }
}
