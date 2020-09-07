using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Xml;
using System.Xml.Serialization;

namespace Serialization
{
    [System.Serializable]
    [XMLSurrogate(typeof(Scene))]
    public class SceneXML : XMLSurrogate
    {
        [XmlAttribute] public int buildIndex;
        [XmlAttribute] public int handle;
        public GameObjectXML[] gameObjectData;

        public SceneXML() { }
        public SceneXML(Scene s)
        {
            Serialize(s);
        }

        public override XMLSurrogate Serialize(object o)
        {
            Scene s = (Scene)o;
          
            if(s != null && SceneManager.GetActiveScene().buildIndex == s.buildIndex)
            { 
                List<GameObjectXML> gameObjects = new List<GameObjectXML>();
                GatherGameObjectXML(ref gameObjects, s.GetRootGameObjects());
                handle = s.handle;
                buildIndex = s.buildIndex;
                gameObjectData = gameObjects.ToArray();
            }

            return this;
        }

        public override XMLSurrogate Deserialize(object o)
        {
            Scene s = (Scene)o;

            if (s != null)
            {
                if (SceneManager.GetActiveScene().buildIndex != buildIndex)
                {
                    SceneManager.sceneLoaded += DeserializeOnLoad;
                    SceneManager.LoadScene(buildIndex);
                }
                else
                    DeserializeOnLoad(s, LoadSceneMode.Single);
            }

            return this;
        }

        private void DeserializeOnLoad(Scene s, LoadSceneMode m)
        {
            SceneManager.sceneLoaded -= DeserializeOnLoad;

            DestroyNonpersistentGameObjects(s.GetRootGameObjects());

            foreach (GameObjectXML goxml in gameObjectData)
                goxml.Deserialize(null);
        }

        private static void DestroyNonpersistentGameObjects(GameObject[] list)
        {
            if(list != null)
                foreach(GameObject go in list)
                    if(go)
                        if (go.GetComponent<XMLSerializeable>() is var sInfo && sInfo && (sInfo.persistentID == null || sInfo.persistentID.Length == 0))
                            Object.DestroyImmediate(go);
                        else
                            DestroyNonpersistentGameObjects(TransformXML.GetChildGameObjects(go));
        }

        private static void GatherGameObjectXML(ref List<GameObjectXML> saveObjects, GameObject[] list)
        {
            foreach (GameObject gameObject in list)
            {
                if (gameObject.GetComponent<XMLSerializeable>())
                    saveObjects.Add(new GameObjectXML(gameObject));

                GatherGameObjectXML(ref saveObjects, TransformXML.GetChildGameObjects(gameObject));
            }
        }
    }
}
