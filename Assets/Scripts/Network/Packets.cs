using System.Collections;
using System.Collections.Generic;
using LiteNetLib.Utils;

namespace Network
{
    public abstract class ABasePacket
    {
        public int ClientID { get; set; }
    }
    #region Connection Handling
    public class ConnectionAttemptResult : ABasePacket, INetSerializable
    {
        public bool Accepted { get; set; }
        public string Message { get; set; }        
        public void Deserialize(NetDataReader reader)
        {
            Accepted = reader.GetBool();
            Message = reader.GetString();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Accepted);
            writer.Put(Message);
        }
    }
    #endregion
    #region Login & Authentication

    public class AuthenticationRequest : ABasePacket, INetSerializable
    {
        public string Username { get; set; }
        public string HashedWord { get; set; }

        public void Deserialize(NetDataReader reader)
        {
            Username = reader.GetString();
            HashedWord = reader.GetString();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Username);
            writer.Put(HashedWord);
        }
    }

    public class AuthenticationResult : ABasePacket, INetSerializable
    {
        public bool Result { get; set; }
        public string Message { get; set; }

        public void Deserialize(NetDataReader reader)
        {
            Result = reader.GetBool();
            Message = reader.GetString();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Result);
            writer.Put(Message);
        }
    }

    #endregion
    #region Lobby Handling    
    public class JoinLobbyRequestPacket : ABasePacket, INetSerializable
    {
        public void Deserialize(NetDataReader reader)
        {
            ClientID = reader.GetInt();
        }

        public void Serialize(NetDataWriter writer)
        {            
        }
    }

    public class JoinLobbyResponsePacket : ABasePacket, INetSerializable
    {
        public bool LobbyStarted { get; set; }
        public string Message { get; set; }

        public void Deserialize(NetDataReader reader)
        {
            LobbyStarted = reader.GetBool();
            Message = reader.GetString();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(LobbyStarted);
            writer.Put(Message);
        }
    }

    public class LobbyClosedResponse : ABasePacket, INetSerializable
    {
        public string Reason { get; set; }
        public void Deserialize(NetDataReader reader)
        {
            Reason = reader.GetString();
        }

        public void Serialize(NetDataWriter writer)
        {
            

        }
    }

    public class PlayerReadyRequest : ABasePacket
    {
    }

    public class AllPlayersReadyResponse : ABasePacket, INetSerializable
    {
        public void Deserialize(NetDataReader reader)
        {
        }
        public void Serialize(NetDataWriter writer)
        {
        }
    }

    public class DeathResponse : ABasePacket
    {

    }

    public class SwitchPhaseResponse : ABasePacket, INetSerializable
    {
        public int Timer { get; set; }
        public PlayerMatchData PlayerData { get; set; }
        public PlayerMatchData Enemy { get; set; }
        // 0 = buy - 1 = combat
        public int Phase { get; set; }

        public void Deserialize(NetDataReader reader)
        {
        }

        public void Serialize(NetDataWriter writer)
        {
        }
    }

    public class ActionListResponse : ABasePacket, INetSerializable
    {
        public ActionDataStructure[] ActionList { get; set; }
        public void Deserialize(NetDataReader reader)
        {
        }

        public void Serialize(NetDataWriter writer)
        {
        }
    }

    #region ShopPackets
    public class ShopListResponse : ABasePacket, INetSerializable
    {
        public string[] Minions { get; set; }

        public void Deserialize(NetDataReader reader)
        {
        }
        public void Serialize(NetDataWriter writer)
        {
        }
    }

    public class MinionBuyRequest : ABasePacket {
        public int MinionIndex { get; set; }
        public int SlotIndex { get; set; }
    }
    public class MinionBuyResponse : ABasePacket, INetSerializable {
        public bool Success { get; set; }
        public int UpdatedGold { get; set; }
        public int SlotIndex { get; set; }
        public PlayerMatchData UpdatedData { get; set; }

        public void Deserialize(NetDataReader reader)
        {
        }

        public void Serialize(NetDataWriter writer)
        {
        }
    }

    #endregion
    #endregion

    #region Debug
    public class TestPacket : ABasePacket, INetSerializable
    {
        public void Deserialize(NetDataReader reader)
        {
            
        }

        public void Serialize(NetDataWriter writer)
        {
            
        }
    }
    #endregion
}

[System.Serializable]
public struct ActionDataStructure
{
    public string Action;
    public int AttackerIndex, DefenderIndex;
    public string User, Target; // melee 0 - ranged 0 - flying 0
                                //   m_0       r_0        f_0

    public static ActionDataStructure GetAction(string action, int attacker, int defender, string user, string target)
    {
        ActionDataStructure a = new ActionDataStructure();
        a.Action = action;
        a.AttackerIndex = attacker;
        a.DefenderIndex = defender;
        a.User = user;
        a.Target = target;
        return a;
    }

