using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ScribeUserConference.CRM2016.Plugins
{
    class PluginConfiguration
    {
        private static string GetValueNode(XmlDocument doc, string key)
        {
            XmlNode node = doc.SelectSingleNode(String.Format("Settings/setting[@name='{0}']", key));
            if (node != null)
            {
                return node.SelectSingleNode("value").InnerText;
            }
            return string.Empty;
        }

        public static Guid GetConfigDataGuid(XmlDocument doc, string label)
        {
            string tempString = GetValueNode(doc, label);

            if (tempString != string.Empty)
            {
                return new Guid(tempString);
            }
            return Guid.Empty;
        }

        public static bool GetConfigDataBool(XmlDocument doc, string label)
        {
            bool retVar;

            if (bool.TryParse(GetValueNode(doc, label), out retVar))
            {
                return retVar;
            }
            else
            {
                return false;
            }
        }

        public static int GetConfigDataInt(XmlDocument doc, string label)
        {
            int retVar;

            if (int.TryParse(GetValueNode(doc, label), out retVar))
            {
                return retVar;
            }
            else
            {
                return -1;
            }
        }

        public static string GetConfigDataString(XmlDocument doc, string label)
        {
            return GetValueNode(doc, label);
        }
    }
}

