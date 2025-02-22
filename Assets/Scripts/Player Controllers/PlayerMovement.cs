using Fusion;
using Fusion.Addons.SimpleKCC;
using Starter.ThirdPersonCharacter;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [Header("References")]
    public SimpleKCC KCC;

    [Header("Movement Setup")]
    public float WalkSpeed = 2f;
    public float SprintSpeed = 5f;
    public float JumpImpulse = 10f;
    public float UpGravity = 25f;
    public float DownGravity = 40f;
    public float RotationSpeed = 8f;

    [Header("Movement Accelerations")]
    public float GroundAcceleration = 55f;
    public float GroundDeceleration = 25f;
    public float AirAcceleration = 25f;
    public float AirDeceleration = 1.3f;


    [Networked]
    public NetworkBool _isWalking { get; private set; }
    [Networked]
    public NetworkBool _isRunning { get; private set; }
    [Networked]
    public Vector3 _moveVelocity { get; private set; }
    [Networked]
    public Vector3 _currentPosition { get; private set; }
    [Networked]
    public Vector3 _currentForward { get; private set; }
    [Networked]
    public NetworkBool _isJumping { get; private set; }
    [Networked]
    public NetworkBool _isFalling { get; private set; }
    [Networked]
    public NetworkBool _isGrounded { get; private set; }
    [Networked]
    public float _currentSpeed { get; private set; }


    public override void FixedUpdateNetwork()
    {
        _isWalking = KCC.RealSpeed > 0.1f && KCC.RealSpeed < 6f && KCC.IsGrounded;
        _isRunning = KCC.RealSpeed >= 6f && KCC.IsGrounded;
        _isGrounded = KCC.IsGrounded;
        _isFalling = KCC.RealVelocity.y < -10f;
        _currentSpeed = KCC.RealSpeed;
        _currentPosition = KCC.Position;
        _currentForward = KCC.LookDirection;
        if (KCC.IsGrounded)
        {
            // Stop jumping
            _isJumping = false;
        }
    }


    public void Move(GameplayInput input, NetworkButtons previousButtons)
    {
        float jumpImpulse = 0f;

        // Comparing current input buttons to previous input buttons - this prevents glitches when input is lost
        if (KCC.IsGrounded && input.Buttons.WasPressed(previousButtons, EInputButton.Jump))
        {
            // Set world space jump vector
            jumpImpulse = JumpImpulse;
            _isJumping = true;
        }

        // It feels better when the player falls quicker
        KCC.SetGravity(KCC.RealVelocity.y >= 0f ? UpGravity : DownGravity);

        float speed = input.Buttons.IsSet(EInputButton.Sprint) ? SprintSpeed : WalkSpeed;

        // var lookRotation = Quaternion.Euler(0f, input.LookRotation.y, 0f);
        // Calculate correct move direction from input (rotated based on camera look)
        var moveDirection = new Vector3(input.MoveDirection.x, 0f, input.MoveDirection.y);
        var desiredMoveVelocity = moveDirection * speed;

        float acceleration;
        if (desiredMoveVelocity == Vector3.zero)
        {
            // No desired move velocity - we are stopping
            acceleration = KCC.IsGrounded ? GroundDeceleration : AirDeceleration;
        }
        else
        {
            // Rotate the character towards move direction over time
            var currentRotation = KCC.TransformRotation;
            var targetRotation = Quaternion.LookRotation(moveDirection);
            var nextRotation = Quaternion.Lerp(currentRotation, targetRotation, RotationSpeed * Runner.DeltaTime);

            KCC.SetLookRotation(nextRotation.eulerAngles);

            acceleration = KCC.IsGrounded ? GroundAcceleration : AirAcceleration;
        }

        _moveVelocity = Vector3.Lerp(_moveVelocity, desiredMoveVelocity, acceleration * Runner.DeltaTime);

        // Ensure consistent movement speed even on steep slope
        if (KCC.ProjectOnGround(_moveVelocity, out var projectedVector))
        {
            _moveVelocity = projectedVector;
        }

        KCC.Move(_moveVelocity, jumpImpulse);
    }
}
