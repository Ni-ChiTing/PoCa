using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class ClientConnect : MonoBehaviour
{
    public Text WaitText;
    // Start is called before the first frame update
    /*
    byte[] data;
    string Error_Message;

    private Thread t;
    public int udpPort = 9050;
    private void GetSeverIP()
    {
        try
        {
            t = new Thread(Receive);
            Debug.Log("Start listen");
            t.IsBackground = true;
            t.Start();
        }
        catch (Exception e)
        {
            Debug.LogError("錯誤信息：" + e.Message);
        }
    }
    Socket sock;
    private void Receive()
    {
            Debug.Log("Wait Data");
            sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint iep = new IPEndPoint(IPAddress.Any, udpPort);
            sock.Bind(iep);
            EndPoint ep = (EndPoint)iep;

            byte[] data = new byte[1024];
            int recv = sock.ReceiveFrom(data, ref ep);
            string stringData = Encoding.ASCII.GetString(data, 0, recv);

            Debug.Log(String.Format("received: {0} from: {1}", stringData, ep.ToString()));
            Thread.Sleep(200);

        sock.Close();
    }
    
    private Socket ServerSocket;
    private IPEndPoint Clients;
    private IPEndPoint Server;
    private EndPoint epSender;
    private byte[] SendData = new byte[1024];
    public int udpPort = 9050;
    //接受数据的字符数组
    private byte[] ReceiveData = new byte[1024];


    // Use this for initialization
    void Start()
    {
        //服务器Socket对象实例化
        ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        //服务器的IP和端口
        Server = new IPEndPoint(IPAddress.Any, udpPort);

        //Socket对象跟服务器端的IP和端口绑定
        ServerSocket.Bind(Server);

        //客户端的IP和端口，端口 0 表示任意端口
        Clients = new IPEndPoint(IPAddress.Any, 0);

        //实例化客户端 终点
        epSender = (EndPoint)Clients;
        ServerSocket.BeginReceiveFrom(ReceiveData, 0, ReceiveData.Length, SocketFlags.None, ref epSender, new AsyncCallback(ReceiveFromClients), epSender);
        Debug.Log("Start");
    }

    /// <summary>
    /// 异步接受，处理数据
    /// </summary>
    /// <param name="iar"></param>
    private void ReceiveFromClients(IAsyncResult iar)
    { 
        int reve = ServerSocket.EndReceiveFrom(iar, ref epSender);
        string str = System.Text.Encoding.UTF8.GetString(ReceiveData, 0, reve);
        Debug.Log("Get"+str);
        ServerSocket.BeginReceiveFrom(ReceiveData, 0, ReceiveData.Length, SocketFlags.None, ref epSender, new AsyncCallback(ReceiveFromClients), epSender);

    }
    */
  
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
