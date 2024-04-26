using System.Collections.Generic;
using System.Linq;
using System.Net;
using System;
using System.Net.Sockets;
using TMPro;
using UnityEngine;
using System.IO;

public class Server : MonoBehaviour
{
    [SerializeField] RectTransform logContent;
    [SerializeField] TMP_Text logTextPrefab;
    [SerializeField] TMP_InputField ipField;
    [SerializeField] TMP_InputField portField;

    private TcpListener listener;
    private List<TcpClient> clients = new List<TcpClient>();
    private List<TcpClient> disclients = new List<TcpClient>();
    private IPAddress ip;
    private int port;

    private bool isOpened;
    public bool IsOpened { get { return isOpened; } }

    private void Start()
    {
        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
        ip = host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
        ipField.text = ip.ToString();
    }

    private void OnDestroy()
    {
        if ( isOpened )
        {
            Close();
        }
    }

    private void Update()
    {
        if ( isOpened == false ) return;

        foreach ( TcpClient client in clients )
        {            
            if ( ClientConnectCheck(client) == false )
            {
                client.Close();
                disclients.Add(client);
                continue;
            }

            NetworkStream stream = client.GetStream();
            if ( stream.DataAvailable )
            {
                StreamReader reader = new StreamReader(stream);
                string text = reader.ReadLine();
                AddLog(text);
                SendAll(text);
            }
        }

        foreach ( TcpClient Client in disclients )
        {
            clients.Remove(Client);
        }
        disclients.Clear();
    }

    public void Open()
    {
        if ( isOpened ) return;
        Debug.Log("Try to Open");

        port = int.Parse(portField.text);

        try
        {
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();

            isOpened = true;
            listener.BeginAcceptTcpClient(AcceptCallback, listener);
        }
        catch ( Exception ex )
        {
            Debug.Log(ex.Message);
        }

    }

    public void Close()
    {
        listener?.Stop();
        listener = null;

        isOpened = false;
        AddLog("Close");
    }

    public void SendAll( string chat )
    {
        foreach ( TcpClient client in clients )
        {
            NetworkStream stream = client.GetStream();
            StreamWriter writer = new StreamWriter(stream);

            try
            {
                writer.WriteLine(chat);
                writer.Flush();
            }
            catch ( Exception ex )
            {
                Debug.Log(ex.Message);
            }
        }
    }

    private void AcceptCallback( IAsyncResult ar )
    {
        if ( isOpened == false ) return;

        TcpClient client = listener.EndAcceptTcpClient(ar);
        clients.Add(client);
        listener.BeginAcceptTcpClient(AcceptCallback, listener);
    }


    private void AddLog( string message )
    {
        Debug.Log($"[Server] {message}");
        TMP_Text newLog = Instantiate(logTextPrefab, logContent);
        newLog.text = message;
    }

    private bool ClientConnectCheck( TcpClient client )
    {
        try
        {
            if ( client != null && client.Client != null && client.Connected )
            {
                if ( client.Client.Poll(0, SelectMode.SelectRead) )
                    return !( client.Client.Receive(new byte [1], SocketFlags.Peek) == 0 );

                return true;
            }
            else
                return false;
        }
        catch ( Exception e )
        {
            AddLog("Connect Check Error");
            AddLog(e.Message);
            return false;
        }
    }
}
