//https://learn.unity.com/tutorial/create-a-simple-messaging-system-with-events
//TODO: check if Unity can support Covariance

using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

/// <summary>
/// This class manages the event system and allows for listening, stopping listening, and triggering events.
/// </summary>
public class EventManager : MonoBehaviour
{

    private Dictionary<System.Type, UnityEvent<Dictionary<string, object>>> eventDictionary;

    private static EventManager eventManager;

    /// <summary>
    /// Singleton instance of EventManager.
    /// </summary>
    public static EventManager Instance
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

    /// <summary>
    /// Initializes the EventManager with an empty event dictionary.
    /// </summary>
    void Init()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<System.Type, UnityEvent<Dictionary<string, object>>>();
        }
    }

    /// <summary>
    /// Registers a listener for an event of the specified type.
    /// </summary>
    /// <param name="type">The type of the event to listen for.</param>
    /// <param name="listener">The listener to register.</param>
    public static void StartListening(System.Type type, UnityAction<Dictionary<string, object>> listener)
    {
        UnityEvent<Dictionary<string, object>> thisEvent = null;
        if (Instance.eventDictionary.TryGetValue(type, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent<Dictionary<string, object>>();
            thisEvent.AddListener(listener);
            Instance.eventDictionary.Add(type, thisEvent);
        }
    }

    /// <summary>
    /// Unregisters a listener for an event of the specified type.
    /// </summary>
    /// <param name="type">The type of the event to stop listening for.</param>
    /// <param name="listener">The listener to unregister.</param>
    public static void StopListening(System.Type type, UnityAction<Dictionary<string, object>> listener)
    {
        if (eventManager == null) return;
        UnityEvent<Dictionary<string, object>> thisEvent = null;
        if (Instance.eventDictionary.TryGetValue(type, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    /// <summary>
    /// Triggers an event of the specified action.
    /// </summary>
    /// <param name="action">The action to trigger the event for.</param>
    public static void TriggerEvent(IAction action)
    {
        UnityEvent<Dictionary<string, object>> thisEvent = null;
        if (Instance.eventDictionary.TryGetValue(action.GetType(), out thisEvent))
        {
            thisEvent.Invoke(action.toDict());
        }
    }
}
