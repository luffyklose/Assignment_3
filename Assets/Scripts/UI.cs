using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [Header("UI References :")] 
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private GameObject inGameCanvas;
    [SerializeField] private Text uiWinnerText;
    [SerializeField] private Button uiRestartButton;
    [SerializeField] private Button uiReplayBUtton;

    [SerializeField] private Board board;
    // Start is called before the first frame update
    void Start()
    {
        uiRestartButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(0);
            GameManager tempGameManager = FindObjectOfType<GameManager>();
            tempGameManager.ChangeGameStates(2);
        });
        uiReplayBUtton.onClick.AddListener(StartReplay);
        board.OnWinAction += OnWinEvent;

        gameOverCanvas.SetActive (false) ;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnWinEvent (BoxState mark) 
    {
        if (mark == BoxState.Empty)
        {
            uiWinnerText.text = "Nobody Wins";
        }
        else
        {
            uiWinnerText.text = mark.ToString() + " Wins";
        }

        gameOverCanvas.SetActive(true);
        inGameCanvas.SetActive(false);
    }

    private void OnDestroy () 
    {
        uiRestartButton.onClick.RemoveAllListeners () ;
        board.OnWinAction -= OnWinEvent ;
    }

    private void StartReplay()
    {
        board.StartReplay();
    }
}
