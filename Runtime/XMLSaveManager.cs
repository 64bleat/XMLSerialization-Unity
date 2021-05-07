using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Serialization
{
    /// <summary>
    /// A collection of methods that save the current scene and load saved games.
    /// </summary>
    public class XMLSaveManager : ScriptableObject
    {
        [SerializeField] private string fileName = "Quicksave";

        public UnityEvent onSave;
        public UnityEvent onLoad;

        private static float originalTime;

        private void OnDestroy()
        {
            GameObjectXML.UnloadResources();
        }

        /// <summary> 
        /// Serializes and saves the current scene to a file. 
        /// </summary>
        public void Quicksave()
        {
            originalTime = Time.timeScale;
            Time.timeScale = 0;

            Scene activeScene = SceneManager.GetActiveScene();
            SceneXML serializableScene = new SceneXML(activeScene);

            XMLSerialization.Save(serializableScene, fileName);
            onSave?.Invoke();

            Time.timeScale = originalTime;
        }

        /// <summary> 
        /// Deserializes a scene and pre-loads the scene if it is not loaded. 
        /// </summary>
        public void Quickload()
        {
            originalTime = Time.timeScale;
            Time.timeScale = 0;

            if (XMLSerialization.TryLoad("Quicksave", out SceneXML data))
            {
                SceneXML.OnSceneSerialized += InvokeOnLoad;
                data.Deserialize(SceneManager.GetActiveScene());
            }
        }

        /// <summary>
        /// This portion of QuickLoad must be completed during the <c>OnSceneLoaded</c> callback
        /// </summary>
        private void InvokeOnLoad()
        {
            SceneXML.OnSceneSerialized -= InvokeOnLoad;
            Time.timeScale = originalTime;
            onLoad?.Invoke();
        }
    }
}
