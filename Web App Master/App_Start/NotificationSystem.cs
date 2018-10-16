using Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Notification
{
    /// <summary>
    /// Self Contained Scheduled Notificaton System...Just Add Notice
    /// </summary>
    [XmlInclude(typeof(EmailNotice))]
    [Serializable]
    [XmlRoot]
    public class NotificationSystem:Serializers.Serializer<NotificationSystem>, ICloneable
    {
        [XmlElement]
        public string m_guid = new System.Guid().ToString();
        [XmlElement]
        public string Guid { get { return m_guid; } set { m_guid = value; } }
     
        public delegate Notice NoticeActionDelegate(Notice n);
        [XmlIgnore]
        private int m_capacity = -1;
        /// <summary>
        /// Self Removing Que...If Notices reaches Capacity the LastItem is removed
        /// </summary>
        [XmlElement]
        public int Capacity { get { return m_capacity; } set { m_capacity = value; } }
        
        private static Notice DefaultAction(Notice n) { return n; }
        public NotificationSystem()
        {
            Guid = System.Guid.NewGuid().ToString();
            Notices = new NoticeBindinglist();
            Notices.ListChanged += Notices_ListChanged;
            NoticeTimer = new System.Timers.Timer();
            NoticeTimer.Elapsed += NoticeTimer_Elapsed;
            NoticeTimer.Interval = m_interval; // Every 5 Minutes by default
            NoticeTimer.Enabled = true;
        }
        public NotificationSystem(string app_name)
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
            None,User,Customer,Administration,Log,Application,Asset,AssetList,Calibration,Notice,Checkout,Checkin,Damaged,OnHold
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
                try
                {
                    if (notice.IsExpirable && notice.IsExpired)
                    {
                        HandleExpiredNotice(new NotificationEvent(notice));
                        break;
                    }
                    if (notice.IsScheduledTime && notice.IsTimed)
                    {

                        HandleOnScheduledTime(new NotificationEvent(notice));
                    }
                    if (!notice.IsTimed)
                    {
                        HandleNonTimedNotice(new NotificationEvent(notice));
                    }
                }catch
                {

                }
            }
        }
        private void Notices_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.ItemDeleted)
            {
                HandleRemovedNotice(new NotificationEvent(Notices[e.NewIndex]));
            }
            if (e.ListChangedType == ListChangedType.ItemAdded)
            {
                HandleAddedNotice(new NotificationEvent(Notices[e.NewIndex]));
            }           
            HandleChangedNotice(new NotificationEvent(Notices[e.NewIndex], e));

        }

        public void Add(Notice notice)
        {
            if (Capacity == -1) { Notices.Add(notice); return; }
            else
            {
                if (Notices.Count==Capacity)
                {
                    Notices.RemoveAt(Notices.Count - 1);
                }
                Notices.Add(notice);
            }
        }
        public bool Remove(Notice notice)
        {
            return Notices.Remove(notice);
        }
        public bool Contains(Notice notice)
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
        public Notice Get(string guid)
        {
            return (from n in Notices where n.Guid == guid select n).ToList().FirstOrDefault();
        }
        public void IndexOf(Notice notice)
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
            try
            {
                return SerializeToXmlString(this);
            }
            catch(Exception ex) { HandleException(new ExcpetionEvent(ex)); return null; }
        }
        public static NotificationSystem Load(string xml)
        {
            try
            {
                return new NotificationSystem().DeserializeFromXmlString<NotificationSystem>(xml);
            }
            catch (Exception ex) { return null; }

        }
        #region Sub-Classes
        [Serializable]
       
        public class NoticeBindinglist : BindingList<Notice>
        {
            public NoticeBindinglist() { this.AllowEdit = AllowNew = AllowRemove = true; }
            public NoticeBindinglist(List<Notice> notices)
            {
                this.AllowEdit = AllowNew = AllowRemove = true;
                foreach (var notice in notices)
                {
                    this.Add(notice);
                }
            }
        }    
        [Serializable]
        public class Notice:Serializers.Serializer<Notice>
        {
            [XmlIgnore]
            public NoticeActionDelegate NoticeAction = DefaultAction;
            /// <summary>
            /// Create a Notification
            /// </summary>
            public Notice()
            {
                this.IsExpirable = false;
                this.Expires = DateTime.Now.AddMonths(12);
                this.Guid = System.Guid.NewGuid().ToString();
                this.NoticeControlNumber = "";               
                this.Data = "";
                this.EmailAddress = new EmailAddress();
                this.EmailAddress.Name = "null.null";
                this.EmailAddress.Email = "null@null.null";
                Created = DateTime.Now;
                Scheduled = Created.AddMonths(1);
                IsTimed = true;
                NoticeType = NoticeType.None;
            }
            /// <summary>
            ///  Create a Notification
            /// </summary>
            /// <param name="sheduledTime">Expiration Date</param>
            /// <param name="noticeAction">ACtion</param>
            public Notice(DateTime sheduledTime, NoticeActionDelegate noticeAction=null)
            {
                this.IsExpirable = false;
                this.Expires = DateTime.Now.AddMonths(12);
                if (noticeAction!=null)NoticeAction = noticeAction;
                Guid = System.Guid.NewGuid().ToString();
                NoticeControlNumber = "";
                
                Data = "";
                this.EmailAddress = new EmailAddress();
                this.EmailAddress.Name = "null.null";
                this.EmailAddress.Email = "null@null.null";
                Created = DateTime.Now;
                Scheduled = sheduledTime;
                IsTimed = true;
                NoticeType = NoticeType.None;
            }
            public Notice(DateTime sheduledTime, NoticeType type, NoticeActionDelegate noticeAction = null)
            {
                this.IsExpirable = false;
                this.Expires = DateTime.Now.AddMonths(12);
                if (noticeAction != null) NoticeAction = noticeAction;
                Guid = System.Guid.NewGuid().ToString();
                NoticeControlNumber = "";  Data = "";
                this.EmailAddress = new EmailAddress();
                this.EmailAddress.Name = "null.null";
                this.EmailAddress.Email = "null@null.null";
                Created = DateTime.Now;
                Scheduled = sheduledTime;
                IsTimed = true;
                NoticeType = type;
            }
            public Notice(DateTime sheduledTime, NoticeType type, EmailAddress email, NoticeActionDelegate noticeAction = null)
            {
                this.IsExpirable = false;
                this.Expires = DateTime.Now.AddMonths(12);
                if (noticeAction != null) NoticeAction = noticeAction;
                Guid = System.Guid.NewGuid().ToString();
                NoticeControlNumber = "";  Data = "";
                this.EmailAddress = email;
                Created = DateTime.Now;
                Scheduled = sheduledTime;
                IsTimed = true;
                NoticeType = type;
            }
            public Notice(DateTime sheduledTime, NoticeType type, EmailAddress email, string controlNumber, NoticeActionDelegate noticeAction = null)
            {
                this.IsExpirable = false;
                this.Expires = DateTime.Now.AddMonths(12);
                if (noticeAction != null) NoticeAction = noticeAction;
                Guid = System.Guid.NewGuid().ToString();
                NoticeControlNumber = controlNumber;
                Data = "";
                this.EmailAddress = email;
                Created = DateTime.Now;
                Scheduled = sheduledTime;
                IsTimed = true;
                NoticeType = type;
            }
            public Notice(DateTime sheduledTime, NoticeType type, EmailAddress email, string controlNumber, string data, NoticeActionDelegate noticeAction = null)
            {
                this.IsExpirable = false;
                this.Expires = DateTime.Now.AddMonths(12);
                if (noticeAction != null) NoticeAction = noticeAction;
                Guid = System.Guid.NewGuid().ToString();
                NoticeControlNumber = controlNumber;
                
                Data = data;
               this.EmailAddress = email;
                Created = DateTime.Now;
                Scheduled = sheduledTime;
                IsTimed = true;
                NoticeType = type;
            }
            public static Notice Load(string guid)
            {
                return Web_App_Master.Pull.Notification(guid);
            }
            public void Save()
            {
                Web_App_Master.Push.Notification(this);
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
                    return Math.Round((Scheduled - DateTime.Now).TotalMinutes);
                }

            }
            [XmlIgnore]
            public double HoursUntil
            {
                get
                {
                    return Math.Round((Scheduled - DateTime.Now).TotalHours);
                }

            }
            [XmlIgnore]
            public double DaysUntil
            {
                get
                {
                    var days = (Scheduled- DateTime.Now).TotalDays;
                    return Math.Round(days);
                }

            }
            [XmlElement]
            public DateTime Scheduled { get; set; }
            [XmlElement]
            public DateTime Expires { get; set; }
            [XmlIgnore]
            public bool IsScheduledTime
            {
                get
                {
                    if (MinutesUntil>0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            [XmlIgnore]
            public bool IsExpired
            {
                get
                {
                    if ((Expires-Created).TotalSeconds > 0)
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
            [XmlElement]
            public bool IsExpirable { get; set; }
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
        public class NotificationEvent : EventArgs
        {
            public NotificationEvent(Notice notice = null, ListChangedEventArgs changeArguments=null)
            {               
               this.Notice = notice;
                this.ChangeArguments = changeArguments;
            }
            public Notice Notice { get; set; }
            public ListChangedEventArgs ChangeArguments { get; set; }
        }
        public class ExcpetionEvent : EventArgs
        {
            public ExcpetionEvent(Exception ex = null)
            {
                this.Exception = ex;
            }
            public Exception Exception { get; set; }
        }
      
        public event EventHandler<ExcpetionEvent> OnException;
        protected  virtual void HandleException(ExcpetionEvent e)
        {
            try
            {
                EventHandler<ExcpetionEvent> handler = OnException;
                if (handler != null)
                {
                    handler(this, e);
                }
            }
            catch { }
        }

        public event EventHandler<NotificationEvent> OnNonTimedNoticeTick;
        protected virtual void HandleNonTimedNotice(NotificationEvent e)
        {
            try
            {
                EventHandler<NotificationEvent> handler = OnNonTimedNoticeTick;
                if (handler != null)
                {
                    handler(this, e);
                }
            }
            catch { }
        }

        public event EventHandler<NotificationEvent> OnScheduledTime;
        protected virtual void HandleOnScheduledTime(NotificationEvent e)
        {
            try
            {
                EventHandler<NotificationEvent> handler = OnScheduledTime;
                if (handler != null)
                {
                    handler(this, e);
                }
            }
            catch {  }
        }
        public event EventHandler<NotificationEvent> OnNoticeExpired;
        protected virtual void HandleExpiredNotice(NotificationEvent e)
        {
            try
            {
                EventHandler<NotificationEvent> handler = OnNoticeExpired;
                if (handler != null)
                {
                    handler(this, e);
                }
            }
            catch { }
        }
        public event EventHandler<NotificationEvent> OnNoticeChanged;
        protected virtual void HandleChangedNotice(NotificationEvent e)
        {
            try
            {
                EventHandler<NotificationEvent> handler = OnNoticeChanged;
                if (handler != null)
                {
                    handler(this, e);
                }
            }
            catch { }
        }
        public event EventHandler<NotificationEvent> OnNoticeRemoved;
        protected virtual void HandleRemovedNotice(NotificationEvent e)
        {
            try
            {
                EventHandler<NotificationEvent> handler = OnNoticeRemoved;
                if (handler != null)
                {
                    handler(this, e);
                }
            }
            catch { }
        }
        public event EventHandler<NotificationEvent> OnNoticeAdded;
        protected virtual void HandleAddedNotice(NotificationEvent e)
        {
            try
            {
                EventHandler<NotificationEvent> handler = OnNoticeAdded;
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

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion
    }
}