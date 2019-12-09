using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ViArtCRM.Models {
    public class Task {

        private TasksContext context;

        public int TaskID { get; set; }

        public string TaskName { get; set; }

        public string TaskDescription { get; set; }

        public int TaskProgress { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsComplete {
            get {
                return TaskProgress >= 100;
            }
        }

        public int Status { get; set; }
    }

    public class TasksContext {
        public string ConnectionString { get; set; }

        public TasksContext(string connectionString) {
            this.ConnectionString = connectionString;
        }

        private MySqlConnection GetConnection() {
            return new MySqlConnection(ConnectionString);
        }

        // CRUD Methods

        public List<Task> GetTasks() {
            List<Task> list = new List<Task>();

            using (MySqlConnection conn = GetConnection()) {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("select * from Tasks", conn);

                using (var reader = cmd.ExecuteReader()) {
                    while (reader.Read()) {
                        list.Add(new Task() {
                            TaskID= Convert.ToInt32(reader["TaskID"]),
                            TaskName = reader["TaskName"].ToString(),
                            TaskDescription = reader["TaskDescription"].ToString(),
                            TaskProgress = Convert.ToInt32(reader["TaskProgress"]),
                            StartDate = Convert.ToDateTime(reader["StartDate"]),
                            EndDate = Convert.ToDateTime(reader["EndDate"]),
                            Status = Convert.ToInt32(reader["TaskStatus"])
                        });
                    }
                }
            }
            return list;
        }

    }

}
