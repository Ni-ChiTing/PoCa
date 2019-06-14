using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartScene : MonoBehaviour
{
    Text Name;
    static GameObject Messagebox;
    public void ClientBtn() {
        Data.IamHost = false;
        
        if (string.IsNullOrEmpty(Name.text)){
            Messagebox = (GameObject)Resources.Load("Simple UI/MessageBox");
            Messagebox = GameObject.Instantiate(Messagebox,GameObject.Find("Canvas").transform) as GameObject;
            Messagebox.transform.localScale = new Vector3(1, 1, 1);            
            Messagebox.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
            Messagebox.GetComponent<RectTransform>().offsetMin = Vector2.zero;
            Messagebox.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            Messagebox.GetComponent<MessageBoxControll>().Content.text = "Name cannot be empty !!";
            Messagebox.GetComponent<MessageBoxControll>().Title.text = "Warning";
            Messagebox.GetComponent<MessageBoxControll>().Close.onClick.AddListener(Close_btn);
            Messagebox.GetComponent<MessageBoxControll>().Confirm.onClick.AddListener(Close_btn);
        }
        else{
            Data.MyName = Name.text;
            SceneManager.LoadScene(1);
        }
        
    }

    public void HostBtn() {
        Data.IamHost = true;

        if (string.IsNullOrEmpty(Name.text)){
            Messagebox = (GameObject)Resources.Load("Simple UI/MessageBox");
            Messagebox = GameObject.Instantiate(Messagebox, GameObject.Find("Canvas").transform) as GameObject;
            Messagebox.transform.localScale = new Vector3(1, 1, 1);
            Messagebox.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
            Messagebox.GetComponent<RectTransform>().offsetMin = Vector2.zero;
            Messagebox.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            Messagebox.GetComponent<MessageBoxControll>().Content.text = "Name cannot be empty !!";
            Messagebox.GetComponent<MessageBoxControll>().Title.text = "Warning";
            Messagebox.GetComponent<MessageBoxControll>().Close.onClick.AddListener(Close_btn);
            Messagebox.GetComponent<MessageBoxControll>().Confirm.onClick.AddListener(Close_btn);
        }
        else{
            Data.MyName = Name.text;
            SceneManager.LoadScene(3);
        }
    }
    public void Close_btn() {
        GameObject.Destroy(Messagebox);
    }
    public void Start()
    {
        Name = GameObject.Find("Canvas/InputField/Text").GetComponent<Text>(); 
    }
    public void Update()
    {
      
    }
}
