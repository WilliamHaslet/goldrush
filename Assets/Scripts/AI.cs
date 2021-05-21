using UnityEngine;

public class AI : MonoBehaviour
{

    [SerializeField] private float randomDirectionTime;
    
    private float randomDirectionTimer;
    private Vector2 direction;
    private Player player;

    private void Start()
    {

        player = GetComponent<Player>();

    }

    private void Update()
    {

        randomDirectionTimer -= Time.deltaTime;

        if (randomDirectionTimer <= 0)
        {

            randomDirectionTimer = randomDirectionTime;

            if (Random.value < 0.5)
            {

                if (direction.y == -1)
                {

                    float rand = Random.value;

                    if (rand < 0.4)
                    {

                        direction = new Vector2(1, 0);

                    }
                    else if (rand < 0.8)
                    {

                        direction = new Vector2(-1, 0);

                    }
                    else
                    {

                        direction = new Vector2(0, 1);

                    }

                }
                else
                {

                    direction = new Vector2(0, -1);

                }
                
            }
            else
            {

                float rand = Random.value;

                if (rand < 0.4)
                {

                    direction = new Vector2(1, 0);

                }
                else if (rand < 0.8)
                {

                    direction = new Vector2(-1, 0);

                }
                else
                {

                    direction = new Vector2(0, 1);

                }

            }

        }

    }

    public Vector2 GetInput()
    {

        return direction;

    }

    public bool IsActive()
    {
        
        return player.GetPlayerName() == null;

    }

}
