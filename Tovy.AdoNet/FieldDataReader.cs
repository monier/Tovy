using Tovy;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tovy.AdoNet
{
    /// <summary>
    /// Implementation of <see cref="IFieldReader"/> that uses <see cref="IDataReader"/> to retrieve field's infos and values
    /// </summary>
    public class FieldDataReader : IFieldReader
    {
        private readonly IDataReader _dataReader;
        /// <summary>
        /// Contains field's indexes ranged by its names
        /// </summary>
        private Dictionary<string, int> _dataIndexMap = null;
        /// <summary>
        /// Instantiates an implementation of <see cref="IFieldReader"/> that uses <see cref="IDataReader"/> to retrieve field's infos and values
        /// </summary>
        /// <param name="dataReader">ADO.NET dataReader</param>
        public FieldDataReader(IDataReader dataReader)
        {
            _dataReader = dataReader;
        }
        /// <summary>
        /// Reads next set of data fields from the data source
        /// </summary>
        /// <returns>true if there's still data to read in the data source</returns>
        public bool Read()
        {
            if (_dataReader.Read())
            {
                if (_dataIndexMap == null)
                {
                    // Creates a map that contains the field name and the field index.
                    // It seems that there's no way yet to retrieve field value only by its name.
                    _dataIndexMap = new Dictionary<string, int>();
                    for(int i = 0; i <= _dataReader.FieldCount - 1; i++)
                    {
                        _dataIndexMap.Add(_dataReader.GetName(i), i);
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns field value from the data source
        /// </summary>
        /// <param name="fieldName">field name</param>
        /// <param name="defaultValueIfNull">value to return if the field's value is null or unset</param>
        /// <returns>field value from the data source</returns>
        public object GetFieldValue(string fieldName, object defaultValueIfNull = null)
        {
            if (_dataIndexMap.ContainsKey(fieldName))
            {
                int index = _dataIndexMap[fieldName];
                if (!_dataReader.IsDBNull(index))
                    return _dataReader.GetValue(index);
                else
                    return defaultValueIfNull;
            }
            return defaultValueIfNull;
        }
    }
}
