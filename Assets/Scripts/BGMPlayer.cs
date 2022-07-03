using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGMPlayer : MonoBehaviour
{
    private AudioSource audioSource;
	private float musicVolume = 1f;
    public Slider volumeSlider;
    public GameObject ObjectMusic;

    void Start()
	{	
		// audioSource.Play();
        ObjectMusic = GameObject.FindWithTag("BackgroundBGM");
		audioSource = ObjectMusic.GetComponent<AudioSource>();	

        musicVolume = PlayerPrefs.GetFloat("volume");
		audioSource.volume = musicVolume;
		volumeSlider.value = musicVolume;
	}

    void Update()
	{
		audioSource.volume = musicVolume;
        PlayerPrefs.SetFloat("volume", musicVolume);
	}
	
	public void updateVolume(float volume)
	{
		musicVolume = volume;
	}

    public void MusicReset()
	{
		PlayerPrefs.DeleteKey("volume");
		audioSource.volume = 1;
		volumeSlider.value = 1;
	}
}
