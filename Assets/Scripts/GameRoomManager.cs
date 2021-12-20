using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class GameRoomManager : MonoBehaviour
{
    private static GameRoomManager instance;
    private GameObject gameManager;
    
    private bool canPlay;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject go in allObjects)
        {
            if (go.name == "GameManager")
            {
                gameManager = go;
                Debug.Log("Get gameobject");
            }
                
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager == null)
        {
            GameObject[] allObjects = FindObjectsOfType<GameObject>();

            foreach (GameObject go in allObjects)
            {
                if (go.name == "GameManager")
                {
                    gameManager = go;
                    Debug.Log("Get gameobject");
                }
                
            }
        }
    }

    public static GameRoomManager GetInstance()
    {
        return instance;
    }

    public void OpponentTicTacToePlay(string msg, int id)
    {
        string[] csv = msg.Split(',');
        int signifier = int.Parse(csv[0]);
        
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
        
        Debug.Log($"Can Play? {canPlay}");
        SceneManager.LoadScene("GameScene");
        //gameManager.GetComponent<GameManager>().FindGameSceneObject();
        if (canPlay)
        {
            gameManager.GetComponent<GameManager>().FindGameSceneObject();
            gameManager.GetComponent<GameManager>().getBoard().GetComponent<Board>().EnterPlayerTurn();
        }
    }
    
    public void DrawMark(string msg, int id)
    {
        string[] csv = msg.Split(',');
        int signifier = int.Parse(csv[0]);
        
        Debug.Log("Enemy draw mark at box " + csv[1]);
        gameManager.GetComponent<GameManager>().EnemyDrawMark(int.Parse(csv[1]));
    }
    
    public void JoiningRoom(string msg, int id)
    {
        string[] csv = msg.Split(',');
        int signifier = int.Parse(csv[0]);
        
        int i = int.Parse(csv[1]);
        if (i == (int)JoinRoomSignifier.StartReceivingStep)
        {
            Debug.Log("Join a room to observe");
            SceneManager.LoadScene("GameScene");
            gameManager.GetComponent<GameManager>().FindGameSceneObject();
        }
        else if (i == (int)JoinRoomSignifier.ReceiveStep)
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
        else if (i == (int)JoinRoomSignifier.EndReceivingStep)
        {
            Debug.Log("get steps finish");
            gameManager.GetComponent<GameManager>().getBoard().GetComponent<Board>().LoadStep();
        }
    }
    
    public void NoRoomCanJoin(string msg, int id)
    {
        Debug.Log("No Room can be joined");
    }
    
    public void DrawMarkOnObserver(string msg, int id)
    {
        string[] csv = msg.Split(',');
        int signifier = int.Parse(csv[0]);
        
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
    
    public void GameOver(string msg, int id)
    {
        string[] csv = msg.Split(',');
        int signifier = int.Parse(csv[0]);
        
        int i = int.Parse(csv[1]);
        if (i == (int)GameOverSignifier.GameOver)
        {
            Debug.Log("Game over, start receive replay data");
            gameManager.GetComponent<GameManager>().ClearStepList();
        }
        else if (i == (int)GameOverSignifier.ReceiveSteps)
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
        else if (i == (int)GameOverSignifier.ReceiveStepsOver)
        {
            Debug.Log("Create local saving file");
            CreateReplayFile();
        }
    }

    private void CreateReplayFile()
    {
        List<GameManager.Step> tempList = gameManager.GetComponent<GameManager>().GetStepList();

        DataManager.SaveSignleGame(tempList);
    }
}

public enum JoinRoomSignifier
{
    StartReceivingStep=0,
    ReceiveStep=1,
    EndReceivingStep=2
}

public enum GameOverSignifier
{
    GameOver=0,
    ReceiveSteps=1,
    ReceiveStepsOver=2
}