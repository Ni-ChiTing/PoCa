using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlay : MonoBehaviour {

    public GameObject orderUI;

    void Start() {
        // hide orderUI from client
        if (!Data.IamHost) orderUI.SetActive(false);

    }
    
    void Update() {

    }
}
