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
        int j = 0;
        foreach ( var name in Data.players){
            player_name[j].text = name;
        }


        //設定人數
        int player_number = Data.PlayerNumber-1;
        for (int i = 0; i < player_number; i++) {
            player_modal[i].SetActive(true);
        }
        for (int i = player_number; i < 3; i++) {
            player_modal[i].SetActive(false);
            player_name[i].text = "";
        }

    }
}
