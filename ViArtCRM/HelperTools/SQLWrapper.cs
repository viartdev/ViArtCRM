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
    }

    public abstract class SQLQuerySettings {
        public virtual string ConnectionString { get; set; }
        public virtual string TableName { get; set; }
        public virtual string QueryString { get; set; }
        public virtual Dictionary<string, string> QueryParams { get; set; }
    }
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
                result = "where";
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

    public class SQLHelperAttribute : Attribute {

        public string DataSourceFieldName { get; set; } = String.Empty;

        public bool IsDataSourceField { get; set; } = true;

        public SQLHelperAttribute() {

        }
    }
}
