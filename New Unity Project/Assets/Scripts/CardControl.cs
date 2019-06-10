using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardControl : MonoBehaviour
{
    public int cardId;
    public Outline outline;

    void Awake()
    {
        outline = GetComponent<Outline>();
    }

    void OnMouseEnter()
    {
        outline.enabled = true;
        if (getParent() < 4)
        {
            Vector3 pos = transform.localPosition;
            transform.localPosition = new Vector3(pos.x, -0.05f, pos.z);
        }
    }

    void OnMouseExit()
    {
        outline.enabled = false;
        Vector3 pos = transform.localPosition;
        transform.localPosition = new Vector3(pos.x, 0, pos.z);
    }

    public int getParent() {
        return int.Parse(transform.parent.name);
    }
}
