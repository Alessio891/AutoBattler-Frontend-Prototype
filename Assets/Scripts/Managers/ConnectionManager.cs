using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;
using Network;
using UnityEngine.SceneManagement;

public class ConnectionManager : MonoBehaviour
{
    EventBasedNetListener Listener;
    public NetManager client;
    public NetPacketProcessor netProcessor;
    public int clientId = -1;

    public static ConnectionManager instance;
    public static NetPeer ServerPeer
    {
        get
        {
            return instance.client.FirstPeer;
        }
    }

    public static void SendPacket<T>(T packet) where T : class,new()
    {
        instance.netProcessor.Send<T>(ServerPeer, packet, DeliveryMethod.ReliableOrdered);
    }
    
    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;
    }

    void Start()
    {

        DontDestroyOnLoad(gameObject);

        Listener = new EventBasedNetListener();
        netProcessor = new NetPacketProcessor();        
        netProcessor.RegisterNestedType<MinionMatchData>(MinionMatchData.Serialize, MinionMatchData.Deserialize);
        netProcessor.RegisterNestedType<PlayerMatchData>(PlayerMatchData.Serialize, PlayerMatchData.Deserialize);
        client = new NetManager(Listener);
        client.Start();        

        Listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod) =>
        {
            netProcessor.ReadAllPackets(dataReader, fromPeer);
        };

        SubscribePacket<JoinLobbyResponsePacket>((packet, proc, cl) => {
            Debug.Log("Lobby respose");
        });

        Listener.PeerDisconnectedEvent += OnDisconnectFromServer;
    }

    public void SubscribePacket<T>( System.Action<T, NetPacketProcessor, NetManager> callback ) where T : class,new()
    {
        netProcessor.SubscribeReusable<T>((packet) =>
       {
           callback(packet, netProcessor, client);
       });
    }

    /// <summary>
    /// What's the difference with normal packet sending and receiving?
    /// This method will unsubscribe after receiving the packet, it's oneshot.
    /// Also it's cleaner (i think?)
    /// </summary>
    /// <typeparam name="T">Packet type to send</typeparam>
    /// <typeparam name="K">Packet type to receive</typeparam>
    /// <param name="packetToSend">Packet data</param>
    /// <param name="callBack">Callback function</param>
    public static void SendPacketWithCallback<T,K>(T packetToSend, System.Action<K> callBack) where T : class, new() where K : class, new()
    {
        instance.netProcessor.SubscribeReusable<K>((packet) =>
        {
            callBack(packet);
            instance.netProcessor.RemoveSubscription<K>();
        });
        SendPacket<T>(packetToSend);

    }


    private void OnDisconnectFromServer(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        try
        {
            int errorCode = disconnectInfo.AdditionalData.GetInt();
            if (errorCode == ResponseCodes.BAD_LOGIN)
            {
                Debug.Log("Disconnected for bad logins: " + disconnectInfo.AdditionalData.GetString());
            }
        } catch
        {
            Debug.Log("Unexpected disconnect from server.");
        }


        if (SceneManager.GetActiveScene().name != "Login")
        {
            SceneManager.LoadScene("Login");
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (client.IsRunning)
            client.PollEvents();
    }

    private void OnDisable()
    {
        if (client != null)
            client.Stop();
    }

    public void Connect()
    {
        client.Connect("localhost", 9050, "SomeConnectionKey");
    }
}
