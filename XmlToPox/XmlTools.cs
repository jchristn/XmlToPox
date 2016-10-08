using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace XmlToPox
{
    public class XmlTools
    {
        #region Public-Static-Methods

        /// <summary>
        /// Converts XML to plain old XML (POX).
        /// </summary>
        /// <param name="xml">The XML string.</param>
        /// <returns>A plain old XML string.</returns>
        public static string Convert(string xml)
        {
            return SanitizeXml(xml);
        }

        /// <summary>
        /// Using XPath, query the XML document.
        /// </summary>
        /// <param name="xml">The XML string.</param>
        /// <param name="path">The path, i.e. /toplevel/parent/key, or in the case of an array, /toplevel/parent[1]/key (index always starts with 1!).</param>
        /// <returns>The contents at the specified path.</returns>
        public static string QueryXml(string xml, string path)
        {
            try
            {
                if (String.IsNullOrEmpty(xml)) return null;
                if (String.IsNullOrEmpty(path)) return null;

                string sanitized = SanitizeXml(xml);
                StringReader sr = new StringReader(sanitized);
                XPathDocument xpd = new XPathDocument(sr);
                XPathNavigator xpn = xpd.CreateNavigator();
                XPathNodeIterator xni = xpn.Select(path);
                string response = null;

                while (xni.MoveNext())
                {
                    if (xni.Current.SelectSingleNode("*") != null)
                    {
                        response = QueryXmlProcessChildren(xni);
                    }
                    else
                    {
                        response = xni.Current.Value;
                    }
                }

                return response;
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region Private-Static-Methods

        private static string XmlEscape(string val)
        {
            if (String.IsNullOrEmpty(val)) return null;
            XmlDocument doc = new XmlDocument();
            var node = doc.CreateElement("root");
            node.InnerText = val;
            return node.InnerXml;
        }

        private static string SanitizeXml(string xml)
        {
            if (String.IsNullOrEmpty(xml)) return null;

            string ret = "";
            XmlDocument doc = new XmlDocument();
            using (StringReader sr = new StringReader(xml))
            {
                using (XmlTextReader xtr = new XmlTextReader(sr) { Namespaces = false })
                {
                    doc.LoadXml(xml);
                }
            }

            if (doc == null) return null;

            using (StringWriter sw = new StringWriter())
            {
                using (XmlWriter xtw = XmlWriter.Create(sw))
                {
                    doc.WriteTo(xtw);
                    xtw.Flush();

                    ret = sw.GetStringBuilder().ToString();
                }
            }

            if (String.IsNullOrEmpty(ret)) return null;

            // remove all namespaces
            XElement xe = XmlRemoveNamespace(XElement.Parse(xml));

            // remove null fields from string
            Regex rgx = new Regex("\\n*\\s*<([\\w_]+)></([\\w_]+)>\\n*");

            return rgx.Replace(xe.ToString(), "");
        }

        private static XElement XmlRemoveNamespace(XElement xml)
        {
            try
            {
                xml.RemoveAttributes();
                if (!xml.HasElements)
                {
                    XElement xe = new XElement(xml.Name.LocalName);
                    xe.Value = xml.Value;

                    foreach (XAttribute attribute in xml.Attributes())
                        xe.Add(attribute);

                    return xe;
                }
                return new XElement(xml.Name.LocalName, xml.Elements().Select(el => XmlRemoveNamespace(el)));
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string QueryXmlProcessChildren(XPathNodeIterator xpni)
        {
            try
            {
                XPathNodeIterator child = xpni.Current.SelectChildren(XPathNodeType.All);

                while (child.MoveNext())
                {
                    if (child.Current.SelectSingleNode("*") != null) QueryXmlProcessChildren(child);
                }

                return child.Current.Value;
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion
    }
}
