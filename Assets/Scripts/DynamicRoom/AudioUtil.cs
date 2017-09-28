using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioUtil : MonoBehaviour
{
    // 音频key
    public const string Click = "click";
    public const string Lose = "lose";
    public const string Win = "win";

    private static GameObject camera
    {
        get { return GameObject.Find("Main Camera"); }
    }
    private static AudioSource[] audioSources
    {
        get { return camera.GetComponents<AudioSource>(); }
    }

    public static void Play(string key)
    {
        foreach (var audio in audioSources)
        {
            if (audio.clip.name.Equals(key))
            {
                audio.Play();
                break;
            }
        }
    }
}
