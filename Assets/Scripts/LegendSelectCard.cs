using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegendSelectCard : GameItem {

	// Use this for initialization
	void Start()
    {
		
	}

    public override void OnMouseDown()
    {
        if (SelectManager.selectedLegend != "") GameObject.Find(SelectManager.selectedLegend).GetComponent<LegendSelectCard>().Unselect();
        SelectManager.selectedLegend = SelectManager.hoveredName;
        gameObject.GetComponent<Renderer>().material.color = Color.red; // Play a shake animation, add a glow, whatever. Just make sure they know they selected it
    }

    public void Unselect()
    {
        gameObject.GetComponent<Renderer>().material.color = Color.white;
    }
	
	public override void OnHover() // Use animations for all this stuff to make it smoother
    {
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 2, gameObject.transform.position.z);
    }

    public override void OnHoverExit()
    {
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 2, gameObject.transform.position.z);
    }
}
