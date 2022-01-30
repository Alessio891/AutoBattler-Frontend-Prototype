using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network;
using LiteNetLib.Utils;
using LiteNetLib;
using UnityEngine.SceneManagement;

public class MatchManager : MonoBehaviour
{    

    
    public static MatchManager instance;

    public List<MinionGraphicComponent> PlayerMelees = new List<MinionGraphicComponent>();
    public List<MinionGraphicComponent> PlayerRanged = new List<MinionGraphicComponent>();
    MinionGraphicComponent PlayerLeftFlying, PlayerRightFlying, EnemyLeftFlying, EnemyRightFlying;
    public List<MinionGraphicComponent> EnemyMelees = new List<MinionGraphicComponent>();
    public List<MinionGraphicComponent> EnemyRanged = new List<MinionGraphicComponent>();    

    public List<Transform> PlayerMeleePositions = new List<Transform>();
    public List<Transform> PlayerRangedPositions = new List<Transform>();
    public Transform PlayerLeftFlyingPosition, PlayerRightFlyingPosition;

    public List<Transform> EnemyMeleePositions = new List<Transform>();
    public List<Transform> EnemyRangedPositions = new List<Transform>();
    public Transform EnemyLeftFlyingPosition, EnemyRightFlyingPosition;

    protected virtual void Awake()
    {
        instance = this;       
    }

    protected virtual void Start()
    {
        ConnectionManager.instance.SubscribePacket<AllPlayersReadyResponse>(OnAllPlayersReady);
        ConnectionManager.instance.SubscribePacket<SwitchPhaseResponse>(OnPhaseChange);
        ConnectionManager.instance.SubscribePacket<ShopListResponse>(OnShopListResponse);
        ConnectionManager.instance.SubscribePacket<DeathResponse>(OnLostMatch);
        Debug.Log("Scene loaded");
        StartCoroutine(DelayedPacketSend());
    }

    IEnumerator DelayedPacketSend()
    {
        yield return null;
        yield return null;
        ConnectionManager.instance.netProcessor.Send(ConnectionManager.ServerPeer, new PlayerReadyRequest() { ClientID = ConnectionManager.instance.clientId }, DeliveryMethod.ReliableOrdered);
    }

    protected virtual void Update()
    {
       
    }   

    void OnLostMatch(DeathResponse packet, NetPacketProcessor proc, NetManager client)
    {
        SceneManager.LoadScene("MainMenu");
    }

    void OnShopListResponse(ShopListResponse packet, NetPacketProcessor processor, NetManager client)
    { }

    void OnAllPlayersReady(AllPlayersReadyResponse packet, NetPacketProcessor processor, NetManager client)
    {
        UIWaitingPlayersOverlay.instance.Hide();        
    }

    void OnPhaseChange(SwitchPhaseResponse packet, NetPacketProcessor processor, NetManager client)
    {
        switch(packet.Phase)
        {
            case 0:
                UIBuyPhase.instance.GoldText.text = packet.PlayerData.Gold.ToString() + "/" + packet.PlayerData.Gold.ToString();
                UIBuyPhase.instance.UpdateShopList(packet.PlayerData.CurrentMinionsInShop);
                UIBuyPhase.instance.startTime = packet.Timer;
                UIBuyPhase.instance.Show();
                break;
            case 1:
                UIBuyPhase.instance.Hide();
                PopulatePlayerArea(packet.PlayerData);
                PopulateEnemyArea(packet.Enemy);
                Debug.Log("Your enemy is " + packet.Enemy.PlayerName);
                StartCoroutine(BattleRoutine(packet.PlayerData, packet.Enemy));
                break;
        }
    }

