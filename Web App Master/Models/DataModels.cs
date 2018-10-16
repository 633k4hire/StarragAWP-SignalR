using Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Web_App_Master.Models
{
    public interface iPollable<T>
    {
        T Poll(T e);
        T Push(T e);
        int Interval();
        void Start();
        void Stop();
    }
    public interface iStorable<T>
    {
        bool Push(T e);
        T Pull(T e);
    }
    [Serializable]
    public class Data_Base_Class : Serializers.Serializer<Data_Base_Class>, iStorable<Data_Base_Class>
    {
        public Data_Base_Class Pull(Data_Base_Class e)
        {
            SQL_Request request = new SQL_Request();
            try
            {
                request.OpenConnection();
                request.SettingsGet();
                if (request.Success)
                {
                    return this.DeserializeFromXmlString<Data_Base_Class>((request.Tag as SettingsDBData).XmlData);
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                LastException = ex;
                return null;
            }
            finally
            {
                request.CloseConnection();
            }
        }

        public bool Push(Data_Base_Class e)
        {
            SQL_Request request = new SQL_Request();
            try
            {
                request.OpenConnection();
                if (!request.SettingsGet(this.Id, false).Success)
                {
                    return (request.SettingsAdd(this.Id, this.SerializeToXmlString(this))).Success;
                }
                else
                {
                    return (request.SettingsUpdate(this.Id, this.SerializeToXmlString(this))).Success;
                }
            }
            catch (Exception ex)
            {
                LastException = ex;
                return false;
            }
            finally
            {
                request.CloseConnection();
            }
        }
        [XmlIgnore]
        private int mPollInterval = 250;
        [XmlElement]
        public int PollInterval
        {
            get { return mPollInterval; }
            set { mPollInterval = value; }
        }
        [XmlIgnore]
        private string mGuid = Guid.NewGuid().ToString();
        [XmlElement]
        public string Id { get { return mGuid; } set { mGuid = value; } }
        [XmlIgnore]
        private string mName = "";
        [XmlElement]
        public string Name { get { return mName; } set { mName = value; } }
        [XmlIgnore]
        private string mXmlData = "";
        [XmlElement]
        public string Data
        {
            get { return mXmlData; }
            set { mXmlData = value; }
        }

        [XmlIgnore]
        public object Tag = null;

        [XmlIgnore]
        public Exception LastException = null;
    }
    [Serializable]
    public class Data_Models
    {
        [Serializable]
        public class ClientData : Data_Base_Class
        {
            public ClientData() { }

            public ClientData(object data)
            {
                if (data is string)
                {

                }
            }

            [XmlIgnore]
            private string mUrl = "";
            [XmlElement]
            public string Url { get { return mUrl; } set { mUrl = value; } }

            [XmlIgnore]
            private string mDesc = "";
            [XmlElement]
            public string Description { get { return mDesc; } set { mDesc = value; } }

            [XmlIgnore]
            private byte[] mBytes = new byte[] { };
            [XmlElement]
            public byte[] Bytes { get { return mBytes; } set { mBytes = value; } }

        }
        [Serializable]
        public class NodeData : Data_Base_Class
        {
            private readonly object _clientlock = new object();
            private readonly object _nodelock = new object();
            private BindingList<ClientData> mClients;
            [XmlElement]
            public BindingList<ClientData> Clients
            {
                get
                {
                    lock (_clientlock)
                    {
                        return mClients;
                    }
                }
                set
                {
                    lock (_clientlock)
                    {
                        mClients = value;
                    }
                }
            }

            private BindingList<NodeData> mNodes;
            [XmlElement]
            public BindingList<NodeData> Nodes
            {
                get
                {
                    lock (_nodelock)
                    {
                        return mNodes;
                    }
                }
                set
                {
                    lock (_nodelock)
                    {
                        mNodes = value;
                    }
                }
            }
        }

        [Serializable]
        public class ServerData : Serializers.XmlSerializable<ServerData>
        {
            [XmlIgnore]
            private string mGuid = Guid.NewGuid().ToString();
            [XmlElement]
            public string Id { get { return mGuid; } set { mGuid = value; } }

            [XmlIgnore]
            private string mName = "";
            [XmlElement]
            public string Name { get { return mName; } set { mName = value; } }

            [XmlIgnore]
            private readonly object _clientlock = new object();

            [XmlIgnore]
            private List<ClientData> mClients;
            [XmlElement]
            public List<ClientData> Clients
            {
                get
                {
                    lock (_clientlock)
                    {
                        return mClients;
                    }
                }
                set
                {
                    lock (_clientlock)
                    {
                        mClients = value;
                    }
                }
            }


        }

    }
    public class Node
    {
        public Node()
        {
            Name = Guid.NewGuid().ToString();
            Title = "";
            ChildNodes = new List<Node>();
        }
        public Node(string title)
        {
            Name = Guid.NewGuid().ToString();
            Title = title;
            ChildNodes = new List<Node>();
        }

        private string mTitle;
        public string Title
        {
            get { return mTitle; }
            set { mTitle = value; }
        }

        private string mName;
        public string Name
        {
            get { return mName; }
            set { mName = value; }
        }

        private string mUniqueId;
        public string UniqueId
        {
            get { return mUniqueId; }
            set { mUniqueId = value; }
        }

        private bool mHasChildren;

        public bool HasChildren
        {
            get { return mHasChildren; }
            set { mHasChildren = value; }
        }


        private List<Node> nodes;
        public List<Node> ChildNodes
        {
            get { return nodes; }
            set { nodes = value; }
        }

    }
}