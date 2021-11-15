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
    GameObject findGameSessionButton, placeHolderGameButton;
    GameObject infoText1, infoText2;
    GameObject board;
    Board newBoard;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "StartScene")
        {
            GameObject[] allObjs = FindObjectsOfType<GameObject>();
            
                    foreach (var go in allObjs)
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
                    }
                    
                    buttonSubmit.GetComponent<Button>().onClick.AddListener(SubmitButtonPressed); 
                    toggleCreate.GetComponent<Toggle>().onValueChanged.AddListener(ToggleCreateValueChanged);
                    toggleLogin.GetComponent<Toggle>().onValueChanged.AddListener(ToggleLoginValueChanged);
                    
                    findGameSessionButton.GetComponent<Button>().onClick.AddListener(FindGameSessionButtonPressed); 
                    placeHolderGameButton.GetComponent<Button>().onClick.AddListener(PlaceHolderGameButtonPressed); 
                    
                    ChangeGameStates(GameStates.login);
        }
        else if (SceneManager.GetActiveScene().name == "GameScene")
        {
            GameObject[] allObjs = FindObjectsOfType<GameObject>();
            
            foreach (var go in allObjs)
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
        if (SceneManager.GetActiveScene().name == "StartScene" && newBoard == null)
        {
            FindBoard();
        }
    }

    public void FindBoard()
    {
        Debug.Log("Start finding board");
        /*Board[] allBoards = FindObjectsOfType<Board>();
        
        foreach (var go in allBoards)
        {
            Debug.Log("1 time");
            if (go.name == "Board")
            {
                newBoard = go;
                Debug.Log("Find Board");
            }
        }*/
        
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        
        foreach (var go in allObjects)
        {
            //Debug.Log("1 time");
            if (go.name == "Board")
            {
                board = go;
                Debug.Log("Find Board");
            }
        }
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
    }

    public void MarkDrawed(int location)
    {
        Debug.Log("Mark at " + location);
        string l = location.ToString();
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.DrawMark + "," + l);
    }

    public void EnemyDrawMark(int location)
    {
        board.GetComponent<Board>().DrawMark(location,false);
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
        }
        else if (newState == GameStates.WaitingForMatch)
        {
            
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
    }
}