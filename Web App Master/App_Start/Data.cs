using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using static Notification.NotificationSystem;

namespace Data
{
    [Serializable]
    public class LogEntry
    {
        public LogEntry()
        {
            Time = DateTime.Now;
            Entry = "";
            Href = "#";
        }
        public LogEntry(string entry)
        {
            Time = DateTime.Now;
            Entry = entry;
            Href = "#";
        }
        public LogEntry(string entry, string href)
        {
            Time = DateTime.Now;
            Entry = entry;
            Href = href;
        }
        [XmlElement]
        public DateTime Time { get; set; }
        [XmlElement]
        public string Entry { get; set; }
        [XmlElement]
        public string Href { get; set; }
        [XmlElement]
        public string TimeString { get { return Time.ToShortTimeString(); } }
    }
    public class UserDataBindingList : BindingList<UserData>
    {
        public UserDataBindingList() { this.AllowEdit = AllowNew = AllowRemove = true; }
        public UserDataBindingList(List<UserData> userData)
        {
            this.AllowEdit = AllowNew = AllowRemove = true;
            foreach (var ud in userData)
            {
                this.Add(ud);
            }
        }
    }
    [Serializable]
    public class UserData:Serializers.Serializer<UserData>,ICloneable
    {
        public UserData()
        {
            Name =  Email = "";
            Log = new List<LogEntry>();
            Notices = new List<Notice>();
            Attachments = new List<Attachment>();
            IsAutoChecked = false;
        }
        [XmlElement]
        public string m_guid = System.Guid.NewGuid().ToString();
        [XmlElement]
        public string Guid { get { return m_guid; } set { m_guid = value; } }
        [XmlElement]
        public string Name { get; set; }
       
        [XmlElement]
        public string Email { get; set; }
        
        [XmlElement]
        public List<LogEntry> Log { get; set; }
        [XmlElement]
        public List<Notice> Notices { get; set; }
        [XmlElement]
        public List<Attachment> Attachments { get; set; }
        [XmlElement]
        public bool IsAutoChecked { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
       
    }
    [Serializable]
    public class Attachment : Serializers.Serializer<Attachment>, ICloneable
    {
        public Attachment()
        {
            Name =  Email = "";
            Files = new List<FileReference>();
            Date = DateTime.Now;

        }
        [XmlElement]
        public DateTime Date { get; set; }
        [XmlElement]
        public string Name { get; set; }       
        [XmlElement]
        public string Email { get; set; }      
        [XmlElement]
        public List<FileReference> Files { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

    }
    [Serializable]
    public class FileReference : Serializers.Serializer<UserData>
    {
        public FileReference()
        {
            Name = Link = "";          
            Date = DateTime.Now;
        }
        public FileReference(string name, string link)
        {
            Name = name; Link = link;
            Date = DateTime.Now;
        }
        [XmlElement]
        public DateTime Date { get; set; }
        [XmlElement]
        public string Name { get; set; }
        [XmlElement]
        public string Link { get; set; }
    }
}