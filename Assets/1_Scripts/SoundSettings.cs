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
}
