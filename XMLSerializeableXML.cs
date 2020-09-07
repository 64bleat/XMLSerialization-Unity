using System.Xml.Serialization;

namespace Serialization
{
    [System.Serializable]
    [XMLSurrogate(typeof(XMLSerializeable))]
    public class XMLSerializeableXML : XMLSurrogate
    {
        [XmlAttribute] public string resourceID;
        [XmlAttribute] public string persistentID;

        public override XMLSurrogate Serialize(object o)
        {
            XMLSerializeable c = o as XMLSerializeable;

            if (c)
            {
                resourceID = c.resourceID;
                persistentID = c.persistentID;
            }

            return this;
        }

        public override XMLSurrogate Deserialize(object o)
        {
            XMLSerializeable c = o as XMLSerializeable;

            if (c)
            {
                c.resourceID = resourceID;
                persistentID = c.persistentID;
            }

            return this;
        }
    }
}
