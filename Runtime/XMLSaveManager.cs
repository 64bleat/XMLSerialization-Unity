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
        public UnityEvent onSave;
        public UnityEvent onLoad;

        private static float originalTime;

        private void OnDestroy()
        {
            GameObjectXML.UnloadResources();
        }

        /// <summary> Serializes and saves the current scene. </summary>
        public void Quicksave()
        {
            float originalTime = Time.timeScale;

            Time.timeScale = 0;
            XMLSerialization.Save(new SceneXML(SceneManager.GetActiveScene()), "Quicksave");
            Time.timeScale = originalTime;
            onSave?.Invoke();
        }

        /// <summary> Deserializes a scene and loads it it is not loaded. </summary>
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

        private void InvokeOnLoad()
        {
            SceneXML.OnSceneSerialized -= InvokeOnLoad;
            Time.timeScale = originalTime;
            onLoad?.Invoke();
        }
    }
}
