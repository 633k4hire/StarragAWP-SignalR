using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
namespace Sync
{
    class XmlHelper
    {
        private void AddData(XmlWriter writer, string rootTitle, string contentTitle, string[] contents)
        {
            try
            {
                writer.WriteStartElement(rootTitle);
                try
                {
                    foreach (var a in contents)
                    {
                        writer.WriteStartElement(contentTitle);
                        writer.WriteString(a);
                        writer.WriteEndElement();
                    }

                }
                catch { }
                writer.WriteEndElement();

            }
            catch { }
        }

        public static string BlankXml(string msg="Blank")
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            StringBuilder builder = new StringBuilder();
            builder.Capacity = 5000;
            int i = 0;
            using (XmlWriter writer = XmlWriter.Create(builder, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement(msg);

                writer.WriteEndElement();
                writer.WriteEndDocument();


            }
            return builder.ToString();
        }
        public static string Combine(string title,string[] xmls)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            StringBuilder builder = new StringBuilder();
            builder.Capacity = 5000;
            int i = 0;
            using (XmlWriter writer = XmlWriter.Create(builder, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement(title);
                foreach (var item in xmls)
                {
                    writer.WriteStartElement(StripTitle(item));
                    writer.WriteString(Strip(item));
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();


            }          
            return builder.ToString();
        }
        private static string StripTitle(string xml)
        {
            try
                {
                var doc = new XmlDocument();

                doc.LoadXml(xml);

                //check for nulls

                var elemList = doc.ChildNodes[1];
                return elemList.Name;
            }
            catch { }
            return "";
        }
        private static string Strip(string xml)
        {
            try
            {
                var doc = new XmlDocument();

                doc.LoadXml(xml);

                //check for nulls

                var elemList = doc.ChildNodes[1];
                return elemList.InnerXml;
            }
            catch { }
            return xml.Replace("", "");
        }
    }
}