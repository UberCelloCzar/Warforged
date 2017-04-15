using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnAurianSelect : MonoBehaviour,  IPointerClickHandler
{
	public void OnPointerClick(PointerEventData eventData)
	{
		StartGame.characterPick = new Warforged.Aurian();
		//SceneManager.LoadScene("WarforgedBoard", LoadSceneMode.Single);

	}

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
}
