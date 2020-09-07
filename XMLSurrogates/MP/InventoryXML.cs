using SHK.Items;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Serialization
{
    [System.Serializable]
    [XMLSurrogate(typeof(Inventory))]
    public class InventoryXML : XMLSurrogate
    {
        public string resourcePath;
        public bool staticReference;
        public int count;
        public int maxCount;

        public override XMLSurrogate Serialize(object o)
        {
            if(o is Inventory i && i)
            {
                resourcePath = i.resourcePath;
                staticReference = i.staticReference;
                count = i.count;
                maxCount = i.maxCount;
            }

            return this;
        }

        public override XMLSurrogate Deserialize(object o)
        {
            if(o is Inventory i && i)
            {
                if(!staticReference)
                {
                    i.count = count;
                    i.maxCount = maxCount;
                }
            }

            return this;
        }
    }
}
