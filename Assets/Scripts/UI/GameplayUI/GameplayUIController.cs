using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameplayUI
{
    [RequireComponent(typeof(GameplayUIManager))]
    public class GameplayUIController : MonoBehaviour
    {
        public GameplayUIManager manager;
        GameplayUIStack stack;

        private PlayerInput playerInput;
        private PlayerInput.OnUIActions onUI;
        private PlayerLevel playerLevel;

        void Awake()
        {
            playerInput = new PlayerInput();
            onUI = playerInput.OnUI;

            onUI.Back.performed += ctx => BackInput();
            onUI.Map.performed += ctx => MapInput();
            onUI.SkillCard.performed += ctx => SkillCardInput();

            stack = manager.stack;

            //playerLevel = PlayerManager.Instance.gameObject.GetComponent<PlayerLevel>();
        }

        private void OnEnable()
        {
            onUI.Enable();
        }
        private void OnDisable()
        {
            onUI.Disable();
        }

        public void BackInput()
        {
            /*
            if (uiStack.Count > 1)
            {
                Pop();
            }
            else
            {
                Push(PanelType.Pause);
            }
            */
        }

        public void MapInput()
        {
            if (stack.Peek() == PanelType.Map)
                stack.Pop();
            else
                stack.Push(PanelType.Map);
        }

        public void SkillCardInput()
        {
            Debug.Log("helloooooo");
            if (stack.Peek() != PanelType.SkillCard)
            {
                playerLevel = PlayerManager.Instance.gameObject.GetComponent<PlayerLevel>();
                if (playerLevel == null)
                {
                    return;
                }
                if (playerLevel.levelSystem.GetSkillCardPoint() != 0)
                {
                    stack.Push(PanelType.SkillCard);
                }
                else
                {
                    Debug.Log("Player does not have skill card point");
                }
            }
            else
                stack.Pop();
        }
    }
}