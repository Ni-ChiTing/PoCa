using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Data
{
    // Scene start
    public static string MyName;
    public static bool IamHost = false;
    public static string HostIP = "";

    // Scene setting
    public static string HostName = "ss";
    public static List<string> players = new List<string>();
    public static string GameName;
    public static int PlayerNumber = 3;
    public static int PlayerCardNumber = 5;
    public static int TableCardNumber = 32;
    public static bool NeedAnimation;
    public static bool NeedDrawCard = false;
    
    
}
