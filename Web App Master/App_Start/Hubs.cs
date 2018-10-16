using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Hub
{
    #region Generics
    public class UpdateRequestEvent : EventArgs
    {
        public UpdateRequestEvent(object obj = null)
        {
            if (obj != null)
            {
                Data = obj;
            }
        }
        public object Data { get; set; }
    }
    [Serializable]
    public class Hub: Serializers.Serializer<Hub>
    {

        public event EventHandler<UpdateRequestEvent> OnDataOut;
        protected virtual void UpdateDataOutSubscibers(UpdateRequestEvent e)
        {
            try
            {
                EventHandler<UpdateRequestEvent> handler = OnDataOut;
                if (handler != null)
                {
                    handler(this, e);
                }
            }
            catch (Exception ex) { UpdateDataOutSubscibers(new UpdateRequestEvent(ex)); }
        }
        public event EventHandler<UpdateRequestEvent> OnDataIn;
        protected virtual void UpdateDataInSubscibers(UpdateRequestEvent e)
        {
            try
            {
                EventHandler<UpdateRequestEvent> handler = OnDataIn;
                if (handler != null)
                {
                    handler(this, e);
                }
            }
            catch (Exception ex) { UpdateDataInSubscibers(new UpdateRequestEvent(ex)); }
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

    }
    public interface iHub: ICloneable
    {
        string Serialize();
        Object Deserialize(string xml);
        object Pull();
        string Push();

    }
    #endregion

    [Serializable]
    public class AssetHub : Hub, iHub
    {
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public object Deserialize(string xml)
        {
            return this.DeserializeFromXmlString<AssetHub>(xml);
        }

        public string Serialize()
        {
            return this.SerializeToXmlString(this);
        }

        public object Pull()
        {
            throw new NotImplementedException();
        }

        public string Push()
        {
            throw new NotImplementedException();
        }

        private List<Asset> m_assets;
        [XmlElement]
        public List<Asset> Assets
        {
            get { return m_assets; }
            set { m_assets = value; }
        }

    }
    [Serializable]
    public class CustomerHub :Hub, iHub
    {
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public object Deserialize(string xml)
        {
            return this.DeserializeFromXmlString<CustomerHub>(xml);
        }

        public string Serialize()
        {
            return this.SerializeToXmlString(this);
        }
        public object Pull()
        {
            throw new NotImplementedException();
        }

        public string Push()
        {
            throw new NotImplementedException();
        }

        private List<Customer> m_customers;
        [XmlElement]
        public List<Customer> Customers
        {
            get { return m_customers; }
            set { m_customers = value; }
        }
    }
    [Serializable]
    public class PersonnelHub :Hub, iHub
    {
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public object Deserialize(string xml)
        {
            return this.DeserializeFromXmlString<PersonnelHub>(xml);
        }

        public string Serialize()
        {
            return this.SerializeToXmlString(this);
        }

        public object Pull()
        {
            throw new NotImplementedException();
        }

        public string Push()
        {
            throw new NotImplementedException();
        }
    }
    [Serializable]
    public class TransactionHub :Hub, iHub
    {
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public object Deserialize(string xml)
        {
            return this.DeserializeFromXmlString<TransactionHub>(xml);
        }

        public string Serialize()
        {
            return this.SerializeToXmlString(this);
        }

        public object Pull()
        {
            throw new NotImplementedException();
        }

        public string Push()
        {
            throw new NotImplementedException();
        }
    }
    [Serializable]
    public class NotificationHub :Hub, iHub
    {
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public object Deserialize(string xml)
        {
            return this.DeserializeFromXmlString<NotificationHub>(xml);
        }

        public string Serialize()
        {
            return this.SerializeToXmlString(this);
        }

        public object Pull()
        {
            throw new NotImplementedException();
        }

        public string Push()
        {
            throw new NotImplementedException();
        }
    }
    [Serializable]
    public class SyncHub :Hub, iHub
    {
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public object Deserialize(string xml)
        {
            return this.DeserializeFromXmlString<SyncHub>(xml);
        }

        public string Serialize()
        {
            return this.SerializeToXmlString(this);
        }

        public object Pull()
        {
            throw new NotImplementedException();
        }

        public string Push()
        {
            throw new NotImplementedException();
        }
    }
    [Serializable]
    public class DataHub :Hub, iHub
    {
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public object Deserialize(string xml)
        {
            return this.DeserializeFromXmlString<DataHub>(xml);
        }

        public string Serialize()
        {
            return this.SerializeToXmlString(this);
        }

        public object Pull()
        {
            throw new NotImplementedException();
        }

        public string Push()
        {
            throw new NotImplementedException();
        }
    }

}
