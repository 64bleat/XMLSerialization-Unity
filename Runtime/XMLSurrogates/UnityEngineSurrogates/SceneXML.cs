using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Serialization
{
    [System.Serializable]
    [XMLSurrogate(typeof(Scene))]
    public class SceneXML : XMLSurrogate
    {
        [XmlAttribute] public int buildIndex;
        [XmlAttribute] public int handle;
        public GameObjectXML[] gameObjectData;

        /// <summary> called immediately after a scene has been fully deserialized </summary>
        public static event Action OnSceneSerialized;

        public SceneXML()
        {
            // Required for serialization
        }

        public SceneXML(Scene s)
        {
            Serialize(s);
        }

        public override XMLSurrogate Serialize(dynamic o)
        {
            if(o is Scene s && SceneManager.GetActiveScene().buildIndex == s.buildIndex)
            { 
                List<GameObjectXML> gameObjects = new List<GameObjectXML>();

                GatherGameObjectXML(gameObjects, s.GetRootGameObjects());
                handle = s.handle;
                buildIndex = s.buildIndex;
                gameObjectData = gameObjects.ToArray();
            }

            return this;
        }

        public override XMLSurrogate Deserialize(object o)
        {
            if(o is Scene currentScene)
            {
                if (SceneManager.GetActiveScene().buildIndex != buildIndex)
                {
                    SceneManager.sceneLoaded += DeserializeOnSceneLoaded;
                    SceneManager.LoadScene(buildIndex);
                }
                else
                    DeserializeOnSceneLoaded(currentScene, LoadSceneMode.Single);
            }

            return this;
        }

        private void DeserializeOnSceneLoaded(Scene loadedScene, LoadSceneMode m)
        {
            SceneManager.sceneLoaded -= DeserializeOnSceneLoaded;

            DestroyNonpersistentGameObjects(loadedScene.GetRootGameObjects());

            foreach (GameObjectXML goxml in gameObjectData)
                goxml.Deserialize(null);

            OnSceneSerialized?.Invoke();
            OnSceneSerialized = null;
        }

        private static void DestroyNonpersistentGameObjects(GameObject[] list)
        {
            if(list != null)
                foreach(GameObject go in list)
                    if(go)
                        if (go.TryGetComponent(out XMLSerializeable sInfo) && (sInfo.persistentID == null || sInfo.persistentID.Length == 0))
                            Object.DestroyImmediate(go);
                        else
                            DestroyNonpersistentGameObjects(TransformXML.GetChildGameObjects(go));
        }

        private static void GatherGameObjectXML(List<GameObjectXML> saveObjects, GameObject[] list)
        {
            foreach (GameObject gameObject in list)
            {
                if (gameObject.TryGetComponent(out XMLSerializeable _))
                    saveObjects.Add(new GameObjectXML(gameObject));

                GatherGameObjectXML(saveObjects, TransformXML.GetChildGameObjects(gameObject));
            }
        }
    }
}
