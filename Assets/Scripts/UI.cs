using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [Header("UI References :")] 
    [SerializeField] private GameObject uiCanvas;
    [SerializeField] private Text uiWinnerText;
    [SerializeField] private Button uiRestartButton;

    [SerializeField] private Board board;
    // Start is called before the first frame update
    void Start()
    {
        uiRestartButton.onClick.AddListener(() => SceneManager.LoadScene(0));
        board.OnWinAction += OnWinEvent;

        uiCanvas.SetActive (false) ;
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

        uiCanvas.SetActive (true) ;
    }

    private void OnDestroy () 
    {
        uiRestartButton.onClick.RemoveAllListeners () ;
        board.OnWinAction -= OnWinEvent ;
    }
}
