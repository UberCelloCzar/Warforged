using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameItem : MonoBehaviour { // Base class everything on the field inherits from.

    public virtual void OnMouseDown() { } // Everything must react
    public virtual void OnHover() { } // Everything must be responsive
    public virtual void OnHoverExit() { } // Everything must become unresponsive
}
