using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DebugHotkeys : MonoBehaviour
{
    [System.Serializable]
    public class HotkeyEvent
    {
        public KeyCode key;
        public UnityEvent onKeyPressed;
    }

    public List<HotkeyEvent> hotkeyEvents = new List<HotkeyEvent>();

    void Update()
    {
        foreach (var hotkey in hotkeyEvents)
        {
            if (Input.GetKeyDown(hotkey.key))
            {
                Debug.Log($"Hotkey pressed: {hotkey.key}");
                hotkey.onKeyPressed?.Invoke();
            }
        }
    }
}
