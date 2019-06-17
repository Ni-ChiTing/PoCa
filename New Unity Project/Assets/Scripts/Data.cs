using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public static class Data
{
    // Scene start
    public static string MyName;
    public static bool IamHost = false;
    public static string HostIP = "";
    public static List<string> playerIP = new List<string>();
    public static List<int> PlayerOneCard = new List<int>();
    public static List<int> PlayerTwoCard = new List<int>();
    public static List<int> PlayerThreeCard = new List<int>();
    public static List<int> PlayerHostCard = new List<int>();
    public static int [] Cards = new int[52];
    public static int NowCardIndex = 0;
    // Scene setting
    public static string HostName = "ss";
    public static List<string> players = new List<string>();
    public static string GameName;
    public static int PlayerNumber = 4;
    public static int PlayerCardNumber = 5;
    public static int TableCardNumber = 32;
    public static bool NeedAnimation = true;
    public static bool NeedDrawCard = true;       
}
