using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherPlayer : MonoBehaviour
{
    public int playerId;
    private Outline outline;

    void Awake() {
        outline = GetComponent<Outline>();
    }

    void OnMouseEnter()
    {
        if (AllCardCon.dragingCard > -1)
        {
            outline.enabled = true;
            //outline.OutlineMode = Outline.Mode.OutlineVisible;
        }
    }

    void OnMouseExit()
    {
        outline.enabled = false;
        //outline.OutlineMode = Outline.Mode.OutlineHidden;
    }

}
