using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class GameServerControll : MonoBehaviour
{
    Socket socket; //socket
    EndPoint clientEnd; //client
    IPEndPoint ipEnd; //port
    string recvStr; //receive string
    byte[] recvData = new byte[1024]; //byte recieve
    byte[] sendData = new byte[1024];
    int recvLen;
    Thread connectThread;
    int UdpPort = 10231;
    EndPoint serverEnd;
    const string TakeCard_ = "T";
    const string AddCardFromTable_ = "A";
    const string DiscardCard_ = "D";
    const string GetNowHandCard_ = "G";
    const string Spade = "S";
    const string Heart = "H";
    const string Diamond = "DI";
    const string Club = "C";
    //data.playerIP[0] is host
    //data.players[0] is host
    //card = 0 -- > 梅花 A card = 1 --> 方塊 A card == 2 --> 愛心 A card == 3 --> 黑桃 A 
    // S 黑桃 H 愛心 DI 方塊 C 梅花
    void InitServerSocket()
    {
        ipEnd = new IPEndPoint(IPAddress.Any, UdpPort);
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        socket.Bind(ipEnd);
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        clientEnd = (EndPoint)sender;
        print("Server waiting for UDP dgram");
        connectThread = new Thread(new ThreadStart(ServerListenClient));
        connectThread.Start();
    }
    public void ServerSendClient(string sendStr) //default send player 1 use SetClient To Change 
    {
        sendData = new byte[1024];
        sendData = Encoding.ASCII.GetBytes(sendStr);
        socket.SendTo(sendData, sendData.Length, SocketFlags.None, clientEnd);
    }
    public void ServerSendAllClient(string sendStr) //Send All Client but not test 
    {
        sendData = new byte[1024];
        sendData = Encoding.ASCII.GetBytes(sendStr);
        EndPoint clientEnds = new IPEndPoint(IPAddress.Any, 0);
        socket.SendTo(sendData, sendData.Length, SocketFlags.None, clientEnds);
    }
    public int CardNameToIndex(string card) //將 card 從 string 表示轉乘 int
    {
        string[] sp = card.Split(',');
        int num = int.Parse(sp[1]);
        if (sp[0] == Spade)
        {
            num = (num - 1) * 4 + 3;
        }
        else if (sp[0] == Diamond)
        {
            num = (num - 1) * 4 + 1;
        }
        else if (sp[0] == Heart)
        {
            num = (num - 1) * 4 + 2;
        }
        else if (sp[0] == Club)
        {
            num = (num - 1) * 4 ;
        }
        return num;
    }
    public string CardIndexToName(int cardindex) //將 card 從 int 表示轉成 string
    {
        string r = "";
        if (cardindex % 4 == 0)
        {
            r = Club + "," + ((cardindex/4) + 1);
        }
        else if (cardindex % 4 == 1)
        {
            r = Diamond + "," + ((cardindex / 4) + 1);
        }
        else if (cardindex % 4 == 2)
        {
            r = Heart + "," + ((cardindex / 4) + 1);
        }
        else if (cardindex % 4 == 3)
        {
            r = Spade + "," + ((cardindex / 4) + 1);
        }
        return r;
    }
    public void SetClientSend(string IP) // change to send other player
    {
        clientEnd = new IPEndPoint(IPAddress.Parse(IP), 0);
        
    }
    void ServerListenClient()
    {
        while (true)
        {   
            recvData = new byte[1024];
            recvLen = socket.ReceiveFrom(recvData, ref clientEnd);
            print("message from: " + clientEnd.ToString());
            recvStr = Encoding.ASCII.GetString(recvData, 0, recvLen);
            print(recvStr);
        }
    }
    public void UnwrapAndSetHandCard(string r)
    {
        string[] sp = r.Split(',');
        int index = Data.players.IndexOf(sp[0]);
        if (index == -1)
        {
            print( "None Find");
        }
        else
        {
            if (index == 0)
            {
                Data.PlayerHostCard.Clear();
                for (int i = 0; i < sp.Length; ++i)
                {
                    Data.PlayerHostCard.Add(int.Parse(sp[i]));
                }
            }
            else if (index == 1)
            {
                Data.PlayerOneCard.Clear();
                for (int i = 0; i < sp.Length; ++i)
                {
                    Data.PlayerOneCard.Add(int.Parse(sp[i]));
                }
            }
            else if (index == 2)
            {
                Data.PlayerTwoCard.Clear();
                for (int i = 0; i < sp.Length; ++i)
                {
                    Data.PlayerTwoCard.Add(int.Parse(sp[i]));
                }
            }
            else if (index == 3)
            {
                Data.PlayerThreeCard.Clear();
                for (int i = 0; i < sp.Length; ++i)
                {
                    Data.PlayerThreeCard.Add(int.Parse(sp[i]));
                }
            }
            
        }
    }
    public string WrapHandCardToString(string Name) // Wrap Hand Card To String 以利server 發送
    {
        int index = Data.players.IndexOf(name);
        if (index == -1)
        {
            return "None Find";
        }
        else
        {
            string r = Name + ",";
            if (index == 0)
            { 
               for(int i = 0; i< Data.PlayerHostCard.Count;++i)
               {
                  if ( i == Data.PlayerHostCard.Count -1)
                  {
                        r = r + i.ToString();
                  } 
                  else
                    {
                        r = r + i.ToString() + ",";
                    }
               }
            }
            else if (index == 1)
            {
                for (int i = 0; i < Data.PlayerOneCard.Count; ++i)
                {
                    if (i == Data.PlayerOneCard.Count - 1)
                    {
                        r = r + i.ToString();
                    }
                    else
                    {
                        r = r + i.ToString() + ",";
                    }
                }
            }
            else if (index == 2)
            {
                for (int i = 0; i < Data.PlayerTwoCard.Count; ++i)
                {
                    if (i == Data.PlayerTwoCard.Count - 1)
                    {
                        r = r + i.ToString();
                    }
                    else
                    {
                        r = r + i.ToString() + ",";
                    }
                }
            }
            else if (index == 3)
            {
                for (int i = 0; i < Data.PlayerThreeCard.Count; ++i)
                {
                    if (i == Data.PlayerThreeCard.Count - 1)
                    {
                        r = r + i.ToString();
                    }
                    else
                    {
                        r = r + i.ToString() + ",";
                    }
                }
            }
            return r;
        }
    }
    public void ResolveMSG(string recv)
    {
        string[] r = recv.Split(',');
        for (int i = 0; i < r.Length; ++i)
        {
            if (i == 0 )
            {
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

            }
        }
    }
    public string FindClientIP(string name)
    {
        int index =  Data.players.IndexOf(name);
        if (index == -1)
        {
            return "None Find";
        }
        else
        {
            return Data.playerIP[index];
        }
    }
    void InitClientSocket()
    {
        ipEnd = new IPEndPoint(IPAddress.Parse(Data.HostIP), UdpPort);
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        socket.ReceiveTimeout = 1000;
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        serverEnd = (EndPoint)sender;
        print("Client waiting for sending UDP dgram");
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
            recvData = new byte[1024];
            recvLen = socket.ReceiveFrom(recvData, ref serverEnd);
            print("message from: " + serverEnd.ToString());
            recvStr = Encoding.ASCII.GetString(recvData, 0, recvLen);
            print(recvStr);
            ResolveMSG(recvStr);
        }
        
    }
    public void ClearAllCard() // Clear each player's card 
    {
        Data.PlayerOneCard.Clear();
        Data.PlayerTwoCard.Clear();
        Data.PlayerThreeCard.Clear();
        Data.PlayerHostCard.Clear();
    }
    public void SetInitCard()
    {
        Random rand = new Random();
        for (int i = 0; i < 52; ++i)
            Data.Cards[i] = i;
        for (int i = 0; i < 52; i++)
        {
            int iTarget = UnityEngine.Random.Range(0, 52);
            int iCardTemp = Data.Cards[i];
            Data.Cards[i] = Data.Cards[iTarget];
            Data.Cards[iTarget] = iCardTemp;
        }
        int cardnum = Data.PlayerCardNumber * Data.PlayerNumber;
        print("Card num = " + cardnum.ToString());
        for (int i = 0; i < cardnum; ++i)
        {
            print(i % (Data.PlayerNumber ));
            if ( i % (Data.PlayerNumber) == 0)
            {
                Data.PlayerHostCard.Add(Data.Cards[i]);
            }
            else if (i % (Data.PlayerNumber) == 1)
            {
                Data.PlayerOneCard.Add(Data.Cards[i]);
            }
            else if (i % (Data.PlayerNumber) == 2)
            {
                Data.PlayerTwoCard.Add(Data.Cards[i]);
            }
            else if (i % (Data.PlayerNumber) == 3)
            {
                Data.PlayerThreeCard.Add(Data.Cards[i]);
            }
        }
        Data.NowCardIndex = cardnum;
    
    }
    public void DiscardPlayerCard(string Name,int index) //To do discard card
    {
        int i = Data.players.IndexOf(name);
        if (i == -1)
        {
            print( "None Find");
        }
        else
        {
            if (i == 0)
            {
                Data.PlayerHostCard.RemoveAt(index);
            }
            else if (i  == 1)
            {
                Data.PlayerOneCard.RemoveAt(index);
            }
            else if (i  == 2)
            {
                Data.PlayerTwoCard.RemoveAt(index);
            }
            else if (i  == 3)
            {
                Data.PlayerThreeCard.RemoveAt(index);
            }
        }
    }
    public void AddHandCard(string Name,int cardindex)
    {
        int i = Data.players.IndexOf(name);
        if (i == -1)
        {
            print("None Find");
        }
        else
        {
            if (i == 0)
            {
                Data.PlayerHostCard.Add(cardindex);
            }
            else if (i == 1)
            {
                Data.PlayerOneCard.Add(cardindex);
            }
            else if (i == 2)
            {
                Data.PlayerTwoCard.RemoveAt(cardindex);
            }
            else if (i == 3)
            {
                Data.PlayerThreeCard.RemoveAt(cardindex);
            }
        }
    }
    public string TakeCard(string Name,int index) //Packing Send Data : funtion is Take card From someone to me index is which card
    {
        return TakeCard_ + "," + Name + "," + index.ToString();
    }
    public string AddCardFromTable(string Name, int tablecard) //Packing Send Data : funtion is Take card from table to me
    {
        ++Data.NowCardIndex; //For test
        return AddCardFromTable_ + "," + Name + "," + tablecard.ToString();
    }
    public string DiscardCard(string Name,int index)
    {
        return DiscardCard_ + "," + Name + "," + index.ToString();
    }
    public void PrintAllHandCard()
    {
        for (int i= 0; i < Data.PlayerNumber; ++i)
        {
            if ( i == 0)
            {
                print("Host Card");
                foreach ( var card in Data.PlayerHostCard)
                {
                    print(card);
                }
            }
            else if (i == 1)
            {
                print("Player 1 Card");
                foreach (var card in Data.PlayerOneCard)
                {
                    print(card);
                }
            }
            else if (i == 2)
            {
                print("Player 2 Card");
                foreach (var card in Data.PlayerTwoCard)
                {
                    print(card);
                }
            }
            else if (i == 3)
            {
                print("Player 3 Card");
                foreach (var card in Data.PlayerThreeCard)
                {
                    print(card);
                }
            }
        }
    }
    void SocketQuit()
    {
        if (connectThread != null)
        {
            connectThread.Interrupt();
            connectThread.Abort();
        }
        if (socket != null)
            socket.Close();
    }
    // Start is called before the first frame update
    void Start()
    {
        if (Data.IamHost)
        {
            ClearAllCard();
            //PrintAllHandCard();
            SetInitCard();
            PrintAllHandCard();
            InitServerSocket();
            ServerSendClient("haha");
            //string hand = WrapHandCardToString(Data.players[0]);
            //hand = GetNowHandCard_ + "," + hand;
            //SetClientSend(Data.playerIP[1]);
            //ServerSendClient(hand);
        }
        else
        {
            InitClientSocket();
            ClientSendServer("AAAA");
        }
        PrintAllHandCard();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
