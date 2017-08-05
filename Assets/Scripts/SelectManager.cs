using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectManager : MonoBehaviour {

    public static string hoveredName = ""; // The currently hovered gameObject
    public static string selectedLegend = ""; // The currently selected Legend
    public static bool active = true; // Whether input is being taken (false if already locked in)

    public void Update()
    {
        UpdateSelection();
    }

    private void UpdateSelection()
    {
        Debug.DrawRay(Camera.main.ScreenPointToRay(Input.mousePosition).origin, Camera.main.ScreenPointToRay(Input.mousePosition).direction.normalized * 50f, Color.red);

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 50f, LayerMask.GetMask("Interactables"))) // Raycast from cursor, check against any interactables
        {
            if (hit.transform.gameObject.name != hoveredName) // If its not already being hovered
            {
                if (active)
                {
                    Debug.Log("Hovered " + hit.transform.gameObject.name); // Hover the card
                    hoveredName = hit.transform.gameObject.name;
                    hit.transform.gameObject.GetComponent<GameItem>().OnHover();
                }
            }
        }
        else // If it is already being hovered
        {
            if (hoveredName != "")
            {
                GameObject.Find(hoveredName).GetComponent<GameItem>().OnHoverExit();
                hoveredName = "";
            }
        }

    }
}
