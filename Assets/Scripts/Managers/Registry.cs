using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class Registry : MonoBehaviour
{
    public static Registry instance;
    private void Awake()
    {
        instance = this;
    }

    public BaseMinionData GetMinion(string id)
    {
        Debug.Log("Searching for minion " + id);
        foreach(MinionRegistryEntry e in Minions)
        {
            if (e.ID == id)
                return e.Minion;
        }
        Debug.Log(id + " not found");
        return null;
    }
#if UNITY_EDITOR
    public void RegisterMinion(string id, BaseMinionData data)
    {
        foreach(MinionRegistryEntry e in Minions)
        {
            if (e.ID == id)
            {
                e.Minion = data;
                return;
            }
        }

        MinionRegistryEntry entry = new MinionRegistryEntry();
        entry.ID = id;
        entry.Minion = data;
        Minions.Add(entry);        
    }
#endif
    public List<MinionRegistryEntry> Minions;
    
}

[System.Serializable]
public class MinionRegistryEntry
{
    public string ID;
    public BaseMinionData Minion;
}
