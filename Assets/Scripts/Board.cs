using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Board : MonoBehaviour
{
    [Header("Input Settings : ")] 
    [SerializeField]
    private LayerMask boxesLayerMask;

    [Header("Mark Sprites : ")] 
    [SerializeField]
    private Sprite spriteX;
    [SerializeField] 
    private Sprite spriteO;

    public UnityAction<BoxState> OnWinAction;
    public BoxState[] boxStates;
    private Camera cam;
    private BoxState currentMark;
    private bool canPlay;
    private LineRenderer lineRenderer;
    private int marksCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main ;
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;

        currentMark = BoxState.X;
        boxStates = new BoxState[9];

        canPlay = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (canPlay && Input.GetMouseButtonDown(0))
        {
            Vector2 clickPosition = cam.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hit = Physics2D.OverlapPoint(clickPosition, boxesLayerMask);

            if (hit)
            {
                MarkBox(hit.GetComponent<Box>());
            }
        }
    }

    private void MarkBox(Box box)
    {
        if (box.isEmpty)
        {
            boxStates[box.index] = currentMark;
            if (currentMark == BoxState.X)
            {
                box.MarkBox(spriteX,BoxState.X);
            }
            else if (currentMark == BoxState.O)
            {
                box.MarkBox(spriteO, BoxState.O);
            }

            marksCount++;

            bool won = CheckWin();
            if (won)
            {
                if (OnWinAction != null)
                {
                    OnWinAction.Invoke(currentMark);
                    Debug.Log(currentMark+" win");
                }

                canPlay = false;
                return;
            }

            if (marksCount == 9)
            {
                if (OnWinAction != null)
                {
                    OnWinAction.Invoke(BoxState.Empty);
                    Debug.Log("Nobody wins");
                }

                canPlay = false;
                return;
            }

            SwitchPlayer();
        }
    }

    private bool CheckWin()
    {
        bool isWin = CheckLine(0, 1, 2) || CheckLine(3, 4, 5) || CheckLine(6, 7, 8) || CheckLine(0, 3, 6) ||
                     CheckLine(1, 4, 7) || CheckLine(2, 5, 8) || CheckLine(0, 4, 8) || CheckLine(2, 4, 6);
        return isWin;
    }

    private bool CheckLine(int a, int b, int c)
    {
        bool result = boxStates[a] != BoxState.Empty && boxStates[a] == boxStates[b] && boxStates[a] == boxStates[c] &&
                      boxStates[b] == boxStates[c];
        if (result)
        {
            DrawLine(a,c);
        }

        Debug.Log(boxStates[a]+" "+boxStates[b]+" "+boxStates[c]);
        return result;
    }

    private void DrawLine(int start, int end)
    {
        lineRenderer.SetPosition(0, transform.GetChild(start).position);
        lineRenderer.SetPosition(1, transform.GetChild(end).position);
        lineRenderer.enabled = true;
    }

    private void SwitchPlayer()
    {
        if (currentMark == BoxState.X)
        {
            currentMark = BoxState.O;
        }
        else
        {
            currentMark = BoxState.X;
        }
    }
}
