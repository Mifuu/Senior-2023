using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameplayUI
{
    [RequireComponent(typeof(GameplayUIManager))]
    public class GameplayUIController : MonoBehaviour
    {
        public static GameplayUIController Instance { get; private set; }

        public GameplayUIManager manager;
        GameplayUIStack stack;

        private PlayerInput playerInput;
        private PlayerInput.OnUIActions onUI;
        private PlayerLevel playerLevel;

        void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;

            playerInput = new PlayerInput();
            onUI = playerInput.OnUI;

            onUI.Back.performed += ctx => BackInput();
            onUI.Map.performed += ctx => MapInput();
            onUI.SkillCard.performed += ctx => SkillCardInput();
            onUI.Inventory.performed += ctx => InventoryInput();

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
            if (stack.Peek() != PanelType.Play && stack.Peek() != PanelType.Gameover)
                stack.Pop();
        }

        public void MapInput()
        {
            /*
            if (stack.Peek() == PanelType.Map)
                stack.Pop();
            else
                stack.Push(PanelType.Map);
            */
        }

        public void SkillCardInput()
        {
            if (stack.Peek() != PanelType.SkillCard)
            {
                if (!PlayerManager.Instance.gameObject.TryGetComponent<PlayerLevel>(out playerLevel)) return;

                if (playerLevel.levelSystem.GetSkillCardPoint() > 0)
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

        public void CloseSkillCardPanel()
        {
            if (stack.Peek() == PanelType.SkillCard)
                stack.Pop();
        }

        public void InventoryInput()
        {
            if (stack.Peek() == PanelType.Inventory)
                stack.Pop();
            else
                stack.Push(PanelType.Inventory);
        }

        public void RespawnTrigger(float respawnTime)
        {
            StartCoroutine(RespawnCR(respawnTime));
        }

        IEnumerator RespawnCR(float respawnTime)
        {
            if (stack.Peek() != PanelType.Respawn)
                stack.Push(PanelType.Respawn);
            manager.respawnPanel.OnDeath(respawnTime);

            yield return new WaitForSeconds(respawnTime);

            stack.PopUntil(PanelType.Respawn);
            // stack.Pop();
        }

        public void GameoverTrigger()
        {
            Debug.Log("trigger gameover");
            stack.Push(PanelType.Gameover);
        }
    }
}