using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ViArtCRM.HelperTools {
    public static class SQLWrapper {
        public static List<T> SelectData<T>(SQLSettings sqlSettings) where T : new() {
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
                            dataRow.GetType().InvokeMember(property.Name,
                                BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty,
                                Type.DefaultBinder, dataRow, new[] { reader[property.Name] });                           
                        }
                        dataList.Add(dataRow);
                    }
                }
            }
            return dataList;
        }
    }

    public class SQLSettings {
        public string ConnectionString { get; set; }
        public string TableName { get; set; }
        public string QueryString { get; set; }
        public Dictionary<string, string> QueryParams { get; set; }
    }
}
