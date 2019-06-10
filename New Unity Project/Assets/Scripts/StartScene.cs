using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScene : MonoBehaviour
{
    public void ClientBtn() {
        SceneManager.LoadScene(1);
    }

    public void HostBtn() {
        SceneManager.LoadScene(3);
    }
}
