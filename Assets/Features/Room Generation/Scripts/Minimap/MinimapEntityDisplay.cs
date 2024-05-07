using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RoomGeneration.Minimap
{
    [RequireComponent(typeof(MinimapDisplay))]
    public class MinimapEntityDisplay : MonoBehaviour
    {
        public static MinimapEntityDisplay instance;

        [Header("Requirements")]
        public MinimapDisplay minimapDisplay;
        public MinimapAnchorEntity minimapAnchorEntity;
        private RoomGenerator roomGenerator;
        public Image image;
        public Transform entityParent;

        // mapping values
        private RoomBoxSnapValue snapValue;
        private Vector3 imageBottomLeft;
        private Vector3 imageTopRight;
        private Vector3 worldBottomLeft;
        private Vector3 worldTopRight;

        Dictionary<MinimapEntity, MinimapEntityIcon> entityIcons = new Dictionary<MinimapEntity, MinimapEntityIcon>();

        public void Init()
        {
            if (instance != null && instance != this)
                Destroy(this.gameObject);
            else
                instance = this;

            roomGenerator = minimapDisplay.roomGenerator;
            snapValue = roomGenerator.roomSet.snapValue;

            CalculateMappingValues();
        }

        void CalculateMappingValues()
        {
            int gridSize = minimapDisplay.GetGridSize();

            // worldBottomLeft = new Vector3(-snapValue.value.x * gridSize / 2, 0, -snapValue.value.z * gridSize / 2);
            // worldTopRight = new Vector3(snapValue.value.x * gridSize / 2, 0, snapValue.value.z * gridSize / 2);

            worldBottomLeft = new Vector3(-snapValue.value.x * gridSize / 2, 0, -snapValue.value.z * gridSize / 2);
            worldTopRight = new Vector3(snapValue.value.x * gridSize / 2, 0, snapValue.value.z * gridSize / 2);
        }

        public void AddEntity(MinimapEntity minimapEntity)
        {
            // check if in dictionary
            if (entityIcons.ContainsKey(minimapEntity))
                return;

            // add new entity icon
            MinimapEntityIcon icon = Instantiate(minimapEntity.iconPrefab, entityParent);

            // add to dictionary
            entityIcons.Add(minimapEntity, icon);

            // anchor if needed
            if (minimapEntity.isAnchor)
            {
                bool isPlayer = minimapEntity.gameObject.transform.parent.GetComponent<PlayerManager>() == PlayerManager.Instance;

                if (isPlayer)
                    minimapAnchorEntity.icon = icon;
            }
        }

        public void RemoveEntity(MinimapEntity minimapEntity)
        {
            // remove from dictionary
            MinimapEntityIcon icon = entityIcons[minimapEntity];
            entityIcons.Remove(minimapEntity);

            // remove entity icon
            Destroy(icon.gameObject);
        }

        public void UpdateEntity(MinimapEntity minimapEntity)
        {
            Vector3 position = minimapEntity.transform.position;
            float rotation = minimapEntity.transform.eulerAngles.y;

            // remap world position to image position
            float x = ((position.x - worldBottomLeft.x) / (worldTopRight.x - worldBottomLeft.x));
            float z = ((position.z - worldBottomLeft.z) / (worldTopRight.z - worldBottomLeft.z));

            imageBottomLeft = new Vector3(image.rectTransform.position.x - image.rectTransform.rect.width * image.rectTransform.lossyScale.x / 2, image.rectTransform.position.y - image.rectTransform.rect.height * image.rectTransform.lossyScale.y / 2, image.rectTransform.position.z);
            imageTopRight = new Vector3(image.rectTransform.position.x + image.rectTransform.rect.width * image.rectTransform.lossyScale.x / 2, image.rectTransform.position.y + image.rectTransform.rect.height * image.rectTransform.lossyScale.y / 2, image.rectTransform.position.z);

            Vector3 iconPos = new Vector3(Mathf.LerpUnclamped(imageBottomLeft.x, imageTopRight.x, x), Mathf.LerpUnclamped(imageBottomLeft.y, imageTopRight.y, z), imageBottomLeft.z);

            MinimapEntityIcon icon = entityIcons[minimapEntity];
            icon.transform.position = iconPos;

            // update rotation
            if (minimapEntity.trackType == MinimapEntityTrackType.PosAndRot)
            {
                icon.transform.rotation = Quaternion.Euler(0, 0, -rotation);
            }
        }

        public bool HasEntity(MinimapEntity minimapEntity)
        {
            return entityIcons.ContainsKey(minimapEntity);
        }

        void OnValidate()
        {
            if (minimapDisplay == null)
                minimapDisplay = GetComponent<MinimapDisplay>();
        }

        void OnDrawGizmos()
        {
            /*
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(worldBottomLeft, 2f);
            Gizmos.DrawSphere(worldTopRight, 2f);
            Gizmos.DrawSphere(imageBottomLeft, 10f);
            Gizmos.DrawSphere(imageTopRight, 10f);
            */
        }
    }

    public enum MinimapEntityTrackType
    {
        Pos,
        PosAndRot
    }
}
