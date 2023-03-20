using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SidebarButtonHandler : MonoBehaviour
{
    public Button sidebarButton;
    public GameObject canvas; 
    public GameObject roadPiece; 

    void Start()
    {
        Button btn = sidebarButton.GetComponent<Button>();
        string btnText = btn.name;
        //btn.onClick.AddListener(() => TaskOnClick(btnText));
        btn.onClick.AddListener(() => createRoadPiece());  
    }

    void createRoadPiece()
    {
        var pos = new Vector3(0, 0, 1); 
        var createImage = Instantiate(roadPiece, pos, Quaternion.identity) as GameObject;
        createImage.transform.localScale += new Vector3(7, 7, 0);
        createImage.transform.SetParent(canvas.transform, false); 
    }




    public void TaskOnClick(string btnText)
    {
        //Output this to console when Button1 or Button3 is clicked
        print("You have selected the '" + btnText + "' Tile!");
    }
}

