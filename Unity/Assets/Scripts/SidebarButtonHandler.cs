using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SidebarButtonHandler : MonoBehaviour
{
    public Button sidebarButton;

    void Start()
    {
        Button btn = sidebarButton.GetComponent<Button>();
        string btnText = btn.name;
        btn.onClick.AddListener(() => TaskOnClick(btnText));
    }
    public void TaskOnClick(string btnText)
    {
        //Output this to console when Button1 or Button3 is clicked
        print("You have selected the '" + btnText + "' Tile!");
    }
}

