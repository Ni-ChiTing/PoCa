using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartScene : MonoBehaviour
{
    Text Name;
    static GameObject Messagebox;
    AndroidJavaObject _ajc;
    public void ClientBtn() {
        Data.IamHost = false;
        if (string.IsNullOrEmpty(Name.text) || Name.text == "Done")
        {
            
            Messagebox = (GameObject)Resources.Load("Simple UI/MessageBox");
            Messagebox = GameObject.Instantiate(Messagebox,GameObject.Find("Canvas").transform) as GameObject;
            Messagebox.transform.localScale = new Vector3(1, 1, 1);            
            Messagebox.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
            Messagebox.GetComponent<RectTransform>().offsetMin = Vector2.zero;
            Messagebox.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            Messagebox.GetComponent<MessageBoxControll>().Content.text = "Name cannot be empty  OR DONE!!";
            Messagebox.GetComponent<MessageBoxControll>().Title.text = "Warning";
            Messagebox.GetComponent<MessageBoxControll>().Close.onClick.AddListener(Close_btn);
            Messagebox.GetComponent<MessageBoxControll>().Confirm.onClick.AddListener(Close_btn);
            

        }
        else{
            Data.MyName = Name.text;
            bool IfWifiOpen = false;
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                //Change the Text
               // m_ReachabilityText = "Not Reachable.";
            }
            //Check if the device can reach the internet via a carrier data network
            else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
            {
               // m_ReachabilityText = "Reachable via carrier data network.";
            }
            //Check if the device can reach the internet via a LAN
            else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
            {
                //m_ReachabilityText = "Reachable via Local Area Network.";
                IfWifiOpen = true;
            }
            if (!IfWifiOpen)
            {
                Messagebox = (GameObject)Resources.Load("Simple UI/MessageBox");
                Messagebox = GameObject.Instantiate(Messagebox, GameObject.Find("Canvas").transform) as GameObject;
                Messagebox.transform.localScale = new Vector3(1, 1, 1);
                Messagebox.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
                Messagebox.GetComponent<RectTransform>().offsetMin = Vector2.zero;
                Messagebox.GetComponent<RectTransform>().offsetMax = Vector2.zero;
                Messagebox.GetComponent<MessageBoxControll>().Content.text = "Please Connect Host !!";
                Messagebox.GetComponent<MessageBoxControll>().Title.text = "Warning";
                Messagebox.GetComponent<MessageBoxControll>().Close.onClick.AddListener(Close_btn);
                Messagebox.GetComponent<MessageBoxControll>().Confirm.onClick.AddListener(Close_btn);
            }
            else
            {
                string ip = _ajc.Call<string>("GetIP");
                bool success2 = _ajc.Call<bool>("showToast", ip);
                Data.HostIP = ip;
                SceneManager.LoadScene(1);
            }
            
        }
        
    }

    public void HostBtn() {
        Data.IamHost = true;

        if (string.IsNullOrEmpty(Name.text) || Name.text == "Done" ){
            Messagebox = (GameObject)Resources.Load("Simple UI/MessageBox");
            Messagebox = GameObject.Instantiate(Messagebox, GameObject.Find("Canvas").transform) as GameObject;
            Messagebox.transform.localScale = new Vector3(1, 1, 1);
            Messagebox.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
            Messagebox.GetComponent<RectTransform>().offsetMin = Vector2.zero;
            Messagebox.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            Messagebox.GetComponent<MessageBoxControll>().Content.text = "Name cannot be empty  OR DONE!!";
            Messagebox.GetComponent<MessageBoxControll>().Title.text = "Warning";
            Messagebox.GetComponent<MessageBoxControll>().Close.onClick.AddListener(Close_btn);
            Messagebox.GetComponent<MessageBoxControll>().Confirm.onClick.AddListener(Close_btn);
            
            //bool success = isWifiApEnabled();
            
            //bool success3 = isWifiApEnabled();
            //bool success23 = _ajc.Call<bool>("showToast", success3.ToString());
        }
        else{
            bool success = _ajc.Call<bool>("CheckWifiAP");
            //bool success2 = _ajc.Call<bool>("showToast", success.ToString());
            if (success){
                Data.MyName = Name.text;
                string ip = _ajc.Call<string>("GetIP");
                Data.HostIP = ip;
                //bool success2 = _ajc.Call<bool>("showToast", ip);
                SceneManager.LoadScene(3);
            }
            else
            {
                Messagebox = (GameObject)Resources.Load("Simple UI/MessageBox");
                Messagebox = GameObject.Instantiate(Messagebox, GameObject.Find("Canvas").transform) as GameObject;
                Messagebox.transform.localScale = new Vector3(1, 1, 1);
                Messagebox.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
                Messagebox.GetComponent<RectTransform>().offsetMin = Vector2.zero;
                Messagebox.GetComponent<RectTransform>().offsetMax = Vector2.zero;
                Messagebox.GetComponent<MessageBoxControll>().Content.text = "Please Open Wifi AP";
                Messagebox.GetComponent<MessageBoxControll>().Title.text = "Warning";
                Messagebox.GetComponent<MessageBoxControll>().Close.onClick.AddListener(Close_btn);
                Messagebox.GetComponent<MessageBoxControll>().Confirm.onClick.AddListener(Close_btn);

            }
        }
    }
    public void Close_btn() {
        GameObject.Destroy(Messagebox);
    }
    public void Start()
    {
        
        _ajc = new AndroidJavaObject("com.androidforunity.UnityFuntion");
        Debug.Log(_ajc);
        Name = GameObject.Find("Canvas/InputField/Text").GetComponent<Text>();
        //bool success = _ajc.Call<bool>("RequestPermission");
    }
    public void Update()
    {
      
    }
    public void FromAndroid(string content)
    {
        Debug.Log("ACL");
    }

    public bool isWifiApEnabled()
    {
        Debug.Log("SSs");
        using (AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"))
        {
            try
            {
                using (var wifiManager = activity.Call<AndroidJavaObject>("getSystemService", "wifi"))
                {
                    Debug.Log("SSS");
                    return wifiManager.Call<bool>("isWifiApEnabled");
                }
            }
            catch (Exception e)
            {
            }
        }
        return false;
    }

}
