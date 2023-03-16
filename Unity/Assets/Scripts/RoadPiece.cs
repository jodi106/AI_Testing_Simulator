using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Drag;

public class RoadPiece : MonoBehaviour {

    public RoadPiece piece;
    Vector2 position;
    float rotation;

    // Start is called before the first frame update
    void Start()
    {
        piece.position = piece.transform.position;
        piece.rotation = 0;
    }

    private void Update()
    {
         GameObject square = GameObject.Find("Square");
         snapToSquare(piece, square);
    }

    void align(RoadPiece current)
    {
        /*if (pieces.Count > 0)
        {
            RoadPiece neighbor = getClosestPiece(pieces, current); 
            if (neighbor != null)
            {
                Vector2 newPos = calcPosition(neighbor);
                positionPiece(current, newPos, neighbor.rotation);
            }
        }*/


        current.positionPiece(current, current.calcPosition(), 15);
        current.position = current.transform.position;


    }

    /**
     * method to snap object to square - test
     */
    void snapToSquare(RoadPiece current, GameObject square)
    {
        Vector2 sqr_pos = square.transform.position;
        print("Inside from right" + (sqr_pos.x + 550 > current.position.x)); 
        print("Inside from left" + (sqr_pos.x - 550 < current.position.x)); 
        print("Inside from top" + (sqr_pos.y + 550 > current.position.y)); 
        print("Inside from bottom" + (sqr_pos.y + 550 < current.position.y)); 
        if (sqr_pos.x + 550 > current.position.x && sqr_pos.x - 550 < current.position.x && sqr_pos.y + 550 > current.position.y && sqr_pos.y - 550 < current.position.y)
        {
            print("in");
            positionPiece(current, sqr_pos, 0);
        }
        current.position = current.transform.position;
    }

    /**
     * changes the position of the piece current to the given position
     */
    void positionPiece (RoadPiece current, Vector2 position, double rotation)
    {
        transform.position = Vector2.MoveTowards(current.position, position, 100000000);
    }

    void rotate(RoadPiece piece, float degree)
    {

        piece.rotation = degree;
    }

    /**
     * calculates the position of the piece, which should be draged to the map
     * based on position of the next closest road piece
     */
    Vector2 calcPosition()
    {
        Vector2 position;
        position.x = -3200;
        position.y = -31000;

        //position.x = Mathf.Cos(rotation) * radius;
        //position.y = Mathf.Sin(rotation) * radius;
        


        return position;
    }

    /**
     * searches the closest road piece to the moving piece by comparing the euclidian distance
     */
    RoadPiece getClosestPiece(Dictionary<RoadPiece, Vector2> pieces, RoadPiece current)
    {
        RoadPiece closest = null;
        Dictionary<RoadPiece, float> distances = new Dictionary<RoadPiece, float>();
        Vector2 position;
        float positionX;
        float positionY;
        float eukl_dist;
        foreach (KeyValuePair<RoadPiece, Vector2> piece in pieces)
        {
            position = piece.Value;
            positionX = position.x;
            positionY = position.y;
            eukl_dist = Mathf.Sqrt((float)SnapController.FastEuclideanDistance(position, current.position));
            distances.Add(piece.Key, eukl_dist);
        };
        float min_dist = int.MaxValue;
        foreach (KeyValuePair<RoadPiece, float> distance in distances) {
            if (min_dist < distance.Value)
            {
                min_dist = distance.Value;
                closest = distance.Key;
            }    
        };
        if (min_dist > 10)
        {
            closest = null;
        }
        return closest;
    }
    
}