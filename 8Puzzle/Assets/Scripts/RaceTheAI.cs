//-------------------------------------------------------------------
// Name: Timothy Ngo
// School Email: timothyngo@nevada.unr.edu
//-------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RaceTheAI : MonoBehaviour
{
    [Header("Player Board")]
    public TileController playerTC;
    public BFSAgent playerBFSAgent;
    public GameObject inputBlocker;
    [Header("AI Board")]
    public TileController AITC;
    public BFSAgent AIBFSAgent;

    [Header("Shuffle Settings")]
    //public int shuffleMax = 20;
    public bool doShuffle = false;
    public int halfMinNumberOfShuffles = 5;
    public int halfMaxNumberOfShuffles = 10;
    public int numberOfShuffles;
    public float timePerMove = 1.0f;
    public float shuffleTimer;
    private Vector3 prevVector = Vector3.zero;

    [Header("Buttons")]
    public GameObject playButton;
    public GameObject howToPlayModal;
    public GameObject goBackButton;
    public GameObject restartButton;

    [Header("Timer")]
    float currentTime;
    public GameObject timer;
    public TextMeshProUGUI currentTimeText;
    public bool gameActive = false;

    // Start is called before the first frame update
    void Start()
    {
        InitGame();
    }
    public void InitGame()
    {
        timer.SetActive(false);
        playButton.SetActive(true);
        inputBlocker.SetActive(true);
        howToPlayModal.SetActive(true);
        restartButton.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (doShuffle && numberOfShuffles > 0)
        {
            shuffleTimer -= Time.deltaTime;
            if (shuffleTimer <= 0)
            {
                prevVector = Shuffle(prevVector);
                shuffleTimer = timePerMove;
                numberOfShuffles -= 1;
            }
            if (numberOfShuffles <= 0)
            {
                doShuffle = false;
                // Start timer here
                timer.SetActive(true);
                gameActive = true;
                inputBlocker.SetActive(false);
                AIBFSAgent.BFSSolver();

                Debug.Log("Finished Shuffling");
            }
        }


        if (gameActive)
        {
            currentTime += Time.deltaTime;

            TimeSpan time = TimeSpan.FromSeconds(currentTime);
            currentTimeText.text = time.Minutes.ToString("D2") + ":" + time.Seconds.ToString("D2");

            if (AIBFSAgent.IsGoalState(playerTC.gameBoard))
            {
                playerBFSAgent.leanSolved.Pulse();
            }
            if (AIBFSAgent.IsGoalState(AITC.gameBoard) || AIBFSAgent.IsGoalState(playerTC.gameBoard))
            {
                PostGame();
                Time.timeScale = 0;
            }
        }
    }

    public void PostGame()
    {
        gameActive = false;
        inputBlocker.SetActive(true);
        restartButton.SetActive(true);
        goBackButton.SetActive(true);
    }
    public void StartRace()
    {
        StartShuffle();
        playButton.SetActive(false);
        howToPlayModal.SetActive(false);
        goBackButton.SetActive(false);
        
    }

    public void StartShuffle()
    {
        if (!doShuffle)
        {
            doShuffle = true;
            shuffleTimer = timePerMove;
            numberOfShuffles = UnityEngine.Random.Range(halfMinNumberOfShuffles, halfMaxNumberOfShuffles) * 2;
        }
        else
        {
            Debug.Log("Shuffle in progress");
        }
    }

    public Vector3 Shuffle(Vector3 prevVector)
    {
        
        Vector3 randomVector = AIBFSAgent.RandomMove(AITC.gameBoard);

        while (randomVector == (-prevVector))
        {
            randomVector = AIBFSAgent.RandomMove(AITC.gameBoard);
        }

        AITC.MoveEmpty(randomVector);
        playerTC.MoveEmpty(randomVector);

        return randomVector;

    }


}
