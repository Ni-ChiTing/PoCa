using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class nextPlayerCon : MonoBehaviour
{
    public Sprite waitBtn;
    public Sprite playerBtn;
    public Image[] btns;
    public static int player;
    
    void Awake()
    {
        btns[0].sprite = playerBtn;
    }

    public void setPlayer(int p)
    {
        player = p;
        for (int i = 0; i < 4; i++) {
            btns[i].sprite = waitBtn;
        }
        btns[player].sprite = playerBtn;
    }
}
