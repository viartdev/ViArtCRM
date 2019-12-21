using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ViArtCRM.Models {
    public class TaskMovingData {
        [DataMember(Name = "taskID")]
        public string taskID { get; set; }
        [DataMember(Name = "currentTaskStatus")]
        public string currentTaskStatus { get; set; }
    }

    public class TaskContainer {
        public int ModuleID { get; set; }
        public List<TaskObject> ToDoTasks { get; set; }
        public List<TaskObject> InModerateTasks { get; set; }
        public List<TaskObject> CompletedTasks { get; set; }

    }
    public class TaskObject {

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
            taskContainer.ModuleID = moduleID;
            taskContainer.ToDoTasks = GetTasks(0, moduleID);
            taskContainer.InModerateTasks = GetTasks(1, moduleID);
            taskContainer.CompletedTasks = GetTasks(2, moduleID);
            return taskContainer;
        }
        public TaskObject GetTaskByID(int id) {
            var tasks = GetTasks();
            return tasks.First(s => s.TaskID == id);
        }

        string GetQueryString(int taskStatus, int moduleID) {
            string qrStr;
            if (taskStatus == -1 && moduleID == -1) {
                qrStr = String.Format("select * from Tasks", taskStatus, moduleID);
            }
            else {
                qrStr = String.Format("select * from Tasks where TaskStatus = {0} and ModuleID = {1}", taskStatus, moduleID);
            }
            return qrStr;

        }
        public List<TaskObject> GetTasks(int taskStatus = -1, int moduleID = -1) {
            List<TaskObject> list = new List<TaskObject>();
            using (MySqlConnection conn = GetConnection()) {
                conn.Open();
                string queryString = GetQueryString(taskStatus, moduleID);
                MySqlCommand cmd = new MySqlCommand(queryString, conn);

                using (var reader = cmd.ExecuteReader()) {
                    while (reader.Read()) {
                        list.Add(new TaskObject() {
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
        public void InsertTask(TaskObject task) {
            using (MySqlConnection conn = GetConnection()) {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("INSERT INTO Tasks(TaskName,TaskDescription,TaskProgress,StartDate,EndDate,TaskStatus,ModuleID) VALUES(@TaskName, @TaskDescription, @TaskProgress, @StartDate, @EndDate, @TaskStatus, @ModuleID)", conn);
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
        public void UpdateTask(TaskObject task) {
            using (MySqlConnection conn = GetConnection()) {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("UPDATE Tasks SET TaskName=@TaskName,TaskDescription = @TaskDescription,TaskProgress = @TaskProgress, StartDate = @StartDate, EndDate = @EndDate,TaskStatus = @TaskStatus, ModuleID = @ModuleID WHERE TaskID = @TaskID", conn);
                cmd.Parameters.AddWithValue("@TaskID", task.TaskID);
                cmd.Parameters.AddWithValue("@TaskName", task.TaskName);
                cmd.Parameters.AddWithValue("@TaskDescription", task.TaskDescription);
                cmd.Parameters.AddWithValue("@TaskProgress", task.TaskProgress);
                cmd.Parameters.AddWithValue("@StartDate", task.StartDate);
                cmd.Parameters.AddWithValue("@EndDate", task.EndDate);
                cmd.Parameters.AddWithValue("@TaskStatus", task.Status);
                cmd.Parameters.AddWithValue("@ModuleID", task.ModuleID);
                cmd.ExecuteNonQuery();
            }
        }
        public int MoveTask(int taskID, int currentTaskStatus, int targetStatus) {
            using (MySqlConnection conn = GetConnection()) {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("UPDATE Tasks SET TaskStatus = @TaskStatus WHERE TaskID = @TaskID", conn);
                cmd.Parameters.AddWithValue("@TaskID", taskID);
                cmd.Parameters.AddWithValue("@TaskStatus", targetStatus);
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected;
            }
        }

    }

}
