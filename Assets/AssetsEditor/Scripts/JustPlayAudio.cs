using UnityEngine;

public class JustPlayAudio : MonoBehaviour
{
    public AudioSource audioToPlay;

    public void PlayIt() {
        if (audioToPlay)
        {
            audioToPlay.Play();

        }
        else
        {
            Debug.Log("Audio source in " + name + " not set.");

        }
    }

}
