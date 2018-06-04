using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListenerList : MonoBehaviour
{
    public List<GameEventListener> gameEventListenersList;

    private void OnEnable()
    {
        foreach(GameEventListener listener in gameEventListenersList)
        {
            if (listener.Event != null)
            {
                listener.Event.RegisterListener(listener);
            }
            else
            {
                Debug.LogError("В "+listener.name+" отсутствует Event");
            }
        }
    }

    private void OnDisable()
    {
        foreach (GameEventListener listener in gameEventListenersList)
        {
            if (listener.Event != null)
            {
                listener.Event.UnregisterListener(listener);
            }
            else
            {
                Debug.LogError("В " + listener.name + " отсутствует Event");
            }
        }
    }
}

[System.Serializable]
public class GameEventListener
{
    public string name;

    [Tooltip("Event to register with.")]
    public GameEvent Event;

    [Tooltip("Response to invoke when Event is raised.")]
    public UnityEvent Response;

    public virtual void OnEventRaised()
    {
        Response.Invoke();
    }
}