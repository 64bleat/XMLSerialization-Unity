using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Serialization
{
    [XMLSurrogate(typeof(GameObjectPool))]
    public class GameObjectPoolXML : XMLSurrogate
    {
        public string resourcePath;
        public GameObjectXML[] instances;

        public override XMLSurrogate Serialize(object o)
        {
            if(o is GameObjectPool p && p)
            {
                List<GameObjectXML> serializedInstances = new List<GameObjectXML>();

                foreach (Transform c in p.transform)
                    if (c.gameObject.activeSelf)
                        serializedInstances.Add(new GameObjectXML(c.gameObject));

                instances = serializedInstances.ToArray();
                resourcePath = p.resourcePath;
            }

            return this;
        }

        public override XMLSurrogate Deserialize(object o)
        {
            if (o is GameObjectPool p && p)
            {
                GameObject prefab = Resources.Load<GameObject>(resourcePath);

                p.resourcePath = resourcePath;
                p.prefab = prefab;

                // Destroy any existing pool
                if (GameObjectPool.pools.TryGetValue(prefab, out var old))
                {
                    Object.DestroyImmediate(old.gameObject);
                    GameObjectPool.pools.Remove(prefab);
                }
                GameObjectPool.pools.Add(prefab, p);

                // Instantiate Serialized Instances
                foreach(GameObjectXML gInfo in instances)
                {
                    TransformXML tInfo = gInfo.componentData[0] as TransformXML;
                    GameObject instance = Object.Instantiate(prefab,tInfo.position, tInfo.rotation, p.transform);
                    instance.SetActive(false);

                    if(gInfo.activeSelf)
                        gInfo.DeserializeGameObject(instance);
                    else
                        p.availableInstances.Push(instance);
                }
            }

            return this;
        }
    }
}
