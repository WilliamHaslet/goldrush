using UnityEngine;
using UnityEngine.UI;

public class ColorSetter : MonoBehaviour
{

    [SerializeField] private Color[] colors;
    [SerializeField] private Image[] colorTiles;
    [SerializeField] private SpriteRenderer[] playerRenderers;
    [SerializeField] private Text[] playerNames;
    [SerializeField] private Image[] playerPreviews;
    [SerializeField] private Transform[] selectorLabels;
    [SerializeField] private ParticleSystem[] winConfetties;

    private int[] selectedColors = { 0, 1, 2, 3 };

    private void Start()
    {
        
        for (int i = 0; i < colors.Length; i++)
        {

            colorTiles[i].color = colors[i];

        }
        
        for (int i = 0; i < 4; i++)
        {

            playerRenderers[i].material.SetColor("_BodyColor", colors[i]);

            playerNames[i].color = colors[i];

            playerPreviews[i].material.SetColor("_BodyColor", colors[i]);

            ParticleSystem.MainModule particles = winConfetties[i].main;

            particles.startColor = colors[i];

        }

    }

    private void Update()
    {

        for (int i = 0; i < 4; i++)
        {

            selectorLabels[i].position = colorTiles[selectedColors[i]].transform.position;

        }

    }
    
    public void MovePlayerColor(int player, int direction)
    {

        bool done = false;

        int index = selectedColors[player] + direction;

        while (!done)
        {

            if (index < 0)
            {

                index = colorTiles.Length - 1;

            }
            else if (index >= colorTiles.Length)
            {

                index = 0;

            }

            if (!ArrayContains(index))
            {

                done = true;

            }
            else
            {

                index += direction;

            }

        }

        int colorIndex = index;

        selectedColors[player] = colorIndex;

        selectorLabels[player].position = colorTiles[colorIndex].transform.position;

        playerRenderers[player].material.SetColor("_BodyColor", colors[colorIndex]);

        playerNames[player].color = colors[colorIndex];

        playerPreviews[player].material.SetColor("_BodyColor", colors[colorIndex]);

        ParticleSystem.MainModule particles = winConfetties[player].main;

        particles.startColor = colors[colorIndex];

    }

    private bool ArrayContains(int value)
    {

        bool hasValue = false;

        for (int i = 0; i < 4; i++)
        {

            if (selectedColors[i] == value)
            {

                hasValue = true;

                break;

            }

        }

        return hasValue;

    }

}
