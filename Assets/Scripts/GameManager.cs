using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    [SerializeField] private float gameTime;
    [SerializeField] private Text winText;
    [SerializeField] private GameObject restartButton;
    [SerializeField] private Player[] players;
    [SerializeField] private Transform[] playerStartPoints;

    private MapGenerator mapGenerator;
    private InputManager inputManager;
    private GameTimer gameTimer;
    private bool gameStarted = false;

    private void Start()
    {

        inputManager = FindObjectOfType<InputManager>();

        mapGenerator = FindObjectOfType<MapGenerator>();

        gameTimer = FindObjectOfType<GameTimer>();

    }

    private void Update()
    {

        if (gameStarted && gameTimer.IsDone())
        {

            EndGame();

        }

    }

    public void StartNewGame()
    {

        mapGenerator.GenerateMap();

        StartPlayers();

        gameStarted = true;

        gameTimer.StartTimer(gameTime * 60);

        winText.text = "";

        restartButton.SetActive(false);

        inputManager.SetCanChangeColor(false);

        for (int i = 0; i < players.Length; i++)
        {

            players[i].ClearConfetti();

        }

    }

    private void EndGame()
    {

        gameStarted = false;

        winText.text = GetWinner() + " Wins!";

        restartButton.SetActive(true);

        inputManager.ClearInput();

    }

    public void StartPlayers()
    {

        gameStarted = true;

        for (int i = 0; i < players.Length; i++)
        {

            players[i].gameObject.SetActive(true);

            players[i].ResetScore();

            players[i].transform.position = playerStartPoints[i].position;

        }

    }

    public string GetWinner()
    {

        gameStarted = false;

        int highestScore = 0;

        int highestScorePlayer = 0;

        for (int i = 0; i < players.Length; i++)
        {

            if (players[i].GetScore() > highestScore)
            {

                highestScore = players[i].GetScore();

                highestScorePlayer = i;

            }

        }

        players[highestScorePlayer].StartConfetti();

        return (highestScorePlayer + 1).ToString();

    }

    public bool GetGameStarted()
    {

        return gameStarted;

    }

    public void SetPlayerName(string name, int index)
    {

        players[index].SetPlayerName(name);

    }

}
