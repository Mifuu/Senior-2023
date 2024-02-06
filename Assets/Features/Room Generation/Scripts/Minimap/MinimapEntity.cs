using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoomGeneration.Minimap
{
    public class MinimapEntity : MonoBehaviour
    {
        [Header("Settings")]
        public MinimapEntityIcon iconPrefab;
        public MinimapEntityTrackType trackType;
        public bool isAnchor = false;

        void OnEnable()
        {
            if (MinimapEntityDisplay.instance == null)
            {
                // Debug.LogError("MinimapEntityDisplay not found in scene. Disabling MinimapEntity.");
                // enabled = false;
                return;
            }

            MinimapEntityDisplay.instance.AddEntity(this);
        }

        void OnDisable()
        {
            if (MinimapEntityDisplay.instance == null)
            {
                // Debug.LogError("MinimapEntityDisplay not found in scene. Disabling MinimapEntity.");
                // enabled = false;
                return;
            }

            MinimapEntityDisplay.instance.RemoveEntity(this);
        }

        void Update()
        {
            if (MinimapEntityDisplay.instance == null) return;

            if (MinimapEntityDisplay.instance.HasEntity(this))
                MinimapEntityDisplay.instance.UpdateEntity(this);
            else
                MinimapEntityDisplay.instance.AddEntity(this);
        }
    }
}