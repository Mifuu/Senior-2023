using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoomGeneration
{
    public class RoomDoorObject : MonoBehaviour
    {
        static Vector3 bin = new Vector3(0, -1000, 0);

        [Header("State")]
        public bool isWall = false;

        [Header("Game Object Settings")]
        public GameObject wallObject;
        public Vector3 wallObjectInitPos;
        public GameObject passageObject;
        public Vector3 passageObjectInitPos;

        private void Awake()
        {
            SetObjectInitPos();

            // Set(isWall);
        }

        private void SetObjectInitPos()
        {
            if (wallObject != null) wallObjectInitPos = wallObject.transform.position;
            if (passageObject != null) passageObjectInitPos = passageObject.transform.position;
        }

        public void Set(bool isWall)
        {
            this.isWall = isWall;
            // if (wallObject != null) wallObject.SetActive(isWall);
            // if (passageObject != null) passageObject.SetActive(!isWall);

            // Client Network Transform have to be spawned active
            if (wallObject != null) wallObject.transform.position = isWall ? wallObjectInitPos : bin;
            if (passageObject != null) passageObject.transform.position = !isWall ? passageObjectInitPos : bin;
        }
    }
}