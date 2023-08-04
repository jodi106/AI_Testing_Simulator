using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class ScrollViewOpener : MonoBehaviour
{
    public GameObject scrollView; // Reference to the Text element
    private static bool isUserGuideOpen = false;

    /// <summary>
    /// 
    /// </summary>
    public void OpenScrollView()
    {
        scrollView.SetActive(!scrollView.activeSelf);
        isUserGuideOpen = !isUserGuideOpen;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns> </returns>
    public static bool IsUserGuideOpen() { return isUserGuideOpen; }
}
