using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using ViArtCRM.HelperTools;

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

        public int TaskID { get; set; }

        public string TaskName { get; set; }

        public string TaskDescription { get; set; }

        public int TaskProgress { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        [SQLHelper(IsDataSourceField = false)]
        public bool IsComplete {
            get {
                return TaskProgress >= 100;
            }
        }
        [SQLHelper(DataSourceFieldName = "TaskStatus")]
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
            List<TaskObject> list = new List<TaskObject>();
            SQLSelectQuerySettings sqlSelectQuerySettings = new SQLSelectQuerySettings {
                ConnectionString = this.ConnectionString,
                TableName = "Tasks",
                QueryParams = new Dictionary<string, string>() { { "TaskID", id.ToString() } }
            };

            list = SQLWrapper.SelectData<TaskObject>(sqlSelectQuerySettings);

            return list.First(s => s.TaskID == id);
        }

        Dictionary<string, string> GetQueryParams(int taskStatus, int moduleID) {
            if (moduleID == -1 && taskStatus == -1)
                return null;
            var queryParams = new Dictionary<string, string>();
            if (taskStatus != -1)
                queryParams.Add("TaskStatus", taskStatus.ToString());
            if (moduleID != -1)
                queryParams.Add("ModuleID", moduleID.ToString());
            return queryParams;
        }
        public List<TaskObject> GetTasks(int taskStatus = -1, int moduleID = -1) {
            List<TaskObject> list = new List<TaskObject>();
            SQLSelectQuerySettings sqlSelectQuerySettings = new SQLSelectQuerySettings {
                ConnectionString = this.ConnectionString,
                TableName = "Tasks",
                QueryParams = GetQueryParams(taskStatus, moduleID)
            };

            list = SQLWrapper.SelectData<TaskObject>(sqlSelectQuerySettings);
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
