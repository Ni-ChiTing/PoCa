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
    private Thread t;
    private AndroidJavaObject _ajc;
    string IPs;
    /*
string broadcastData = "HELLO";
private static Socket sock;
private static IPEndPoint iep1;
private static byte[] data;
private Thread t;

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
}
static int upCount = 0;
static object lockObj = new object();
const bool resolveNames = false;
static void p_PingCompleted(object sender, PingCompletedEventArgs e)
{
string ip = (string)e.UserState;
if (e.Reply != null && e.Reply.Status == IPStatus.Success)
{
if (resolveNames)
{
string name;
try
{
IPHostEntry hostEntry = Dns.GetHostEntry(ip);
name = hostEntry.HostName;
}
catch (SocketException ex)
{
name = "?";
}
Debug.Log( ip + " " + name +"is up:" + e.Reply.RoundtripTime + "ms");
}
else
{
Debug.Log(ip + " " + "is up:" + e.Reply.RoundtripTime + "ms");
}
lock (lockObj)
{
upCount++;
}
}
else if (e.Reply == null)
{
Debug.Log("Pinging" +ip+" failed. (Null Reply object?)");
}
}
static void pingIP()
{
Debug.Log("start");
string hostName = System.Net.Dns.GetHostName();
string ipBase = System.Net.Dns.GetHostEntry(hostName).AddressList[0].ToString();
string[] ipParts = ipBase.Split('.');
ipBase = ipParts[0] + "." + ipParts[1] + "." + ipParts[2] + ".";
for (int i = 1; i < 255; i++)
{
string ip = ipBase + i.ToString();

System.Net.NetworkInformation.Ping p = new System.Net.NetworkInformation.Ping();
p.PingCompleted += new PingCompletedEventHandler(p_PingCompleted);
p.SendAsync(ip, 100, ip);
}
}
*/
    public string getsubnet()
    {
        string hostName = System.Net.Dns.GetHostName();
        string ipBase = System.Net.Dns.GetHostEntry(hostName).AddressList[0].ToString();
        string[] ipParts = ipBase.Split('.');
        ipBase = ipParts[0] + "." + ipParts[1] + "." + ipParts[2] ;
        return ipBase;
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
        //BroadcastIP();

        //Create UDP Client for broadcasting the serve
        //t = new Thread(pingIP);
        //t.Start();
        _ajc = new AndroidJavaObject("com.androidforunity.UnityFuntion");
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
    public void Player_btn()
    {
        ShowDiaResearch();
        //GameObject.Destroy(Messagebox);
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
    public void ShowDiaResearch()
    {
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
    public void Do_clk()
    {
        //bool success23 = _ajc.Call<bool>("showToast", "wait search!!");
        Messagebox.GetComponent<MessageBoxControll>().Confirm.gameObject.SetActive(false);
        Messagebox.GetComponent<MessageBoxControll>().Content.text = "Scanning .. .. ..";
        IPs = _ajc.Call<string>("startPingService", getsubnet());
        player2.text = IPs;
        Debug.Log(IPs);
        setPlayers(IPs);
        GameObject.Destroy(Messagebox);
    }
    public void Close_btn()
    {
        //string a = _ajc.Call<string>("startPingService", getsubnet());
        GameObject.Destroy(Messagebox);
    }
    public void setPlayers(string scannresult)
    {
        string[] ipParts = scannresult.Split(',');
        string hostName = System.Net.Dns.GetHostName();
        string ipBase = System.Net.Dns.GetHostEntry(hostName).AddressList[0].ToString();
        for ( int i = 0; i < ipParts.Length - 1; ++ i )
        {
            if ( ipParts[i] != ipBase)
            {
                Debug.Log(ipParts[i]);
                InitSocket(ipParts[i]);
            }
            
        }
    }
    string editString = "hello wolrd"; //编辑框文字

    //以下默认都是私有的成员
    Socket socket; //目标socket
    EndPoint serverEnd; //服务端
    IPEndPoint ipEnd; //服务端端口
    string recvStr; //接收的字符串
    string sendStr; //发送的字符串
    byte[] recvData = new byte[1024]; //接收的数据，必须为字节
    byte[] sendData = new byte[1024]; //发送的数据，必须为字节
    int recvLen; //接收的数据长度
    Thread connectThread; //连接线程
    int UdpPort = 10230;
    //初始化
    void InitSocket(string ip)
    {
        //定义连接的服务器ip和端口，可以是本机ip，局域网，互联网
        ipEnd = new IPEndPoint(IPAddress.Parse(ip),UdpPort);
        //定义套接字类型,在主线程中定义
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        //定义服务端
        socket.ReceiveTimeout = 1000;
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        serverEnd = (EndPoint)sender;
        print("waiting for sending UDP dgram");

        //建立初始连接，这句非常重要，第一次连接初始化了serverEnd后面才能收到消息
        SocketSend("hello");

        //开启一个线程连接，必须的，否则主线程卡死
        connectThread = new Thread(new ThreadStart(SocketReceive));
        connectThread.Start();
    }

    void SocketSend(string sendStr)
    {
        //清空发送缓存
        sendData = new byte[1024];
        //数据类型转换
        sendData = Encoding.ASCII.GetBytes(sendStr);
        //发送给指定服务端
        socket.SendTo(sendData, sendData.Length, SocketFlags.None, ipEnd);
    }

    //服务器接收
    void SocketReceive()
    {
        //进入接收循环
        while (true)
        {
            //对data清零
            recvData = new byte[1024];
            //获取客户端，获取服务端端数据，用引用给服务端赋值，实际上服务端已经定义好并不需要赋值
            recvLen = socket.ReceiveFrom(recvData, ref serverEnd);
            print("message from: " + serverEnd.ToString()); //打印服务端信息
            //输出接收到的数据
            recvStr = Encoding.ASCII.GetString(recvData, 0, recvLen);
            print(recvStr);
            break;
        }
        SocketQuit();
    }

    //连接关闭
    void SocketQuit()
    {
        //关闭线程
        if (connectThread != null)
        {
            connectThread.Interrupt();
            connectThread.Abort();
        }
        //最后关闭socket
        if (socket != null)
            socket.Close();
    }
}
