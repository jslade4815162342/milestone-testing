using UnityEngine;


/// <summary>
/// Add to any bottle alongside XRGrabInteractable.
///
/// HOW POURING WORKS:
///   While the player holds the bottle AND tilts it past pourAngleThreshold
///   AND the bottle spout is inside a DrinkGlass trigger zone, a timer counts
///   up. Once it reaches pourDuration the ingredient is registered in the glass.
///   The player must re-tilt to pour again (one pour per tilt).
///
/// SETUP:
///   1. Add XRGrabInteractable to the bottle.
///   2. Add this component. Set Ingredient Type.
///   3. Add a non-trigger Collider for physics.
///   4. (Optional) assign Pour Particles (on spout child) and Pour Audio Clip.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class BottleInteractable : MonoBehaviour
{
    [Header("Ingredient")]
    public IngredientType ingredientType = IngredientType.Vodka;

    [Header("Pour Settings")]
    [Tooltip("Angle from upright at which pouring starts. 90 = horizontal, 110+ = past horizontal.")]
    [Range(60f, 150f)]
    public float pourAngleThreshold = 100f;

    [Tooltip("Seconds the bottle must stay tilted before the pour registers.")]
    [Range(0.2f, 3f)]
    public float pourDuration = 1f;

    [Header("Feedback (optional)")]
    public ParticleSystem pourParticles;
    public AudioClip pourAudioClip;

    // ── Private ───────────────────────────────────────────────────────────────
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable _grab;
    private AudioSource _audio;
    private DrinkGlass _glassInRange;
    private float _pourTimer;
    private bool _isPouring;
    private bool _poured; // prevent multi-pour per tilt

    private void Awake()
    {
        _grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        if (pourAudioClip != null)
        {
            _audio = gameObject.AddComponent<AudioSource>();
            _audio.clip = pourAudioClip;
            _audio.loop = true;
            _audio.spatialBlend = 1f;
            _audio.playOnAwake = false;
        }
    }

    private void Update()
    {
        // Only act while the player is holding the bottle
        if (_grab == null || !_grab.isSelected)
        {
            StopPour();
            return;
        }

        bool tilted      = Vector3.Angle(transform.up, Vector3.up) >= pourAngleThreshold;
        bool glassReady  = _glassInRange != null && !_glassInRange.IsFull;

        if (tilted && glassReady)
        {
            if (!_isPouring) StartPour();
            _pourTimer += Time.deltaTime;

            if (!_poured && _pourTimer >= pourDuration)
            {
                _glassInRange.AddIngredient(ingredientType);
<<<<<<< HEAD
                if (gameObject.CompareTag("Vodka"))
                {
                    BarAudioManager.Instance?.PlayBottlePickup();
                }
=======
>>>>>>> 4326a873d027be8e6526b89cc4d75f09529d16ed
                _poured = true;
            }
        }
        else
        {
            StopPour();
            if (!tilted) _poured = false; // reset so next tilt can pour again
        }
    }

    // Called by DrinkGlass OnTrigger events
    public void SetGlass(DrinkGlass glass)  => _glassInRange = glass;
    public void ClearGlass()               => _glassInRange = null;

    private void StartPour()
    {
        _isPouring = true;
        pourParticles?.Play();
        _audio?.Play();
    }

    private void StopPour()
    {
        if (!_isPouring) return;
        _isPouring = false;
        _pourTimer = 0f;
        pourParticles?.Stop();
        _audio?.Stop();
    }
}
