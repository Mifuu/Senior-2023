using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInput playerInput;
    public PlayerInput.OnFootActions onFoot;
    private PlayerMotor motor;
    private PlayerLook look;
    private PlayerShoot shoot;
    private PlayerDash dash;
    public PlayerSwitchWeapon switchWeapon;
    private Coroutine shootingCoroutine;
    private SkillManager skillManager;

    void Awake()
    {
        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;
        motor = GetComponent<PlayerMotor>();
        look = GetComponent<PlayerLook>();
        shoot = GetComponent<PlayerShoot>();
        dash = GetComponent<PlayerDash>();
        skillManager = GetComponent<SkillManager>();   

        onFoot.Jump.performed += (ctx) => motor.Jump();
        onFoot.Shoot.started += (ctx) => StartShooting();
        onFoot.Shoot.canceled += (ctx) => StopShooting();
        onFoot.Dash.performed += ctx => dash.Dash(onFoot.Movement.ReadValue<Vector2>());
        onFoot.SwitchWeapon.performed += ctx => {
            float value = ctx.action.ReadValue<float>();
            switchWeapon.SwitchWeapon(value);
        };
        onFoot.NormalSkill.performed += (ctx) =>  skillManager.ActivateNormalSkill();
        onFoot.UltimateSkill.performed += (ctx) => skillManager.ActivateUltimateSkill();

    }

    void FixedUpdate()
    {
        motor.ProcessMove(onFoot.Movement.ReadValue<Vector2>());
    }

    void LateUpdate()
    {
        look.ProcessLook(onFoot.Look.ReadValue<Vector2>());
    }

    private void OnEnable()
    {
        onFoot.Enable();
    }

    private void OnDisable()
    {
        onFoot.Disable();
    }

    public void StartShooting()
    {
        if (shootingCoroutine == null)
        {
            shootingCoroutine = StartCoroutine(ContinuousShooting());
        }
    }

    public void StopShooting()
    {
        if (shootingCoroutine != null)
        {
            StopCoroutine(shootingCoroutine);
            shootingCoroutine = null;
        }
    }

    private IEnumerator ContinuousShooting()
    {
        while (true)  
        {
            shoot.ShootBullet();  
            yield return null;  
        }
    }
}