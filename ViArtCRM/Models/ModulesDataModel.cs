using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ViArtCRM.Models
{
    public class TaskModule
    {
        public int ModuleID { get; set; }

        public string ModuleName { get; set; }

        public string ModuleDescription { get; set; }
    }
    public class TaskModuleContext {
        public string ConnectionString { get; set; }

        public TaskModuleContext(string connectionString) {
            this.ConnectionString = connectionString;
        }

        private MySqlConnection GetConnection() {
            return new MySqlConnection(ConnectionString);
        }

        // CRUD Methods      

        public List<TaskModule> GetModules() {
            List<TaskModule> list = new List<TaskModule>();
            using (MySqlConnection conn = GetConnection()) {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(String.Format("select * from Modules"), conn);

                using (var reader = cmd.ExecuteReader()) {
                    while (reader.Read()) {
                        list.Add(new TaskModule() {
                            ModuleID = Convert.ToInt32(reader["ModuleID"]),
                            ModuleName = reader["ModuleName"].ToString(),
                            ModuleDescription = reader["ModuleDescription"].ToString()                           
                        });
                    }
                }
            }
            return list;
        }
        public void InsertModule(TaskModule taskModule) {
            using (MySqlConnection conn = GetConnection()) {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("INSERT INTO Modules(ModuleName,ModuleDescription) VALUES(@ModuleName, @ModuleDescription)", conn);
                cmd.Parameters.AddWithValue("@ModuleName", taskModule.ModuleName);
                cmd.Parameters.AddWithValue("@ModuleDescription", taskModule.ModuleDescription);             
                cmd.ExecuteNonQuery();
            }
        }
    }
}
