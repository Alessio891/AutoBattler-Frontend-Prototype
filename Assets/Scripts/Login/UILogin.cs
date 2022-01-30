using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Network;
using LiteNetLib.Utils;
using LiteNetLib;

public class UILogin : MonoBehaviour
{
    public InputField User, Password;

    public static UILogin instance;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        ConnectionManager.instance.SubscribePacket<ConnectionAttemptResult>(OnConnectionResponse);
        ConnectionManager.instance.SubscribePacket<AuthenticationResult>(OnLoginAttemptResponse);
    }

    public void Login()
    {
        ConnectionManager.instance.Connect();
    }

    void OnConnectionResponse(ConnectionAttemptResult packet, NetPacketProcessor netProcessor, NetManager client)
    {
        if (packet.Accepted)
        {
            Debug.Log("Connection accepted with message " + packet.Message);
            ConnectionManager.instance.clientId = packet.ClientID;            
            string user = User.text;
            string pass = Password.text;
            netProcessor.Send(client.FirstPeer, new AuthenticationRequest() { ClientID = packet.ClientID, Username = user, HashedWord = pass }, DeliveryMethod.ReliableOrdered);
        }
        else
            Debug.Log("Connection refused with message " + packet.Message);
    }

    void OnLoginAttemptResponse(AuthenticationResult packet, NetPacketProcessor netProcessor, NetManager client)
    {
        if (packet.Result)
        {
            OnLoginSuccessful();
        }
        else
        {
            OnLoginFailed(packet.Message);
        }
    }

    public void OnLoginSuccessful() {
        SceneManager.LoadScene("MainMenu");
    }
    public void OnLoginFailed(string message) {
        Debug.Log("Login Failed: " + message);
    }
}
