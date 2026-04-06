using UnityEngine;

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
public class BarAudioManager : MonoBehaviour
{
    public static BarAudioManager Instance { get; private set; }

    [Header("SFX")]
    public AudioClip clinkClip;
    public AudioClip successClip;
    public AudioClip failClip;
    public AudioClip shakerClip;

    [Header("Ambient")]
    public AudioClip ambientClip;
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

    public void PlayClink()   => Play(clinkClip);
    public void PlaySuccess() => Play(successClip);
    public void PlayFail()    => Play(failClip);
    public void PlayShaker()  => Play(shakerClip);

    private void Play(AudioClip clip)
    {
        if (clip != null) _sfx.PlayOneShot(clip, sfxVolume);
    }
}
