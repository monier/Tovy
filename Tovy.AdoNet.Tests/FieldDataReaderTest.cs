using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tovy;
using Tovy.AdoNet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tovy.Extended;

namespace Tovy.AdoNet.Tests
{
    [TestClass]
    public class FieldDataReaderTest
    {
        private const string Category = "Tovy.AdoNet";

        public TestContext TestContext { get; set; }

        [TestMethod]
        [TestCategory(Category)]
        public void Map()
        {
            DataTable dt = new DataTable();
            DataRow row = null;
            IDataReader dataReader = null;
            IFieldMapper fieldMapper = new FieldMapper();

            dt.Columns.Add(new DataColumn("id", typeof(int)));
            dt.Columns.Add(new DataColumn("name", typeof(string)));
            dt.Columns.Add(new DataColumn("prop01", typeof(int)));
            dt.Columns.Add(new DataColumn("unset"));
            row = dt.NewRow();
            row["id"] = 5;
            row["name"] = "idIs5";
            row["prop01"] = DBNull.Value;
            dt.Rows.Add(row);
            row = dt.NewRow();
            row["id"] = DBNull.Value;
            row["name"] = "idWithDBNull";
            row["prop01"] = 2;
            dt.Rows.Add(row);
            dataReader = dt.CreateDataReader();
            var entities = fieldMapper.Map<MyEntity01>(new FieldDataReader(dataReader)).ToList();
            Assert.IsTrue(entities.Count == 2, "All entities are created");
            Assert.IsTrue(entities[0].Id == 5, "Field mapping of int is correct");
            Assert.IsTrue(entities[0].Name == "idIs5", "Field mapping of string is correct");
            Assert.IsTrue(entities[0].Prop01 == 99, "Field custom default value is set");
            Assert.IsTrue(entities[0].Unset == default(string), "Field decorated but not returned by the data source is mapped with type's default value");
            Assert.IsTrue(entities[0].UnsetWithDefaultValue == "unset", "Field decorated but not returned by the data source is mapped with field's default value");
            Assert.IsTrue(entities[0].IgnoreDecorated == default(string), "Field decorated with [Ignore] is ignored");
            Assert.IsTrue(entities[0].IgnoreNotDecorated == default(string), "Field not decorated with [Ignore] but not returned by the data source is ignored");
            Assert.IsTrue(entities[1].Id == default(int), "Field mapping of int with DBNull value is correct");
            Assert.IsTrue(entities[1].Prop01 == 2, "Field with default value is mapped with its' correct value if provided by the data source");
        }

        [TestMethod]
        [TestCategory(Category)]
        public void MapUsingFieldPrefix()
        {
            DataTable dt = new DataTable();
            DataRow row = null;
            IDataReader dataReader = null;
            IFieldMapper fieldMapper = new FieldMapper();

            dt.Columns.Add(new DataColumn("pref.id", typeof(int)));
            dt.Columns.Add(new DataColumn("pref.name", typeof(string)));
            dt.Columns.Add(new DataColumn("pref.prop01", typeof(int)));
            dt.Columns.Add(new DataColumn("pref.unset"));
            row = dt.NewRow();
            row["pref.id"] = 5;
            row["pref.name"] = "idIs5";
            row["pref.prop01"] = DBNull.Value;
            dt.Rows.Add(row);
            dataReader = dt.CreateDataReader();
            var entities = fieldMapper.Map<MyEntity02>(new FieldDataReader(dataReader)).ToList();
            Assert.IsTrue(entities.Count == 1, "Entity is created");
            Assert.IsTrue(entities[0].Id == 5, "Int field with prefix is succesfully mapped");
            Assert.IsTrue(entities[0].Name == "idIs5", "String field with prefix is succesfully mapped");
        }

        [TestMethod]
        [TestCategory(Category)]
        public void MapMassive()
        {
            DataTable dt = new DataTable();
            IDataReader dataReader = null;
            DataRow row = null;
            IFieldMapper fieldMapper = new FieldMapper();

            dt.Columns.Add(new DataColumn("id", typeof(int)));
            dt.Columns.Add(new DataColumn("name", typeof(string)));
            dt.Columns.Add(new DataColumn("prop01", typeof(int)));
            dt.Columns.Add(new DataColumn("unset"));

            for(int i = 0; i <= 5000; i++)
            {
                row = dt.NewRow();
                row["id"] = i;
                row["name"] = string.Format("name_{0}", i);
                row["prop01"] = i % 2;
                dt.Rows.Add(row);
            }
            dataReader = dt.CreateDataReader();
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var entities = fieldMapper.Map<MyEntity01>(new FieldDataReader(dataReader)).ToList();
            watch.Stop();
            TestContext.WriteLine("Elapsed: {0}", watch.ElapsedMilliseconds);
        }

        [TestMethod]
        [TestCategory(Category)]
        public void MassiveComparePortableAndExtended()
        {
            DataTable dt = new DataTable();
            DataRow row = null;
            IFieldMapper fieldMapper = null;

            dt.Columns.Add(new DataColumn("id", typeof(int)));
            dt.Columns.Add(new DataColumn("name", typeof(string)));
            dt.Columns.Add(new DataColumn("prop01", typeof(int)));
            dt.Columns.Add(new DataColumn("unset"));

            for (int i = 0; i <= 5000; i++)
            {
                row = dt.NewRow();
                row["id"] = i;
                row["name"] = string.Format("name_{0}", i);
                row["prop01"] = i % 2;
                dt.Rows.Add(row);
            }
            using (IDataReader dataReader = dt.CreateDataReader())
            {
                fieldMapper = new FieldMapper();
                Stopwatch watchPortable = new Stopwatch();
                watchPortable.Start();
                var entitiesPortable = fieldMapper.Map<MyEntity01>(new FieldDataReader(dataReader)).ToList();
                watchPortable.Stop();
                TestContext.WriteLine("Elapsed Portable: {0}", watchPortable.ElapsedMilliseconds);
            }

            using (IDataReader dataReader = dt.CreateDataReader())
            {
                fieldMapper = new FastFieldMapper();
                Stopwatch watchExtended = new Stopwatch();
                watchExtended.Start();
                var entitiesExtended = fieldMapper.Map<MyEntity01>(new FieldDataReader(dataReader)).ToList();
                watchExtended.Stop();
                TestContext.WriteLine("Elapsed Extended: {0}", watchExtended.ElapsedMilliseconds);
            }
        }

        private class MyEntity01
        {
            [Field("id")]
            public int Id { get; set; }
            [Field("name")]
            public string Name { get; set; }
            [Field("prop01", DefaultValueIfNull = 99)]
            public int Prop01 { get; set; }
            [Field("unset")]
            public string Unset { get; set; }
            [Field("unsetDef", DefaultValueIfNull ="unset")]
            public string UnsetWithDefaultValue { get; set; }
            [IgnoreField]
            public string IgnoreDecorated { get; set; }
            public string IgnoreNotDecorated { get; set; }
        }
        [FieldPrefix("pref.")]
        private class MyEntity02 : MyEntity01
        {
        }
    }
}
