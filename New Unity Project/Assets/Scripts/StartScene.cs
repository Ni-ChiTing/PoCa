using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartScene : MonoBehaviour
{
    Text Name;
    public void ClientBtn() {
        Data.IamHost = false;
        SceneManager.LoadScene(1);
    }

    public void HostBtn() {
        Data.IamHost = true;
        SceneManager.LoadScene(3);
    }
    public void Start()
    {
        Name = GameObject.Find("Canvas/InputField/Text").GetComponent<Text>(); 
    }
    public void Update()
    {
        Data.MyName = Name.text;
    }
}
