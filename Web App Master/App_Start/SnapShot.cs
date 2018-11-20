using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using Web_App_Master.Models;

namespace Web_App_Master.App_Start
{
    public class SnapShot
    {
    }
    [Serializable]
    public class SnapShotData: Local_Storage_Class
    {        
        public void Add(SnapShotEntry entry)
        {
            SnapShots.Add(entry);
            this.Write(this.Filename);
        }
       
        public SnapShotData()
        {
            this.Name = "Starrag Snap Shot Database";
            this.Id = "StarragAwpSnapShotData";           
        }
        [XmlElement]
        public List<SnapShotEntry> SnapShots = new List<SnapShotEntry>();
        [XmlElement]
        public DateTime Timestamp = DateTime.Now;
        [XmlElement]
        public string Filename = "/SnapShot/snap.db";

    }
    [Serializable]
    public class SnapShotEntry : Serializers.Serializer<SnapShotEntry>
    {
        [XmlElement]
        public DateTime Timestamp = DateTime.Now;
        [XmlIgnore]
        private string mXmlData = "";
        [XmlElement]
        public string XmlData
        {
            get { return mXmlData; }
            set { mXmlData = value; }
        }
        [XmlIgnore]
        private string mGuid = Guid.NewGuid().ToString();
        [XmlElement]
        public string Id { get { return mGuid; } set { mGuid = value; } }
        [XmlIgnore]
        private string mName = "SnapShotEntry";
        [XmlElement]
        public string Name { get { return mName; } set { mName = value; } }
        [XmlElement]
        public List<string> CurrentAssetInventory
        { 
            get { return (from a in Global.AssetCache where a.IsOut = false select a.AssetNumber).ToList(); }
        }
        

    }


}