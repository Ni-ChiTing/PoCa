using System.Collections;using System.Collections.Generic;using UnityEngine;using UnityEngine.UI;

public class GamePlay : MonoBehaviour {    public GameObject orderUI;    public GameObject player_1;    public GameObject player_2;    public GameObject player_3;    public GameObject player_4;

    public Text p2_text;    public Text p3_text;    public Text p4_text;    int myID = 0;


    void Start() {
        // hide orderUI from client
        if (!Data.IamHost) orderUI.SetActive(false);        
        // hide extra ppl
        if (Data.players.Count == 2) {
            player_3.SetActive(false);
            player_4.SetActive(false);
            p3_text.enabled = false;
            p4_text.enabled = false;
        } else if (Data.players.Count == 3) {
            player_4.SetActive(false);
            p4_text.enabled = false;
        }        // get myid        for (int i = 0; i < Data.players.Count; i++) if (Data.players[i] == Data.MyName) myID = i;        // setup player name        if (myID == 1) {

        } else if (myID == 2) {
            p2_text.text = Data.players[0];
        } else if (myID == 3) {
            p2_text.text = Data.players[0];
            p3_text.text = Data.players[1];
        } else if (myID == 4) {
            p2_text.text = Data.players[0];
            p3_text.text = Data.players[1];
            p4_text.text = Data.players[2];
        }    }

    void Update() {    }}