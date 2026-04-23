using UnityEngine;

public class BarAudioManager : MonoBehaviour
{
    public static BarAudioManager Instance { get; private set; }

    [Header("Audio")]
    public AudioClip ambientClip;
    public AudioClip bottlePickupClip;
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

    public void PlayBottlePickup() => Play(bottlePickupClip);

    private void Play(AudioClip clip)
    {
        if (clip != null) _sfx.PlayOneShot(clip, sfxVolume);
    }
}