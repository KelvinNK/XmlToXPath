using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace XmlToXPath
{
    class Program
    {
        private const string PathXml = "C:\\Diplomas\\teste4.xml";

        static void Main(string[] args)
        {
            using FileStream fs = new FileStream(PathXml, FileMode.Open);
            var doc = LoadDocument(fs);
            List<string> XPaths = new List<string>();
            foreach (XmlNode item in doc.ChildNodes)
            {
                XPaths.AddRange(ListHierarchyToXpath(item));
            }

            XPaths.Sort();
            StringBuilder XPathsBuilder = new StringBuilder();

            foreach (var item in XPaths)
            {
                XPathsBuilder.AppendLine(item);
            }

            File.WriteAllText("C:\\Users\\Kelvin\\Desktop\\Kelvin\\XmlToXPath\\XmlToXPath.txt", XPathsBuilder.ToString());
                
        }

        static List<string> ListHierarchyToXpath(XmlNode node)
        {

            List<string> XPaths = new List<string>();
            foreach (XmlNode item in node.ChildNodes)
            {
                if(item.ChildNodes.Count > 0)
                {
                    XPaths.AddRange(ListHierarchyToXpath(item));
                }
                var xPathNode = FindXPath(item);
                if (xPathNode is not null)
                    XPaths.Add(xPathNode);
            }
            return XPaths;
        }

        static XmlDocument LoadDocument(Stream input)
        {
            XmlDocument document = new XmlDocument();
            document.PreserveWhitespace = true;
            document.Load(input);

            return document;
        }

        static string FindXPath(XmlNode node)
        {
            StringBuilder builder = new StringBuilder();
            while (node != null)
            {
                switch (node.NodeType)
                {
                    case XmlNodeType.Attribute:
                        builder.Insert(0, "/@" + node.Name);
                        node = ((XmlAttribute)node).OwnerElement;
                        break;
                    case XmlNodeType.Element:
                        int index = FindElementIndex((XmlElement)node);
                        builder.Insert(0, "/" + node.Name + "[" + index + "]");
                        node = node.ParentNode;
                        break;
                    case XmlNodeType.Document:
                        return builder.ToString();
                    default:
                        return null;
                }
            }
            throw new ArgumentException("Node não existe");
        }

        static int FindElementIndex(XmlElement element)
        {
            XmlNode parentNode = element.ParentNode;
            if (parentNode is XmlDocument)
            {
                return 1;
            }
            XmlElement parent = (XmlElement)parentNode;
            int index = 1;
            foreach (XmlNode candidate in parent.ChildNodes)
            {
                if (candidate is XmlElement && candidate.Name == element.Name)
                {
                    if (candidate == element)
                    {
                        return index;
                    }
                    index++;
                }
            }
            throw new ArgumentException("Não foi possivel encontrar um elemento");
        }
    }
}
