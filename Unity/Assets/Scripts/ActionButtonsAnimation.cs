using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class handles the animation for action buttons in a Unity scene.
/// </summary>
public class ActionButtonsAnimation : MonoBehaviour
{

    /// <summary>
    /// This method is called when the game object is enabled.
    /// It initializes the game object's scale and animates it using LeanTween.
    /// </summary>
    private void OnEnable()
    {
        gameObject.transform.localScale = new Vector3(0, 0, 0);
        LeanTween.scale(gameObject, new Vector3(0.01f, 0.01f, 0.01f), 0.1f);
    }

    /// <summary>
    /// This method is called to animate the action button when it moves.
    /// It resets the game object's scale and animates it using LeanTween.
    /// </summary>
    public void onMove()
    {
        gameObject.transform.localScale = new Vector3(0, 0, 0);
        LeanTween.scale(gameObject, new Vector3(0.01f, 0.01f, 0.01f), 0.1f);
    }
}