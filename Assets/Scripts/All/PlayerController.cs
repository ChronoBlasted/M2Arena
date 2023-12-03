using BaseTemplate.Behaviours;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoSingleton<PlayerController>
{
    //Data
    [Header("Data")]
    [Range(0, .3f)][SerializeField] float _movementXSmoothing = .05f;
    [Range(0, 1f)][SerializeField] float _malusAirMovementSpeed = .36f;
    [SerializeField] float _movementSpeed = 1;
    [SerializeField] float _jumpForce = 1;
    [SerializeField] int _gravityScale = 3;

    [SerializeField] LayerMask _groundLayer;

    //Ref
    [Header("References")]
    [SerializeField] Rigidbody2D _rb;
    [SerializeField] BoxCollider2D _bodyCollider;
    [SerializeField] PlayerAnimator _playerAnimator;
    [SerializeField] SpriteRenderer _sprite;

    //Cache
    PlayerInputAction _mainInputMap;

    float _moveRightFloat, targetXVelocity;
    bool _isRunning,
         _isGrounded,
         _isLeftWall,
         _isRightWall;

    Vector3 _velocity = Vector3.zero;

    public void Init()
    {
        _mainInputMap = new PlayerInputAction();
        _mainInputMap.Player.Enable();

        _mainInputMap.Player.Jump.started += Jump;
        _mainInputMap.Player.Jump.performed += StopJumping;
        _mainInputMap.Player.Jump.canceled += StopJumping;

        _mainInputMap.Player.Movement.started += Movement;
        _mainInputMap.Player.Movement.canceled += Movement;

        _mainInputMap.Player.Run.started += Run;
        _mainInputMap.Player.Run.canceled += StopRunning;

        _mainInputMap.Player.Interact.started += Interact;
    }

    #region BaseInput
    bool IsGrounded()
    {
        float extraHeight = .05f;

        RaycastHit2D raycastHit = Physics2D.BoxCast(_bodyCollider.bounds.center, _bodyCollider.bounds.size, 0f, Vector2.down, extraHeight, _groundLayer);


        return raycastHit.collider != null;
    }
    bool IsLeftWall()
    {
        float extraHeight = .05f;

        RaycastHit2D raycastHit = Physics2D.BoxCast(_bodyCollider.bounds.center, _bodyCollider.bounds.size, 0f, Vector2.left, extraHeight, _groundLayer);

        return raycastHit.collider != null;
    }
    bool IsRightWall()
    {
        float extraHeight = .05f;

        RaycastHit2D raycastHit = Physics2D.BoxCast(_bodyCollider.bounds.center, _bodyCollider.bounds.size, 0f, Vector2.right, extraHeight, _groundLayer);

        return raycastHit.collider != null;
    }


    void Movement(InputAction.CallbackContext ctx)
    {
        _moveRightFloat = ctx.ReadValue<float>();

        if (_moveRightFloat > 0) _sprite.flipX = false;
        if (_moveRightFloat < 0) _sprite.flipX = true;
    }

    void Run(InputAction.CallbackContext ctx)
    {
        _isRunning = true;
    }
    void StopRunning(InputAction.CallbackContext ctx)
    {
        _isRunning = false;
    }

    void Jump(InputAction.CallbackContext ctx)
    {
        if (_isGrounded)
        {
            Vector2 jumpVector = new Vector2(0, _jumpForce);
            _rb.AddForce(jumpVector, ForceMode2D.Impulse);
        }
    }
    void StopJumping(InputAction.CallbackContext ctx)
    {
        if (_rb.velocity.y > 0)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y / 1.5f);
        }
    }

    void Interact(InputAction.CallbackContext ctx)
    {

    }
    #endregion


    void FixedUpdate()
    {
        _isGrounded = IsGrounded();
        _isLeftWall = IsLeftWall();
        _isRightWall = IsRightWall();

        MovePlayer();
    }

    void MovePlayer()
    {
        targetXVelocity = _moveRightFloat * _movementSpeed;

        if (_isRunning) targetXVelocity *= 2;
        if (_isGrounded == false) targetXVelocity *= _malusAirMovementSpeed;

        Vector3 targetVelocity = new Vector2(targetXVelocity, _rb.velocity.y);
        _rb.velocity = Vector3.SmoothDamp(_rb.velocity, targetVelocity, ref _velocity, _movementXSmoothing);
    }
}