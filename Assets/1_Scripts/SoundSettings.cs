using UnityEngine;

[CreateAssetMenu(fileName = "SoundSettings", menuName = "Settings/Sound Settings")]
public class SoundSettings : ScriptableObject
{
    [Range(0f, 1f)]
    public float masterVolume = 1f;

    [Range(0f, 1f)]
    public float musicVolume = 1f;

    [Range(0f, 1f)]
    public float sfxVolume = 1f;

    [Header("UI Sounds")]
    public AudioClip buttonClickSound; 
    public AudioClip buttonHoverSound;

    [Header("Player Audio")]
    public AudioClip[] footstepSounds;

    [Header("Interaction SFX")]
    public AudioClip itemPickUpSound;
    public AudioClip itemSpawnSound;
    public AudioClip facilityBuySound;
    public AudioClip sleepTransitionSound;
    public AudioClip ladderTransitionSound;
    public AudioClip endShiftSound;
}
