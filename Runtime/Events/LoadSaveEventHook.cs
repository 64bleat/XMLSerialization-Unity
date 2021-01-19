using UnityEngine;
using UnityEngine.Events;

namespace Serialization
{
    /// <summary>
    /// These events are called after loading and saving.
    /// </summary>
    public class LoadSaveEventHook : MonoBehaviour
    {
        public XMLSaveManager saveManager;
        public UnityEvent onLoad;
        public UnityEvent onSave;

        private void Awake()
        {
            saveManager.onLoad.AddListener(InvokeLoad);
            saveManager.onSave.AddListener(InvokeSave);
        }

        private void OnDestroy()
        {
            saveManager.onLoad.RemoveListener(InvokeLoad);
            saveManager.onSave.RemoveListener(InvokeSave);
        }

        private void InvokeLoad()
        {
            onLoad?.Invoke();
        }

        private void InvokeSave()
        {
            onSave?.Invoke();
        }
    }
}
