
using System;
using System.Collections;
using System.Collections.Generic;

using Entity;

using UnityEngine;
using static Drag;

public class RoadPiece : MonoBehaviour {


    Boolean rotateable = false;

    public Road road;

    public RoadType roadType;

    private RoadPieceController roadPieceController;


    // Start is called before the first frame update
    void Start()
    {

        rotateable = true;

        var mainCamera = GameObject.Find("Main Camera");
        this.roadPieceController = mainCamera.GetComponent<RoadPieceController>();

        Renderer renderer = GetComponent<Renderer>();

        // Get the bounds of the renderer
        Bounds bounds = renderer.bounds;

        // Get a reference to the Box Collider component attached to the game object
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();

        // Set the size of the collider to match the size of the game object
        boxCollider.size = bounds.size;

    }

    private void Update()
    {
        // Test with to snap to simple rectangle
        //GameObject square = GameObject.Find("Square");
        //snapToSquare(piece, square);
        if (rotateable && Input.GetKeyDown(KeyCode.A))
        {
            this.transform.Rotate(0, 0, 15);

        }
        else if(rotateable && Input.GetKeyDown(KeyCode.D))
        {
            this.transform.Rotate(0, 0, -15);

        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            rotateable = false;
        }



    }

    void OnMouseUp()
    {
        var clickedObject = gameObject;

        roadPieceController.Align(clickedObject);


        // Code to handle the click event using the clickedObject reference
    }



}