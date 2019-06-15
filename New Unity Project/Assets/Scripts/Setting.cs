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
    ushort maxpacketsize = 10000;
    int broadcastPort = 47777;
    int broadcastKey = 1000;
    int broadcastVersion = 1;
    int broadcastSubVersion = 1;

    string broadcastData = "HELLO";
    private static Socket sock;
    private static IPEndPoint iep1;
    private static byte[] data;
    private Thread t;

    public int udpPort = 9050;

    public void BroadcastIP()
    {
        sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        iep1 = new IPEndPoint(IPAddress.Broadcast, udpPort);
        data = Encoding.ASCII.GetBytes(Data.HostIP);
        sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
        t = new Thread(BroadcastMessage);
        t.Start();
    }
    private void BroadcastMessage()
    {
        while (true)
        {
            sock.SendTo(data, iep1);
            Debug.Log("Broadcast");
            Thread.Sleep(3000);
        }
    }
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
        BroadcastIP();
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
        Data.players.Add ( player2.text);
        Data.players.Add(player3.text);
        Data.players.Add(player4.text);
        Data.GameName = game_name.text;
        Data.PlayerNumber = int.Parse(player_number.text);
        Data.PlayerCardNumber = int.Parse(player_card_number.text);
        Data.TableCardNumber = int.Parse(table_card_number.text);
        if (need_animation.isOn == true) Data.NeedAnimation = true;
        else Data.NeedAnimation = false;
        if (need_draw_card.isOn == true) Data.NeedDrawCard = true;
        else Data.NeedDrawCard = false;

        SceneManager.LoadScene(2);
    }
}
