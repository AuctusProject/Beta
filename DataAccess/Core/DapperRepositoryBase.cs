using Auctus.Util.DapperAttributes;
using Auctus.Util.Exceptions;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Auctus.DataAccess.Core
{
    public abstract class DapperRepositoryBase
    {
        private readonly string _connectionString;
        protected const int _defaultTimeout = 30;
        protected readonly IConfigurationRoot Configuration;
        protected SqlConnection CurrentConnection { get; private set; }
        protected IDbTransaction CurrentTransaction { get; private set; }
        protected bool TransactionOn { get; private set; }

        public abstract string TableName { get; }

        #region Constructor
        protected DapperRepositoryBase(IConfigurationRoot configuration)
        {
            Configuration = configuration;
            _connectionString = configuration.GetSection("ConnectionString:SQL").Get<string>();
        }
        #endregion

        #region Connection
        protected SqlConnection CreateConnection()
        {
            var connection = new SqlConnection(_connectionString);
            connection.Open();
            return connection;
        }

        protected void SetDapperTransaction(SqlConnection connection, IDbTransaction transaction)
        {
            CurrentConnection = connection;
            CurrentTransaction = transaction;
            TransactionOn = true;
        }

        protected void ClearDapperTransaction()
        {
            TransactionOn = false;
            CurrentConnection = null;
            CurrentTransaction = null;
        }
        #endregion

        #region Query
        protected IEnumerable<T> Query<T>(string sql, dynamic param = null, int commandTimeout = _defaultTimeout)
        {
            if (TransactionOn)
                return SqlMapper.Query<T>(CurrentConnection, sql, param, CurrentTransaction, true, commandTimeout, CommandType.Text);
            else
            {
                using (var connection = CreateConnection())
                {
                    return SqlMapper.Query<T>(connection, sql, param, null, true, commandTimeout, CommandType.Text);
                }
            }
        }

        protected IEnumerable<IDictionary<string, object>> Query(string sql, dynamic param = null, int commandTimeout = _defaultTimeout)
        {
            if (TransactionOn)
                return SqlMapper.Query(CurrentConnection, sql, param, CurrentTransaction, true, commandTimeout, CommandType.Text);
            else
            {
                using (var connection = CreateConnection())
                {
                    return SqlMapper.Query(connection, sql, param, null, true, commandTimeout, CommandType.Text);
                }
            }
        }

        protected IEnumerable<TParent> QueryParentChild<TParent, TChild, TParentKey>(string sql, Func<TParent, TParentKey> parentKeySelector,
            Func<TParent, IList<TChild>> childSelector, string splitOn, dynamic param = null, int commandTimeout = _defaultTimeout)
        {
            Dictionary<TParentKey, TParent> cache = new Dictionary<TParentKey, TParent>();
            if (TransactionOn)
            {
                CurrentConnection.Query<TParent, TChild, TParent>(sql,
                (parent, child) =>
                {
                    return HandleQueryParentChild<TParent, TChild, TParentKey>(ref cache, parentKeySelector, childSelector, parent, child);
                },
                param as object, CurrentTransaction, true, splitOn, commandTimeout, CommandType.Text);
            }
            else
            {
                using (var connection = CreateConnection())
                {
                    connection.Query<TParent, TChild, TParent>(sql,
                    (parent, child) =>
                    {
                        return HandleQueryParentChild<TParent, TChild, TParentKey>(ref cache, parentKeySelector, childSelector, parent, child);
                    },
                    param as object, null, true, splitOn, commandTimeout, CommandType.Text);
                }
            }
            return cache.Values;
        }

        private TParent HandleQueryParentChild<TParent, TChild, TParentKey>(ref Dictionary<TParentKey, TParent> cache, Func<TParent, TParentKey> parentKeySelector,
            Func<TParent, IList<TChild>> childSelector, TParent parent, TChild child)
        {
            if (!cache.ContainsKey(parentKeySelector(parent)))
                cache.Add(parentKeySelector(parent), parent);

            TParent cachedParent = cache[parentKeySelector(parent)];
            if (child != null)
            {
                IList<TChild> children = childSelector(cachedParent);
                children.Add(child);
            }
            return cachedParent;
        }

        protected SqlMapper.GridReader QueryMultiple(string sql, dynamic param = null, int commandTimeout = _defaultTimeout)
        {
            if (TransactionOn)
                return SqlMapper.QueryMultiple(CurrentConnection, sql, param, CurrentTransaction, commandTimeout, CommandType.Text);
            else
            {
                using (var connection = CreateConnection())
                {
                    return SqlMapper.QueryMultiple(connection, sql, param, null, commandTimeout, CommandType.Text);
                }
            }
        }

        protected IEnumerable<TReturn> Query<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, string splitOn, dynamic param = null, int commandTimeout = _defaultTimeout)
        {
            if (TransactionOn)
                return SqlMapper.Query<TFirst, TSecond, TReturn>(CurrentConnection, sql, map, param, CurrentTransaction, true, splitOn, commandTimeout, CommandType.Text);
            else
            {
                using (var connection = CreateConnection())
                {
                    return SqlMapper.Query<TFirst, TSecond, TReturn>(connection, sql, map, param, null, true, splitOn, commandTimeout, CommandType.Text);
                }
            }
        }

        protected IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TReturn>(string sql, Func<TFirst, TSecond, TThird, TReturn> map, string splitOn, dynamic param = null, int commandTimeout = _defaultTimeout)
        {
            if (TransactionOn)
                return SqlMapper.Query<TFirst, TSecond, TThird, TReturn>(CurrentConnection, sql, map, param, CurrentTransaction, true, splitOn, commandTimeout, CommandType.Text);
            else
            {
                using (var connection = CreateConnection())
                {
                    return SqlMapper.Query<TFirst, TSecond, TThird, TReturn>(connection, sql, map, param, null, true, splitOn, commandTimeout, CommandType.Text);
                }
            }
        }

        protected IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, string splitOn, dynamic param = null, int commandTimeout = _defaultTimeout)
        {
            if (TransactionOn)
                return SqlMapper.Query<TFirst, TSecond, TThird, TFourth, TReturn>(CurrentConnection, sql, map, param, CurrentTransaction, true, splitOn, commandTimeout, CommandType.Text);
            else
            {
                using (var connection = CreateConnection())
                {
                    return SqlMapper.Query<TFirst, TSecond, TThird, TFourth, TReturn>(connection, sql, map, param, null, true, splitOn, commandTimeout, CommandType.Text);
                }
            }
        }

        protected IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, string splitOn, dynamic param = null, int commandTimeout = _defaultTimeout)
        {
            if (TransactionOn)
                return SqlMapper.Query<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(CurrentConnection, sql, map, param, CurrentTransaction, true, splitOn, commandTimeout, CommandType.Text);
            else
            {
                using (var connection = CreateConnection())
                {
                    return SqlMapper.Query<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(connection, sql, map, param, null, true, splitOn, commandTimeout, CommandType.Text);
                }
            }
        }

        protected IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map, string splitOn, dynamic param = null, int commandTimeout = _defaultTimeout)
        {
            if (TransactionOn)
                return SqlMapper.Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(CurrentConnection, sql, map, param, CurrentTransaction, true, splitOn, commandTimeout, CommandType.Text);
            else
            {
                using (var connection = CreateConnection())
                {
                    return SqlMapper.Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(connection, sql, map, param, null, true, splitOn, commandTimeout, CommandType.Text);
                }
            }
        }

        protected IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map, string splitOn, dynamic param = null, int commandTimeout = _defaultTimeout)
        {
            if (TransactionOn)
                return SqlMapper.Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(CurrentConnection, sql, map, param, CurrentTransaction, true, splitOn, commandTimeout, CommandType.Text);
            else
            {
                using (var connection = CreateConnection())
                {
                    return SqlMapper.Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(connection, sql, map, param, null, true, splitOn, commandTimeout, CommandType.Text);
                }
            }
        }
        #endregion

        #region Execute
        protected virtual int Execute(string sql, dynamic param = null, int commandTimeout = _defaultTimeout, IDbTransaction transaction = null)
        {
            using (var connection = CreateConnection())
            {
                return SqlMapper.Execute(connection, sql, param, transaction, commandTimeout, CommandType.Text);
            }
        }

        protected virtual int ExecuteReturningIdentity(string sql, dynamic param = null, int commandTimeout = _defaultTimeout, IDbTransaction transaction = null)
        {
            using (var connection = CreateConnection())
            {
                return SqlMapper.ExecuteScalar<int>(connection, ParseIdentityCommandQuery(sql), param, transaction, commandTimeout, CommandType.Text);
            }
        }

        protected string ParseIdentityCommandQuery(string sql)
        {
            //return string.Concat(sql, "; SELECT LAST_INSERT_ID();");
            return string.Concat(sql, "; SELECT SCOPE_IDENTITY();");
        }
        #endregion

        protected IEnumerable<T> SelectAll<T>()
        {
            return Select<T>(null, null);
        }

        public IEnumerable<T> SelectByParameters<T>(DynamicParameters criteria, String orderBy = "")
        {
            string pairs = null;
            if (criteria != null && criteria.ParameterNames.Any())
                pairs = GetSqlPairs(criteria.ParameterNames, " AND ");

            return Select<T>(pairs, criteria, orderBy);
        }

        private IEnumerable<T> Select<T>(string pairs, DynamicParameters criteria, String orderBy = "")
        {
            string sql = string.Format("SELECT * FROM [{0}] WITH(NOLOCK) ", TableName);
            if (!string.IsNullOrEmpty(pairs))
                sql += " WHERE " + pairs;

            if (!string.IsNullOrEmpty(orderBy))
                sql += " ORDER BY " + orderBy;

            return Query<T>(sql, criteria);
        }

        protected void Insert<T>(T obj, string tableName = null)
        {
            PropertyContainer propertyContainer = ParseProperties(obj);
            bool identity = !string.IsNullOrEmpty(propertyContainer.Identity);
            IEnumerable<string> columns = identity ? propertyContainer.ValueNames : propertyContainer.AllNames;
            var sql = string.Format("INSERT INTO [{0}] ({1}) VALUES(@{2})",
                tableName ?? TableName,
                string.Join(", ", columns.Select(item => string.Format("[{0}]", item))),
                string.Join(", @", columns));

            if (identity)
            {
                int id = ExecuteReturningIdentity(sql, propertyContainer.ValueParameters);
                SetIdentity<T>(obj, id, propertyContainer.Identity);
            }
            else
                Execute(sql, propertyContainer.AllParameters);
        }

        protected int Update<T>(T obj, string tableName = null, bool isTransactional = false)
        {
            PropertyContainer propertyContainer = ParseProperties(obj);
            string sqlKeyPairs = GetSqlPairs(propertyContainer.KeyNames, " AND ");
            string sqlValuePairs = GetSqlPairs(propertyContainer.ValueNames, ", ");

            string sqlCriticalRestrictionPairs = GetSqlCriticalRestrictionPairs(propertyContainer.CriticalRestrictions);
            if (!string.IsNullOrEmpty(sqlCriticalRestrictionPairs))
                sqlKeyPairs += " AND " + sqlCriticalRestrictionPairs;

            string sql = string.Format("UPDATE [{0}] SET {1} WHERE {2} ", tableName ?? TableName, sqlValuePairs, sqlKeyPairs);
            int rowsAffected = Execute(sql, propertyContainer.AllParameters);

            if (isTransactional && rowsAffected == 0 && !string.IsNullOrEmpty(sqlCriticalRestrictionPairs))
                throw new InvalidOperationException($"Invalid update on [{tableName}] with critical restriction and 0 rows affected.");

            return rowsAffected;
        }

        protected int Delete<T>(T obj, string tableName = null, bool isTransactional = false)
        {
            PropertyContainer propertyContainer = ParseProperties(obj);
            string sqlKeyPairs = GetSqlPairs(propertyContainer.KeyNames, " AND ");

            string sqlCriticalRestrictionPairs = GetSqlCriticalRestrictionPairs(propertyContainer.CriticalRestrictions);
            if (!string.IsNullOrEmpty(sqlCriticalRestrictionPairs))
                sqlKeyPairs += " AND " + sqlCriticalRestrictionPairs;

            string sql = string.Format("DELETE FROM [{0}] WHERE {1} ", tableName ?? TableName, sqlKeyPairs);
            int rowsAffected = Execute(sql, propertyContainer.KeyWithCriticalsParameters);

            if (isTransactional && rowsAffected == 0 && !string.IsNullOrEmpty(sqlCriticalRestrictionPairs))
                throw new InvalidOperationException($"Invalid delete on [{tableName}] with critical restriction and 0 rows affected.");

            return rowsAffected;
        }

        private static PropertyContainer ParseProperties<T>(T obj)
        {
            PropertyContainer propertyContainer = new PropertyContainer();
            IEnumerable<PropertyInfo> properties = typeof(T).GetProperties().Where(c => c.DeclaringType == typeof(T) || c.IsDefined(typeof(DapperKeyAttribute), false));
            foreach (PropertyInfo property in properties)
            {
                // Skip reference types (but still include string!)
                if (property.PropertyType.GetTypeInfo().IsInterface || (property.PropertyType.GetTypeInfo().IsClass && property.PropertyType != typeof(string)))
                    continue;

                // Skip methods without a public setter
                if (property.GetSetMethod() == null)
                    continue;

                // Skip methods without db type defined
                if (!property.IsDefined(typeof(DapperTypeAttribute), false))
                    continue;

                object value = typeof(T).GetProperty(property.Name).GetValue(obj, null);
                DbType dbType = ((DapperTypeAttribute)property.GetCustomAttribute(typeof(DapperTypeAttribute))).DbType;
                bool isKey = property.IsDefined(typeof(DapperKeyAttribute), false);
                bool isCriticalCommand = property.IsDefined(typeof(DapperCriticalRestrictionAttribute), false);

                if (isKey && isCriticalCommand)
                    throw new InvalidOperationException("Critical restriction cannot be set on Key.");

                if (isKey)
                    propertyContainer.AddKey(property.Name, value, dbType, ((DapperKeyAttribute)property.GetCustomAttribute(typeof(DapperKeyAttribute))).Identity);
                else
                    propertyContainer.AddValue(property.Name, value, dbType, isCriticalCommand);

                if (isCriticalCommand)
                    propertyContainer.AddCriticalRestriction(property.Name, ((DapperCriticalRestrictionAttribute)property.GetCustomAttribute(typeof(DapperCriticalRestrictionAttribute))).SqlOperation);
            }
            return propertyContainer;
        }

        private static string GetSqlPairs(IEnumerable<string> keys, string separator)
        {
            return string.Join(separator, keys.Select(key => string.Format("[{0}]=@{0}", key)));
        }

        private static string GetSqlCriticalRestrictionPairs(Dictionary<string, string> criticalRestrictions)
        {
            string sql = "";
            foreach (var pair in criticalRestrictions)
                sql += string.Format("[{0}]{1}@{0}", pair.Key, pair.Value);
            return sql;
        }

        private void SetIdentity<T>(T obj, int id, string identity)
        {
            if (!string.IsNullOrEmpty(identity))
            {
                PropertyInfo propertyInfo = obj.GetType().GetProperty(identity);
                if (propertyInfo.PropertyType == typeof(int))
                {
                    propertyInfo.SetValue(obj, id, null);
                }
            }
        }

        private class PropertyContainer
        {
            private string _identity;
            private readonly DynamicParameters _keyParameters;
            private readonly DynamicParameters _valueParameters;
            private readonly DynamicParameters _allParameters;
            private readonly DynamicParameters _keyWithCriticalsParameters;
            private readonly Dictionary<string, string> _criticalRestrictions;

            internal PropertyContainer()
            {
                _keyParameters = new DynamicParameters();
                _valueParameters = new DynamicParameters();
                _allParameters = new DynamicParameters();
                _keyWithCriticalsParameters = new DynamicParameters();
                _criticalRestrictions = new Dictionary<string, string>();
            }

            internal string Identity
            {
                get { return _identity; }
            }

            internal IEnumerable<string> KeyNames
            {
                get { return _keyParameters.ParameterNames; }
            }

            internal IEnumerable<string> ValueNames
            {
                get { return _valueParameters.ParameterNames; }
            }

            internal IEnumerable<string> AllNames
            {
                get { return _allParameters.ParameterNames; }
            }

            internal DynamicParameters KeyParameters
            {
                get { return _keyParameters; }
            }

            internal DynamicParameters ValueParameters
            {
                get { return _valueParameters; }
            }

            internal DynamicParameters AllParameters
            {
                get { return _allParameters; }
            }

            internal DynamicParameters KeyWithCriticalsParameters
            {
                get { return _keyWithCriticalsParameters; }
            }

            internal Dictionary<string, string> CriticalRestrictions
            {
                get { return _criticalRestrictions; }
            }

            internal void AddKey(string name, object value, DbType type, bool identity)
            {
                ulong id;
                if (identity && (value == null || (ulong.TryParse(value.ToString(), out id) && id == 0)))
                {
                    if (!string.IsNullOrEmpty(_identity))
                        throw new Exception("Incorrect identity parameter.");
                    _identity = name;
                }
                Add(_keyParameters, name, value, type);
                Add(_allParameters, name, value, type);
                Add(_keyWithCriticalsParameters, name, value, type);
            }

            internal void AddValue(string name, object value, DbType type, bool isCriticalRestriction)
            {
                Add(_valueParameters, name, value, type);
                Add(_allParameters, name, value, type);
                if (isCriticalRestriction)
                    Add(_keyWithCriticalsParameters, name, value, type);
            }

            internal void AddCriticalRestriction(string name, string commandOperantion)
            {
                if (_criticalRestrictions.ContainsKey(name))
                    throw new InvalidOperationException($"Duplicate critical restriction on {name}.");
                _criticalRestrictions.Add(name, commandOperantion);
            }

            private void Add(DynamicParameters param, string name, object value, DbType type)
            {
                param.Add(name, value, type);
            }
        }
    }
}