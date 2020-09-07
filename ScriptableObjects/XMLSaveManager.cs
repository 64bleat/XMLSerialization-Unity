using UnityEngine;
using UnityEngine.SceneManagement;

namespace Serialization
{
    /// <summary>
    /// A collection of methods that save the current scene and load saved games.
    /// </summary>
    public class XMLSaveManager : ScriptableObject
    {
        public BroadcastChannel messageChannel;
        public static bool isRunning = false;

        private void OnDestroy()
        {
            GameObjectXML.UnloadResources();
        }

        /// <summary> Serializes and saves the current scene. </summary>
        public void Quicksave()
        {
            float originalTime = Time.timeScale;

            Time.timeScale = 0;

            if (messageChannel)
                messageChannel.Broadcast("Quicksaved");

            XMLSerialization.Save(new SceneXML(SceneManager.GetActiveScene()), "Quicksave");

            Time.timeScale = originalTime;
        }

        /// <summary> Deserializes a scene and loads it it is not loaded. </summary>
        public void Quickload()
        {
            isRunning = true;
            float originalTime = Time.timeScale;
            Time.timeScale = 0;
            SceneXML data = XMLSerialization.Load<SceneXML>("Quicksave");

            if (data != null)
                data.Deserialize(SceneManager.GetActiveScene());

            if (messageChannel)
                messageChannel.Broadcast("Quickloaded");

            Time.timeScale = originalTime;
            isRunning = false;
        }
    }
}
