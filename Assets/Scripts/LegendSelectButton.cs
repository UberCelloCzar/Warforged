using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegendSelectButton : GameItem {

    // Use this for initialization
    void Start()
    {

    }

    public override void OnMouseDown()
    {
        if (SelectManager.selectedLegend != "")
        {
            Debug.Log("Locked in " + SelectManager.selectedLegend); // Pass it back to the manager then server, trigger waiting dialog, then wait.
            SelectManager.active = false;
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - .2f, gameObject.transform.position.z);
        }
    }

    public override void OnHover()
    {
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - .02f, gameObject.transform.position.z);
    }

    public override void OnHoverExit()
    {
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + .02f, gameObject.transform.position.z);
    }
}
