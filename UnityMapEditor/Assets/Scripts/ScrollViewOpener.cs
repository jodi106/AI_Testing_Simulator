using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ScrollViewOpener : MonoBehaviour
{
    public GameObject scrollView; // Reference to the Text element
    private static bool isUserGuideOpen = false;

    public void OpenScrollView()
    {
        scrollView.SetActive(!scrollView.activeSelf);
        isUserGuideOpen = !isUserGuideOpen;
    }

    public static bool IsUserGuideOpen() { return isUserGuideOpen; }
}
