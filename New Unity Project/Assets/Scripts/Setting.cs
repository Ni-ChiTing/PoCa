#define DEBUG //如果define 就不會等有ACK才能進入遊戲 最後要不define才確保每個人都收到開始遊戲
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Setting : MonoBehaviour {
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
    private Thread t;
    private AndroidJavaObject _ajc;
    string IPs;
    int PlayerCount;
    Socket socket;
    EndPoint serverEnd;
    IPEndPoint ipEnd;
    string recvStr;
    string sendStr;
    byte[] recvData = new byte[1024];
    byte[] sendData = new byte[1024];
    int recvLen;
    Thread connectThread;
    int UdpPort = 10230;
    //Object thisLock = new Object();
    bool IfGo = false;
    enum InfoState { HostName, PlayerNumber, PlayerCardNumber, TableCardNumber, NeedDrawCard, NeedAnimation, Players, Done };
    InfoState state;
    /*SocketSend(Data.HostName);
                SocketSend(Data.PlayerNumber.ToString());
                SocketSend(Data.PlayerCardNumber.ToString());
                SocketSend(Data.TableCardNumber.ToString());
                SocketSend(Data.NeedDrawCard.ToString());
                SocketSend(Data.NeedAnimation.ToString());*/

    Dropdown gameModeDropDown;
    Dropdown playerNumDropDown;
    InputField playerCardNum;

    public string getsubnet() {
        string hostName = System.Net.Dns.GetHostName();
        string ipBase = System.Net.Dns.GetHostEntry(hostName).AddressList[0].ToString();
        string[] ipParts = ipBase.Split('.');
        ipBase = ipParts[0] + "." + ipParts[1] + "." + ipParts[2];
        return ipBase;
    }
    public void Start() {
        _ajc = new AndroidJavaObject("com.androidforunity.UnityFuntion");
        player2 = GameObject.Find("Canvas/conn/connect1/Text").GetComponent<Text>();
        player3 = GameObject.Find("Canvas/conn/connect2/Text").GetComponent<Text>();
        player4 = GameObject.Find("Canvas/conn/connect3/Text").GetComponent<Text>();
        game_name = GameObject.Find("Canvas/set/bg/game/GameModeDropdown/Label").GetComponent<Text>();
        player_number = GameObject.Find("Canvas/set/bg/set/pn/PlayerNumberDropDown/Label").GetComponent<Text>();
        player_card_number = GameObject.Find("Canvas/set/bg/set/cn/CardNumberInputField/Text").GetComponent<Text>();
        //table_card_number = GameObject.Find("Canvas/set/bg/set/TableCardInputField/Text").GetComponent<Text>();
        need_animation = GameObject.Find("Canvas/set/bg/toggle/Toggle").GetComponent<Toggle>();
        need_draw_card = GameObject.Find("Canvas/set/bg/toggle/Toggle1").GetComponent<Toggle>();

        gameModeDropDown = GameObject.Find("Canvas/set/bg/game/GameModeDropdown").GetComponent<Dropdown>();
        playerNumDropDown = GameObject.Find("Canvas/set/bg/set/pn/PlayerNumberDropDown").GetComponent<Dropdown>();
        playerCardNum = GameObject.Find("Canvas/set/bg/set/cn/CardNumberInputField").GetComponent<InputField>();

        gameModeDropDown.onValueChanged.AddListener(delegate { gameMode_onChange(); });
        playerNumDropDown.onValueChanged.AddListener(delegate { playerNumber_onChange(); });

        gameMode_onChange();
    }
    public void startBtn() {
        int temp;
        Data.GameName = game_name.text;
        Data.PlayerNumber = int.Parse(player_number.text); ;
        print("Player num = " + Data.PlayerNumber.ToString());
        Data.players.Add(player2.text);
        Data.players.Add(player3.text);
        Data.players.Add(player4.text);
        if (need_animation.isOn == true) Data.NeedAnimation = true;
        else Data.NeedAnimation = false;
        if (need_draw_card.isOn == true) Data.NeedDrawCard = true;
        else Data.NeedDrawCard = false;
        if (int.TryParse(player_card_number.text, out temp) == false) {
            ShowDia("warning", "Player card number need number !!");
        } else {
            Data.PlayerCardNumber = int.Parse(player_card_number.text);
            ShowDiaData();
            StartTransmitData();
            GameObject.Destroy(Messagebox);
            SendStart();
            SceneManager.LoadScene(2);
        }

    }
    public void StartTransmitData() {
        string[] ipParts = IPs.Split(',');
        string hostName = System.Net.Dns.GetHostName();
        string ipBase = System.Net.Dns.GetHostEntry(hostName).AddressList[0].ToString();
        for (int i = 0; i < ipParts.Length - 1; ++i) {
            if (ipParts[i] != ipBase) {
                Debug.Log(ipParts[i]);
                ipEnd = new IPEndPoint(IPAddress.Parse(ipParts[i]), UdpPort);
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                socket.ReceiveTimeout = 1000;
                IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                serverEnd = (EndPoint)sender;
                print("waiting for sending UDP dgram");
                state = InfoState.HostName;
                SendAndAckReceive();
                socket.Close();
            }
        }
    }

    public void ReceiveStartACK() {
        recvData = new byte[1024];
        recvLen = socket.ReceiveFrom(recvData, ref serverEnd);
        recvStr = Encoding.ASCII.GetString(recvData, 0, recvLen);
        while (recvStr != "ACK") {
            SocketSend("GO");
            recvData = new byte[1024];
            recvLen = socket.ReceiveFrom(recvData, ref serverEnd);
            recvStr = Encoding.ASCII.GetString(recvData, 0, recvLen);
        }
    }
    public void SendAndAckReceive() {
        int index = 0;
        while (state != InfoState.Done) {
            print(state);
            string str = "";
            switch (state) {
                case InfoState.HostName:
                    str = Data.HostName;
                    break;
                case InfoState.NeedAnimation:
                    str = Data.NeedAnimation.ToString();
                    break;
                case InfoState.NeedDrawCard:
                    str = Data.NeedDrawCard.ToString();
                    break;
                case InfoState.PlayerCardNumber:
                    str = Data.PlayerCardNumber.ToString();
                    break;
                case InfoState.PlayerNumber:
                    str = Data.PlayerNumber.ToString();
                    break;
                case InfoState.Players:
                    str = Data.players[index];
                    break;
            }
            SocketSend(str);
            recvData = new byte[1024];
            recvLen = socket.ReceiveFrom(recvData, ref serverEnd);
            recvStr = Encoding.ASCII.GetString(recvData, 0, recvLen);
            switch (state) {
                case InfoState.HostName:
                    if (recvStr == "ACK")
                        state = InfoState.NeedAnimation;
                    break;
                case InfoState.NeedAnimation:
                    if (recvStr == "ACK")
                        state = InfoState.NeedDrawCard;
                    break;
                case InfoState.NeedDrawCard:
                    if (recvStr == "ACK")
                        state = InfoState.PlayerCardNumber;
                    break;
                case InfoState.PlayerCardNumber:
                    if (recvStr == "ACK")
                        state = InfoState.PlayerNumber;
                    break;
                case InfoState.PlayerNumber:
                    if (recvStr == "ACK")
                        state = InfoState.Players;
                    break;
                case InfoState.Players:
                    if (index == Data.PlayerNumber - 1) {
                        state = InfoState.Done;
                    } else {
                        print("index" + index.ToString());
                        ++index;
                        state = InfoState.Players;
                    }
                    break;
            }
        }
        SocketSend("DONE");
        print("SSS");
    }

    void SendStart() {
        print("START SEND");
        string[] ipParts = IPs.Split(',');
        string hostName = System.Net.Dns.GetHostName();
        string ipBase = System.Net.Dns.GetHostEntry(hostName).AddressList[0].ToString();
        for (int i = 0; i < ipParts.Length - 1; ++i) {
            if (ipParts[i] != ipBase) {
                Debug.Log(ipParts[i]);
                ipEnd = new IPEndPoint(IPAddress.Parse(ipParts[i]), UdpPort);
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                socket.ReceiveTimeout = 1000;
                IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                serverEnd = (EndPoint)sender;
                //print("waiting for sending UDP dgram");
                state = InfoState.HostName;
                SocketSend("GO");
#if DEBUG
                ReceiveStartACK();
#endif
                socket.Close();
            }

        }
    }
    public void Player_btn() {
        ShowDiaResearch();
        //GameObject.Destroy(Messagebox);
    }
    public void ShowDia(string title, string context) {
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
    public void ShowDiaResearch() {
        Messagebox = (GameObject)Resources.Load("Simple UI/MessageBox_r");
        Messagebox = GameObject.Instantiate(Messagebox, GameObject.Find("Canvas").transform) as GameObject;
        Messagebox.transform.localScale = new Vector3(1, 1, 1);
        Messagebox.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        Messagebox.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        Messagebox.GetComponent<RectTransform>().offsetMax = Vector2.zero;
        Messagebox.GetComponent<MessageBoxControll>().Content.text = "Click to search and wait!";
        Messagebox.GetComponent<MessageBoxControll>().Title.text = "Information";
        Messagebox.GetComponent<MessageBoxControll>().Close.onClick.AddListener(Close_btn);
        Messagebox.GetComponent<MessageBoxControll>().Confirm.onClick.AddListener(Do_clk);
    }
    public void ShowDiaData() {
        Messagebox = (GameObject)Resources.Load("Simple UI/MessageBox_r");
        Messagebox = GameObject.Instantiate(Messagebox, GameObject.Find("Canvas").transform) as GameObject;
        Messagebox.transform.localScale = new Vector3(1, 1, 1);
        Messagebox.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        Messagebox.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        Messagebox.GetComponent<RectTransform>().offsetMax = Vector2.zero;
        Messagebox.GetComponent<MessageBoxControll>().Content.text = "Transmit Data";
        Messagebox.GetComponent<MessageBoxControll>().Title.text = "Information";
        Messagebox.GetComponent<MessageBoxControll>().Close.onClick.AddListener(Close_btn);
        Messagebox.GetComponent<MessageBoxControll>().Confirm.gameObject.SetActive(false);
    }
    public void DoMes() {
        Messagebox.GetComponent<MessageBoxControll>().Confirm.gameObject.SetActive(false);
        Messagebox.GetComponent<MessageBoxControll>().Content.text = "Scanning .. .. ..";
    }
    public void Do_clk() {
        //bool success23 = _ajc.Call<bool>("showToast", "wait search!!");

        //DoMes();
        IPs = _ajc.Call<string>("startPingService", getsubnet());
        Debug.Log(IPs);
        PlayerCount = 0;
        setPlayers(IPs);
        PrintINFO();
        GameObject.Destroy(Messagebox);

    }
    public void Close_btn() {
        //string a = _ajc.Call<string>("startPingService", getsubnet());
        GameObject.Destroy(Messagebox);
    }
    public void setPlayers(string scannresult) {
        Data.playerIP.Clear();
        Data.players.Clear();
        string[] ipParts = scannresult.Split(',');
        string hostName = System.Net.Dns.GetHostName();
        string ipBase = System.Net.Dns.GetHostEntry(hostName).AddressList[0].ToString();
        Data.players.Add(Data.MyName);
        Data.playerIP.Add(ipBase);
        for (int i = 0; i < ipParts.Length - 1; ++i) {
            if (ipParts[i] != ipBase) {
                Debug.Log(ipParts[i]);
                InitSocket(ipParts[i]);

            }

        }
    }

    //初始化
    void InitSocket(string ip) {
        ipEnd = new IPEndPoint(IPAddress.Parse(ip), UdpPort);
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        socket.ReceiveTimeout = 1000;
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        serverEnd = (EndPoint)sender;
        print("waiting for sending UDP dgram");
        SocketSend("hello");
        SocketReceive();
    }


    void SocketSend(string sendStr) {
        sendData = new byte[1024];
        sendData = Encoding.ASCII.GetBytes(sendStr);
        socket.SendTo(sendData, sendData.Length, SocketFlags.None, ipEnd);
    }

    void SocketReceive() {
        string data = "";
        recvData = new byte[1024];
        recvLen = socket.ReceiveFrom(recvData, ref serverEnd);
        print("message from: " + serverEnd.ToString());
        data = serverEnd.ToString();
        string[] realip = data.Split(':');
        Data.playerIP.Add(realip[0]);
        recvStr = Encoding.ASCII.GetString(recvData, 0, recvLen);
        data += "," + recvStr;
        if (PlayerCount == 0) {
            player2.text = recvStr;
            //++PlayerCount;
        } else if (PlayerCount == 1) {
            player3.text = recvStr;
            //++PlayerCount;
        } else if (PlayerCount == 2) {
            player4.text = recvStr;

        } else {
            Debug.Log("player is full");
        }
        ++PlayerCount;
        print(recvStr);
        /*
        while (true)
        {
            if (IfGo)
            {
                SocketSend("INFO");
                SocketSend(Data.HostName);
                SocketSend(Data.PlayerNumber.ToString());
                SocketSend(Data.PlayerCardNumber.ToString());
                SocketSend(Data.TableCardNumber.ToString());
                SocketSend(Data.NeedDrawCard.ToString());
                SocketSend(Data.NeedAnimation.ToString());
                foreach (var i in Data.players)
                {
                    SocketSend(i);
                }
                SocketSend("done");
                SocketSend("GO");
                break;
            }
            else
            {
                SocketSend("Don't GO");
            }
            Thread.Sleep(500);
        }*/
        SocketQuit();
    }

    void SocketQuit() {
        /*
        if (connectThread != null)
        {
            connectThread.Interrupt();
            connectThread.Abort();
        }*/
        if (socket != null)
            socket.Close();
    }
    public void PrintINFO() {
        print("Hostname = " + Data.MyName);
        print("Player num = " + PlayerCount.ToString());
        print("Host IP = " + Data.HostIP);
        int j = 0;
        foreach (var i in Data.players) {
            print("Player " + j.ToString() + " = " + i);
            ++j;
        }
        j = 0;
        foreach (var i in Data.playerIP) {
            print("Player IP" + j.ToString() + " = " + i);
            ++j;
        }
    }
    void Update() {

    }
    void OnApplicationQuit() {
        SocketQuit();
    }

    private void gameMode_onChange() {
        print("gameMode_onChange(): " + gameModeDropDown.value);
        switch (gameModeDropDown.value) {
            case 0: // 21點
                print("gameMode: 21點");
                playerCardNum.text = "2";
                break;
            case 1: // 大老二
                print("gameMode: 大老二");
                playerCardNum.text = "13";
                break;
            case 2: // 抽鬼牌
                print("gameMode: 抽鬼牌");
                playerCardNum.text = "13";
                break;
            case 3: // 橋牌
                print("gameMode: 橋牌");
                playerCardNum.text = "13";
                break;
            case 4: // 牌7
                print("gameMode: 牌7");
                playerCardNum.text = "13";
                break;
        };
        updateSetting();
    }

    private void playerNumber_onChange() {
        print("playerNumber_onChange(): " + player_number.text);
        updateSetting();
    }

    private void updateSetting() {
        //table_card_number.text = (52 - int.Parse(player_number.text) * int.Parse(playerCardNum.text)).ToString();
    }
    
}

