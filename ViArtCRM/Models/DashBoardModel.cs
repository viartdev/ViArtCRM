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

    public class SubTaskMovingData
    {
        [DataMember(Name = "taskID")]
        public string taskID { get; set; }
        [DataMember(Name = "SubTaskName")]
        public string SubTaskName { get; set; }
    }


    public class TaskContainer {
        public int ModuleID { get; set; }
        public List<TaskObject> ToDoTasks { get; set; }
        public List<TaskObject> InModerateTasks { get; set; }
        public List<TaskObject> CompletedTasks { get; set; }

    }
    public class TaskObject {
        [SQLHelper(IsKeyField = true)]
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

    public class SubTask {
        public int SubTaskID { get; set; }

        public string SubTaskName { get; set; }

        public int isComplete { get; set; }

        public int TaskID { get; set; }
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
        Dictionary<string, string> GetQueryParamsSubTasks(int taskID) {
            if (taskID == -1)
                return null;
            var queryParams = new Dictionary<string, string>();
            if (taskID != -1)
                queryParams.Add("TaskID", taskID.ToString());

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
        public List<SubTask> GetSubTasks(int taskID = -1) {
            List<SubTask> list = new List<SubTask>();
            SQLSelectQuerySettings sqlSelectQuerySettings = new SQLSelectQuerySettings {
                ConnectionString = this.ConnectionString,
                TableName = "SubTasks",
                QueryParams = GetQueryParamsSubTasks(taskID)
            };

            list = SQLWrapper.SelectData<SubTask>(sqlSelectQuerySettings);
            return list;
        }

        public void UpdateSubTask (int taskID, string subTaskName)
        {
            SubTask subTask = new SubTask();
            subTask.isComplete = 0;
            subTask.SubTaskName = subTaskName;
            subTask.TaskID = taskID;
            SQLInsertQuerySettings sqlInsertQuerySettings = new SQLInsertQuerySettings()
            {
                TableName = "SubTasks",
                ConnectionString = this.ConnectionString
            };
            SQLWrapper.InsertData<SubTask>(subTask, sqlInsertQuerySettings);
        }
    

        public void InsertTask(TaskObject task) {
            SQLInsertQuerySettings sqlInsertQuerySettings = new SQLInsertQuerySettings() {
                TableName = "Tasks",
                ConnectionString = this.ConnectionString
            };
            SQLWrapper.InsertData<TaskObject>(task, sqlInsertQuerySettings);
        }
        public void UpdateTask(TaskObject task) {
            SQLUpdateQuerySettings sqlUpdateQuerySettings = new SQLUpdateQuerySettings() {
                ConnectionString = this.ConnectionString,
                TableName = "Tasks"
            };
            SQLWrapper.UpdateData<TaskObject>(task, sqlUpdateQuerySettings);
        }
        public void MoveTask(int taskID, int currentTaskStatus, int targetStatus) {
            SQLUpdateQuerySettings sqlUpdateQuerySettings = new SQLUpdateQuerySettings {
                ConnectionString = this.ConnectionString,
                TableName = "Tasks",
                QueryParams = new Dictionary<string, string> {
                    {"TaskStatus",targetStatus.ToString()}
                },
                WhereParams = new Dictionary<string, string> {
                    {"TaskID",taskID.ToString() }
                }
            };

            SQLWrapper.UpdateData(sqlUpdateQuerySettings);

            //using (MySqlConnection conn = GetConnection()) {
            //    conn.Open();
            //    MySqlCommand cmd = new MySqlCommand("UPDATE Tasks SET TaskStatus = @TaskStatus WHERE TaskID = @TaskID", conn);
            //    cmd.Parameters.AddWithValue("@TaskID", taskID);
            //    cmd.Parameters.AddWithValue("@TaskStatus", targetStatus);
            //    int rowsAffected = cmd.ExecuteNonQuery();
            //    return rowsAffected;
            //}
        }

    }

}
