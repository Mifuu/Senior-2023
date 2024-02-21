using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayUI
{
    public class GameplayUIManager : MonoBehaviour
    {
        public static GameplayUIManager Instance { get; private set; }

        [Header("Requirements")]
        public GameplayUIStack stack;
        public GameplayUIController controller;
        public GameplayUIPlayPanel playPanel;

        void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }

        void Start()
        {
            stack.Push(PanelType.Play);
        }

        void OnValidate()
        {
            if (!stack && TryGetComponent(out stack))
                stack.manager = this;
            if (!controller && TryGetComponent(out controller))
                controller.manager = this;
        }
    }
}