using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ViArtCRM.HelperTools;

namespace ViArtCRM.Models {
    public class User {
        [SQLHelper(DataSourceFieldName = "UserID")]
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int UserRole { get; set; }

    }

    public class UserContext {
        public string ConnectionString { get; set; }

        public UserContext(string connectionString) {
            this.ConnectionString = connectionString;
        }

        private MySqlConnection GetConnection() {
            return new MySqlConnection(ConnectionString);
        }
        public List<User> GetUsers() {
            List<User> list = new List<User>();
            var sqlSettings = new SQLSelectQuerySettings();
            sqlSettings.ConnectionString = ConnectionString;
            sqlSettings.TableName = "Users";
            sqlSettings.QueryParams = new Dictionary<string, string>();
            sqlSettings.QueryParams.Add("UserID", "1");

            list = SQLWrapper.SelectData<User>(sqlSettings);
            return list;
        }

       public User GetUserByID(int userID)
        {
            List<User> list = new List<User>();
            SQLSelectQuerySettings sqlSelectQuerySettings = new SQLSelectQuerySettings
            {
                ConnectionString = this.ConnectionString,
                TableName = "Users",
                QueryParams = new Dictionary<string, string>() { { "UserID", userID.ToString() } }
            };

            list = SQLWrapper.SelectData<User>(sqlSelectQuerySettings);

            return list.First(s => s.UserID == userID);
        }
    }
}
