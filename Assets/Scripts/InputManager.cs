using UnityEngine;

public class InputManager : MonoBehaviour
{

    private struct InputState
    {
        public float x;
        public float y;
    }

    private GameManager gameManager;
    private ColorSetter colorSetter;
    private InputState[] inputStates;
    private InputState[] colorInputs;
    private bool[] canSwitchColor = { true, true, true, true };
    private bool canChangeColor = true;

    private void Start()
    {

        gameManager = FindObjectOfType<GameManager>();

        colorSetter = FindObjectOfType<ColorSetter>();

        inputStates = new InputState[4];

        for (int i = 0; i < 4; i++)
        {

            inputStates[i] = new InputState();

        }

        colorInputs = new InputState[4];

        for (int i = 0; i < 4; i++)
        {

            colorInputs[i] = new InputState();

        }

    }

    private void Update()
    {

        if (canChangeColor)
        {

            for (int i = 0; i < 4; i++)
            {

                if (colorInputs[i].x != 0)
                {

                    if (canSwitchColor[i])
                    {

                        canSwitchColor[i] = false;

                        colorSetter.MovePlayerColor(i, (int)Mathf.Sign(colorInputs[i].x));

                    }

                }
                else
                {

                    canSwitchColor[i] = true;

                }

            }

        }

    }

    public Vector2 GetInput(int index)
    {

        return new Vector2(inputStates[index].x, inputStates[index].y);

    }

    public void SetInput(string inputState, int index)
    {

        if (gameManager.GetGameStarted())
        {

            inputStates[index] = JsonUtility.FromJson<InputState>(inputState);

        }
        else
        {

            colorInputs[index] = JsonUtility.FromJson<InputState>(inputState);

        }

    }

    public void SetCanChangeColor(bool canChange)
    {

        canChangeColor = canChange;

    }

    public void ClearInput()
    {

        for (int i = 0; i < 4; i++)
        {

            inputStates[i] = new InputState()
            {
                x = 0,
                y = 0
            };

        }

    }

}
