#define DEBUG //ForDebug without ACK
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClientConnect : MonoBehaviour
{
    public Text WaitText;
    
    Socket socket; //socket
    //EndPoint clientEnd; //client
    IPEndPoint ipEnd; //port
    string recvStr; //receive string
    string sendStr; //send string
    byte[] recvData = new byte[1024]; //byte recieve
    byte[] sendData = new byte[1024]; 
    int recvLen; 
    Thread connectThread; 
    int UdpPort = 10230;
    bool IfCon = false;
    bool GO = false;
    bool IfReset = false;
    enum InfoState { HostName, PlayerNumber, PlayerCardNumber, TableCardNumber, NeedDrawCard, NeedAnimation, Players, Done };
    InfoState state;
    UdpClient udpClient = null;
    EndPoint serverEnd;
    //初始化
    void InitSocket()
    {
        IfCon = false;
        string hostName = System.Net.Dns.GetHostName();
        string ipBase = System.Net.Dns.GetHostEntry(hostName).AddressList[0].ToString();
        ipEnd = new IPEndPoint(IPAddress.Parse(Data.HostIP), UdpPort);
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        socket.Bind(new IPEndPoint(IPAddress.Parse(ipBase), UdpPort));
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        serverEnd = (EndPoint)sender;
        print("waiting for sending UDP dgram");
        connectThread = new Thread(new ThreadStart(ClientReceiveServer));
        connectThread.Start();
    }
    public void ClientSendServer(string s)
    {
        sendData = new byte[1024];
        sendData = Encoding.ASCII.GetBytes(s);
        socket.SendTo(sendData, sendData.Length, SocketFlags.None, ipEnd);
    }
    public void ClientReceiveServer()
    {
        
        while (true)
        {
            ClientSendServer(Data.MyName);
            recvData = new byte[1024];
            recvLen = socket.ReceiveFrom(recvData, ref serverEnd);
            print("message from: " + serverEnd.ToString());
            recvStr = Encoding.ASCII.GetString(recvData, 0, recvLen);
            print(recvStr);
            if (recvStr == "ACK")
            {
                break;
            }
            
        }
        print("GetName");
        state = InfoState.HostName;
        while (state != InfoState.Done)
        {
            print("Client Now State = "+state);
            recvData = new byte[1024];
            recvLen = socket.ReceiveFrom(recvData, ref serverEnd);
            print("message from: " + serverEnd.ToString());
            recvStr = Encoding.ASCII.GetString(recvData, 0, recvLen);
            print(recvStr);
            switch (state)
            {
                case InfoState.HostName:
                    Data.HostName = recvStr;
                    state = InfoState.NeedAnimation;
                    break;
                case InfoState.NeedAnimation:
                    Data.NeedAnimation = bool.Parse(recvStr);
                    state = InfoState.NeedDrawCard;
                    break;
                case InfoState.NeedDrawCard:
                    Data.NeedDrawCard = bool.Parse(recvStr);
                    state = InfoState.PlayerCardNumber;
                    break;
                case InfoState.PlayerCardNumber:
                    Data.PlayerCardNumber = int.Parse(recvStr);
                    state = InfoState.PlayerNumber;
                    break;
                case InfoState.PlayerNumber:
                    Data.PlayerNumber = int.Parse(recvStr);
                    state = InfoState.Players;
                    break;
                case InfoState.Players:
                    if (recvStr == "DONE")
                        state = InfoState.Done;
                    else
                    {
                        Data.players.Add(recvStr);
                        state = InfoState.Players;
                    }
                    break;
            }
            ClientSendServer("ACK");

        }
        print("DATA GET DOne");
        while (true)
        {
            recvData = new byte[1024];
            recvLen = socket.ReceiveFrom(recvData, ref serverEnd);
            recvStr = Encoding.ASCII.GetString(recvData, 0, recvLen);
            print(recvStr);
            if (recvStr == "GO")
            {
                GO = true;
#if DEBUG
                ClientSendServer("ACK");
#endif
                break;
            }
        }
        SocketQuit();
    }
    public void recvUdpBroadcast()
    {
        print("recvUdpBroadcast()");
            
        UdpClient udpClient = new UdpClient();
        udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, UdpPort));
        udpClient.Client.EnableBroadcast = true;

        var from = new IPEndPoint(0, 0);
        Task.Run(() => {
            while (true)
            {
                var recvBuffer = udpClient.Receive(ref from);
                Data.HostIP = (Encoding.UTF8.GetString(recvBuffer));
                IfCon = true;
                break;
            }
        });
        
        
        
    }

    /*
    void GetData()
    {
        IfCon = false;
        state = InfoState.HostName;
        ipEnd = new IPEndPoint(IPAddress.Any, UdpPort);
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        socket.Bind(ipEnd);
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        clientEnd = (EndPoint)sender;
        print("waiting for UDP dgram 22");
        connectThread = new Thread(new ThreadStart(SocketGetData));
        connectThread.Start();
    }
    void SocketSend(string sendStr)
    {
        sendData = new byte[1024];
        sendData = Encoding.ASCII.GetBytes(sendStr);
        socket.SendTo(sendData, sendData.Length, SocketFlags.None, clientEnd);
    }
    void SocketGetData()
    {
        
        while (state != InfoState.Done)
        {
            print(state);
            recvData = new byte[1024];
            recvLen = socket.ReceiveFrom(recvData, ref clientEnd);
            print("message from: " + clientEnd.ToString());
             
            recvStr = Encoding.ASCII.GetString(recvData, 0, recvLen);
            print(recvStr);
            switch (state)
            {
                case InfoState.HostName:
                    Data.HostName = recvStr;
                    state = InfoState.NeedAnimation;
                    break;
                case InfoState.NeedAnimation:
                    Data.NeedAnimation = bool.Parse(recvStr);
                    state = InfoState.NeedDrawCard;
                    break;
                case InfoState.NeedDrawCard:
                    Data.NeedDrawCard = bool.Parse(recvStr);
                    state = InfoState.PlayerCardNumber;
                    break;
                case InfoState.PlayerCardNumber:
                    Data.PlayerCardNumber = int.Parse(recvStr);
                    state = InfoState.PlayerNumber;
                    break;
                case InfoState.PlayerNumber:
                    Data.PlayerNumber = int.Parse(recvStr);
                    state = InfoState.Players;
                    break;
                case InfoState.Players:
                    if (recvStr == "DONE")
                        state = InfoState.Done;
                    else
                    {
                        Data.players.Add(recvStr);
                        state = InfoState.Players;
                    }
                    break;
            }
            SocketSend("ACK");
            
        }
        print("DATA GET DOne");
        while (true)
        {
            recvData = new byte[1024];
            recvLen = socket.ReceiveFrom(recvData, ref clientEnd);
            recvStr = Encoding.ASCII.GetString(recvData, 0, recvLen);
            print(recvStr);
            if (recvStr == "GO")
            {
                GO = true;
                #if DEBUG
                SocketSend("ACK");
                #endif
                break;
            }
        }
        SocketQuit();

    }*/
    public void Reset_clk()
    {
        WaitText.text = "Waiting ....";
        SocketQuit();
        state = InfoState.HostName;
        recvUdpBroadcast();
    }
    /*
    void SocketReceive()
    {
        
        recvData = new byte[1024];
        recvLen = socket.ReceiveFrom(recvData, ref clientEnd);
        print("message from: " + clientEnd.ToString());
        string data = clientEnd.ToString();
        string[] realip = data.Split(':');
        Data.HostIP = realip[0];
        print("host IP = "+ Data.HostIP);
        recvStr = Encoding.ASCII.GetString(recvData, 0, recvLen);
        print(recvStr)
        sendStr = Data.MyName;
        SocketSend(sendStr);
        IfCon = true;
        /*while (true)
        {
            recvData = new byte[1024];
            recvLen = socket.ReceiveFrom(recvData, ref clientEnd);
            print("message from: " + clientEnd.ToString()); 
            recvStr = Encoding.ASCII.GetString(recvData, 0, recvLen);
            print(recvStr);
            if(recvStr == "INFO")
            {
                recvData = new byte[1024];
                recvLen = socket.ReceiveFrom(recvData, ref clientEnd);
                print("message from: " + clientEnd.ToString());
                recvStr = Encoding.ASCII.GetString(recvData, 0, recvLen);
                Data.HostName = recvStr;
                recvData = new byte[1024];
                recvLen = socket.ReceiveFrom(recvData, ref clientEnd);
                print("message from: " + clientEnd.ToString());
                recvStr = Encoding.ASCII.GetString(recvData, 0, recvLen);
                Data.PlayerNumber = int.Parse(recvStr);
                recvLen = socket.ReceiveFrom(recvData, ref clientEnd);
                print("message from: " + clientEnd.ToString());
                recvStr = Encoding.ASCII.GetString(recvData, 0, recvLen);
                Data.PlayerCardNumber = int.Parse(recvStr);
                recvLen = socket.ReceiveFrom(recvData, ref clientEnd);
                print("message from: " + clientEnd.ToString());
                recvStr = Encoding.ASCII.GetString(recvData, 0, recvLen);
                Data.TableCardNumber = int.Parse(recvStr);
                recvLen = socket.ReceiveFrom(recvData, ref clientEnd);
                print("message from: " + clientEnd.ToString());
                recvStr = Encoding.ASCII.GetString(recvData, 0, recvLen);
                Data.NeedDrawCard = bool.Parse(recvStr);
                recvLen = socket.ReceiveFrom(recvData, ref clientEnd);
                print("message from: " + clientEnd.ToString());
                recvStr = Encoding.ASCII.GetString(recvData, 0, recvLen);
                Data.NeedAnimation = bool.Parse(recvStr);
            }
            if (recvStr == "GO")
            {
                GO = true;
                break;
            }
        }
        SocketQuit();
        print("Name Done");
    }
*/
    void SocketQuit()
    {
        if (connectThread != null)
        {
            connectThread.Interrupt();
            connectThread.Abort();
        }
        if (socket != null)
            socket.Close();
        print("disconnect");
    }
    void Start()
    {
        recvUdpBroadcast();
        //InitSocket();
    }
    // Update is called once per frame
    void Update()
    {
        if (IfCon)
        {
            InitSocket();
            WaitText.text = "Connect to server , wating Host";
           // GetData();
        }
        if (GO)
        {
            PrintINFO();
            SceneManager.LoadScene(2);

        }

        if (Input.GetKeyDown(KeyCode.Escape)) 
            SceneManager.LoadScene(0);

    }
    public void PrintINFO()
    {
        print("Hostname = " + Data.MyName);
        print("Player num = " + Data.PlayerNumber.ToString());
        print("Host IP = " + Data.HostIP);
        int j = 0;
        foreach (var i in Data.players)
        {
            print("Player " + j.ToString() + " = " + i);
        }
    }
    void OnApplicationQuit()
    {
        SocketQuit();
        
    }


}
