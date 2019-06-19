using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class GameServerControll : MonoBehaviour {
    Socket Ssocket; //socket
    EndPoint SclientEnd; //client
    IPEndPoint SipEnd; //port
    string SrecvStr; //receive string
    string SsendStr;
    byte[] SrecvData = new byte[1024]; //byte recieve
    byte[] SsendData = new byte[1024];
    int SrecvLen;
    Thread SconnectThread;
    int UdpPort = 10231;

    Socket socket;
    EndPoint serverEnd;
    IPEndPoint ipEnd;
    string recvStr;
    string sendStr;
    byte[] recvData = new byte[1024];
    byte[] sendData = new byte[1024];
    int recvLen;
    Thread connectThread;

    int readyClient = 0;

    bool haveGive = false;

    const string TakeCard_ = "T";
    const string AddCardFromTable_ = "A";
    const string DiscardCard_ = "D";
    const string GetNowHandCard_ = "G";
    const string GetAllCard_ = "AC";
    const string Info_ = "I";
    const string Spade = "S";
    const string Heart = "H";
    const string Diamond = "DI";
    const string Club = "C";
    const string ClientGetInfoReply_ = "R";
    const string GiveCard_ = "GC";
    const string PutOnTable_ = "PT";

    UdpClient udpClient;

    string lastReceive;

    //data.playerIP[0] is host
    //data.players[0] is host
    //card = 0 -- > 梅花 A card = 1 --> 方塊 A card == 2 --> 愛心 A card == 3 --> 黑桃 A 
    // S 黑桃 H 愛心 DI 方塊 C 梅花

    public void giveCard(int cardId, int receiver) {
        string mesg = GiveCard_ + "," + cardId.ToString() + "," + receiver.ToString();
        print("send_give" + mesg);
        if (Data.IamHost)
            ServerSendAllClient(mesg);
        else
            ClientSendServer(mesg);
    }
    public void putOnTable(int card, float x, float z) {
        string mesg = PutOnTable_ + "," + card.ToString() + "," + x.ToString() + "," + z.ToString();
        print("send_table" + mesg);

        if (Data.IamHost)
            ServerSendAllClient(mesg);
        else
            ClientSendServer(mesg);
    }

    public static int PosToId( int goalPos, int viewerId, int playerNum) {
        return (goalPos + viewerId) % playerNum;
    }

    public static int IdToPos(int goalId, int viewerId, int playerNum) {
        return (goalId + (playerNum - viewerId)) % playerNum;
    }

    public void InitServerSocket() {
        udpClient = new UdpClient();
        udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, UdpPort));
        udpClient.Client.EnableBroadcast = true;
    }

    public void ServerSendClient(string endpoint_ip, string send_msg) {
        var data = Encoding.UTF8.GetBytes(send_msg);
        udpClient.Send(data, data.Length, endpoint_ip, UdpPort);
    }

    public void ServerSendAllClient(string send_msg) {
        print("send " + send_msg);
        var data = Encoding.UTF8.GetBytes(send_msg);
        string subnet_s = getsubnet();
        udpClient.Send(data, data.Length, subnet_s, UdpPort);
    }

    void ServerListenClient() {
        Task.Run(() => {
            var from = new IPEndPoint(0, 0);
            while (true) {
                var recvBuffer = udpClient.Receive(ref from);
                // HERE! Do something after received data.
                print("server get " + Encoding.UTF8.GetString(recvBuffer));
                ResolveMSG(Encoding.UTF8.GetString(recvBuffer));
            }
        });
    }
        
    void InitClientSocket() {
        udpClient = new UdpClient();
        udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, UdpPort));
        udpClient.Client.EnableBroadcast = true;
    }

    public void ClientSendServer(string send_msg) {
        var data = Encoding.UTF8.GetBytes(send_msg);
        udpClient.Send(data, data.Length, Data.HostIP, UdpPort);
    }

    public void ClientReceiveServer() {
        Task.Run(() => {
            var from = new IPEndPoint(0, 0);
            while (true) {
                var recvBuffer = udpClient.Receive(ref from);
                print("c r s " + Encoding.UTF8.GetString(recvBuffer));
                ResolveMSG(Encoding.UTF8.GetString(recvBuffer));
            }
        });
    }

    public string getsubnet() {
        string hostName = System.Net.Dns.GetHostName();
        string ipBase = System.Net.Dns.GetHostEntry(hostName).AddressList[0].ToString();
        string[] ipParts = ipBase.Split('.');
        ipBase = ipParts[0] + "." + ipParts[1] + "." + ipParts[2] + ".255";
        return ipBase;
    }

    /*
    void InitServerSocket() {
        SipEnd = new IPEndPoint(IPAddress.Any, UdpPort);
        Ssocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        Ssocket.Bind(SipEnd);
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        SclientEnd = new IPEndPoint(IPAddress.Parse(Data.playerIP[1]), 0);
        print("waiting for UDP dgram");
        SconnectThread = new Thread(new ThreadStart(ServerListenClient));
        SconnectThread.Start();
    }
    public void ServerSendClient(string SsendStr) //default send player 1 use SetClient To Change 
    {
        print("Single Send  = " + SsendStr + "to" + SclientEnd.ToString());
        SsendData = new byte[1024];
        SsendData = Encoding.ASCII.GetBytes(SsendStr);
        Ssocket.SendTo(SsendData, SsendData.Length, SocketFlags.None, SclientEnd);
    }
    public void ServerSendAllClient(string SsendStr) //Send All Client but not test 
    {
        print("Send  = " + SsendStr);
        for ( int i = 1; i <Data.PlayerNumber; ++i)
        {
            print("Send IP is " + Data.playerIP[i]);
            SetClientSend(Data.playerIP[i]);
            ServerSendClient(SsendStr);

        }
        SetClientSend(Data.playerIP[1]);
        //SsendData = new byte[1024];
       // SsendData = Encoding.ASCII.GetBytes(sendStr);
       // EndPoint clientEnds = new IPEndPoint(IPAddress.Any, 0);
       // Ssocket.SendTo(SsendData, SsendData.Length, SocketFlags.None, clientEnds);
    }

    public void SetClientSend(string IP) // change to send other player
    {
        SclientEnd = new IPEndPoint(IPAddress.Parse(IP), 0);

    }
    void ServerListenClient() {
        while (true) {
            SrecvData = new byte[1024];
            SrecvLen = Ssocket.ReceiveFrom(SrecvData, ref SclientEnd);
            print("message from: " + SclientEnd.ToString());
            SrecvStr = Encoding.ASCII.GetString(SrecvData, 0, SrecvLen);
            print(SrecvStr);
            ResolveMSG(SrecvStr);
        }
    }

    void InitClientSocket() {
        ipEnd = new IPEndPoint(IPAddress.Parse(Data.HostIP), UdpPort);
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        serverEnd = (EndPoint)sender;
        print("waiting for sending UDP dgram");
        ClientSendServer("hello");
        connectThread = new Thread(new ThreadStart(ClientReceiveServer));
        connectThread.Start();
    }
    public void ClientSendServer(string s) {
        sendData = new byte[1024];
        sendData = Encoding.ASCII.GetBytes(s);
        socket.SendTo(sendData, sendData.Length, SocketFlags.None, ipEnd);
    }
    public void ClientReceiveServer() {
        while (true) {
            /*
            recvData = new byte[1024];
            recvLen = socket.ReceiveFrom(recvData, ref serverEnd);
            print("message from: " + serverEnd.ToString());
            recvStr = Encoding.ASCII.GetString(recvData, 0, recvLen);
            print(recvStr);
            recvData = new byte[1024];
            recvLen = socket.ReceiveFrom(recvData, ref serverEnd);
            print("message from: " + serverEnd.ToString());
            recvStr = Encoding.ASCII.GetString(recvData, 0, recvLen);
            print(recvStr);
            ResolveMSG(recvStr);
        }

    }

*/

    public int CardNameToIndex(string card) //將 card 從 string 表示轉乘 int
    {
        string[] sp = card.Split(',');
        int num = int.Parse(sp[1]);
        if (sp[0] == Spade) {
            num = (num - 1) * 4 + 3;
        } else if (sp[0] == Diamond) {
            num = (num - 1) * 4 + 1;
        } else if (sp[0] == Heart) {
            num = (num - 1) * 4 + 2;
        } else if (sp[0] == Club) {
            num = (num - 1) * 4;
        }
        return num;
    }
    public string CardIndexToName(int cardindex) //將 card 從 int 表示轉成 string
    {
        string r = "";
        if (cardindex % 4 == 0) {
            r = Club + "," + ((cardindex / 4) + 1);
        } else if (cardindex % 4 == 1) {
            r = Diamond + "," + ((cardindex / 4) + 1);
        } else if (cardindex % 4 == 2) {
            r = Heart + "," + ((cardindex / 4) + 1);
        } else if (cardindex % 4 == 3) {
            r = Spade + "," + ((cardindex / 4) + 1);
        }
        return r;
    }
    public void UnwrapAndSetHandCard(string r) {
        print("Unwrap ss = " + r);
        string[] sp = r.Split(',');
        int index = Data.players.IndexOf(sp[0]);
        for (int i = 0; i < sp.Length; ++i) {
            print(sp[i]);
        }
        if (index == -1) {
            print("None Find");
        } else {
            if (index == 0) {
                Data.PlayerHostCard.Clear();
                for (int i = 1; i < sp.Length; ++i) {
                    Data.PlayerHostCard.Add(int.Parse(sp[i]));
                }
            } else if (index == 1) {
                Data.PlayerOneCard.Clear();
                for (int i = 1; i < sp.Length; ++i) {
                    Data.PlayerOneCard.Add(int.Parse(sp[i]));
                }
            } else if (index == 2) {
                Data.PlayerTwoCard.Clear();
                for (int i = 1; i < sp.Length; ++i) {
                    Data.PlayerTwoCard.Add(int.Parse(sp[i]));
                }
            } else if (index == 3) {
                Data.PlayerThreeCard.Clear();
                for (int i = 1; i < sp.Length; ++i) {
                    Data.PlayerThreeCard.Add(int.Parse(sp[i]));
                }
            }

        }
    }
    public void UnwrapAndSetHandCard(string name, string r) {
        print("Unwrap ss = " + r);
        string[] sp = r.Split(',');
        int index = Data.players.IndexOf(name);
        if (index == -1) {
            print("None Find");
        } else {
            if (index == 0) {
                Data.PlayerHostCard.Clear();
                for (int i = 0; i < sp.Length; ++i) {
                    Data.PlayerHostCard.Add(int.Parse(sp[i]));
                }
            } else if (index == 1) {
                Data.PlayerOneCard.Clear();
                for (int i = 0; i < sp.Length; ++i) {
                    Data.PlayerOneCard.Add(int.Parse(sp[i]));
                }
            } else if (index == 2) {
                Data.PlayerTwoCard.Clear();
                for (int i = 0; i < sp.Length; ++i) {
                    Data.PlayerTwoCard.Add(int.Parse(sp[i]));
                }
            } else if (index == 3) {
                Data.PlayerThreeCard.Clear();
                for (int i = 0; i < sp.Length; ++i) {
                    Data.PlayerThreeCard.Add(int.Parse(sp[i]));
                }
            }

        }
    }
    public string WrapAllCard()
    {
        string card = GetAllCard_ + ",";
        for (int i = 0; i < 52; ++i)
        {
            if ( i == 51)
                card = card + Data.Cards[i].ToString() ;
            else
                card = card + Data.Cards[i].ToString() + ",";

        }
        return card;
    }
    public void UnwrapAllCard(string card)
    {
        string[] sp = card.Split(',');
        for (int i = 0; i < 52; ++i)
        {
            Data.Cards[i] = int.Parse(sp[i+1]);
        }
        AllCardCon.allCardCon.StartDistribute();
    }
    public string WrapHandCardToString(string name) // Wrap Hand Card To String 以利server 發送
    {
        int index = Data.players.IndexOf(name);
        print("name = " + name);
        if (index == -1) {
            return "None Find";
        } else {
            string r = name + ",";
            if (index == 0) {
                for (int i = 0; i < Data.PlayerHostCard.Count; ++i) {
                    if (i == Data.PlayerHostCard.Count - 1) {
                        r = r + Data.PlayerHostCard[i].ToString();
                    } else {
                        r = r + Data.PlayerHostCard[i].ToString() + ",";
                    }
                }
            } else if (index == 1) {
                for (int i = 0; i < Data.PlayerOneCard.Count; ++i) {
                    if (i == Data.PlayerOneCard.Count - 1) {
                        r = r + Data.PlayerOneCard[i].ToString();
                    } else {
                        r = r + Data.PlayerOneCard[i].ToString() + ",";
                    }
                }
            } else if (index == 2) {
                for (int i = 0; i < Data.PlayerTwoCard.Count; ++i) {
                    if (i == Data.PlayerTwoCard.Count - 1) {
                        r = r + Data.PlayerTwoCard[i].ToString();
                    } else {
                        r = r + Data.PlayerTwoCard[i].ToString() + ",";
                    }
                }
            } else if (index == 3) {
                for (int i = 0; i < Data.PlayerThreeCard.Count; ++i) {
                    if (i == Data.PlayerThreeCard.Count - 1) {
                        r = r + Data.PlayerThreeCard[i].ToString();
                    } else {
                        r = r + Data.PlayerThreeCard[i].ToString() + ",";
                    }
                }
            }
            return r;
        }
    }
    public void ResolveMSG(string recv) // Can Modify by you
    {
        if (lastReceive == recv)
            return;
        lastReceive = recv;


        string[] r = recv.Split(',');
        print("recv " + recv);
        for (int i = 0; i < r.Length; ++i) {
            if (i == 0) {
                if (r[i] == DiscardCard_)
                {
                    print("Discrd");
                }
                else if (r[i] == TakeCard_)
                {
                    print("Take Other's Card");
                }
                else if (r[i] == AddCardFromTable_)
                {
                    print("Add Card From Tabel");
                }
                else if (r[i] == GetNowHandCard_)
                {
                    print("Someone's Hand Card");
                }
                else if (r[i] == GetAllCard_)
                {
                    print("Get All Card");
                    UnwrapAllCard(recv);
                }
                else if (r[i] == Info_)
                {
                    GetInfo(r);
                }
                else if (r[i] == ClientGetInfoReply_)
                {
                    if (++readyClient == Data.PlayerNumber - 1)
                    {
                        //Send cards
                        ServerSendAllClient(WrapAllCard());
                        //AllCardCon.allCardCon.StartDistribute();
                    }
                }
                else if (r[i] == GiveCard_)
                {
                    if (Data.IamHost)
                        ServerSendAllClient(recv);
                    AllCardCon.allCardCon.Give(IdToPos(int.Parse(r[2]), Data.myId, Data.PlayerNumber), int.Parse(r[1]));
                }
                else if (r[i] == PutOnTable_)
                {
                    if (Data.IamHost)
                        ServerSendAllClient(recv);
                    AllCardCon.allCardCon.PutOnTable(new Vector3(float.Parse(r[2]), 0f, float.Parse(r[3])), int.Parse(r[1]));
                }
            }
        }
    }
    public void GetInfo(string[] r) {
        AllCardCon.allCardCon.mId = int.Parse(r[1]);
        Data.PlayerNumber = int.Parse(r[2]);
        Data.PlayerCardNumber = int.Parse(r[3]);
        Data.NeedAnimation = int.Parse(r[4]) == 1;
        GameObject.Find("ani_Text_Button").GetComponent<Text>().text = (Data.NeedAnimation) ? "發牌過程on" : "發牌過程off";
        Data.NeedDrawCard = int.Parse(r[5]) == 1;
        print(AllCardCon.allCardCon.mId.ToString() + Data.PlayerNumber.ToString() + Data.PlayerCardNumber.ToString() +  Data.NeedAnimation.ToString() + Data.NeedAnimation.ToString());
        ClientSendServer(ClientGetInfoReply_);
    }
    public string FindClientIP(string name) {
        int index = Data.players.IndexOf(name);
        if (index == -1) {
            return "None Find";
        } else {
            return Data.playerIP[index];
        }
    }
    public void ClearAllCard() // Clear each player's card 
    {
        Data.PlayerOneCard.Clear();
        Data.PlayerTwoCard.Clear();
        Data.PlayerThreeCard.Clear();
        Data.PlayerHostCard.Clear();
    }
    public void SetInitCard() {
        if (Data.IamHost)
        {
            System.Random rand = new System.Random();
            int iTarget = 0, iCardTemp = 0;
            for (int i = 0; i < 52; i++)
                Data.Cards[i] = i;
            for (int i = 0; i < 52; i++)
            {
                iTarget = UnityEngine.Random.Range(0, 52);
                iCardTemp = Data.Cards[i];
                Data.Cards[i] = Data.Cards[iTarget];
                Data.Cards[iTarget] = iCardTemp;
            }
        }
    }
    public void DiscardPlayerCard(string name, int index) //To do discard card
    {
        int i = Data.players.IndexOf(name);
        if (i == -1) {
            print("None Find");
        } else {
            if (i == 0) {
                Data.PlayerHostCard.RemoveAt(index);
            } else if (i == 1) {
                Data.PlayerOneCard.RemoveAt(index);
            } else if (i == 2) {
                Data.PlayerTwoCard.RemoveAt(index);
            } else if (i == 3) {
                Data.PlayerThreeCard.RemoveAt(index);
            }
        }
    }
    public void AddHandCard(string name, int cardindex) {
        int i = Data.players.IndexOf(name);
        print("index" + i.ToString());
        if (i == -1) {
            print("None Find");
        } else {
            if (i == 0) {
                Data.PlayerHostCard.Add(cardindex);
            } else if (i == 1) {
                Data.PlayerOneCard.Add(cardindex);
            } else if (i == 2) {
                Data.PlayerTwoCard.Add(cardindex);
            } else if (i == 3) {
                Data.PlayerThreeCard.Add(cardindex);
            }
        }
    }
    public string TakeCard(string Name, int index) //Packing Send Data : funtion is Take card From someone to me index is which card
    {
        return TakeCard_ + "," + Name + "," + index.ToString();
    }
    public string AddCardFromTable(string Name, int tablecard) //Packing Send Data : funtion is Take card from table to me
    {
        ++Data.NowCardIndex; //For test
        return AddCardFromTable_ + "," + Name + "," + tablecard.ToString();
    }
    public string DiscardCard(string Name, int index) {
        return DiscardCard_ + "," + Name + "," + index.ToString();
    }
    public void PrintAllHandCard() {
        for (int i = 0; i < Data.PlayerNumber; ++i) {
            if (i == 0) {
                print("Host Card");
                foreach (var card in Data.PlayerHostCard) {
                    print(card);
                }
            } else if (i == 1) {
                print("Player 1 Card");
                foreach (var card in Data.PlayerOneCard) {
                    print(card);
                }
            } else if (i == 2) {
                print("Player 2 Card");
                foreach (var card in Data.PlayerTwoCard) {
                    print(card);
                }
            } else if (i == 3) {
                print("Player 3 Card");
                foreach (var card in Data.PlayerThreeCard) {
                    print(card);
                }
            }
        }
    }
    void SocketQuit() {
        if (connectThread != null) {
            connectThread.Interrupt();
            connectThread.Abort();
        }
        if (socket != null)
            socket.Close();
    }
    // Start is called before the first frame update
    void Start() {
        if (Data.IamHost) {
            ClearAllCard();
            PrintAllHandCard();
            SetInitCard();
            PrintAllHandCard();
            InitServerSocket();

            ServerListenClient();
           
        } else {

            InitClientSocket();            
            ClientReceiveServer();
        }
        PrintAllHandCard();
        
    }

    public void send_click()  // TEST FUNCTION
    {

        // SsendStr = "fghfghgh";
        /*SsendStr = TakeCard_+"," + Data.players[0];
        ServerSendClient(SsendStr);
        SsendStr = AddCardFromTable_ +","+ Data.players[0];
        ServerSendClient(SsendStr);
        SsendStr = DiscardCard_ + "," + Data.players[0];
        ServerSendClient(SsendStr);

        SsendStr = GetNowHandCard_ + "," + Data.players[0];
        ServerSendClient(SsendStr);
        
        Data.MyName = "sss";
        Data.players.Add("sss");
        SetInitCard();
        PrintAllHandCard();
        SsendStr = WrapHandCardToString(Data.MyName);
        UnwrapAndSetHandCard(SsendStr);
        PrintAllHandCard();
        print("-----------ADD-------------------");
        AddHandCard(Data.MyName, Data.Cards[Data.NowCardIndex]);
        PrintAllHandCard();
        print("--------------Discard----------------");
        DiscardPlayerCard(Data.MyName, 0);
        PrintAllHandCard();*/
        /*
        for (int i = 0; i< Data.playerIP.Count; ++i)
        {
            print("Player ip = " + Data.playerIP[i]);
        }
        ServerSendClient(Data.playerIP[1], "SSS");
        print("1");
        ServerSendAllClient("AAAAA");
        print("2");
        ServerSendClient(Data.playerIP[1], WrapAllCard());
        print("3");
        */
        if (Data.IamHost) {
            //Send info
            for (int i = 1; i < Data.PlayerNumber; i++)
            {
                string mesg = Info_ + "," + i.ToString() + "," + Data.PlayerNumber.ToString() + "," + Data.PlayerCardNumber.ToString() + "," +
                   ((Data.NeedAnimation) ? "1" : "0") + "," + ((Data.NeedDrawCard) ? "1" : "0");
                ServerSendClient(Data.playerIP[i], mesg);
            }

            //Send cards
            //ServerSendAllClient(WrapAllCard());
            //AllCardCon.allCardCon.StartDistribute();

        } else {
            ClientSendServer("SG_DATA-1");
        }
    }
    // Update is called once per frame
    void Update() {
        /*
        if (!haveGive && Data.IamHost) {
            haveGive = true;
            ServerSendAllClient(WrapAllCard());
        }*/
    }
}
