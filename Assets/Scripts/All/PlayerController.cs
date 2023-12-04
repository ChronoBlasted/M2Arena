using BaseTemplate.Behaviours;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoSingleton<PlayerController>
{
    //Data
    [Header("Ref")]
    [SerializeField] PlayerInputController _playerInputController;
    [SerializeField] PlayerWeaponController _playerWeaponController;
    [SerializeField] PlayerMovementController _playerMovementController;

    public void Init()
    {
        _playerInputController.Init();
        _playerWeaponController.Init();
        _playerMovementController.Init();
    }
}