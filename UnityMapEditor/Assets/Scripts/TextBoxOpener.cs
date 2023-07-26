using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class TextBoxOpener : MonoBehaviour
{
    public GameObject textBox; // Reference to the Text element

    public void OpenTextBox()
    {
        textBox.SetActive(!textBox.activeSelf);
    }
}
