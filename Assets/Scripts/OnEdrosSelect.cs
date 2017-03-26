using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;
using Warforged;

public class OnEdrosSelect : MonoBehaviour, IPointerClickHandler{


	public void OnPointerClick(PointerEventData eventData)
	{
        StartGame.characterPick = new Edros();
		//SceneManager.LoadScene("WarforgedBoard",LoadSceneMode.Single);

        // Set up some sort of selector indication here
	}

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}
}
