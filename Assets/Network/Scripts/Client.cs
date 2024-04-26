using System.IO;
using System;
using System.Net.Sockets;
using TMPro;
using UnityEngine;

public class Client : MonoBehaviour
{
    [SerializeField] Chat chat;

    [SerializeField] TMP_InputField nameField;
    [SerializeField] TMP_InputField ipField;
    [SerializeField] TMP_InputField portField;

    private TcpClient client;
    private NetworkStream stream;
    private StreamWriter writer;
    private StreamReader reader;

    private string clientName;
    private string ip;
    private int port;

    private bool isConnected;
    public bool IsCconnected { get { return isConnected; } }

    public void Connect()
    {
        if ( isConnected ) return;

        clientName = nameField.text;
        ip = ipField.text;
        port = int.Parse(portField.text);

        try
        {
            client = new TcpClient(ip, port);
            stream = client.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);

            isConnected = true;
        }
        catch ( Exception ex )
        {
            Debug.Log(ex.Message);
        }
    }

    public void DisConnect()
    {
        writer?.Close();
        writer = null;
        reader?.Close();
        reader = null;
        stream?.Close();
        stream = null;
        client?.Close();
        client = null;

        isConnected = false;
    }

    public void SendChat( string chatText )
    {
        if ( IsCconnected == false ) return;

        try
        {
            writer.WriteLine($"{clientName} : {chatText}");
            writer.Flush(); //버퍼 클리어
        }
        catch ( Exception ex )
        {
            Debug.Log(ex.Message);
        }
    }

    private void Update()
    {
        if ( IsCconnected == false ) return;
        if ( stream.DataAvailable == false ) return;
        string text = reader.ReadLine();
        ReceiveChat(text);
    }

    public void ReceiveChat( string chatText )
    {
        chat.AddMessage(chatText);
    }

    private void AddMessage( string message )
    {
        Debug.Log($"[Client] {message}");
        chat.AddMessage($"[Client] {message}");
    }
}
