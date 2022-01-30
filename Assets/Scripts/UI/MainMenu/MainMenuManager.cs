using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Network;
using LiteNetLib.Utils;
using LiteNetLib;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{

    public WaitingLobbyOverlay WaitingOverlay;

    private void Start()
    {
        ConnectionManager conn = ConnectionManager.instance;

        conn.SubscribePacket<JoinLobbyResponsePacket>(HandleLobbyResponse);
    }

    void HandleLobbyResponse(JoinLobbyResponsePacket packet, NetPacketProcessor processor, NetManager client) {     
        if (!packet.LobbyStarted)
        {
            WaitingOverlay.StopWaiting();
            Debug.Log("There was an error with the server and the lobby didn't start");
            return;
        }
        StartCoroutine(GameStart());
        Debug.Log("Lobby started!!!");
    }

    IEnumerator GameStart()
    {

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("GameScene");
        while (!asyncLoad.isDone)
        {
            yield return null;
        }        

    }

    public void JoinLobby() {
        NetPacketProcessor processor = ConnectionManager.instance.netProcessor;
        processor.Send(ConnectionManager.instance.client.FirstPeer, new JoinLobbyRequestPacket()
        {
            ClientID = ConnectionManager.instance.clientId,           
        }, DeliveryMethod.ReliableOrdered);
        WaitingOverlay.StartAnimation();
    }
}
