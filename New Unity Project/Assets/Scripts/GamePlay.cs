using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlay : MonoBehaviour {

    public GameObject orderUI;
    public GameObject player_1;
    public GameObject player_2;
    public GameObject player_3;
    public GameObject player_4;

    public Text p2_text;
    public Text p3_text;
    public Text p4_text;

    public Text p1btn;
    public Text p2btn;
    public Text p3btn;
    public Text p4btn;



    int myID = 0;


    void Start() {
        // hide orderUI from client
        if (!Data.IamHost) orderUI.SetActive(false);

        // hide extra ppl
        if (Data.PlayerNumber == 2) {
            player_3.SetActive(false);
            player_4.SetActive(false);
            p3_text.enabled = false;
            p4_text.enabled = false;
        } else if (Data.PlayerNumber == 3) {
            player_4.SetActive(false);
            p4_text.enabled = false;
        }

        // get myid
        for (int i = 0; i < Data.PlayerNumber; i++) {
            string temp_a = Data.players[i];
            string temp_b = Data.MyName;
            if (temp_a.Equals(temp_b, StringComparison.Ordinal)) {
                print("Nvidia YES");
                myID = i;
            }
        }

        Data.myId = myID;
        print("change my id " + myID.ToString());


        // setup player name
        try {
            if (myID == 0) {
                p2_text.text = Data.players[1];
                p3_text.text = Data.players[2];
                p4_text.text = Data.players[3];
            } else if (myID == 1) {
                p2_text.text = Data.players[0];
                p3_text.text = Data.players[3];
                p4_text.text = Data.players[2];
            } else if (myID == 2) {
                p2_text.text = Data.players[3];
                p3_text.text = Data.players[1];
                p4_text.text = Data.players[0];
            } else if (myID == 3) {
                p2_text.text = Data.players[2];
                p3_text.text = Data.players[0];
                p4_text.text = Data.players[1];
            }

        } catch (Exception e) {
        }

        try
        {
            p1btn.text = Data.players[0];
            p2btn.text = Data.players[1];
            p3btn.text = Data.players[2];
            p4btn.text = Data.players[3];
        }
        catch (Exception e)
        {
        }
    }

    void Update() {

    }
}
