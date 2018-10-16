using Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Schedule
{
    [Serializable]
    [XmlRoot]
    public class ScheduleSystem : Serializers.Serializer<NotificationLibrary>
    {
        public string m_guid = new System.Guid().ToString();
        public string Guid { get { return m_guid; } set { m_guid = value; } }
        public delegate Task TaskActionDelegate(Task n);

        private static Task DefaultAction(Task n) { return n; }
        public ScheduleSystem()
        {
            Guid = System.Guid.NewGuid().ToString();
            Notices = new NoticeBindinglist();
            Notices.ListChanged += Notices_ListChanged;
            NoticeTimer = new System.Timers.Timer();
            NoticeTimer.Elapsed += NoticeTimer_Elapsed;
            NoticeTimer.Interval = m_interval; // Every 5 Minutes by default
            NoticeTimer.Enabled = true;
        }
        public ScheduleSystem(string app_name)
        {

            Guid = app_name;
            Notices = new NoticeBindinglist();
            Notices.ListChanged += Notices_ListChanged;
            NoticeTimer = new System.Timers.Timer();
            NoticeTimer.Elapsed += NoticeTimer_Elapsed;
            NoticeTimer.Interval = m_interval; // Every 5 Minutes by default
            NoticeTimer.Enabled = true;
        }

        [Serializable]
        public enum NoticeType
        {
            None, User, Customer, Administration, Log, Application, Asset, Calibration, Other
        }

        [XmlElement]
        public NoticeBindinglist Notices { get; set; }
        [XmlElement]
        public double Interval { get { return m_interval; } set { if (value >= 60000) { m_interval = value; NoticeTimer.Interval = value; } } }
        [XmlIgnore]
        private System.Timers.Timer NoticeTimer;
        [XmlElement]
        private double m_interval = (1000 * 60) * 5;
        [XmlElement]
        private int Count { get { return Notices.Count; } }

        private void NoticeTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            HandleTimerTick(new TimerTickEvent(this));
            foreach (var notice in Notices)
            {
                if (notice.IsExpired && notice.IsTimed)
                {
                    HandleExpiredNotice(new NoticeUpdateEvent(notice));
                }
            }
        }
        private void Notices_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.ItemDeleted)
            {
                HandleRemovedNotice(new NoticeUpdateEvent(Notices[e.NewIndex]));
            }
            if (e.ListChangedType == ListChangedType.ItemAdded)
            {
                HandleAddedNotice(new NoticeUpdateEvent(Notices[e.NewIndex]));
            }
            HandleChangedNotice(new NoticeUpdateEvent(Notices[e.NewIndex], e));

        }

        public void Add(Task notice)
        {
            Notices.Add(notice);
        }
        public bool Remove(Task notice)
        {
            return Notices.Remove(notice);
        }
        public bool Contains(Task notice)
        {
            return Notices.Contains(notice);
        }
        public bool Contains(string guid)
        {
            try
            {
                return Convert.ToBoolean((from n in Notices where n.Guid == guid select n).ToList().Count);
            }
            catch { return false; }

        }
        public Task Get(string guid)
        {
            return (from n in Notices where n.Guid == guid select n).ToList().FirstOrDefault();
        }
        public void IndexOf(Task notice)
        {
            Notices.IndexOf(notice);
        }
        public void Start()
        {
            NoticeTimer.Enabled = true;
        }
        public void Stop()
        {
            NoticeTimer.Enabled = false;
        }
        public string Save()
        {
            return SerializeToXmlString(this);
        }
        public static ScheduleSystem Load(string xml)
        {
            return new ScheduleSystem().DeserializeFromXmlString<ScheduleSystem>(xml);
        }
        #region Sub-Classes
        [Serializable]
        public class NoticeBindinglist : BindingList<Task>
        {
            public NoticeBindinglist() { this.AllowEdit = AllowNew = AllowRemove = true; }
            public NoticeBindinglist(List<Task> notices)
            {
                this.AllowEdit = AllowNew = AllowRemove = true;
                foreach (var notice in notices)
                {
                    this.Add(notice);
                }
            }
        }
        [Serializable]
        public class Task : Serializers.Serializer<Task>
        {
            public TaskActionDelegate NoticeAction = DefaultAction;
            /// <summary>
            /// Create a Notification
            /// </summary>
            public Task()
            {
                Guid = System.Guid.NewGuid().ToString();
                NoticeControlNumber = Name = Data = "";
                this.EmailAddress = new EmailAddress();
                this.EmailAddress.Name = "null.null";
                this.EmailAddress.Email = "null@null.null";
                Created = DateTime.Now;
                ScheduledTime = Created.AddMonths(1);
                IsTimed = true;
                NoticeType = NoticeType.None;
            }
            /// <summary>
            ///  Create a Notification
            /// </summary>
            /// <param name="expires">Expiration Date</param>
            /// <param name="noticeAction">ACtion</param>
            public Task(DateTime expires, TaskActionDelegate noticeAction = null)
            {
                if (noticeAction != null) NoticeAction = noticeAction;
                Guid = System.Guid.NewGuid().ToString();
                NoticeControlNumber = Name = Data = "";
                this.EmailAddress = new EmailAddress();
                this.EmailAddress.Name = "null.null";
                this.EmailAddress.Email = "null@null.null";
                Created = DateTime.Now;
                ScheduledTime = expires;
                IsTimed = true;
                NoticeType = NoticeType.None;
            }
            public Task(DateTime expires, NoticeType type, TaskActionDelegate noticeAction = null)
            {
                if (noticeAction != null) NoticeAction = noticeAction;
                Guid = System.Guid.NewGuid().ToString();
                NoticeControlNumber = Name = Data = "";
                this.EmailAddress = new EmailAddress();
                this.EmailAddress.Name = "null.null";
                this.EmailAddress.Email = "null@null.null";
                Created = DateTime.Now;
                ScheduledTime = expires;
                IsTimed = true;
                NoticeType = type;
            }
            public Task(DateTime expires, NoticeType type, EmailAddress email, TaskActionDelegate noticeAction = null)
            {
                if (noticeAction != null) NoticeAction = noticeAction;
                Guid = System.Guid.NewGuid().ToString();
                NoticeControlNumber = Name = Data = "";
                this.EmailAddress = email;
                Created = DateTime.Now;
                ScheduledTime = expires;
                IsTimed = true;
                NoticeType = type;
            }
            public Task(DateTime expires, NoticeType type, EmailAddress email, string controlNumber, TaskActionDelegate noticeAction = null)
            {
                if (noticeAction != null) NoticeAction = noticeAction;
                Guid = System.Guid.NewGuid().ToString();
                NoticeControlNumber = controlNumber;
                Name = Data = "";
                this.EmailAddress = email;
                Created = DateTime.Now;
                ScheduledTime = expires;
                IsTimed = true;
                NoticeType = type;
            }
            public Task(DateTime expires, NoticeType type, EmailAddress email, string controlNumber, string data, TaskActionDelegate noticeAction = null)
            {
                if (noticeAction != null) NoticeAction = noticeAction;
                Guid = System.Guid.NewGuid().ToString();
                NoticeControlNumber = controlNumber;
                Name = "";
                Data = data;
                this.EmailAddress = email;
                Created = DateTime.Now;
                ScheduledTime = expires;
                IsTimed = true;
                NoticeType = type;
            }
            public Task(DateTime expires, NoticeType type, EmailAddress email, string controlNumber, string data, string name, TaskActionDelegate noticeAction = null)
            {
                if (noticeAction != null) NoticeAction = noticeAction;
                Guid = System.Guid.NewGuid().ToString();
                NoticeControlNumber = controlNumber;
                Name = name;
                Data = data;
                this.EmailAddress = email;
                Created = DateTime.Now;
                ScheduledTime = expires;
                IsTimed = true;
                NoticeType = type;
            }

            public static Task Load(string guid)
            {
                var db = Web_App_Master.Pull.Setting(guid);
                var notice = new Task().DeserializeFromXmlString<Task>(db.XmlData);
                return notice;
            }
            public void Save()
            {
                SettingsDBData db = new SettingsDBData();
                db.Appname = this.Guid;
                db.XmlData = this.SerializeToXmlString(this);
                Web_App_Master.Push.Setting(db);
            }
            [XmlElement]
            public string NoticeControlNumber { get; set; }
            [XmlElement]
            public NoticeType NoticeType { get; set; }
            [XmlElement]
            public string Guid { get; set; }
            [XmlElement]
            public string Data { get; set; }
            [XmlElement]
            public string Name { get { return this.EmailAddress.Name; } set { this.EmailAddress.Name = value; } }
            [XmlElement]
            public EmailAddress EmailAddress { get; set; }
            [XmlElement]
            public string Email { get { return this.EmailAddress.Email; } set { this.EmailAddress.Email = value; } }
            [XmlElement]
            public DateTime Created { get; set; }
            [XmlIgnore]
            public double MinutesUntil
            {
                get
                {
                    return (ScheduledTime - Created).TotalMinutes;
                }

            }
            [XmlIgnore]
            public double HoursUntil
            {
                get
                {
                    return (ScheduledTime - Created).TotalHours;
                }

            }
            [XmlIgnore]
            public double DaysUntil
            {
                get
                {
                    return (ScheduledTime - Created).TotalDays;
                }

            }
            [XmlElement]
            public DateTime ScheduledTime { get; set; }
           // [XmlElement]
           // public DateTime Expires { get; set; }
            [XmlElement]
            public bool IsExpired
            {
                get
                {
                    if (MinutesUntil > 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            [XmlElement]
            public bool IsTimed { get; set; }



        }
        #endregion
        #region Events
        public class TimerTickEvent : EventArgs
        {
            public TimerTickEvent(object obj)
            {
                this.Notice = obj;
            }
            public object Notice { get; set; }
        }
        public class NoticeUpdateEvent : EventArgs
        {
            public NoticeUpdateEvent(Task notice = null, ListChangedEventArgs changeArguments = null)
            {
                this.Notice = notice;
                this.ChangeArguments = changeArguments;
            }
            public Task Notice { get; set; }
            public ListChangedEventArgs ChangeArguments { get; set; }
        }
        public event EventHandler<NoticeUpdateEvent> OnNoticeExpired;
        protected virtual void HandleExpiredNotice(NoticeUpdateEvent e)
        {
            try
            {
                EventHandler<NoticeUpdateEvent> handler = OnNoticeExpired;
                if (handler != null)
                {
                    handler(this, e);
                }
            }
            catch { }
        }
        public event EventHandler<NoticeUpdateEvent> OnNoticeChanged;
        protected virtual void HandleChangedNotice(NoticeUpdateEvent e)
        {
            try
            {
                EventHandler<NoticeUpdateEvent> handler = OnNoticeChanged;
                if (handler != null)
                {
                    handler(this, e);
                }
            }
            catch { }
        }
        public event EventHandler<NoticeUpdateEvent> OnNoticeRemoved;
        protected virtual void HandleRemovedNotice(NoticeUpdateEvent e)
        {
            try
            {
                EventHandler<NoticeUpdateEvent> handler = OnNoticeRemoved;
                if (handler != null)
                {
                    handler(this, e);
                }
            }
            catch { }
        }
        public event EventHandler<NoticeUpdateEvent> OnNoticeAdded;
        protected virtual void HandleAddedNotice(NoticeUpdateEvent e)
        {
            try
            {
                EventHandler<NoticeUpdateEvent> handler = OnNoticeAdded;
                if (handler != null)
                {
                    handler(this, e);
                }
            }
            catch { }
        }
        public event EventHandler<TimerTickEvent> OnTimerTick;
        protected virtual void HandleTimerTick(TimerTickEvent e)
        {
            try
            {
                EventHandler<TimerTickEvent> handler = OnTimerTick;
                if (handler != null)
                {
                    handler(this, e);
                }
            }
            catch { }
        }

        #endregion
    }
}