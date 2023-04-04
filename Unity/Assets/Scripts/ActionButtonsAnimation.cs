using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionButtonsAnimation : MonoBehaviour
{
    private void OnEnable()
    {
        gameObject.transform.localScale = new Vector3(0, 0, 0);
        LeanTween.scale(gameObject, new Vector3(0.01f, 0.01f, 0.01f), 0.1f);
    }

    public void onMove()
    {
        gameObject.transform.localScale = new Vector3(0, 0, 0);
        LeanTween.scale(gameObject, new Vector3(0.01f, 0.01f, 0.01f), 0.1f);
    }
}