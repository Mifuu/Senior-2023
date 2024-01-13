using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoomGenUtil
{
    public class RoomRotUtil : MonoBehaviour
    {
        public static Vector3 GetRot90Pos(Vector3 pos, int rot90)
        {
            switch (rot90)
            {
                case 0:
                    return pos;
                case 1:
                    return new Vector3(pos.z, pos.y, -pos.x);
                case 2:
                    return new Vector3(-pos.x, pos.y, -pos.z);
                case 3:
                    return new Vector3(-pos.z, pos.y, pos.x);
            }

            return pos;
        }

        public static Vector3Int GetRot90Pos(Vector3Int pos, int rot90)
        {
            switch (rot90)
            {
                case 0:
                    return pos;
                case 1:
                    return new Vector3Int(pos.z, pos.y, -pos.x);
                case 2:
                    return new Vector3Int(-pos.x, pos.y, -pos.z);
                case 3:
                    return new Vector3Int(-pos.z, pos.y, pos.x);
            }

            return pos;
        }

        public static Vector3Int GetRot90Grid(Vector3Int pos, int rot90)
        {
            switch (rot90)
            {
                case 0:
                    return pos;
                case 1:
                    return new Vector3Int(pos.z, pos.y, -pos.x) - new Vector3Int(0, 0, 1);
                case 2:
                    return new Vector3Int(-pos.x, pos.y, -pos.z) - new Vector3Int(1, 0, 1);
                case 3:
                    return new Vector3Int(-pos.z, pos.y, pos.x) - new Vector3Int(1, 0, 0);
            }

            return pos;
        }
    }
}