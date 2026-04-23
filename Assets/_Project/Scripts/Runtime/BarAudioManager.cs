using UnityEngine;

<<<<<<< HEAD
=======
/// <summary>
/// Simple singleton for bar sound effects and ambient audio.
///
/// SETUP:
///   1. Add to the GameManager GameObject.
///   2. Assign audio clips in the Inspector (all optional — game works without them).
///
/// Free SFX resources:
///   freesound.org — search "glass clink", "liquid pour", "cocktail shaker"
///   Assets/_Project/Audio/SFX/ is the intended drop folder.
/// </summary>
>>>>>>> 4326a873d027be8e6526b89cc4d75f09529d16ed
public class BarAudioManager : MonoBehaviour
{
    public static BarAudioManager Instance { get; private set; }

<<<<<<< HEAD
    [Header("Audio")]
    public AudioClip ambientClip;
    public AudioClip bottlePickupClip;
=======
    [Header("SFX")]
    public AudioClip clinkClip;
    public AudioClip successClip;
    public AudioClip failClip;
    public AudioClip shakerClip;

    [Header("Ambient")]
    public AudioClip ambientClip;
>>>>>>> 4326a873d027be8e6526b89cc4d75f09529d16ed
    [Range(0f, 1f)] public float ambientVolume = 0.25f;
    [Range(0f, 1f)] public float sfxVolume     = 0.9f;

    private AudioSource _sfx;
    private AudioSource _ambient;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        _sfx = gameObject.AddComponent<AudioSource>();
        _sfx.playOnAwake = false;

        _ambient = gameObject.AddComponent<AudioSource>();
        _ambient.loop = true;
        _ambient.playOnAwake = false;
        _ambient.volume = ambientVolume;
    }

    private void Start()
    {
        if (ambientClip != null)
        {
            _ambient.clip = ambientClip;
            _ambient.Play();
        }
    }

<<<<<<< HEAD
    public void PlayBottlePickup() => Play(bottlePickupClip);
=======
    public void PlayClink()   => Play(clinkClip);
    public void PlaySuccess() => Play(successClip);
    public void PlayFail()    => Play(failClip);
    public void PlayShaker()  => Play(shakerClip);
>>>>>>> 4326a873d027be8e6526b89cc4d75f09529d16ed

    private void Play(AudioClip clip)
    {
        if (clip != null) _sfx.PlayOneShot(clip, sfxVolume);
    }
<<<<<<< HEAD
}
=======
}
>>>>>>> 4326a873d027be8e6526b89cc4d75f09529d16ed
