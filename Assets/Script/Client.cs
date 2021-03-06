﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.IO;
using System;

public class Client : MonoBehaviour
{

    public String clientName;
    public bool isHost;

    private bool socketReady;
    private TcpClient socket;
    private NetworkStream stream;
    private StreamWriter writer;
    private StreamReader reader;
    public static int plus=-1;
    private List<GameClient> players = new List<GameClient>();
    private int col;
    public string returntype;
    public string returnEnd="";
    private int cout=3;
    public static bool turn = false;
    private void Start()
    {
        plus++;
        DontDestroyOnLoad(gameObject);
       
    }
    public bool ConnectToServer(string host, int port)
    {
        if (socketReady)
            return false;

        try
        {
            socket = new TcpClient(host, port);
            stream = socket.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);
            socketReady = true;
            Debug.Log("클라이언트 연결");
        }
        catch (Exception e)
        {
            Debug.Log("Socket error : " + e.Message);
        }

        return socketReady;
    }

    private void Update()
    {
        if(turn)
        {
            foreach (GameClient c in players)
            {
                players.Remove(c);
            }
            turn = false;

            Destroy(this.gameObject);
        }
        if (socketReady)
        {
            if (stream.DataAvailable)
            {
                string data = reader.ReadLine();
                if (data != null)
                    returntype = OnInconinData(data);
            }
        }
    }
    public string Access()
    {
        if (socketReady)
        {
            if (stream.DataAvailable)
            {
                string data = reader.ReadLine();
                if (data != null)
                    return OnInconinData(data);
            }
        }
        return "";
    }

    public void Send(string data)
    {
        if (!socketReady)
            return;
        writer.WriteLine(data);
        writer.Flush();
    }

    //서버에 메시지 보네기
    public string OnInconinData(string data)
    {
        Debug.Log("Client : " + data);
        string[] aData = data.Split('|');

        switch (aData[0])
        {
            case "SWHO":
                for (int i = 1; i < aData.Length; i++)
                {
                    UserConnected(aData[i], false);
                }
                Send("CWHO|" + clientName + "|" + ((isHost) ? 1 : 0).ToString());
                break;
            case "SCNN":
                UserConnected(aData[1], false);
                break;
            case "SEND":
                GameManager.gameManager.OverEnd((int.Parse(aData[1])));
                break;
            default:
                return data;
        }
        return "";
        /*string[] aData = data.Split('|');
        int Srow = int.Parse(aData[0]);
        Scol = int.Parse(aData[1]);
        return Srow;*/
    }

    private void UserConnected(string name, bool host)
    {
        GameClient c = new GameClient();
        c.name = name;
        
        players.Add(c);
        Debug.Log(players.Count);
        
        if (players.Count >= cout)
        {
            GameManager2.Instance.StartGame();
        }
    }
     
    private void OnApplicationQuit()
    {
        CloseSocket();
    }
    private void OnDisable()
    {
        CloseSocket();
    }
    private void CloseSocket()
    {
        if (!socketReady)
            return;
        writer.Close();
        reader.Close();
        socket.Close();
        socketReady = false;

    }

}

public class GameClient
{
    public string name;
    public bool isHost;

}
