using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace ViArtCRM.Helpers {
    public abstract class SQLContext {
        //    public virtual string ConnectionString { get; set; }
        //    public virtual string TableName { get; set; }   

        //    public SQLContext(string connectionString) {
        //        this.ConnectionString = connectionString;
        //    }
        //    private MySqlConnection GetConnection() {
        //        return new MySqlConnection(ConnectionString);
        //    }
        //    public virtual List<object> GetData() {
        //        List<object> list = new List<object>();
        //        using (MySqlConnection conn = GetConnection()) {
        //            conn.Open();

        //            MySqlCommand cmd = new MySqlCommand(String.Format("select * from {0}", TableName), conn);

        //            using (var reader = cmd.ExecuteReader()) {
        //                while (reader.Read()) {                                                                        

        //                    foreach (PropertyInfo propertyInfo in instance.GetProperties()) {
        //                        if (propertyInfo.CanRead) {
        //                            object firstValue = propertyInfo.GetValue(first, null);
        //                            object secondValue = propertyInfo.GetValue(second, null);
        //                            if (!object.Equals(firstValue, secondValue)) {
        //                                return false;
        //                            }
        //                        }
        //                    }


        //                    //list.Add(new Task() {
        //                    //    TaskID = Convert.ToInt32(reader["TaskID"]),
        //                    //    TaskName = reader["TaskName"].ToString(),
        //                    //    TaskDescription = reader["TaskDescription"].ToString(),
        //                    //    TaskProgress = Convert.ToInt32(reader["TaskProgress"]),
        //                    //    StartDate = Convert.ToDateTime(reader["StartDate"]),
        //                    //    EndDate = Convert.ToDateTime(reader["EndDate"]),
        //                    //    Status = Convert.ToInt32(reader["TaskStatus"])
        //                    //});
        //                }
        //            }
        //        }
        //        return list;
        //    }
    }
}
