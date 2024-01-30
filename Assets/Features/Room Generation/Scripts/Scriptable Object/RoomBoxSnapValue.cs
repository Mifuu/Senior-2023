using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoomGeneration
{
    [CreateAssetMenu(fileName = "SnapValue", menuName = "Room Generation/SnapValue", order = 3)]
    public class RoomBoxSnapValue : ScriptableObject
    {
        public Vector3 value = new Vector3(2, 5, 2);
    }
}