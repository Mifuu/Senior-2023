using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class GameplayUI : MonoBehaviour
    {
        public static GameplayUI Instance { get; private set; }
        [ReadOnly] public Stack<PanelType> uiStack = new Stack<PanelType>();

        public UIObject[] uiPanels;

        void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }

        /// <summary>remove the top and return its type</summary>

        public PanelType Pop()
        {
            PanelType popped = uiStack.Pop();

            DisablePanel(popped);

            return popped;
        }

        /// <summary>push new top and return its type</summary>
        public PanelType Push(PanelType panelType)
        {
            uiStack.Push(panelType);

            EnablePanel(panelType);

            return panelType;
        }

        /// <summary>return the top without removing it</summary>
        public PanelType Peek()
        {
            return uiStack.Peek();
        }

        private void EnablePanel(PanelType panelType)
        {
            bool found = false;

            foreach (UIObject uiObject in uiPanels)
            {
                if (uiObject.panelType == panelType)
                {
                    uiObject.panelObject.SetActive(true);
                    found = true;
                }
            }

            if (!found)
                Debug.LogError($"PanelType {panelType} not found in uiPanels", gameObject);
        }

        private void DisablePanel(PanelType panelType)
        {
            bool found = false;

            foreach (UIObject uiObject in uiPanels)
            {
                if (uiObject.panelType == panelType)
                {
                    uiObject.panelObject.SetActive(false);
                    found = true;
                }
            }

            if (!found)
                Debug.LogError($"PanelType {panelType} not found in uiPanels", gameObject);
        }

        [System.Serializable]
        public class UIObject
        {
            public GameObject panelObject;
            public PanelType panelType;
        }

        public enum PanelType
        {
            Gameplay,
            Pause,
            Gameover,
            Minimap,
        }
    }
}