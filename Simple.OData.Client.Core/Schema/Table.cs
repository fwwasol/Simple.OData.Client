using System;
using System.Collections.Generic;
using System.Linq;
using Simple.OData.Client.Extensions;

#pragma warning disable 1591

namespace Simple.OData.Client
{
    public class Table
    {
        private readonly Schema _schema;
        private readonly string _actualName;
        private readonly EdmEntityType _entityType;
        private readonly Table _baseTable;
        private readonly Lazy<TableCollection> _lazyDerivedTables;
        //private readonly Lazy<ColumnCollection> _lazyColumns;
        private readonly Lazy<Key> _lazyPrimaryKey;

        internal Table(string name, EdmEntityType entityType, Table baseTable, Schema schema)
        {
            _actualName = name;
            _entityType = entityType;
            _baseTable = baseTable;
            _schema = schema;
            _lazyDerivedTables = new Lazy<TableCollection>(GetDerivedTables);
            //_lazyColumns = new Lazy<ColumnCollection>(GetColumns);
            _lazyPrimaryKey = new Lazy<Key>(GetPrimaryKey);
        }

        public override string ToString()
        {
            return _actualName;
        }

        internal Schema Schema
        {
            get { return _schema; }
        }

        internal string HomogenizedName
        {
            get { return _actualName.Homogenize(); }
        }

        public string ActualName
        {
            get { return _actualName; }
        }

        public EdmEntityType EntityType
        {
            get { return _entityType; }
        }

        public Table BaseTable
        {
            get { return _baseTable; }
        }

        public Table FindDerivedTable(string tableName)
        {
            return _lazyDerivedTables.Value.Find(tableName);
        }

        public bool HasDerivedTable(string tableName)
        {
            return _lazyDerivedTables.Value.Contains(tableName);
        }

        public Key PrimaryKey
        {
            get { return _lazyPrimaryKey.Value; }
        }

        public IDictionary<string, object> GetKey(string tableName, IDictionary<string, object> record)
        {
            var keyNames = GetKeyNames();
            return record.Where(x => keyNames.Contains(x.Key)).ToIDictionary();
        }

        public IList<string> GetKeyNames()
        {
            return this.PrimaryKey != null && this.PrimaryKey.AsEnumerable().Any()
                       ? this.PrimaryKey.AsEnumerable().ToList()
                       : _baseTable != null
                             ? _baseTable.GetKeyNames()
                             : new string[] {};
        }

        private TableCollection GetDerivedTables()
        {
            return new TableCollection(_schema.Model.GetDerivedTables(this));
        }

        private Key GetPrimaryKey()
        {
            return _schema.Model.GetPrimaryKey(this);
        }
    }
}
