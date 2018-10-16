using Helpers;
using Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using Web_App_Master;

namespace Sync
{
    [Serializable]
    public class SyncUpdate:Serializers.Serializer<SyncUpdate>
    {
        public static SyncUpdate Load(string xml)
        {
            SyncUpdate syn = new SyncUpdate();
            syn = syn.DeserializeFromXmlString<SyncUpdate>(xml);
            return syn;
        }
        public static string Save(SyncUpdate syn)
        {
            return syn.SerializeToXmlString(syn);
        }
        [XmlElement]
        public DataStore Library { get; set; }
        [XmlElement]
        public Settings Settings { get; set; }
        [XmlElement]
        public NotificationSystem Notices { get; set; }
        public SyncUpdate() { }
        public SyncUpdate(List<Asset> assets, DataStore _Library,Settings _Settings, NotificationSystem _Notices)
        {
            Library = _Library;
            Settings = _Settings;
            Notices = _Notices;
            Library.Assets = assets;
        }
        public SyncUpdate(List<Asset> assets, string _Library, string _Settings, string _Notices)
        {
            Library = new DataStore();
            Settings = new Settings();
            Notices = new NotificationSystem();


            Library = Library.DeserializeFromXmlString<DataStore>(  _Library);
            Settings = Settings.DeserializeFromXmlString<Settings>( _Settings);
            Notices = Notices.DeserializeFromXmlString<NotificationSystem>( _Notices);
            Library.Assets = assets;
        }


    }
}