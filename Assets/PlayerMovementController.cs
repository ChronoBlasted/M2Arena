using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class PlayerMovementController : MonoBehaviour
{
    [Header("Rendering")]
    [SerializeField] SpriteRenderer _sprite;
    [SerializeField] Transform _weaponHolder;

    [Header("Movement")]
    [SerializeField] float _movementSpeed = 100.0f;
    [SerializeField] float _groundedBufferTime = 0.15f;
    [SerializeField] ParticleSystem _footstepParticles;

    [Header("Jumping")]
    [SerializeField] float _jumpBufferTime = 0.1f;
    [SerializeField] float _jumpForce = 400.0f;
    [SerializeField] float _gravityScale = 100.0f;
    [SerializeField] float _fallGravityMultiplier = 3.0f;
    [SerializeField] ParticleSystem _landingParticles;

    [Header("Ground Collision")]
    [SerializeField] LayerMask _groundLayer;

    [Header("Projectile Collision")]
    [SerializeField] float _knockbackForce = 2000f;
    [SerializeField] float _knockbackTime = 0.25f;
    [SerializeField] ParticleSystem _hitParticles;
    [SerializeField] ParticleSystem _dieParticles;

    [Header("Ref")]
    [SerializeField] Rigidbody2D _rb;
    [SerializeField] BoxCollider2D _bodyCollider;
    [SerializeField] Animator _animator;

    //Cache
    float _horizontalMovement;
    bool _jump;
    bool _jumpHeld;
    bool _isRunning;
    bool _isGrounded;
    float _jumpTimer;
    float _groundedTimer;
    float _knockbackTimer = 0f;
    bool _isFalling;
    ParticleSystem.EmissionModule _footstepEmission;

    public void Init()
    {
        _footstepEmission = _footstepParticles.emission;
    }


    private void Update()
    {
        if (_jump)
        {
            _jumpTimer = Time.time + _jumpBufferTime;
        }

        if (_horizontalMovement != 0)
        {
            // Switch weaponholder + Sprite Side

            if (_isGrounded)
            {
                _footstepEmission.rateOverTime = 20f;
            }
        }
        else
        {
            _footstepEmission.rateOverTime = 0f;
        }
    }

    private void FixedUpdate()
    {
        CheckIfGrounded();
        CheckIfFalling();
        HandleMovement();
        HandleJumping();
        ModifyPhysics();
    }

    public void SetHorizontalMovement(float speed)
    {
        _horizontalMovement = speed;
    }

    public void SetRunning(bool isRunning)
    {
        _isRunning = isRunning;
    }

    public void SetJump(bool isJumping)
    {
        _jump = isJumping;
    }

    public void SetJumpHeld(bool isJumpHeld)
    {
        _jumpHeld = isJumpHeld;
    }

    void CheckIfGrounded()
    {
        float extraHeight = .05f;

        RaycastHit2D raycastHit = Physics2D.BoxCast(_bodyCollider.bounds.center, _bodyCollider.bounds.size, 0f, Vector2.down, extraHeight, _groundLayer);

        _isGrounded = raycastHit.collider != null;

        if (_isGrounded)
        {
            _groundedTimer = _groundedBufferTime;
        }
        else
        {
            _groundedTimer -= Time.deltaTime;
        }
    }

    void CheckIfFalling()
    {
        _isFalling = _rb.velocity.y < 0;

        if (_isFalling)
        {
            _animator.SetTrigger("Fall");
        }
    }

    private void HandleMovement()
    {
        if (Time.time < _knockbackTimer)
        {
            return;
        }

        _rb.velocity = new Vector2(_horizontalMovement * _movementSpeed, _rb.velocity.y);
    }

    private void HandleJumping()
    {
        if (_jumpTimer > Time.time && _groundedTimer > 0)
        {
            _rb.AddForce(new Vector2(0, _jumpForce), ForceMode2D.Impulse);

            _jumpTimer = 0;
            _groundedTimer = 0;

            _animator.SetTrigger("Jump");
        }
    }

    private void ModifyPhysics()
    {
        if (_isGrounded)
        {
            _rb.gravityScale = 0;
        }
        else
        {
            _rb.gravityScale = _gravityScale;

            if (_isFalling)
            {
                _rb.gravityScale = _gravityScale * _fallGravityMultiplier;
            }
            else if (_rb.velocity.y > 0 && _jumpHeld)
            {
                _rb.gravityScale = _gravityScale / 2;
            }
        }
    }
}
