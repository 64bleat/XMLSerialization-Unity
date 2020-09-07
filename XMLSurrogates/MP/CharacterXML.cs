using SHK.Creatures;
using SHK.Items;
using System.Collections.Generic;
using UnityEngine;

namespace Serialization
{
    [System.Serializable]
    [XMLSurrogate(typeof(Character))]
    public class CharacterXML : XMLSurrogate
    {
        public bool isPlayer;
        public InventoryXML[] inventory;
        public ResourceItemXML[] resources;

        public override XMLSurrogate Serialize(object o)
        {
            if (o is Character c && c)
            {
                isPlayer = c.isPlayer;
                
                // Inventory
                inventory = new InventoryXML[c.inventory.Count];
                for (int i = 0; i < inventory.Length; i++)
                    inventory[i] = new InventoryXML().Serialize(c.inventory[i]) as InventoryXML;

                // Resources
                resources = new ResourceItemXML[c.resources.Count];
                for (int i = 0; i < resources.Length; i++)
                    resources[i] = new ResourceItemXML(c.resources[i]);
            }

            return this;
        }

        public override XMLSurrogate Deserialize(object o)
        {
            if(o is Character c && c)
            {
                c.SetPlayer(isPlayer);

                // Inventory
                c.inventory.Clear();
                foreach(InventoryXML item in inventory)
                    if (Resources.Load<ScriptableObject>(item.resourcePath) is Inventory inv && inv)
                    {
                        if (!item.staticReference)
                            item.Deserialize(inv = Object.Instantiate(inv));

                        InventoryManager.PickUp(c, inv);
                    }

                // Resources
                for (int i = 0; i < resources.Length; i++)
                    if (i < c.resources.Count)
                    {
                        resources[i].Deserialize(c.resources[i]);
                        foreach (var action in c.resources[i].OnValueChange)
                            action?.Invoke(c.resources[i].resourceType, c.resources[i].value);
                    }
            }

            return this;
        }
    }

    [System.Serializable]
    public class ResourceItemXML : XMLSurrogate
    {
        public string resourcePath;
        public int value;
        public int maxValue;

        public ResourceItemXML() { }
        public ResourceItemXML(ResourceItem r)
        {
            Serialize(r);
        }

        public override XMLSurrogate Serialize(object o)
        {
            if (o is ResourceItem r)
            {
                resourcePath = r.resourceType.resourcePath;
                value = r.value;
                maxValue = r.maxValue;
            }

            return this;
        }

        public override XMLSurrogate Deserialize(object o)
        {
            if(o is ResourceItem r)
            {
                r.value = value;
                r.maxValue = maxValue;
                if (!r.resourceType || !r.resourceType.resourcePath.Equals(resourcePath))
                    r.resourceType = Resources.Load<ResourceType>(resourcePath);
            }

            return this;
        }

        public static ResourceItem FindResourceItem(List<ResourceItem> list, string resourcePath)
        {
            foreach (ResourceItem item in list)
                if (item.resourceType && item.resourceType.resourcePath.Equals(resourcePath))
                    return item;

            return null;
        }
    }
}
