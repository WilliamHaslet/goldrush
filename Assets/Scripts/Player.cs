using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    
    [SerializeField] private Text nameText;
    [SerializeField] private Text scoreText;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private ParticleSystem cameraConfetti;
    [SerializeField] private int playerIndex;

    private string playerName;
    private int score;

    private void LateUpdate()
    {

        Vector3 target = transform.position;

        target.z = -1;

        playerCamera.transform.position = target;

    }

    public void UpdateScore(int scoreIncrease)
    {

        score += scoreIncrease;

        scoreText.text = "Score: " + score.ToString();

    }

    public void ResetScore()
    {

        score = 0;

        scoreText.text = "Score: " + score.ToString();

    }

    public void StartConfetti()
    {

        cameraConfetti.Play();

    }
    
    public void ClearConfetti()
    {

        cameraConfetti.Stop();

        cameraConfetti.Clear();

    }

    public int GetScore()
    {

        return score;

    }

    public int GetPlayerIndex()
    {

        return playerIndex;

    }

    public string GetPlayerName()
    {

        return playerName;

    }

    public void SetPlayerName(string newName)
    {

        playerName = newName;

        nameText.text = playerName;

    }

}
