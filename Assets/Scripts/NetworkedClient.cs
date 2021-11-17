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

        foreach (GameObject go in allObjects)
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
            if (canPlay)
            {
                gameManager.GetComponent<GameManager>().FindBoard();
                gameManager.GetComponent<GameManager>().getBoard().GetComponent<Board>().EnterPlayerTurn();
            }
        }
        else if (signifier == ServerToClientSignifiers.DrawMark)
        {
            Debug.Log("Enemy draw mark at box " + csv[1]);
            gameManager.GetComponent<GameManager>().EnemyDrawMark(int.Parse(csv[1]));
        }
        else if (signifier == ServerToClientSignifiers.JoiningRoom)
        {
            int i = int.Parse(csv[1]);
            if (i == 0)
            {
                Debug.Log("Join a room to observe");
                SceneManager.LoadScene("GameScene");
                gameManager.GetComponent<GameManager>().FindBoard();
            }
            else if (i == 1)
            {
                int location = int.Parse(csv[2]);
                int temp = int.Parse(csv[3]);
                BoxState state = BoxState.Empty;
                if (temp == 0)
                    state = BoxState.O;
                else if (temp == 1)
                    state = BoxState.X;
                //gameManager.GetComponent<GameManager>().getBoard().GetComponent<Board>().SetBoxMarked(location, state);
                gameManager.GetComponent<GameManager>().AddStep(location, state);
            }
            else if (i == 2)
            {
                Debug.Log("get steps finish");
                gameManager.GetComponent<GameManager>().getBoard().GetComponent<Board>().LoadStep();
            }
        }
        else if (signifier == ServerToClientSignifiers.NoRoomCanJoin)
        {
            Debug.Log("No Room can be joined");
        }
        else if (signifier == ServerToClientSignifiers.DrawMarkOnObserver)
        {
            int location = int.Parse(csv[1]);
            int temp = int.Parse(csv[2]);
            BoxState state = BoxState.Empty;
            if (temp == 0)
                state = BoxState.O;
            else if (temp == 1)
                state = BoxState.X;
            gameManager.GetComponent<GameManager>().getBoard().GetComponent<Board>().SetBoxMarked(location, state);
            Debug.Log("Observer mark " + location + " " + state);
        }
        else if (signifier == ServerToClientSignifiers.GameOver)
        {
            int i = int.Parse(csv[1]);
            if (i == 0)
            {
                Debug.Log("Game over, start receive replay data");
                gameManager.GetComponent<GameManager>().ClearStepList();
            }
            else if (i == 1)
            {
                int location = int.Parse(csv[2]);
                int temp = int.Parse(csv[3]);
                BoxState state = BoxState.Empty;
                if (temp == 0)
                    state = BoxState.O;
                else if (temp == 1)
                    state = BoxState.X;
                gameManager.GetComponent<GameManager>().AddStep(location, state);
            }
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
        public const int GameOver = 6;
        public const int AskForGSList = 7;
        public const int AskJoinGS = 8;
        public const int JoinRandomRoom = 9;
    }

    public static class ServerToClientSignifiers
    {
        public const int LoginResponse = 1;
        public const int GameSessionStarted = 2;
        public const int OpponentTicTacToePlay = 3;
        public const int DrawMark = 4;
        public const int GSList = 5;
        public const int JoiningRoom = 6;
        public const int NoRoomCanJoin = 7;
        public const int DrawMarkOnObserver = 8;
        public const int GameOver = 9;
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
        public const int ReadGSList = 7;
    }
}
