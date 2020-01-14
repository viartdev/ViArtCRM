using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ViArtCRM.HelperTools {
    public static class SQLWrapper {
        //public static List<T> SelectData<T>(SQLQuerySettings sqlSettings, string[] fieldNames) {

        //}

        public static List<T> SelectData<T>(SQLSelectQuerySettings sqlSettings) where T : new() {
            List<T> dataList = new List<T>();
            using (MySqlConnection conn = new MySqlConnection(sqlSettings.ConnectionString)) {
                conn.Open();
                string queryString = sqlSettings.QueryString;
                MySqlCommand cmd = new MySqlCommand(queryString, conn);
                if (sqlSettings.QueryParams != null)
                    foreach (var param in sqlSettings.QueryParams) {
                        cmd.Parameters.AddWithValue(param.Key, param.Value);
                    }
                using (var reader = cmd.ExecuteReader()) {
                    while (reader.Read()) {
                        Type type = typeof(T);
                        PropertyInfo[] properties = type.GetProperties();
                        var dataRow = new T();
                        foreach (PropertyInfo property in properties) {
                            string fieldName = property.Name;
                            bool isDataSourceField = true;

                            var attributes = property.GetCustomAttributes(true);
                            foreach (var attr in attributes) {
                                SQLHelperAttribute sqlHelperAttribute = attr as SQLHelperAttribute;
                                if (sqlHelperAttribute != null) {
                                    if (sqlHelperAttribute.DataSourceFieldName != String.Empty)
                                        fieldName = sqlHelperAttribute.DataSourceFieldName;
                                    isDataSourceField = sqlHelperAttribute.IsDataSourceField;
                                }
                            }
                            if (isDataSourceField) {
                                dataRow.GetType().InvokeMember(property.Name,
                                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty,
                                    Type.DefaultBinder, dataRow, new[] { reader[fieldName] });
                            }
                        }
                        dataList.Add(dataRow);
                    }
                }
            }
            return dataList;
        }

        public static void InsertData<T>(T dataObject, SQLInsertQuerySettings sqlInsertQuerySettings) {
            sqlInsertQuerySettings.QueryParams = GetPropertiesDictionary<T>(dataObject);
            string queryString = sqlInsertQuerySettings.QueryString;
            using (MySqlConnection conn = new MySqlConnection(sqlInsertQuerySettings.ConnectionString)) {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(queryString, conn);
                foreach (var item in sqlInsertQuerySettings.QueryParams) {
                    cmd.Parameters.AddWithValue(String.Format("@{0}", item.Key), item.Value);
                }
                cmd.ExecuteNonQuery();
            }

        }

        public static void UpdateData<T>(T dataObject, SQLUpdateQuerySettings sqlUpdateQuerySettings) {
            sqlUpdateQuerySettings.QueryParams = GetPropertiesDictionary<T>(dataObject);
            if (sqlUpdateQuerySettings.WhereParams == null)
                sqlUpdateQuerySettings.WhereParams = GetKeyFieldParam<T>(dataObject);
            string queryString = sqlUpdateQuerySettings.QueryString;
            using (MySqlConnection conn = new MySqlConnection(sqlUpdateQuerySettings.ConnectionString)) {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(queryString, conn);
                foreach (var item in sqlUpdateQuerySettings.QueryParams) {
                    cmd.Parameters.AddWithValue(String.Format("@{0}", item.Key), item.Value);
                }             
                foreach (var item in sqlUpdateQuerySettings.WhereParams) {
                    cmd.Parameters.AddWithValue(String.Format("@{0}", item.Key), String.Format("{0}", item.Value));
                }
                cmd.ExecuteNonQuery();
            }
        }


        public static void UpdateData(SQLUpdateQuerySettings sqlUpdateQuerySettings) {
            string queryString = sqlUpdateQuerySettings.QueryString;
            using (MySqlConnection conn = new MySqlConnection(sqlUpdateQuerySettings.ConnectionString)) {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(queryString, conn);
                foreach (var item in sqlUpdateQuerySettings.QueryParams) {
                    cmd.Parameters.AddWithValue(String.Format("@{0}", item.Key), item.Value);
                }                
                foreach (var item in sqlUpdateQuerySettings.WhereParams) {
                    cmd.Parameters.AddWithValue(String.Format("@{0}", item.Key), String.Format("{0}", item.Value));
                }
                cmd.ExecuteNonQuery();
            }
        }

        static Dictionary<string, string> GetKeyFieldParam<T>(T dataObject) {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo property in properties) {
                string fieldName = property.Name;
                bool isKeyField = false;
                var attributes = property.GetCustomAttributes(true);
                foreach (var attr in attributes) {
                    SQLHelperAttribute sqlHelperAttribute = attr as SQLHelperAttribute;
                    if (sqlHelperAttribute != null) {
                        if (sqlHelperAttribute.DataSourceFieldName != String.Empty)
                            fieldName = sqlHelperAttribute.DataSourceFieldName;
                        isKeyField = sqlHelperAttribute.IsKeyField;
                    }
                }
                if (isKeyField) {
                    string fieldValue = property.GetValue(dataObject).ToString();
                    dictionary.Add(fieldName, fieldValue);
                    return dictionary;
                }
            }
            throw new Exception("No Key Fields was found in data model");
        }

        private static Dictionary<string, string> GetPropertiesDictionary<T>(T dataObject) {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo property in properties) {
                string fieldName = property.Name;
                bool isDataSourceField = true;
                bool isKeyField = false;

                var attributes = property.GetCustomAttributes(true);
                foreach (var attr in attributes) {
                    SQLHelperAttribute sqlHelperAttribute = attr as SQLHelperAttribute;
                    if (sqlHelperAttribute != null) {
                        if (sqlHelperAttribute.DataSourceFieldName != String.Empty)
                            fieldName = sqlHelperAttribute.DataSourceFieldName;
                        isDataSourceField = sqlHelperAttribute.IsDataSourceField;
                        isKeyField = sqlHelperAttribute.IsKeyField;
                    }
                }
                if (isDataSourceField && !isKeyField) {
                    string fieldValue = property.GetValue(dataObject).ToString();
                    if (property.PropertyType == typeof(DateTime)) {
                        fieldValue = ((DateTime)property.GetValue(dataObject)).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    dictionary.Add(fieldName, fieldValue);
                }
            }
            return dictionary;
        }


    }

    public abstract class SQLQuerySettings {
        public virtual string ConnectionString { get; set; }
        public virtual string TableName { get; set; }
        public virtual string QueryString { get; set; }
        public virtual Dictionary<string, string> QueryParams { get; set; }
    }
    #region SQLInsertQuerySettings
    public class SQLInsertQuerySettings : SQLQuerySettings {

        public new string QueryString { get { return GetQueryString(); } }
        private string GetQueryString() {
            string queryString = String.Empty;
            queryString = String.Format("INSERT INTO {0} ({1}) VALUES ({2}) ", TableName, GetFieldNamesString(QueryParams), GetValuesString(QueryParams));
            return queryString;
        }

        private static string GetFieldNamesString(Dictionary<string, string> queryParams) {
            string result = String.Empty;
            string[] fieldNames = queryParams.Select(s => s.Key).ToArray();
            if (fieldNames != null) {
                if (fieldNames.Length > 0) {
                    foreach (var item in fieldNames) {
                        result += String.Format("{0},", item);
                    }
                    result = result.Remove(result.Length - 1, 1);
                }
            }
            return result;
        }

        private static string GetValuesString(Dictionary<string, string> queryParams) {
            string result = String.Empty;
            string[] fieldValues = queryParams.Select(s => s.Key).ToArray();
            if (fieldValues != null) {
                if (fieldValues.Length > 0) {
                    foreach (var item in fieldValues) {
                        result += String.Format("@{0},", item);
                    }
                    result = result.Remove(result.Length - 1, 1);
                }
            }
            return result;
        }

    }
    #endregion
    #region SQLUpdateQuerySettings
    public class SQLUpdateQuerySettings : SQLQuerySettings {
        public Dictionary<string, string> WhereParams { get; set; }
        public new string QueryString { get { return GetQueryString(); } }
        private string GetQueryString() {
            string queryString = String.Empty;
            queryString = String.Format("UPDATE {0} SET {1} WHERE {2}", TableName, GetValuesString(QueryParams), GetWhereString(WhereParams));
            return queryString;
        }

        private static string GetValuesString(Dictionary<string, string> queryParams) {
            string result = String.Empty;
            foreach (var item in queryParams) {
                result += String.Format("{0}=@{1},", item.Key, item.Key);
            }
            result = result.Remove(result.Length - 1, 1);
            return result;
        }

        private static string GetWhereString(Dictionary<string, string> whereParams) {
            string result = String.Empty;
            if (whereParams != null && whereParams.Count > 0) {
                foreach (var param in whereParams) {
                    result += String.Format("{0}=@{1} and", param.Key, param.Key);
                }
                result = result.Remove(result.Length - 3, 3);
            }
            return result;
        }
    }
    #endregion
    #region SQLSelectQuerySettings
    public class SQLSelectQuerySettings : SQLQuerySettings {
        public new string QueryString { get { return GetQueryString(); } }
        public string[] FieldNames { get; set; }
        private string GetQueryString() {
            string queryString = String.Empty;
            queryString = String.Format("select {0} from {1} {2}", GetFieldNamesString(FieldNames), TableName, GetConditionsString(QueryParams));
            return queryString;
        }

        private static string GetConditionsString(Dictionary<string, string> queryParams) {
            string result = String.Empty;
            if (queryParams != null && queryParams.Count > 0) {
                result = "where ";
                foreach (var param in queryParams) {
                    result += String.Format(" {0}=@{1} and", param.Key, param.Key);
                }
                result = result.Remove(result.Length - 3, 3);
            }
            return result;
        }

        private static string GetFieldNamesString(string[] fieldNames) {
            string result = "*";
            if (fieldNames != null) {
                if (fieldNames.Length > 0) {
                    result = String.Empty;
                    foreach (var item in fieldNames) {
                        result += String.Format("{0},", item);
                    }
                    result = result.Remove(result.Length - 1, 1);
                }
            }
            return result;
        }
    }
    #endregion
    public class SQLHelperAttribute : Attribute {

        public string DataSourceFieldName { get; set; } = String.Empty;

        public bool IsDataSourceField { get; set; } = true;

        public bool IsKeyField { get; set; } = false; //To do

        public SQLHelperAttribute() {

        }
    }
}
