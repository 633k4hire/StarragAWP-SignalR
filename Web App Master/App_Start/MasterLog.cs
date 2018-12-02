using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Web_App_Master.App_Start
{
    [Serializable]
    public class MasterLog:Serializers.Serializer<MasterLog>
    {
        public  string LastEntry()
        {
            return Entries.Last();
        }
        public  void Add(string entry)
        {
            Entries.Add(entry);
            Push.MasterLog(this);
        }
        [XmlElement]
        public  List<string> Entries = new List<string>();
    }
}