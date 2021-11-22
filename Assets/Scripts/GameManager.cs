using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    GameObject inputFieldUserName, inputFieldPassword, buttonSubmit, toggleLogin, toggleCreate;
    GameObject networkedClient;
    GameObject findGameSessionButton, placeHolderGameButton,observerButton;
    GameObject infoText1, infoText2;
    GameObject board;
    private GameObject myChat,
        enemyChat,
        myChatText,
        enemyChatText,
        chatInput,
        sendButton,
        hiButton,
        sorryButton,
        ggButton,
        thanksButton;
    private List<Step> stepList;

    private float playerChatCounter;
    private float enemyChatCounter;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        stepList = new List<Step>();
    }

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "StartScene")
        {
            GameObject[] allObjs = FindObjectsOfType<GameObject>();
            
                    foreach (GameObject go in allObjs)
                    {
                        if (go.name == "inputFieldUserName")
                            inputFieldUserName = go;
                        else if (go.name == "inputFieldPassword")
                            inputFieldPassword = go;
                        else if (go.name == "buttonSubmit")
                            buttonSubmit = go;
                        else if (go.name == "toggleCreate")
                            toggleCreate = go;
                        else if (go.name == "toggleLogin")
                            toggleLogin = go;
                        else if (go.name == "networkedClient")
                            networkedClient = go;
                        else if (go.name == "FindGameSessionButton")
                            findGameSessionButton = go;
                        else if (go.name == "PlaceHolderGameButton")
                            placeHolderGameButton = go;
                        else if (go.name == "InfoText1")
                            infoText1 = go;
                        else if (go.name == "InfoText2")
                            infoText2 = go;
                        else if (go.name == "ObserverButton")
                            observerButton = go;
                    }
                    
                    buttonSubmit.GetComponent<Button>().onClick.AddListener(SubmitButtonPressed); 
                    toggleCreate.GetComponent<Toggle>().onValueChanged.AddListener(ToggleCreateValueChanged);
                    toggleLogin.GetComponent<Toggle>().onValueChanged.AddListener(ToggleLoginValueChanged);
                    
                    findGameSessionButton.GetComponent<Button>().onClick.AddListener(FindGameSessionButtonPressed); 
                    placeHolderGameButton.GetComponent<Button>().onClick.AddListener(PlaceHolderGameButtonPressed);
                    observerButton.GetComponent<Button>().onClick.AddListener(ObserverButtonPressed);
                    
                    ChangeGameStates(GameStates.login);
        }
        else if (SceneManager.GetActiveScene().name == "GameScene")
        {
            GameObject[] allObjs = FindObjectsOfType<GameObject>();
            
            foreach (GameObject go in allObjs)
            {
               if (go.name == "networkedClient")
                    networkedClient = go;
               else if (go.name == "Board")
                   board = go;
            }
        }
    }
    
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.A))
        // {
        //     ChangeGameStates(GameStates.login);
        // }
        //
        // if (Input.GetKeyDown(KeyCode.S))
        // {
        //     ChangeGameStates(GameStates.MainMenu);
        // }
        //
        // if (Input.GetKeyDown(KeyCode.D))
        // {
        //     ChangeGameStates(GameStates.WaitingForMatch);
        // }
        //
        // if (Input.GetKeyDown(KeyCode.F))
        // {
        //     ChangeGameStates(GameStates.PlayingTicTacToe);
        // }
        if (SceneManager.GetActiveScene().name == "GameScene" && board == null)
        {
            FindGameSceneObject();
        }

        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            if (myChat.activeSelf)
            {
                playerChatCounter += Time.deltaTime;
                if (playerChatCounter >= 5.0)
                {
                    myChat.SetActive(false);
                    playerChatCounter = 0.0f;
                }
            }
    
            if (enemyChat.activeSelf)
            {
                enemyChatCounter += Time.deltaTime;
                if (enemyChatCounter >= 5.0)
                {
                    enemyChat.SetActive(false);
                    enemyChatCounter = 0.0f;
                }
            }
        }
    }

    public void FindGameSceneObject()
    {
        Debug.Log("Start finding board");

        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        
        foreach (GameObject go in allObjects)
        {
            //Debug.Log("1 time");
            if (go.name == "Board")
            {
                board = go;
                //Debug.Log("Find Board");
            }
            else if (go.name == "MyChat")
            {
                myChat = go;
            }
            else if (go.name == "EnemyChat")
            {
                enemyChat = go;
            }
            else if (go.name == "ChatInput")
            {
                chatInput = go;
            }
            else if (go.name == "SendButton")
            {
                sendButton = go;
            }
            else if (go.name == "MyChatText")
            {
                myChatText = go;
            }
            else if (go.name == "EnemyChatText")
            {
                enemyChatText = go;
            }
            else if (go.name == "HiButton")
            {
                hiButton = go;
            }
            else if (go.name == "SorryButton")
            {
                sorryButton = go;
            }
            else if (go.name == "GGButton")
            {
                ggButton = go;
            }
            else if (go.name == "ThanksButton")
            {
                thanksButton = go;
            }
        }
        
        //sendButton.GetComponent<Button>().onClick.AddListener(SendChatMessage);
        sendButton.GetComponent<Button>().onClick.AddListener(()=>
        {
            SendChatMessage();
        });
        hiButton.GetComponent<Button>().onClick.AddListener(()=>
        {
            SendChatMessage("Hi");
        });
        sorryButton.GetComponent<Button>().onClick.AddListener(()=>
        {
            SendChatMessage("Sorry");
        });
        ggButton.GetComponent<Button>().onClick.AddListener(()=>
        {
            SendChatMessage("GG");
        });
        thanksButton.GetComponent<Button>().onClick.AddListener(()=>
        {
            SendChatMessage("Thanks");
        });

        myChat.SetActive(false);
        enemyChat.SetActive(false);
    }
    
    private void SubmitButtonPressed()
    {
        string n = inputFieldUserName.GetComponent<InputField>().text;
        string p = inputFieldPassword.GetComponent<InputField>().text;

        if (toggleLogin.GetComponent<Toggle>().isOn)
            networkedClient.GetComponent<NetworkedClient>()
                .SendMessageToHost(ClientToServerSignifiers.Login + "," + n + "," + p);
        else
            networkedClient.GetComponent<NetworkedClient>()
                .SendMessageToHost(ClientToServerSignifiers.CreateAccount + "," + n + "," + p);
    }
    
    private void ToggleCreateValueChanged(bool newValue)
    {
        toggleLogin.GetComponent<Toggle>().SetIsOnWithoutNotify(!newValue);
    }
    
    private void ToggleLoginValueChanged(bool newValue)
    {
        toggleCreate.GetComponent<Toggle>().SetIsOnWithoutNotify(!newValue);
    }
    
    private void FindGameSessionButtonPressed()
    {
        Debug.Log("Find Room Sent");
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.AddToGameSessionQueue + "");
        ChangeGameStates(GameStates.WaitingForMatch);
    }
    
    private void PlaceHolderGameButtonPressed()
    {
        Debug.Log("Start Game Sent");
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.TicTacToePlay + "");
        stepList = new List<Step>();
    }

    private void ObserverButtonPressed()
    {
        Debug.Log("Random a room");
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.JoinRandomRoom+"");
        stepList = new List<Step>();
    }

    public void GameOver()
    {
        Debug.Log("Game Over");
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.GameOver + "");
    }

    public void MarkDrawed(int location,BoxState state)
    {
        Debug.Log("Mark at " + location);
        string l = location.ToString();
        string s = (-1).ToString();
        if (state == BoxState.X)
        {
            s = 1.ToString();
        }
        else if (state == BoxState.O)
        {
            s = 0.ToString();
        }

        networkedClient.GetComponent<NetworkedClient>()
            .SendMessageToHost(ClientToServerSignifiers.DrawMark + "," + l + "," + s);
    }

    public void EnemyDrawMark(int location)
    {
        board.GetComponent<Board>().DrawMark(location,false);
        if(!board.GetComponent<Board>().CheckGameOver())
            board.GetComponent<Board>().EnterPlayerTurn();
    }

    public void ChangeGameStates(int newState)
    {
        inputFieldUserName.SetActive(false);
        inputFieldPassword.SetActive(false);
        buttonSubmit.SetActive(false);
        toggleLogin.SetActive(false);
        toggleCreate.SetActive(false);
        findGameSessionButton.SetActive(false);
        placeHolderGameButton.SetActive(false);
        infoText1.SetActive(false);
        infoText2.SetActive(false);
        observerButton.SetActive(false);

        if (newState == GameStates.login)
        {
            inputFieldUserName.SetActive(true);
            inputFieldPassword.SetActive(true);
            buttonSubmit.SetActive(true);
            toggleLogin.SetActive(true);
            toggleCreate.SetActive(true);
            infoText1.SetActive(true);
            infoText2.SetActive(false);
        }
        else if (newState == GameStates.MainMenu)
        {
            findGameSessionButton.SetActive(true);
            observerButton.SetActive(true);
        }
        else if (newState == GameStates.WaitingForMatch)
        {
            observerButton.SetActive(false);
            infoText1.SetActive(true);
        }
        else if (newState == GameStates.PlayingTicTacToe)
        {
            placeHolderGameButton.SetActive(true);
        }
    }

    public bool getCanPlay()
    {
        return networkedClient.GetComponent<NetworkedClient>().getCanPlay();
    }

    public GameObject getBoard()
    {
        return board;
    }

    public List<Step> GetStepList()
    {
        return stepList;
    }

    public bool IsStepListEmpty()
    {
        if (stepList.Count > 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void AddStep(int location, BoxState state)
    {
        int count = stepList.Count;
        stepList.Add(new Step(count,location,state));
        Debug.Log("Add Step " + location + " " + state);
    }

    public void ClearStepList()
    {
        stepList = new List<Step>();
    }
    
    private void SendChatMessage(string content="")
    {
        string c;
        if (content.Length == 0)
        {
            c = chatInput.GetComponent<InputField>().text;
        }
        else
        {
            c = content;
        }
        
        if (c.Length != 0)
        {
            networkedClient.GetComponent<NetworkedClient>()
                .SendMessageToHost(ClientToServerSignifiers.SendChatMessage + "," + c);
            if (myChat.activeSelf)
            {
                myChatText.GetComponent<Text>().text = c;
                playerChatCounter = 0.0f;
            }
            else
            {
                myChat.SetActive(true);
                myChatText.GetComponent<Text>().text = c;
                playerChatCounter = 0.0f;
            }
            chatInput.GetComponent<InputField>().text = "";
        }
    }

    public void SetEnemyChat(String message)
    {
        if (enemyChat.activeSelf)
        {
            enemyChatText.GetComponent<Text>().text = message;
            enemyChatCounter = 0.0f;
        }
        else
        {
            enemyChat.SetActive(true);
            enemyChatText.GetComponent<Text>().text = message;
            enemyChatCounter = 0.0f;
        }
    }

    public class Step
    {
        private int order;
        private int location;
        private BoxState state;

        public Step()
        {
            order = -1;
            location = -1;
            state = BoxState.Empty;
        }

        public Step(int order, int location, BoxState state)
        {
            this.order = order;
            this.location = location;
            this.state = state;
        }

            public void SetValue(int order, int location, BoxState state)
        {
            this.order = order;
            this.location = location;
            this.state = state;
        }

        public int Order
        {
            get
            {
                return order;
            }

            set
            {
                order = value;
            }
        }

        public int Location
        {
            get
            {
                return location;
            }

            set
            {
                location = value;
            }
        }

        public BoxState State
        {
            get
            {
                return state;
            }

            set
            {
                state = value;
            }
        }
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
        public const int SendChatMessage = 10;
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
        public const int SendChatMessage = 10;
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