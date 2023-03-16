using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadPiece : MonoBehaviour { 

    Vector2 position;
    double rotation;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void align(RoadPiece current)
    {
        if (pieces.Count > 0)
        {
            RoadPiece neighbor = getClosestPiece(pieces, current);
            Vector2 newPos = calcPosition(neighbor);
            positionPiece(current, newPos, neighbor.rotation);
        }
        

    }

    void positionPiece (RoadPiece current, Vector2 position, double rotation)
    {


    }

    Vector2 calcPosition(RoadPiece neighbor)
    {
        Vector2 position;
        position.x = 0;
        position.y = 0;

    // x = r * cos(a) + posX
    // y = r * sin(a) + posY

        return position;
    }

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
            eukl_dist = (float) SnapController.FastEuclideanDistance(position, current.position);
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
        return closest;
    }
    
}