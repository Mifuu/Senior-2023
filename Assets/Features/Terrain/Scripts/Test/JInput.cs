using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JInput : MonoBehaviour
{
    private PlayerInput playerInput;
    public PlayerInput.OnFootActions onFoot;
    private JCharacter character;

    void Awake()
    {
        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;
        character = GetComponent<JCharacter>();
        onFoot.Jump.performed += (ctx) => character.Jump();
    }

    void FixedUpdate()
    {
        character.ProcessMove(onFoot.Movement.ReadValue<Vector2>());
    }

    void LateUpdate()
    {
        if (Input.GetKey(KeyCode.Escape)) return;
        character.ProcessLook(onFoot.Look.ReadValue<Vector2>());
    }

    private void OnEnable()
    {
        onFoot.Enable();
    }

    private void OnDisable()
    {
        onFoot.Disable();
    }
}