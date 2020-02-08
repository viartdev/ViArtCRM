using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ViArtCRM.HelperTools;

namespace ViArtCRM.Models {
    public class User {
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
            list = SQLWrapper.SelectData<User>(sqlSettings);
            return list;
        }

        string GetPasswordHash(string password) {
            using (MD5 instance = MD5.Create()) {
                return HashHelper.GetMd5Hash(instance, password);
            }
        }
        public User GetUserByLoginAndPassword(string login, string password) {
            List<User> list = new List<User>();
            SQLSelectQuerySettings sqlSelectQuerySettings = new SQLSelectQuerySettings {
                ConnectionString = this.ConnectionString,
                TableName = "Users",
                QueryParams = new Dictionary<string, string>() { { "Login", login }, { "Password", GetPasswordHash(password) } }
            };

            list = SQLWrapper.SelectData<User>(sqlSelectQuerySettings);
            return list.FirstOrDefault(s => s.Login == login);
        }
        public User GetUserByLogin(string login) {
            List<User> list = new List<User>();
            SQLSelectQuerySettings sqlSelectQuerySettings = new SQLSelectQuerySettings {
                ConnectionString = this.ConnectionString,
                TableName = "Users",
                QueryParams = new Dictionary<string, string>() { { "Login", login } }
            };

            list = SQLWrapper.SelectData<User>(sqlSelectQuerySettings);
            return list.FirstOrDefault(s => s.Login == login);
        }

        public User GetUserByID(int userID) {
            List<User> list = new List<User>();
            SQLSelectQuerySettings sqlSelectQuerySettings = new SQLSelectQuerySettings {
                ConnectionString = this.ConnectionString,
                TableName = "Users",
                QueryParams = new Dictionary<string, string>() { { "UserID", userID.ToString() } }
            };

            list = SQLWrapper.SelectData<User>(sqlSelectQuerySettings);

            return list.First(s => s.UserID == userID);
        }
    }
    public class UserSession {
        public int UserID { get; set; }
        public string Login { get; set; }
        [SQLHelper(DataSourceFieldName = "Session")]
        public string USession { get; set; }
    }
    public class SessionContext {
        public string ConnectionString { get; set; }

        public SessionContext(string connectionString) {
            this.ConnectionString = connectionString;
        }

        private MySqlConnection GetConnection() {
            return new MySqlConnection(ConnectionString);
        }
        void RemoveUserSession(User user) {
            throw new NotImplementedException();
        }
        string GetNewSessionKey(User user) {
            string sessionKey = String.Empty;
            string initString = String.Format("{0}_{1}_{2}", user.UserID, DateTime.Now.Millisecond, user.Login);
            using (MD5 instance = MD5.Create()) {
                sessionKey = HashHelper.GetMd5Hash(instance, initString);
            }
            return sessionKey;
        }

        public UserSession CreateUserSession(User user) {
            var serverUserSession = GetUserSession(user);
            if (serverUserSession.UserID != -1)
                ClearUserServerSession(user);
            UserSession userSession = new UserSession {
                UserID = user.UserID,
                Login = user.Login,
                USession = GetNewSessionKey(user)
            };
            SQLInsertQuerySettings sqlInsertQuerySettings = new SQLInsertQuerySettings() {
                TableName = "UsersSessions",
                ConnectionString = this.ConnectionString
            };
            SQLWrapper.InsertData<UserSession>(userSession, sqlInsertQuerySettings);
            return userSession;
        }

        public void ClearUserServerSession(User user) {
            SQLRemoveQuerySettings sqlRemoveQuerySettings = new SQLRemoveQuerySettings {
                ConnectionString = this.ConnectionString,
                TableName = "UsersSessions",
                WhereParams = new Dictionary<string, string>() {
                   { "Login", user.Login }, { "UserID",  user.UserID.ToString() }
                }
            };
            SQLWrapper.RemoveData(sqlRemoveQuerySettings);
        }

        public UserSession GetUserSession(User user) {
            UserSession userSession = new UserSession { UserID = -1 };
            SQLSelectQuerySettings sqlSelectQuerySettings = new SQLSelectQuerySettings {
                ConnectionString = this.ConnectionString,
                TableName = "UsersSessions",
                QueryParams = new Dictionary<string, string>() { { "UserID", user.UserID.ToString() }, { "Login", user.Login } }
            };
            var list = SQLWrapper.SelectData<UserSession>(sqlSelectQuerySettings);
            if (list.Count == 1) {
                userSession = list[0];
            }
            //else if (list.Count > 1) {
            //    RemoveUserSession(user);
            //}
            //else if (list.Count == 0) {
            //    userSession = CreateUserSession(user);
            //}

            return userSession;
        }


    }
}
