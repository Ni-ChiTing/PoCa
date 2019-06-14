using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBoxControll : MonoBehaviour
{
    public Text Title;
    public Text Content;
    public Button Confirm;
    public Button Close;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(Title);
        Debug.Log(Content);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
