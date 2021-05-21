using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{

    [SerializeField] private Text timerText;

    private float timer;
    private bool timerStarted;

    private void Update()
    {

        if (timerStarted)
        {

            if (timer > 0)
            {

                timer -= Time.deltaTime;

                if (timer <= 0)
                {

                    timerStarted = false;

                    timer = 0;

                }

                timerText.text = Mathf.FloorToInt(timer / 60).ToString("d2") + ":" + Mathf.FloorToInt(timer % 60).ToString("d2");

            }

        }

    }

    public float GetTime()
    {

        return timer;

    }
    
    public bool IsDone()
    {

        return timer <= 0;

    }

    public void StartTimer(float time)
    {

        timer = time;

        timerStarted = true;

    }

}
