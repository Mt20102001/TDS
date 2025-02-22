using UnityEngine;
using Starter.ThirdPersonCharacter;

public class VisualController : MonoBehaviour
{
    [SerializeField] private Player stateController;
    [SerializeField] private Animator animator;

    [Header("Sounds")]
    public AudioClip[] FootstepAudioClips;
    public AudioClip LandingAudioClip;
    [Range(0f, 1f)]
    public float FootstepAudioVolume = 0.5f;

    [Header("Feedbacks")]
    public GameObject walkPaticales;
    public GameObject runPaticales;
    public GameObject landingPaticales;

    [Header("Animator")]
    [SerializeField] private int indexCombatLayer;

    // Animation IDs
    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;

    private void Awake()
    {
        AssignAnimationIDs();
    }


    private bool flagHaveWeapon;
    private void LateUpdate()
    {
        UpdateMovementFeedbacks();

        if (animator == null)
            return;

        animator.SetFloat(_animIDSpeed, stateController.RealSpeed, 0.15f, Time.deltaTime);
        animator.SetFloat(_animIDMotionSpeed, 1f);
        animator.SetBool(_animIDJump, stateController.IsJumping);
        animator.SetBool(_animIDGrounded, stateController.IsGrounded);
        animator.SetBool(_animIDFreeFall, stateController.IsFalling);

        // Aim
        flagHaveWeapon = stateController.CurrentWeaponID != -1;
        float targetWeight = flagHaveWeapon ? 1f : 0f;
        if (!Mathf.Approximately(animator.GetLayerWeight(indexCombatLayer), targetWeight))
        {
            animator.SetLayerWeight(indexCombatLayer, targetWeight);
        }
    }

    private bool flagLanding;
    private bool flagMoving;
    private bool flagRunning;

    private void UpdateMovementFeedbacks()
    {
        flagMoving = stateController.IsWalking;
        flagRunning = stateController.IsRunning;
        flagLanding = !(stateController.IsJumping && !stateController.IsGrounded);

        if (walkPaticales.activeSelf != flagMoving)
            walkPaticales.SetActive(flagMoving);

        if (runPaticales.activeSelf != flagRunning)
            runPaticales.SetActive(flagRunning);

        if (landingPaticales.activeSelf != flagLanding)
            landingPaticales.SetActive(flagLanding);
    }


    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    // Animation event
    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight < 0.5f)
            return;

        if (FootstepAudioClips.Length > 0)
        {
            var index = Random.Range(0, FootstepAudioClips.Length);
            AudioSource.PlayClipAtPoint(FootstepAudioClips[index], stateController.CurrentPos, FootstepAudioVolume);
        }
    }

    // Animation event
    private void OnLand(AnimationEvent animationEvent)
    {
        AudioSource.PlayClipAtPoint(LandingAudioClip, stateController.CurrentPos, FootstepAudioVolume);
    }
}