    public static void Serialize(NetDataWriter writer, ActionDataStructure data)
    {
        writer.Put(data.Action);
        writer.Put(data.AttackerIndex);
        writer.Put(data.DefenderIndex);
        writer.Put(data.User);
        writer.Put(data.Target);
    }

    public static ActionDataStructure Deserialize(NetDataReader reader)
    {
        ActionDataStructure ret = new ActionDataStructure();
        ret.Action = reader.GetString();
        ret.AttackerIndex = reader.GetInt();
        ret.DefenderIndex = reader.GetInt();
        ret.User = reader.GetString();
        ret.Target = reader.GetString();
        return ret;
    }
}

[System.Serializable]
public struct MinionMatchData 
{
    public string MinionID;
    public int MinionAttack, MinionHP;
    public string[] Effects;

    public static void Serialize(NetDataWriter writer, MinionMatchData data)
    {
        writer.Put(data.MinionID);
        writer.Put(data.MinionAttack);
        writer.Put(data.MinionHP);
        if (data.Effects == null)
            data.Effects = new string[0];
        writer.Put(data.Effects.Length);
        for (int i = 0; i < data.Effects.Length; i++)
            writer.Put(data.Effects[i]);
    }

    public static MinionMatchData Deserialize(NetDataReader reader)
    {
        MinionMatchData m = new MinionMatchData();
        m.MinionID = reader.GetString();
        m.MinionAttack = reader.GetInt();
        m.MinionHP = reader.GetInt();
        int length = reader.GetInt();
        m.Effects = new string[length];
        for (int i = 0; i < length; i++)
            m.Effects[i] = reader.GetString();
        return m;
    }

    public static MinionMatchData GetClone(MinionMatchData original)
    {
        MinionMatchData m = new MinionMatchData();

        m.MinionID = original.MinionID;
        m.MinionAttack = original.MinionAttack;
        m.MinionHP = original.MinionHP;
        m.Effects = original.Effects;

        return m;

    }
}

[System.Serializable]
public class PlayerMatchData
{
    public int ClientID;
    public string PlayerName;
    public int HP;
    public int Gold = 6;

    public bool Dead = false;

    public bool Ready = false;

    public int CurrentTier = 1;

    public MinionMatchData[] CurrentMinionsInShop;
    public MinionMatchData[] MeleeMinions = new MinionMatchData[4];
    public MinionMatchData[] RangedMinions = new MinionMatchData[4];
    public MinionMatchData RightFlyingMinion, LeftFlyingMinion;

    public PlayerMatchData()
    {
        CurrentMinionsInShop = new MinionMatchData[0];
        MeleeMinions = new MinionMatchData[4];
        RangedMinions = new MinionMatchData[6];
        HP = 20;
    }

    public static void Serialize(NetDataWriter writer, PlayerMatchData  data) {
        if (data == null)
            return;
        writer.Put(data.ClientID);
        writer.Put(data.PlayerName);
        writer.Put(data.HP);
        writer.Put(data.Gold);
        writer.Put(data.Ready);
        writer.Put(data.CurrentTier);

        writer.Put(data.CurrentMinionsInShop.Length);
        foreach (MinionMatchData m in data.CurrentMinionsInShop)
            MinionMatchData.Serialize(writer, m);

        writer.Put(data.MeleeMinions.Length);
        foreach (MinionMatchData m in data.MeleeMinions)
            MinionMatchData.Serialize(writer, m);

        writer.Put(data.RangedMinions.Length);
        foreach (MinionMatchData m in data.RangedMinions)
            MinionMatchData.Serialize(writer, m);

        MinionMatchData.Serialize(writer, data.RightFlyingMinion);
        MinionMatchData.Serialize(writer, data.LeftFlyingMinion);
    }
    public static PlayerMatchData Deserialize(NetDataReader reader)
    {
        PlayerMatchData p = new PlayerMatchData();

        p.ClientID = reader.GetInt();
        p.PlayerName = reader.GetString();
        p.HP = reader.GetInt();
        p.Gold = reader.GetInt();
        p.Ready = reader.GetBool();
        p.CurrentTier = reader.GetInt();

        int inShopLength = reader.GetInt();
        p.CurrentMinionsInShop = new MinionMatchData[inShopLength];
        for(int i = 0; i < inShopLength; i++)
        {
            p.CurrentMinionsInShop[i] = MinionMatchData.Deserialize(reader);
        }

        int meleeLength = reader.GetInt();
        p.MeleeMinions = new MinionMatchData[meleeLength];
        for (int i = 0; i < meleeLength; i++)
        {
            p.MeleeMinions[i] = MinionMatchData.Deserialize(reader);
        }

        int rangedLength = reader.GetInt();
        p.RangedMinions = new MinionMatchData[rangedLength];
        for (int i = 0; i < rangedLength; i++)
        {
            p.RangedMinions[i] = MinionMatchData.Deserialize(reader);
        }

        p.RightFlyingMinion = MinionMatchData.Deserialize(reader);
        p.LeftFlyingMinion = MinionMatchData.Deserialize(reader);
        return p;
    }
}