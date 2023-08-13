using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

/// <summary>
/// This class handles the user guide, once the user presses on the help button.It will either show, or hide the guide
/// </summary>
public class ScrollViewOpener : MonoBehaviour
{
    public GameObject scrollView; // Reference to the Text element
    private static bool isUserGuideOpen = false;

    /// <summary>
    /// This method will open the scrollview or close it, dependent in which state the guide currently is
    /// </summary>
    public void OpenScrollView()
    {
        scrollView.SetActive(!scrollView.activeSelf);
        isUserGuideOpen = !isUserGuideOpen;
    }

    /// <summary>
    /// This method return the current state of the user guide. 
    /// </summary>
    /// <returns> Returns a boolean that states, whether the guide is currently open, or closed </returns>
    public static bool IsUserGuideOpen() { return isUserGuideOpen; }
}
