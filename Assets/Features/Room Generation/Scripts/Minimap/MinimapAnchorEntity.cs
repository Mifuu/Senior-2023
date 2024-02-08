using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RoomGeneration.Minimap
{
    public class MinimapAnchorEntity : MonoBehaviour
    {
        public MinimapEntityIcon icon;

        [Header("Requirements")]
        public Image image;

        [ReadOnly][SerializeField] private Vector3 initialPosition;

        public void Awake()
        {
            initialPosition = image.rectTransform.position;
        }

        public void LateUpdate()
        {
            if (icon != null)
            {
                Vector3 delta = icon.transform.localPosition;
                image.rectTransform.position = initialPosition - delta;
            }
        }
    }
}