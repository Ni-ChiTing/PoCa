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
    void Start()
    {
        Debug.Log("received!!");
        GetSeverIP();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
