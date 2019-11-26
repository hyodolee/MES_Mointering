using System;
using System.Text;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Data;

namespace SmartFactory
{
    public class Fomatter
    {
        public static String XmlWriter(Object obj)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());
            using (MemoryStream memoryStream = new MemoryStream())
            {
                xmlSerializer.Serialize(memoryStream, obj);
                return Encoding.UTF8.GetString(memoryStream.GetBuffer()).ToString();
            }
        }

        public static Object XmlDeserialize(String xmlData)
        {
            object obj;
            using (XmlReader reader = XmlReader.Create(new StringReader(xmlData)))
            {
                reader.MoveToContent();
                switch (reader.Name)
                {
                    case "Machine":
                        obj = new XmlSerializer(typeof(Machine)).Deserialize(reader);
                        break;
                    case "ArrayOfMachine":
                        obj = new XmlSerializer(typeof(List<Machine>)).Deserialize(reader);
                        break;
                    default:
                        throw new NotSupportedException("Unexpected: " + reader.Name);
                };
                return obj;
            }
        }

        public static DataTable xmlDataToDataTable(string xmlData)
        {
            try
            {
                DataSet dataSet = new DataSet();
                dataSet.ReadXml(new System.IO.StringReader(xmlData));
                return dataSet.Tables[0];
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return new DataTable();
            }

        }
    }
}
