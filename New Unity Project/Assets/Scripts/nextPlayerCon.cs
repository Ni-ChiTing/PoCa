using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class nextPlayerCon : MonoBehaviour
{
    public Sprite waitBtn;
    public Sprite playerBtn;
    public Image[] btns;
    public EventTrigger[] btnET;
    public Text[] texts;
    public static int player;
    
    void Awake()
    {
        btns[0].sprite = playerBtn;
        for (int i = 0; i < Data.PlayerNumber; i++) {
            btns[i].enabled = true;
            texts[i].enabled = true;
            // 除了host 沒人可以按按鈕
            if (Data.IamHost == false) {
                btnET[i].enabled = false;
            }
        }
        for (int i = Data.PlayerNumber; i < 4; i++) {
            btns[i].enabled = false;
            texts[i].enabled = false;
        }

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
