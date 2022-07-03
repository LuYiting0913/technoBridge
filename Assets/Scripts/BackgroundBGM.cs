using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundBGM : MonoBehaviour
{
    private void Awake()
	{
		GameObject[] musicObj = GameObject.FindGameObjectsWithTag("BackgroundBGM");
		if (musicObj.Length > 1)
		{
			Destroy(this.gameObject);
		}
		DontDestroyOnLoad(this.gameObject);
	}
}
