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

    public class GroupContainer
    { 
        public List<GroupMessage> groupMessageList { get; set; }
    }
    public class ChatContext
    {
        public string ConnectionString { get; set; }
        public GroupContainer GetGroupsContainer()
        {
            GroupContainer groupContainer = new GroupContainer();
            groupContainer.groupMessageList = GetGroups();
            return groupContainer;
        }
        public ChatContext(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public List<GroupMessage> GetGroups()
        {
            List<GroupObject> list = new List<GroupObject>();
            SQLSelectQuerySettings sqlSelectQuerySettings = new SQLSelectQuerySettings
            {
                ConnectionString = this.ConnectionString,
                TableName = "GroupChat",
     
            };

            list = SQLWrapper.SelectData<GroupObject>(sqlSelectQuerySettings);
            List<GroupMessage> groupMessageList = new List<GroupMessage>();
            foreach (GroupObject item in list)
            {
                GroupMessage newGroup = new GroupMessage();
                newGroup.groupObject = item;
                newGroup.messageObject = GetMessageByID(item.LastMessageID);
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