    public void PopulatePlayerArea(PlayerMatchData player)
    {
        foreach(MinionGraphicComponent m in PlayerMelees)
        {
            Destroy(m.gameObject);
        }
        foreach (MinionGraphicComponent m in PlayerRanged)
            Destroy(m.gameObject);
        PlayerRanged.Clear();
        PlayerMelees.Clear();
        if (PlayerLeftFlying != null)
            Destroy(PlayerLeftFlying.gameObject);
        if (PlayerRightFlying != null)
            Destroy(PlayerRightFlying.gameObject);

        for (int i = 0; i < player.MeleeMinions.Length; i++)
        {
            MinionMatchData m = player.MeleeMinions[i];
            BaseMinionData minionData = Registry.instance.GetMinion(m.MinionID);
            if (minionData != null)
            {
                MinionGraphicComponent p = GameObject.Instantiate<MinionGraphicComponent>(minionData.GraphicPrefab);
                p.transform.position = PlayerMeleePositions[i].position;
                p.transform.forward= PlayerMeleePositions[i].forward;
                PlayerMelees.Add(p);
            }
        }
        for (int i = 0; i < player.RangedMinions.Length; i++)
        {
            MinionMatchData m = player.RangedMinions[i];
            BaseMinionData minionData = Registry.instance.GetMinion(m.MinionID);
            if (minionData != null)
            {
                MinionGraphicComponent p = GameObject.Instantiate<MinionGraphicComponent>(minionData.GraphicPrefab);
                p.transform.position = PlayerRangedPositions[i].position;
                p.transform.forward = PlayerRangedPositions[i].forward;
                PlayerRanged.Add(p);
            }
        }

        BaseMinionData rightFlying = Registry.instance.GetMinion(player.RightFlyingMinion.MinionID);
        if (rightFlying != null)
        {
            PlayerRightFlying = GameObject.Instantiate<MinionGraphicComponent>(rightFlying.GraphicPrefab);
            PlayerRightFlying.transform.position = PlayerRightFlyingPosition.position;
        }
        BaseMinionData leftFlying = Registry.instance.GetMinion(player.LeftFlyingMinion.MinionID);
        if (leftFlying != null)
        {
            PlayerLeftFlying = GameObject.Instantiate<MinionGraphicComponent>(leftFlying.GraphicPrefab);
            PlayerLeftFlying.transform.position = PlayerLeftFlyingPosition.position;
        }
    }
    public void PopulateEnemyArea(PlayerMatchData player)
    {
        foreach (MinionGraphicComponent m in EnemyMelees)
        {
            Destroy(m.gameObject);
        }
        foreach (MinionGraphicComponent m in EnemyRanged)
            Destroy(m.gameObject);

        EnemyRanged.Clear();
        EnemyMelees.Clear();
        if (EnemyLeftFlying != null)
            Destroy(EnemyLeftFlying.gameObject);
        if (EnemyRightFlying != null)
            Destroy(EnemyRightFlying.gameObject);

        for (int i = 0; i < player.MeleeMinions.Length; i++)
        {
            MinionMatchData m = player.MeleeMinions[i];
            BaseMinionData minionData = Registry.instance.GetMinion(m.MinionID);
            if (minionData != null)
            {
                MinionGraphicComponent p = GameObject.Instantiate<MinionGraphicComponent>(minionData.GraphicPrefab);
                p.transform.position = EnemyMeleePositions[i].position;
                p.transform.forward = EnemyMeleePositions[i].forward;
                EnemyMelees.Add(p);
            }
        }

        for (int i = 0; i < player.RangedMinions.Length; i++)
        {
            MinionMatchData m = player.RangedMinions[i];
            BaseMinionData minionData = Registry.instance.GetMinion(m.MinionID);
            if (minionData != null)
            {
                MinionGraphicComponent p = GameObject.Instantiate<MinionGraphicComponent>(minionData.GraphicPrefab);
                p.transform.position = EnemyRangedPositions[i].position;
                p.transform.forward = EnemyRangedPositions[i].forward;
                EnemyRanged.Add(p);
            }
        }

        BaseMinionData rightFlying = Registry.instance.GetMinion(player.RightFlyingMinion.MinionID);
        if (rightFlying != null)
        {
            EnemyRightFlying = GameObject.Instantiate<MinionGraphicComponent>(rightFlying.GraphicPrefab);
            EnemyRightFlying.transform.position = EnemyRightFlyingPosition.position;
        }
        BaseMinionData leftFlying = Registry.instance.GetMinion(player.LeftFlyingMinion.MinionID);
        if (leftFlying != null)
        {
            EnemyLeftFlying = GameObject.Instantiate<MinionGraphicComponent>(leftFlying.GraphicPrefab);
            EnemyLeftFlying.transform.position = EnemyLeftFlyingPosition.position;
        }
    }    

    void ResolveAttack(ref MinionMatchData attackerMinion, ref MinionMatchData victimMinion,
                              MinionGraphicComponent attackerGFX, MinionGraphicComponent victimGFX, 
                              List<MinionGraphicComponent> AttackerMinions,
                              List<MinionGraphicComponent> VictimMinions,
                              bool twoWays = true)
    {
        attackerMinion.MinionHP -= victimMinion.MinionAttack;
        if (twoWays)
            victimMinion.MinionHP -= attackerMinion.MinionAttack;

        if (twoWays)
        {
            if (attackerMinion.MinionHP <= 0)
            {
                attackerGFX.DieAnimation();
                AttackerMinions.Remove(attackerGFX);
                Destroy(attackerGFX.gameObject);
            }
        }

        if (victimMinion.MinionHP <= 0)
        {
            victimGFX.DieAnimation();
            VictimMinions.Remove(victimGFX);
            Destroy(victimGFX.gameObject);
        }
    }

