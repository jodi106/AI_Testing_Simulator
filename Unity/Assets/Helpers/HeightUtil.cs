using UnityEngine;

public static class HeightUtil
{
    public const float VEHICLE_SELECTED = -0.04f;
    public const float VEHICLE_DESELECTED = -0.03f;
    public const float PATH_SELECTED = -0.025f;
    public const float PATH_DESELECTED = -0.02f;
    public const float WAYPOINT_INDICATOR = -0.01f; // 
    public const float WAYPOINT_INDICATOR_SELECTED = -0.6f; //
    public const float WAYPOINT_DESELECTED = -0.035f;
    public const float WAYPOINT_SELECTED = -0.05f;

    public static Vector3 SetZ(Vector3 vector, float z)
    {
        vector.z = z;
        return vector;
    }
}