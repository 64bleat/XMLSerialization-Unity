using SHK.Items;

namespace Serialization
{
    [XMLSurrogate(typeof(InventoryObject))]
    public class InventoryObjectXML : XMLSurrogate
    {
        public bool enableCountDown;
        public float lifeTime;

        public override XMLSurrogate Serialize(object o)
        {
            InventoryObject i = o as InventoryObject;

            if(i)
            {
                enableCountDown = i.countDownDestroy;
                lifeTime = i.lifeTime;
            }

            return this;
        }

        public override XMLSurrogate Deserialize(object o)
        {
            InventoryObject i = o as InventoryObject;

            if(i)
            {
                i.countDownDestroy = enableCountDown;
                i.lifeTime = lifeTime;
            }

            return this;
        }
    }
}
