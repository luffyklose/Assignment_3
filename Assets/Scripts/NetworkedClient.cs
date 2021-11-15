using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class NetworkedClient : MonoBehaviour
{
    int connectionID;
    int maxConnections = 1000;
    int reliableChannelID;
    int unreliableChannelID;
    int hostID;
    int socketPort = 5491;
    byte error;
    bool isConnected = false;
    int ourClientID;
    private bool canPlay;

    GameObject gameManager;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (var go in allObjects)
        {
            if (go.name == "GameManager")
                gameManager = go;
        }
        
        Connect();
    }
    
    void Update()
    {
        // if(Input.GetKeyDown(KeyCode.S))
        //     SendMessageToHost("Hello from client");
        
        UpdateNetworkConnection();
    }

    private void UpdateNetworkConnection()
    {
        if (isConnected)
        {
            int recHostID;
            int recConnectionID;
            int recChannelID;
            byte[] recBuffer = new byte[1024];
            int bufferSize = 1024;
            int dataSize;
            NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recHostID, out recConnectionID, out recChannelID, recBuffer, bufferSize, out dataSize, out error);

            switch (recNetworkEvent)
            {
                case NetworkEventType.ConnectEvent:
                    Debug.Log("connected.  " + recConnectionID);
                    ourClientID = recConnectionID;
                    break;
                case NetworkEventType.DataEvent:
                    string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                    ProcessReceivedMsg(msg, recConnectionID);
                    //Debug.Log("got msg = " + msg);
                    break;
                case NetworkEventType.DisconnectEvent:
                    isConnected = false;
                    Debug.Log("disconnected.  " + recConnectionID);
                    break;
            }
        }
    }
    
    private void Connect()
    {

        if (!isConnected)
        {
            Debug.Log("Attempting to create connection");

            NetworkTransport.Init();

            ConnectionConfig config = new ConnectionConfig();
            reliableChannelID = config.AddChannel(QosType.Reliable);
            unreliableChannelID = config.AddChannel(QosType.Unreliable);
            HostTopology topology = new HostTopology(config, maxConnections);
            hostID = NetworkTransport.AddHost(topology, 0);
            Debug.Log("Socket open.  Host ID = " + hostID);

            connectionID = NetworkTransport.Connect(hostID, "192.168.2.24", socketPort, 0, out error); // server is local on network

            if (error == 0)
            {
                isConnected = true;

                Debug.Log("Connected, id = " + connectionID);

            }
        }
    }
    
    public void Disconnect()
    {
        NetworkTransport.Disconnect(hostID, connectionID, out error);
    }
    
    public void SendMessageToHost(string msg)
    {
        byte[] buffer = Encoding.Unicode.GetBytes(msg);
        NetworkTransport.Send(hostID, connectionID, reliableChannelID, buffer, msg.Length * sizeof(char), out error);
    }

    private void ProcessReceivedMsg(string msg, int id)
    {
        Debug.Log("msg received = " + msg + ".  connection id = " + id);

        string[] csv = msg.Split(',');
        int signifier = int.Parse(csv[0]);

        if (signifier == ServerToClientSignifiers.LoginResponse)
        {
            int loginResultSignifier = int.Parse(csv[1]);
            
            if (loginResultSignifier == LoginResponses.Success)
                gameManager.GetComponent<GameManager>().ChangeGameStates(GameManager.GameStates.MainMenu);
        }
        else if (signifier == ServerToClientSignifiers.GameSessionStarted)
        {
            Debug.Log("Find Room");
            gameManager.GetComponent<GameManager>().ChangeGameStates(GameManager.GameStates.PlayingTicTacToe);
        }
        else if (signifier == ServerToClientSignifiers.OpponentTicTacToePlay)
        {
            Debug.Log("Our next action no longer beckons");
            int temp = int.Parse(csv[1]);
            Debug.Log(temp);
            if (temp == 1)
            {
                canPlay = true;
            }
            else if (temp == 0)
            {
                canPlay = false;
            }
            else
            {
                Debug.Log("CanPlay get error");
            }
            SceneManager.LoadScene("GameScene");
            gameManager.GetComponent<GameManager>().FindBoard();
        }
        else if (signifier == ServerToClientSignifiers.DrawMark)
        {
            Debug.Log("Enemy draw mark at box " + csv[1]);
            gameManager.GetComponent<GameManager>().EnemyDrawMark(int.Parse(csv[1]));
        }
    }

    public bool getCanPlay()
    {
        return canPlay;
    }

    public bool IsConnected()
    {
        return isConnected;
    }
    
    public static class ClientToServerSignifiers
    {
        public const int Login = 1;
        public const int CreateAccount = 2;
        public const int AddToGameSessionQueue = 3;
        public const int TicTacToePlay = 4;
        public const int DrawMark = 5;
    }

    public static class ServerToClientSignifiers
    {
        public const int LoginResponse = 1;
        public const int GameSessionStarted = 2;
        public const int OpponentTicTacToePlay = 3;
        public const int DrawMark = 4;
    }
 
    public static class LoginResponses
    {
        public const int Success = 1;
        public const int FailureNameInUse = 2;
        public const int FailureNameNotFound = 3;
        public const int FailureIncorrectPassword = 4; 
    }

    public static class GameStates
    {
        public const int login = 1;
        public const int MainMenu = 2;
        public const int WaitingForMatch = 3;
        public const int PlayingTicTacToe = 4;
        public const int MyTurn = 5;
        public const int EnemyTurn = 6;
    }
}
