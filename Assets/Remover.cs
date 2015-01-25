﻿using UnityEngine;
using System.Collections;

public class Remover : MonoBehaviour
{
	public GameObject splash;
	
	void OnTriggerEnter2D(Collider2D col)
	{
		// If the player hits the trigger...
		if(col.gameObject.tag == "Player")
		{
			// ... instantiate the splash where the player falls in.
			//Instantiate(splash, col.transform.position, transform.rotation);
			// ... destroy the player.
			Destroy (col.gameObject);
			// ... reload the level.
			StartCoroutine("ReloadGame");
		}
	}

	IEnumerator ReloadGame()
	{			
		// ... pause briefly
		yield return new WaitForSeconds(2);
		// ... and then reload the level.
		Application.LoadLevel(Application.loadedLevel);
	}
}