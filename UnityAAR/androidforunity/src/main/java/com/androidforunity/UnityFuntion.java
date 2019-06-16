package com.androidforunity;

import android.Manifest;
import android.app.Activity;
import android.content.Context;
import android.content.pm.PackageManager;
import android.net.wifi.WifiInfo;
import android.net.wifi.WifiManager;
import android.support.v4.app.ActivityCompat;
import android.support.v4.content.ContextCompat;
import android.text.format.Formatter;
import android.util.Log;
import android.widget.Toast;

import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;
import java.net.InetAddress;
import java.net.NetworkInterface;
import java.net.SocketException;
import java.util.Enumeration;

public class UnityFuntion {
    private Activity _unityActivity;

    Activity getActivity() {
        if (null == _unityActivity) {
            try {
                Class<?> classtype = Class.forName("com.unity3d.player.UnityPlayer");
                Activity activity = (Activity) classtype.getDeclaredField("currentActivity").get(classtype);
                _unityActivity = activity;
            } catch (ClassNotFoundException e) {

            } catch (IllegalAccessException e) {

            } catch (NoSuchFieldException e) {

            }
        }
        return _unityActivity;
    }

    boolean callUnity(String gameObjectName, String functionName, String args) {
        try {
            Class<?> classtype = Class.forName("com.unity3d.player.UnityPlayer");
            Method method = classtype.getMethod("UnitySendMessage", String.class, String.class, String.class);
            method.invoke(classtype, gameObjectName, functionName, args);
            return true;
        } catch (ClassNotFoundException e) {

        } catch (NoSuchMethodException e) {

        } catch (IllegalAccessException e) {

        } catch (InvocationTargetException e) {

        }
        return false;
    }

    public boolean showToast(String content) {
        Toast.makeText(getActivity(), content, Toast.LENGTH_SHORT).show();
        //这里是主动调用Unity中的方法，该方法之后unity部分会讲到
        callUnity("Canvas", "FromAndroid", "hello unity i'm android");
        return true;
    }

    public boolean CheckWifiAP() {

        WifiManager wifi = (WifiManager) getActivity().getApplicationContext().getSystemService(Context.WIFI_SERVICE);
        try {
            Method method = wifi.getClass().getMethod("isWifiApEnabled");
            method.setAccessible(true);
            return (Boolean) method.invoke(wifi);

        } catch (NoSuchMethodException e) {
            // TODO Auto-generated catch block
            e.printStackTrace();
        } catch (Exception e) {
            e.printStackTrace();
        }
        return  false;
    }
    public boolean RequestPermission(){

        int permissionCheck = ContextCompat.checkSelfPermission(getActivity(), Manifest.permission.ACCESS_WIFI_STATE);
        Toast.makeText(getActivity(), "已經拿到權限囉!", Toast.LENGTH_SHORT).show();
        //沒有權限時
        if (permissionCheck != PackageManager.PERMISSION_GRANTED) {
            Toast.makeText(getActivity(), "NO 權限!", Toast.LENGTH_SHORT).show();
            ActivityCompat.requestPermissions(getActivity(),
                    new String[]{Manifest.permission.ACCESS_WIFI_STATE},
                    1);
        } else {
            Toast.makeText(getActivity(), "已經拿到權限囉!", Toast.LENGTH_SHORT).show();
        }
        return true;
    }
    public String GetIP(){
        String ip = "";
        try {
            Enumeration<NetworkInterface> enumNetworkInterfaces = NetworkInterface
                    .getNetworkInterfaces();
            while (enumNetworkInterfaces.hasMoreElements()) {
                NetworkInterface networkInterface = enumNetworkInterfaces
                        .nextElement();
                Enumeration<InetAddress> enumInetAddress = networkInterface
                        .getInetAddresses();
                while (enumInetAddress.hasMoreElements()) {
                    InetAddress inetAddress = enumInetAddress.nextElement();

                    if (inetAddress.isSiteLocalAddress()) {
                        ip = inetAddress.getHostAddress() ;
                    }
                }
            }

        } catch (SocketException e) {
            // TODO Auto-generated catch block
            e.printStackTrace();
            ip += "Something Wrong! " + e.toString() + "\n";
        }
        return ip;
    }
    public String startPingService(String subnet)
    {
        String IPs= "";
        try {

            WifiManager mWifiManager = (WifiManager) getActivity().getApplicationContext().getSystemService(Context.WIFI_SERVICE);
            WifiInfo mWifiInfo = mWifiManager.getConnectionInfo();

            Thread t;
            for (int i=1;i<255;i++){
                String host = subnet + "." + i;
                if (InetAddress.getByName(host).isReachable(100)){

                    Log.w("DeviceDiscovery", "Reachable Host: " + String.valueOf(host) +" is reachable!");
                    IPs += host +',';
                }
                else
                {
                    Log.e("DeviceDiscovery", "Not Reachable Host: " + String.valueOf(host));

                }
            }


        }
        catch(Exception e){
            //System.out.println(e);
        }
        return  IPs;
    }
}
