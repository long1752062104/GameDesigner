namespace Example2
{
    using System.Collections.Generic;
    using UnityEngine;

    public class RoamingPath : MonoBehaviour
    {
        public List<Vector3> localWaypoints = new List<Vector3>();
        public List<Vector3> waypointsList = new List<Vector3>();
        public bool waypointsFoldout;
    }
}