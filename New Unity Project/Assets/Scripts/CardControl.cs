using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardControl : MonoBehaviour
{
    public int cardId;
    public Outline outline;
    public bool ani = false;
    public AudioSource AC;

    void Awake()
    {
        outline = GetComponent<Outline>();
        AC = GameObject.Find("audio").GetComponent<AudioSource>();
        AC.clip = Resources.Load<AudioClip>("finger_snap");
    }
    void Update()
    {
        //動畫><
        if (ani)
        {
            if (getParent() > 0)
            {
                Vector3 dir = Vector3.zero - transform.localPosition;
                dir.y = 0;
                dir = Vector3.Normalize(dir);
                transform.localPosition = (transform.localPosition + 0.1f * dir);


            }
            else
            {
                transform.rotation = Quaternion.Euler(new Vector3(90, 180, 0));
                Vector3 pos = transform.position;
                pos.z -= 0.1f;
                transform.position = pos;
                AC.PlayOneShot(AC.clip, 0.05f);
                //Debug.Log("ho");
            }
        }
    }
    void OnTriggerEnter(Collider collider)
    {
        if (string.Equals(collider.transform.tag, "Player"))
        {
            ani = false;
            AllCardCon.allCardCon.Give(collider.GetComponent<OtherPlayer>().playerId, cardId);
        }
        if (string.Equals(collider.transform.tag, "Me"))
        {
            ani = false;
            AllCardCon.allCardCon.Give(0, cardId);
            AC.PlayOneShot(AC.clip, 0.05f);
        }
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
