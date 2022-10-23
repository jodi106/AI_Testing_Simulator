//https://learn.unity.com/tutorial/create-a-simple-messaging-system-with-events
//TODO: check if Unity can support Covariance

using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class EventManager : MonoBehaviour
{

    private Dictionary<System.Type, UnityEvent<Dictionary<string, object>>> eventDictionary;

    private static EventManager eventManager;

    public static EventManager instance
    {
        get
        {
            if (!eventManager)
            {
                eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;

                if (!eventManager)
                {
                    Debug.LogError("There needs to be one active EventManger script on a GameObject in your scene.");
                }
                else
                {
                    eventManager.Init();
                }
            }

            return eventManager;
        }
    }

    void Init()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<System.Type, UnityEvent<Dictionary<string, object>>>();
        }
    }

    public static void StartListening(System.Type type, UnityAction<Dictionary<string, object>> listener)
    {
        UnityEvent<Dictionary<string, object>> thisEvent = null;
        if (instance.eventDictionary.TryGetValue(type, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent<Dictionary<string, object>>();
            thisEvent.AddListener(listener);
            instance.eventDictionary.Add(type, thisEvent);
        }
    }

    public static void StopListening(System.Type type, UnityAction<Dictionary<string, object>> listener)
    {
        if (eventManager == null) return;
        UnityEvent<Dictionary<string, object>> thisEvent = null;
        if (instance.eventDictionary.TryGetValue(type, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public static void TriggerEvent(IAction action)
    {
        UnityEvent<Dictionary<string, object>> thisEvent = null;
        if (instance.eventDictionary.TryGetValue(action.GetType(), out thisEvent))
        {
            thisEvent.Invoke(action.toDict());
        }
    }
}
