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
    Socket socket; //目标socket
    EndPoint clientEnd; //客户端
    IPEndPoint ipEnd; //侦听端口
    string recvStr; //接收的字符串
    string sendStr; //发送的字符串
    byte[] recvData = new byte[1024]; //接收的数据，必须为字节
    byte[] sendData = new byte[1024]; //发送的数据，必须为字节
    int recvLen; //接收的数据长度
    Thread connectThread; //连接线程
    int UdpPort = 10230;

    //初始化
    void InitSocket()
    {
        //定义侦听端口,侦听任何IP
        ipEnd = new IPEndPoint(IPAddress.Any, UdpPort);
        //定义套接字类型,在主线程中定义
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        //服务端需要绑定ip
        socket.Bind(ipEnd);
        //定义客户端
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        clientEnd = (EndPoint)sender;
        print("waiting for UDP dgram");

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
        //发送给指定客户端
        socket.SendTo(sendData, sendData.Length, SocketFlags.None, clientEnd);
    }

    //服务器接收
    void SocketReceive()
    {
        //进入接收循环
        while (true)
        {
            //对data清零
            recvData = new byte[1024];
            //获取客户端，获取客户端数据，用引用给客户端赋值
            recvLen = socket.ReceiveFrom(recvData, ref clientEnd);
            print("message from: " + clientEnd.ToString()); //打印客户端信息
            //输出接收到的数据
            recvStr = Encoding.ASCII.GetString(recvData, 0, recvLen);
            print(recvStr);
            //将接收到的数据经过处理再发送出去
            sendStr = "From Server: " + recvStr;
            SocketSend(sendStr);
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
        print("disconnect");
    }
    void Start()
    {
        InitSocket();
    }
    // Update is called once per frame
    void Update()
    {
        
    }

}
