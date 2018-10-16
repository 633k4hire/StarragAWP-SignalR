using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Web_App_Master
{
    public class SyncEvent : EventArgs
    {
        public SyncEvent(WebAppHub.HubData obj = null)
        {
            if (obj != null)
            {
                Data = obj;
            }
        }
        public WebAppHub.HubData Data { get; set; }
    }

    [Serializable]
    public class WebAppHub : Serializers.Serializer<WebAppHub>
    {       
        public class Group
        {
            public SyncDelegate SyncAction;
            public event EventHandler<ExceptionEvent> OnException;
            protected virtual void Error(ExceptionEvent e)
            {
                try
                {
                    EventHandler<ExceptionEvent> handler = OnException;
                    if (handler != null)
                    {
                        handler(this, e);
                    }
                }
                catch { }
            }
            public event EventHandler<SyncEvent> OnSync;
            public virtual void SyncData(SyncEvent e)
            {
                try
                {
                    EventHandler<SyncEvent> handler = OnSync;
                    if (handler != null)
                    {
                        handler(this, e);
                    }
                }
                catch (Exception ex) { Error(new ExceptionEvent(ex)); }
            }
           

            public Group(Header h, Footer f, RightPanel r, LeftPanel l)
            {
                //wire all panels together
                SyncAction = ToSpokes;
                Header = h;Footer = f;RightPanel = r;LeftPanel = l;
                //from header
                Header.OnSync += FromSpokes;
                

                Footer.OnSync += FromSpokes;

                LeftPanel.OnSync += FromSpokes;

                RightPanel.OnSync += FromSpokes;

            }

            private void FromSpokes(object sender, SyncEvent e)
            {
                SyncData(new SyncEvent(e.Data));
            }

            private HubData ToSpokes(HubData data)
            {
                Header?.IncomingSyncData(data);
                return data;
            }

            public Header Header;
            public Footer Footer;
            public RightPanel RightPanel;
            public LeftPanel LeftPanel;
        }
        public delegate HubData SyncDelegate(HubData data);
        public enum ControlGroup
        {
            Header,LeftPanel,RightPanel,Footer,Other
        }
        public class HubSync
        {
            /// <summary>
            /// Outgoing Data For Other Control Groups
            /// </summary>
            public event EventHandler<SyncEvent> OnSync;
            public virtual void SyncData( SyncEvent e)
            {
                try
                {
                    EventHandler<SyncEvent> handler = OnSync;
                    if (handler != null)
                    {
                        handler(this, e);
                    }
                }
                catch (Exception ex) { Error(new ExceptionEvent(ex)); }
            }



            private string m_ID = Guid.NewGuid().ToString();
            [XmlElement]
            public string ID
            {
                get { return m_ID; }
                private set { m_ID = value; }
            }

            private string m_name;
            [XmlElement]
            public string Name
            {
                get { return m_name; }
                set { m_name = value; }
            }

            private DateTime m_date;
            [XmlElement]
            public DateTime Date
            {
                get { return m_date; }
                set { m_date = value; }
            }

            [XmlIgnore]
            private readonly object _Lock = new object();

            public event EventHandler<ExceptionEvent> OnException;
            protected virtual void Error( ExceptionEvent e)
            {
                try
                {
                    EventHandler<ExceptionEvent> handler = OnException;
                    if (handler != null)
                    {
                        handler(this, e);
                    }
                }
                catch { }
            }
            public ControlGroup Group = ControlGroup.Other;
            public SyncDelegate IncomingSyncData;
            public List<System.Web.UI.Control> Controls = new List<System.Web.UI.Control>();
            public void Update(HubData data)
            {
                this.SyncData(new SyncEvent(data));
            }
        }
        [Serializable]
        public class HubData
        {
            public Exception Exception=null;
            public ControlGroup Group = ControlGroup.Other;
            public string Name = "";
            public object Tag;
            public string Command = "";
            public string CommandArgument = "";
            public System.Web.UI.Control Control;
        }
       
        public class Header : HubSync
        {
           
            public Header()
            {
                this.Group = ControlGroup.Header;
            }

            
        }
        public class LeftPanel : HubSync
        {
            public LeftPanel()
            {
                this.Group = ControlGroup.LeftPanel;
            }
        }
        public class RightPanel : HubSync
        {
            public RightPanel()
            {
                this.Group = ControlGroup.RightPanel;
            }
        }
        public class Footer : HubSync
        {
            public Footer()
            {
                this.Group = ControlGroup.Footer;
            }
        }

        
       

    }
}