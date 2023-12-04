using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    [SerializeField] PlayerMovementController _playerMovementController;
    [SerializeField] PlayerWeaponController _playerWeaponController;

    float _horizontalInput;
    bool _jumpInput;
    bool _jumpHeldInput;
    bool _runningInput;

    bool _fireInput;
    bool _interactInput;

    bool _inputChanged;

    PlayerInputAction _mainInputMap;

    float _lastHorizontalInput;
    bool _lastJumpInput;
    bool _lastJumpHeldInput;
    bool _lastRunningInput;

    bool _lastFireInput;
    bool _lastInteractInput;

    public float HorizontalInput { get => _horizontalInput; set => _horizontalInput = value; }
    public bool JumpInput { get => _jumpInput; set => _jumpInput = value; }
    public bool JumpHeldInput { get => _jumpHeldInput; set => _jumpHeldInput = value; }
    public bool RunningInput { get => _runningInput; set => _runningInput = value; }

    public bool InputChanged { get => _inputChanged; set => _inputChanged = value; }

    public bool InteractInput { get => _interactInput; set => _interactInput = value; }
    public bool FireInput { get => _fireInput; set => _fireInput = value; }

    public void Init()
    {
        _mainInputMap = new PlayerInputAction();
        _mainInputMap.Player.Enable();

        _mainInputMap.Player.Jump.started += Jump;
        _mainInputMap.Player.Jump.performed += StopJumpInput;
        _mainInputMap.Player.Jump.canceled += StopJumpHeld;

        _mainInputMap.Player.Movement.started += Movement;
        _mainInputMap.Player.Movement.canceled += Movement;

        _mainInputMap.Player.Run.started += Run;
        _mainInputMap.Player.Run.canceled += StopRunning;

        _mainInputMap.Player.Interact.started += Interact;

        _mainInputMap.Player.Fire.started += Fire;
    }

    private void Update()
    {
        _inputChanged =
           _lastHorizontalInput != HorizontalInput ||
            _lastJumpInput != _jumpInput ||
            _lastJumpHeldInput != _jumpHeldInput ||
            _lastRunningInput != _runningInput ||
            _lastInteractInput != _interactInput ||
            _lastFireInput != _fireInput;

        _playerMovementController.SetHorizontalMovement(HorizontalInput);
        _playerMovementController.SetRunning(RunningInput);
        _playerMovementController.SetJump(JumpInput);
        _playerMovementController.SetJumpHeld(JumpHeldInput);

        if (_fireInput)
        {
            _playerWeaponController.Attack();
        }

        // TODO Interact with Interface
    }


    void Jump(InputAction.CallbackContext ctx)
    {
        _jumpInput = true;
        _jumpHeldInput = true;
    }
    void StopJumpInput(InputAction.CallbackContext ctx)
    {
        _jumpInput = false;

    }
    void StopJumpHeld(InputAction.CallbackContext ctx)
    {
        _jumpInput = false;
        _jumpHeldInput = false;
    }

    void Run(InputAction.CallbackContext ctx)
    {
        _runningInput = true;
    }
    void StopRunning(InputAction.CallbackContext ctx)
    {
        _runningInput = false;
    }

    void Movement(InputAction.CallbackContext ctx)
    {
        _horizontalInput = ctx.ReadValue<float>();
    }

    void Fire(InputAction.CallbackContext ctx)
    {
        _fireInput = true;
    }
    void Interact(InputAction.CallbackContext ctx)
    {
        _interactInput = true;
    }
}
