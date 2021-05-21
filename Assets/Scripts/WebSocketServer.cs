using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class WebSocketServer : MonoBehaviour
{

    private struct AsyncData
    {
        public int index;
        public byte[] bytes;
    }

    [SerializeField] private int port = 11111;

    private TcpListener server;
    private Action<string, int> callback;
    private List<TcpClient> clients = new List<TcpClient>();
    private List<NetworkStream> streams = new List<NetworkStream>();

    private void OnApplicationQuit()
    {

        if (server != null)
        {

            StopServer();

        }

    }

    private void Update()
    {

        if (server != null)
        {

            if (server.Pending())
            {

                server.BeginAcceptTcpClient(new AsyncCallback(ConnectClient), server);

            }

            for (int i = 0; i < streams.Count; i++)
            {

                if (streams[i].DataAvailable)
                {

                    StartRead(i);

                }

            }

        }

    }

    public void StartServer(string ipAddress, Action<string, int> readCallback)
    {

        server = new TcpListener(IPAddress.Parse(ipAddress), port);

        server.Start();

        callback = readCallback;

        Debug.Log(string.Format("Server started at {0} on port {1}", ipAddress, port));

    }

    public void StopServer()
    {

        try
        {

            server.Stop();

            server = null;

        }
        catch (Exception e)
        {

            Debug.LogError(e);

        }

    }

    private void ConnectClient(IAsyncResult asyncResult)
    {

        TcpClient client = ((TcpListener)asyncResult.AsyncState).EndAcceptTcpClient(asyncResult);

        clients.Add(client);

        streams.Add(client.GetStream());

        Debug.Log("Client Connected");

    }

    private void StartRead(int index)
    {

        byte[] readBytes = new byte[clients[index].Available];

        AsyncData asyncData = new AsyncData
        {
            index = index,
            bytes = readBytes
        };

        streams[index].BeginRead(readBytes, 0, readBytes.Length, new AsyncCallback(ReadCallback), asyncData);

    }

    private void ReadCallback(IAsyncResult asyncResult)
    {

        AsyncData asyncData = (AsyncData)asyncResult.AsyncState;

        int byteCount = streams[asyncData.index].EndRead(asyncResult);

        if (Encoding.UTF8.GetString(asyncData.bytes, 0, byteCount).StartsWith("GET"))
        {

            Handshake(asyncData.bytes, asyncData.index);

            Debug.Log("Handshake Complete");

        }
        else
        {

            string message = WebSocketUnmask(asyncData.bytes);

            Debug.Log("WebSocket Received: " + message);

            callback.Invoke(message, asyncData.index);

        }

    }

    public void StartWrite(string message, int index)
    {

        byte[] sendBytes = Encoding.UTF8.GetBytes(message);

        AsyncData asyncData = new AsyncData()
        {
            index = index,
            bytes = sendBytes
        };

        streams[asyncData.index].BeginWrite(sendBytes, 0, sendBytes.Length, new AsyncCallback(WriteCallback), asyncData);

    }

    private void WriteCallback(IAsyncResult asyncResult)
    {

        AsyncData asyncData = (AsyncData)asyncResult.AsyncState;

        streams[asyncData.index].EndWrite(asyncResult);

        string sentMessage = Encoding.UTF8.GetString(asyncData.bytes);

        Debug.Log("WebSocket Sent: " + sentMessage);

        if (sentMessage == "DisconnectSuccess")
        {

            clients.Remove(clients[asyncData.index]);
            
            streams.Remove(streams[asyncData.index]);

            Debug.Log("Client Disconnected");

        }

    }

    private void Handshake(byte[] bytes, int index)
    {

        string message = Encoding.UTF8.GetString(bytes);

        string swk = Regex.Match(message, "Sec-WebSocket-Key: (.*)").Groups[1].Value.Trim();

        string swka = swk + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";

        byte[] swkaSha1 = System.Security.Cryptography.SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(swka));

        string swkaSha1Base64 = Convert.ToBase64String(swkaSha1);

        StartWrite("HTTP/1.1 101 Switching Protocols\r\n" + "Connection: Upgrade\r\n" + "Upgrade: websocket\r\n" + "Sec-WebSocket-Accept: " + swkaSha1Base64 + "\r\n\r\n", index);

    }

    private string WebSocketUnmask(byte[] bytes)
    {

        string message = string.Empty;

        bool mask = (bytes[1] & 0b10000000) != 0;

        ulong msglen = (ulong)(bytes[1] - 128);

        int offset = 2;

        if (msglen == 126)
        {

            msglen = BitConverter.ToUInt16(new byte[] { bytes[3], bytes[2] }, 0);

            offset = 4;

        }
        else if (msglen == 127)
        {

            msglen = BitConverter.ToUInt64(new byte[] { bytes[5], bytes[4], bytes[3], bytes[2], bytes[9], bytes[8], bytes[7], bytes[6] }, 0);

            offset = 10;

        }

        if (msglen == 0)
        {

            Debug.LogError("Message length is zero");

        }
        else if (mask)
        {

            byte[] decoded = new byte[msglen];

            byte[] masks = new byte[4] { bytes[offset], bytes[offset + 1], bytes[offset + 2], bytes[offset + 3] };

            offset += 4;

            for (int i = 0; (ulong)i < msglen; i++)
            {

                decoded[i] = (byte)(bytes[offset + i] ^ masks[i % 4]);

            }

            message = Encoding.UTF8.GetString(decoded);

        }
        else
        {

            Debug.LogError("Message not masked");

        }

        return message;

    }

}
