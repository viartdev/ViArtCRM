using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ViArtCRM.Models {
    public class TaskContainer {
        public List<Task> ToDoTasks { get; set; }
        public List<Task> InModerateTasks { get; set; }
        public List<Task> CompletedTasks { get; set; }

    }
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

        public int ModuleID { get; set; }
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

        public TaskContainer GetTaskContainer(int moduleID) {
            TaskContainer taskContainer = new TaskContainer();
            taskContainer.ToDoTasks = GetTasks(0, moduleID);
            taskContainer.InModerateTasks = GetTasks(1, moduleID);
            taskContainer.CompletedTasks = GetTasks(2, moduleID);
            return taskContainer;
        }

        private List<Task> GetTasks(int taskStatus, int moduleID) {
            List<Task> list = new List<Task>();
            using (MySqlConnection conn = GetConnection()) {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(String.Format("select * from Tasks where TaskStatus = {0} and ModuleID = {1}", taskStatus, moduleID), conn);

                using (var reader = cmd.ExecuteReader()) {
                    while (reader.Read()) {
                        list.Add(new Task() {
                            TaskID = Convert.ToInt32(reader["TaskID"]),
                            TaskName = reader["TaskName"].ToString(),
                            TaskDescription = reader["TaskDescription"].ToString(),
                            TaskProgress = Convert.ToInt32(reader["TaskProgress"]),
                            StartDate = Convert.ToDateTime(reader["StartDate"]),
                            EndDate = Convert.ToDateTime(reader["EndDate"]),
                            Status = Convert.ToInt32(reader["TaskStatus"]),
                            ModuleID = Convert.ToInt32(reader["ModuleID"])
                        });
                    }
                }
            }
            return list;
        }
        public void InsertTask(Task task) {
            using (MySqlConnection conn = GetConnection()) {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("INSERT INTO Tasks(TaskName,TaskDescription,TaskProgress,StartDate,EndDate,TaskStatus) VALUES(@TaskName, @TaskDescription, @TaskProgress, @StartDate, @EndDate, @TaskStatus)", conn);
                cmd.Parameters.AddWithValue("@TaskName", task.TaskName);
                cmd.Parameters.AddWithValue("@TaskDescription", task.TaskDescription);
                cmd.Parameters.AddWithValue("@TaskProgress", 0);
                cmd.Parameters.AddWithValue("@StartDate", task.StartDate);
                cmd.Parameters.AddWithValue("@EndDate", task.EndDate);
                cmd.Parameters.AddWithValue("@TaskStatus", 0);
                cmd.Parameters.AddWithValue("@ModuleID", task.ModuleID);
                cmd.ExecuteNonQuery();
            }
        }

    }

}
