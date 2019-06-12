using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameScene : MonoBehaviour
{
    GameObject[] player_modal = new GameObject[3];
    Text[] player_name = new Text[3];
    // Start is called before the first frame update
    void Start()
    {
        // 設定player modal
        player_modal[0] = GameObject.Find("people/guy1");
        player_modal[1] = GameObject.Find("people/guy2");
        player_modal[2] = GameObject.Find("people/guy3");

        // 設定player name
        player_name[0] = GameObject.Find("Canvas/P2").GetComponent<Text>();
        player_name[1] = GameObject.Find("Canvas/P3").GetComponent<Text>();
        player_name[2] = GameObject.Find("Canvas/P4").GetComponent<Text>();
        player_name[0].text = Data.Player2;
        player_name[1].text = Data.Player3;
        player_name[2].text = Data.Player4;

        //設定人數
        int player_number = Data.PlayerCardNumber;
        for (int i = player_number; i < 4; i++) {
            player_modal[i].SetActive(false);
            player_name[i].text = "";
        }

        // 設定host可按按鈕 其他人不行     
        if (Data.IamHost == false)
        {
            Debug.Log(Data.IamHost);
            GameObject.Find("Canvas/nameBtn").GetComponent<nextPlayerCon>().enabled = false;
        }
        else {

        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