    IEnumerator BattleRoutine(PlayerMatchData player, PlayerMatchData enemy)
    {

        while(true)
        {
            if (PlayerMelees.Count > 0 && EnemyMelees.Count > 0)
            {
                // Both players have melees. Resolve front most melees attacks
                MinionGraphicComponent playerMinion = PlayerMelees[0];
                MinionGraphicComponent enemyMinion = EnemyMelees[0];

                yield return StartCoroutine(playerMinion.AttackAnimation(enemyMinion));
                yield return StartCoroutine(enemyMinion.AttackAnimation(playerMinion));

                ResolveAttack(ref player.MeleeMinions[0], ref enemy.MeleeMinions[0], 
                              playerMinion, enemyMinion, PlayerMelees, EnemyMelees);

            } else if (PlayerMelees.Count > 0)
            {
                if (EnemyRanged.Count > 0)
                {
                    // Only local player has melees, enemy has ranged. Move melees in ranged range
                    MinionGraphicComponent playerMinion = PlayerMelees[0];
                    iTween.MoveTo(playerMinion.gameObject, EnemyRangedPositions[0].position - playerMinion.transform.forward * 2.5f, 2.5f);
                    yield return new WaitForSeconds(1.5f);
                    MinionGraphicComponent enemyMinion = EnemyRanged[0];
                    yield return StartCoroutine(playerMinion.AttackAnimation(enemyMinion));

                    ResolveAttack(ref player.MeleeMinions[0], ref enemy.RangedMinions[0],
                                  playerMinion, EnemyRanged[0], PlayerMelees, EnemyRanged, false);
                   
                }
            } else if (EnemyMelees.Count > 0)
            {
                if (PlayerRanged.Count > 0)
                {
                    // Only enemy player has melees, local player has ranged. Move melees in ranged range
                    MinionGraphicComponent enemyMinion = EnemyMelees[0];
                    iTween.MoveTo(enemyMinion.gameObject, PlayerRangedPositions[0].position - enemyMinion.transform.forward * 2.5f, 2.5f);
                    yield return new WaitForSeconds(1.5f);
                    MinionGraphicComponent playerMinion = PlayerRanged[0];
                    yield return StartCoroutine(enemyMinion.AttackAnimation(playerMinion));

                    ResolveAttack(ref enemy.MeleeMinions[0], ref player.RangedMinions[0],
                                  enemyMinion, playerMinion, EnemyMelees, PlayerRanged, false);
                }
            }
            // Resolve ranged attacks
            for (int i = 0; i < 6; i++)
            {
                if (PlayerRanged.Count > 0)
                {
                    // Player has ranged, prioritize flying then melee

                    if (EnemyLeftFlying != null)
                    {

                    }

                    if (EnemyMelees.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(player.RangedMinions[i].MinionID))
                        {
                            MinionGraphicComponent enemyMinion = EnemyMelees[0];
                            yield return StartCoroutine(PlayerRanged[i].AttackAnimation(EnemyMelees[0]));

                            ResolveAttack(ref player.RangedMinions[i], ref enemy.MeleeMinions[0],
                                          PlayerRanged[i], EnemyMelees[0], PlayerRanged, EnemyMelees, false);
                        }
                    }
                }

                if (PlayerMelees.Count > 0 && EnemyRanged.Count > 0)
                {
                    if (!string.IsNullOrEmpty(enemy.RangedMinions[i].MinionID))
                    {
                        MinionGraphicComponent playerMinion = PlayerMelees[0];
                        yield return StartCoroutine(EnemyRanged[i].AttackAnimation(PlayerMelees[0]));

                        ResolveAttack(ref enemy.RangedMinions[i], ref player.MeleeMinions[0],
                            EnemyRanged[i], playerMinion, EnemyRanged, PlayerMelees, false);
                    }
                }
            }

            if (
                (PlayerMelees.Count <= 0 && PlayerRanged.Count <= 0) ||
                (EnemyMelees.Count <= 0 && EnemyRanged.Count <= 0)
                )
            {
                Debug.Log("Attack finished");
                ConnectionManager.instance.netProcessor.Send<PlayerReadyRequest>(ConnectionManager.ServerPeer,
                    new PlayerReadyRequest() { ClientID = ConnectionManager.instance.clientId }, DeliveryMethod.ReliableOrdered);
                break;
            }

            yield return null;
        }

        yield return null;
    }
}
