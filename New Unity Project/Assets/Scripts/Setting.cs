using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    Text player2;
    Text player3;
    Text player4;
    Text game_name;
    Text player_number;
    Text player_card_number;
    Text table_card_number;
    Toggle need_animation;
    Toggle need_draw_card;
    static GameObject Messagebox;

    string broadcastData = "HELLO";
    private static Socket sock;
    private static IPEndPoint iep1;
    private static byte[] data;
    private Thread t;
    /*
    public int udpPort = 9050;
    UdpClient client;


    public void BroadcastIP()
    {
        //sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        client = new UdpClient(new IPEndPoint(IPAddress.Any, udpPort));
        iep1 = new IPEndPoint(IPAddress.Broadcast, udpPort);

        data = Encoding.ASCII.GetBytes(Data.HostIP);

        t = new Thread(BroadcastMessage);
        t.Start();
    }
    private void BroadcastMessage()
    {
        while (true)
        {
            client.Send(data, data.Length, iep1);
            Debug.Log("send broadcast");
            Thread.Sleep(1000);
        }
    }*/
    public void Start()
    {
        /*
        if (!NetworkTransport.IsStarted)
        {
            NetworkTransport.Init();
        }
        GlobalConfig globalconfig = new GlobalConfig();
        globalconfig.MaxPacketSize = maxpacketsize;
        NetworkTransport.Init(globalconfig);
        broadcastData = "NetworkManager:" + I + ":" + NetworkManager.singleton.networkPort;
        */
        //BroadcastIP();

        //Create UDP Client for broadcasting the server
        string hostName = System.Net.Dns.GetHostName();
        string localIP = System.Net.Dns.GetHostEntry(hostName).AddressList[0].ToString();
        Debug.Log(hostName);
        for (int i = 0; i < Dns.GetHostEntry(hostName).AddressList.Length ; i++)
        {
            Debug.Log(Dns.GetHostEntry(hostName).AddressList[i].ToString());
        }
        
        player2 = GameObject.Find("Canvas/conn/connect1/Text").GetComponent<Text>();
        player3 = GameObject.Find("Canvas/conn/connect2/Text").GetComponent<Text>();
        player4 = GameObject.Find("Canvas/conn/connect3/Text").GetComponent<Text>();
        game_name = GameObject.Find("Canvas/set/bg/game/Dropdown/Label").GetComponent<Text>();
        player_number = GameObject.Find("Canvas/set/bg/set/PlayerNumDropDown/Label").GetComponent<Text>();
        player_card_number = GameObject.Find("Canvas/set/bg/set/PlayerCardInputField/Text").GetComponent<Text>();
        table_card_number = GameObject.Find("Canvas/set/bg/set/TableCardInputField/Text").GetComponent<Text>();
        need_animation = GameObject.Find("Canvas/set/bg/Toggle").GetComponent<Toggle>();
        need_draw_card = GameObject.Find("Canvas/set/bg/Toggle2").GetComponent<Toggle>();
    }
    public void startBtn()
    {
        int temp;
        Data.players.Add(player2.text);
        Data.players.Add(player3.text);
        Data.players.Add(player4.text);
        Data.GameName = game_name.text;
        Data.PlayerNumber = int.Parse(player_number.text);
        if (need_animation.isOn == true) Data.NeedAnimation = true;
        else Data.NeedAnimation = false;
        if (need_draw_card.isOn == true) Data.NeedDrawCard = true;
        else Data.NeedDrawCard = false;
        if (int.TryParse(player_card_number.text,out temp) == false)
        {
            ShowDia("warning", "Player card number need number !!");
        }
        else
        {
            Data.PlayerCardNumber = int.Parse(player_card_number.text);
            if (int.TryParse(table_card_number.text, out temp) == false)
            {
                ShowDia("warning", "Table card number need number !!");
            }
            else
            {
                Data.TableCardNumber = int.Parse(table_card_number.text);
                SceneManager.LoadScene(2);
            }
        }        
        
    }
    public void ShowDia(string title,string context)
    {
        Messagebox = (GameObject)Resources.Load("Simple UI/MessageBox");
        Messagebox = GameObject.Instantiate(Messagebox, GameObject.Find("Canvas").transform) as GameObject;
        Messagebox.transform.localScale = new Vector3(1, 1, 1);
        Messagebox.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        Messagebox.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        Messagebox.GetComponent<RectTransform>().offsetMax = Vector2.zero;
        Messagebox.GetComponent<MessageBoxControll>().Content.text = context;
        Messagebox.GetComponent<MessageBoxControll>().Title.text = title;
        Messagebox.GetComponent<MessageBoxControll>().Close.onClick.AddListener(Close_btn);
        Messagebox.GetComponent<MessageBoxControll>().Confirm.onClick.AddListener(Close_btn);
    }
    public void Close_btn()
    {
        GameObject.Destroy(Messagebox);
    }
}
