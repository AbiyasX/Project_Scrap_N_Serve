using UnityEngine;

public class CharacterAudio : MonoBehaviour
{
    public void PlayFootstep()
    {
        if (AudioManager.Instance == null) return;
        if (AudioManager.Instance.soundSettings == null) return;
             
        AudioClip[] clips = AudioManager.Instance.soundSettings.footstepSounds;
              
        if (clips != null && clips.Length > 0)
        {
            int randomIndex = Random.Range(0, clips.Length);
            AudioManager.Instance.PlaySFX(clips[randomIndex]);
        }
    }
}