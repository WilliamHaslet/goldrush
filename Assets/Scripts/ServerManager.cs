using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServerManager : MonoBehaviour
{

    [SerializeField] private WebSocketServer webSocketServer;
    [SerializeField] private Dropdown hostIPDropdown;
    [SerializeField] private Text connectCodeText;
    [SerializeField] private Transform connectedClientsList;
    [SerializeField] private GameObject clientPrefab;
    [SerializeField] private Text[] playerPointerNames;

    private List<string> messages = new List<string>();
    private InputManager inputManager;
    private GameManager gameManager;

    private void Start()
    {

        inputManager = FindObjectOfType<InputManager>();

        gameManager = FindObjectOfType<GameManager>();

        PopulateHostDropdown();

        // Auto start
        //hostIPDropdown.SetValueWithoutNotify(1);
        
        //StartServer();

        //FindObjectOfType<GameManager>().StartNewGame();

    }

    private void Update()
    {
        
        foreach (string message in messages)
        {

            if (message.StartsWith("Request:Name:"))
            {

                int offset = "Request:Name:".Length;

                AddPlayer(message.Substring(offset, message.Length - (offset + 1)), int.Parse(message.Substring(message.Length - 1)));

            }
            else if (message.StartsWith("Request:Disconnect"))
            {



            }

        }

        messages.Clear();

    }

    private void PopulateHostDropdown()
    {

        List<string> addresses = new List<string>();

        foreach (IPAddress address in Dns.GetHostAddresses(Dns.GetHostName()))
        {

            if (address.AddressFamily == AddressFamily.InterNetwork)
            {

                addresses.Add(address.ToString());

            }

        }

        hostIPDropdown.AddOptions(addresses);

        SetHostAddress();

    }

    public void SetHostAddress()
    {

        connectCodeText.text = ConnectionCode.Encode(hostIPDropdown.captionText.text);

    }
    
    public void StartServer()
    {

        webSocketServer.StartServer(hostIPDropdown.captionText.text, Callback);

    }

    public void StopServers()
    {

        webSocketServer.StopServer();

    }

    private void AddPlayer(string name, int index)
    {

        GameObject newClient = Instantiate(clientPrefab, connectedClientsList);

        newClient.GetComponentInChildren<Text>().text = name;

        gameManager.SetPlayerName(name, index);

        playerPointerNames[index].transform.parent.gameObject.SetActive(true);

        playerPointerNames[index].text = name;

        Debug.Log("Player " + index + ", " + name + " added");

    }
    
    private void RemoveConnectedClient(string clientName)
    {

        for (int i = 0; i < connectedClientsList.childCount; i++)
        {

            if (connectedClientsList.GetChild(i).GetComponentInChildren<Text>().text == clientName)
            {

                Destroy(connectedClientsList.GetChild(i).gameObject);

                break;

            }

        }

    }

    private void Callback(string message, int index)
    {

        if (message.StartsWith("Request:"))
        {

            messages.Add(message + index);
            
        }
        else
        {

            inputManager.SetInput(message, index);

        }

    }

}
