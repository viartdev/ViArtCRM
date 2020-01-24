using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ViArtCRM.HelperTools;

namespace ViArtCRM.Models
{
    public class GroupObject
    {
        public int GroupID { get; set; }

        public string GroupName { get; set; }

        public int LastMessageID { get; set; }
    }

    public class MessageObject
    {
        public int MessageID { get; set; }
        public string MessageText { get; set; }
        public DateTime MessageDate { get; set; }
        public int UserID { get; set; }
        public int GroupID { get; set; }
    }


    public class GroupMessage
    {
        public GroupObject groupObject { get; set; }
        public MessageObject messageObject { get; set; }
    }

    public class GroupUser
    {
        public int GroupUserID { get; set; }
        public int GroupID { get; set; }
        public int UserID { get; set; }
    }
    public class GroupContainer
    { 
        public List<GroupMessage> groupMessageList { get; set; }
    }
    public class ChatContext
    {
        public string ConnectionString { get; set; }
        public GroupContainer GetGroupsContainer()
        {
            int userID = 1;                                         //userID
            GroupContainer groupContainer = new GroupContainer();
            groupContainer.groupMessageList = GetGroup(userID);
            return groupContainer;
        }
        public ChatContext(string connectionString)
        {
            this.ConnectionString = connectionString;
        }
        public List<GroupUser> getGroupsByUser(int UserID)
        {
            List<GroupUser> groups = new List<GroupUser>();
            SQLSelectQuerySettings sqlSelectQuerySettings = new SQLSelectQuerySettings
            {
                ConnectionString = this.ConnectionString,
                TableName = "ChatGroupUsers",
                QueryParams = GetQueryParams(UserID),
            };
            groups= SQLWrapper.SelectData<GroupUser>(sqlSelectQuerySettings);
            return groups;
        }
        Dictionary<string, string> GetQueryParams(int UserID)
        {
            if (UserID == -1)
                return null;
            var queryParams = new Dictionary<string, string>();
            if (UserID != -1)
                queryParams.Add("UserID", UserID.ToString());

            return queryParams;
        }
        public GroupObject GetGroupByID(int id)
        {
            List<GroupObject> list = new List<GroupObject>();
            SQLSelectQuerySettings sqlSelectQuerySettings = new SQLSelectQuerySettings
            {
                ConnectionString = this.ConnectionString,
                TableName = "GroupChat",
                QueryParams = new Dictionary<string, string>() { { "GroupID", id.ToString() } }
            };

            list = SQLWrapper.SelectData<GroupObject>(sqlSelectQuerySettings);

            return list.First(s => s.GroupID == id);
        }
        public List<GroupMessage> GetGroup(int UserID)
        {
            List<GroupObject> list = new List<GroupObject>();
            List<GroupUser> groups = getGroupsByUser(UserID);
            List<GroupMessage> groupMessageList = new List<GroupMessage>();

            foreach (GroupUser item in groups)
            {
                GroupObject group = GetGroupByID(item.GroupID);
                GroupMessage newGroup = new GroupMessage();
                newGroup.groupObject = group;
                newGroup.messageObject = GetMessageByID(group.LastMessageID);
                groupMessageList.Add(newGroup);
            }
            return groupMessageList;
        }
        public MessageObject GetMessageByID(int id)
        {
            List<MessageObject> list = new List<MessageObject>();
            SQLSelectQuerySettings sqlSelectQuerySettings = new SQLSelectQuerySettings
            {
                ConnectionString = this.ConnectionString,
                TableName = "ChatMessages",
                QueryParams = new Dictionary<string, string>() { { "MessageID", id.ToString() } }
            };

            list = SQLWrapper.SelectData<MessageObject>(sqlSelectQuerySettings);

            return list.First(s => s.MessageID == id);
        }


    }


}
