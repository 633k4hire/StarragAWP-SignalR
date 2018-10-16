

namespace Serializers
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    //BINARY
    [Serializable]
    public class MemoryBinarySerializable
    {
        public byte[] WriteMemory()
        {            
            byte[] bytes;
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(stream, this);
                bytes = stream.ToArray();
            }
            return bytes;
        }
        public static object ReadMemory(byte[] bytes)
        {
            object obj = null;
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(bytes, 0, bytes.Length);
                //stream.Flush();
                stream.Seek(0, SeekOrigin.Begin);
                BinaryFormatter bf = new BinaryFormatter();
                obj = (object)bf.Deserialize(stream);
            }
            return obj;
        }
    }
    [Serializable]
    public class FileBinarySerializable
    {
        public void Write(string path)
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                using (Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    formatter.Serialize(stream, this);
                }
            }
            catch { }
        }
        public static object Read(string path)
        {
            object obj = new object();
            try
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(path,
                                          FileMode.Open,
                                          FileAccess.Read,
                                          FileShare.Read);
                obj = (object)formatter.Deserialize(stream);
                stream.Close();
            }
            catch { }
            return obj;
        }
    }
    [Serializable]
    public class BinarySerializable : FileBinarySerializable
    {
        public byte[] WriteMemory()
        {
            byte[] bytes;
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(stream, this);
                bytes = stream.ToArray();
            }
            return bytes;
        }
        public static object ReadMemory(byte[] bytes)
        {
            object obj = null;
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(bytes, 0, bytes.Length);
                //stream.Flush();
                stream.Seek(0, SeekOrigin.Begin);
                BinaryFormatter bf = new BinaryFormatter();
                obj = (object)bf.Deserialize(stream);
            }
            return obj;
        }
    }

    /// <summary>
    /// Serialize a serializable object to XML string.
    /// </summary>
    /// <typeparam name="T">Type of object</typeparam>
    public class XmlSerializable<T>
    {
        /// <summary>
        /// Serialize a serializable object to XML string.
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="xmlObject">Type of object</param>
        /// <param name="useNamespaces">Use of XML namespaces</param>
        /// <returns>XML string</returns>
        public string SerializeToXmlString(object xmlObject, bool useNamespaces = false)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            MemoryStream memoryStream = new MemoryStream();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
            xmlTextWriter.Formatting = Formatting.Indented;

            if (useNamespaces)
            {
                XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
                xmlSerializerNamespaces.Add("", "");
                xmlSerializer.Serialize(xmlTextWriter, xmlObject, xmlSerializerNamespaces);
            }
            else
                xmlSerializer.Serialize(xmlTextWriter, xmlObject);

            string output = Encoding.UTF8.GetString(memoryStream.ToArray());
            string _byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
            if (output.StartsWith(_byteOrderMarkUtf8))
            {
                output = output.Remove(0, _byteOrderMarkUtf8.Length);
            }

            return output;
        }

        /// <summary>
        /// Serialize a serializable object to XML string and create a XML file.
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="xmlObject">Type of object</param>
        /// <param name="filename">XML filename with .XML extension</param>
        /// <param name="useNamespaces">Use of XML namespaces</param>
        public void SerializeToXmlFile(object xmlObject, string filename, bool useNamespaces = false)
        {
            try
            {
                File.WriteAllText(filename, SerializeToXmlString(xmlObject, useNamespaces));
            }
            catch
            {
                throw new Exception();
            }
        }

        /// <summary>
        /// Deserialize XML string to an object.
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="xml">XML string</param>
        /// <returns>XML-deserialized object</returns>
        public TT DeserializeFromXmlString<TT>(string xml) where TT : new()
        {
            TT xmlObject = new TT();
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(TT));
            StringReader stringReader = new StringReader(xml);
            if (xml=="")
            {
                return new TT();
            }
            xmlObject = (TT)xmlSerializer.Deserialize(stringReader);
            return xmlObject;
        }

        /// <summary>
        /// Deserialize XML string from XML file to an object.
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="filename">XML filename with .XML extension</param>
        /// <returns>XML-deserialized object</returns>
        public TT DeserializeFromXmlFile<TT>(string filename) where TT : new()
        {
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException();
            }

            return DeserializeFromXmlString<TT>(File.ReadAllText(filename));
        }
    }

    public class m_MultiSerialiazable<T>: XmlSerializable<T>{}
    public class Serializer<T> : m_MultiSerialiazable<T> { }
    
}
