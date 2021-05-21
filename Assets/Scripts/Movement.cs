using UnityEngine;

public class Movement : MonoBehaviour
{

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float gravity;
    [SerializeField] private float maxHeight;
    [Header("Mining")]
    [SerializeField] private float breakDelay;
    [SerializeField] private LayerMask tileLayer;
    [SerializeField] private ParticleSystem sparksSide;
    [SerializeField] private ParticleSystem sparksUp;
    [SerializeField] private ParticleSystem sparksDown;

    private Player player;
    private AI ai;
    private InputManager inputManager;
    private MapGenerator mapGenerator;
    private Rigidbody2D playerRigidbody;
    private Animator playerAnimator;
    private Vector2 moveDirection;
    private float breakTimer;
    private float currentGravity;
    private int playerIndex;
    private bool grounded;
    private bool mineAnimationPlaying;

    private void Start()
    {

        player = GetComponent<Player>();

        ai = GetComponent<AI>();

        playerRigidbody = GetComponent<Rigidbody2D>();

        inputManager = FindObjectOfType<InputManager>();

        mapGenerator = FindObjectOfType<MapGenerator>();

        playerAnimator = GetComponentInChildren<Animator>();

        playerAnimator.SetFloat("mineSpeed", 1 / breakDelay);

        playerIndex = player.GetPlayerIndex();

        breakTimer = breakDelay;

    }

    private void Update()
    {

        grounded = mapGenerator.IsGrounded(transform.position);

        Vector2 input = inputManager.GetInput(playerIndex);

        if (ai.IsActive())
        {

            input = ai.GetInput();

        }

        if (input.x != 0 || input.y != 0)
        {

            if (Mathf.Abs(input.x) >= Mathf.Abs(input.y))
            {

                moveDirection = new Vector2(Mathf.Sign(input.x), 0);

            }
            else
            {

                moveDirection = new Vector2(0, Mathf.Sign(input.y));

            }

            if (moveDirection.x != 0)
            {

                transform.localScale = new Vector3(-moveDirection.x, 1, 1);

            }

            if (mapGenerator.IsBlocked((Vector2)transform.position + moveDirection))
            {

                if (!mineAnimationPlaying)
                {

                    if (moveDirection.x != 0)
                    {

                        playerAnimator.SetTrigger("mineSide");

                        sparksSide.Play();

                    }
                    else if (moveDirection.y == 1)
                    {

                        playerAnimator.SetTrigger("mineUp");

                        sparksUp.Play();

                    }
                    else
                    {

                        playerAnimator.SetTrigger("mineDown");

                        sparksDown.Play();

                    }

                    mineAnimationPlaying = true;

                }

                breakTimer -= Time.deltaTime;

                if (breakTimer <= 0)
                {

                    player.UpdateScore(mapGenerator.BreakTile((Vector2)transform.position + moveDirection));

                    breakTimer = breakDelay;

                    mineAnimationPlaying = false;

                }

            }
            else if (moveDirection.y == 1 && grounded)
            {

                mapGenerator.BuildLadder(transform.position);

                breakTimer = breakDelay;

                mineAnimationPlaying = false;

            }
            else
            {

                breakTimer = breakDelay;

                mineAnimationPlaying = false;

            }

            playerAnimator.SetBool("moving", true);

        }
        else
        {

            moveDirection = Vector2.zero;

            breakTimer = breakDelay;

            mineAnimationPlaying = false;

            playerAnimator.SetBool("moving", false);

        }

        if (grounded)
        {

            currentGravity = 0;

        }
        else
        {

            currentGravity += gravity * Time.deltaTime;

        }

        if (transform.position.y > maxHeight)
        {

            transform.position = new Vector3(transform.position.x, maxHeight, 0);

        }

    }

    private void FixedUpdate()
    {

        if (grounded)
        {

            if (moveDirection == Vector2Int.zero || mapGenerator.IsBlocked((Vector2)transform.position + moveDirection))
            {

                playerRigidbody.position = new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));

            }
            else if (moveDirection.y == 0)
            {

                playerRigidbody.position = new Vector2(playerRigidbody.position.x, Mathf.Round(transform.position.y));

            }

        }

        Vector2 velocity = moveDirection * moveSpeed * Time.fixedDeltaTime;

        if (!grounded)
        {

            velocity.y = currentGravity;

        }

        playerRigidbody.velocity = velocity;

    }

}
