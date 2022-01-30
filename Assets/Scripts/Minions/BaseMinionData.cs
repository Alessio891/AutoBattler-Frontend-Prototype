using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;

#if UNITY_EDITOR
using Network;
using UnityEditor;
#endif

public class BaseMinionData : ScriptableObject
{
    public string UID;

    public string Name;
    [Multiline(5)]
    public string Description;

    [Range(1, 5)]
    public int Tier = 1;

    public Sprite Portrait;

    public int Attack, MaxHP;
    [HideInInspector]
    public int CurrentHP;

    public MinionType Type;

    public MinionGraphicComponent GraphicPrefab;

#if UNITY_EDITOR
    [MenuItem("Assets/Create/Minion Data/Basic Minion Data")]
    public static void CreateAsset()
    {
        BaseMinionData asset = ScriptableObject.CreateInstance<BaseMinionData>();
        AssetDatabase.CreateAsset(asset, "Assets/Data/Minions/New Minion Data.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;

    }
   
    public void LoadJsonData(string id, Dictionary<string,object> data)
    {
        UID = id;
        Name = data["Name"].ToString();
        Description = data["Description"].ToString();
        MaxHP = int.Parse(data["MaxHP"].ToString());
        Attack = int.Parse(data["Attack"].ToString());
        Tier = int.Parse(data["Tier"].ToString());
        string spritePath = data["Sprite"].ToString();
        string gfxPath = data["Graphic"].ToString();

        Portrait = Resources.Load<Sprite>(spritePath);
        GraphicPrefab = Resources.Load<MinionGraphicComponent>(gfxPath);

        Type = (MinionType)System.Enum.Parse(typeof(MinionType), data["Type"].ToString());

    }
    
    [MenuItem("Assets/Load Minions from JSON")]
    static void LoadFromJSON()
    {
        string path = Application.dataPath + "/Data/JSON/Minions";
        Registry r = (Registry)AssetDatabase.LoadAssetAtPath<Registry>("Assets/Prefabs/System/[Registry].prefab");
        AssetDatabase.StartAssetEditing();
        foreach (var f in System.IO.Directory.GetFiles(path))
        {            
            if (f.EndsWith(".json"))
            {
                Dictionary<string, object> data = (Dictionary<string,object>)MiniJSON.Json.Deserialize(System.IO.File.ReadAllText(f));
                foreach(KeyValuePair<string, object> pair in data) 
                {
                    
                    string assetPath = Application.dataPath + "/Data/Minions/" + pair.Key + ".asset";
                    if (System.IO.File.Exists(assetPath))
                    {
                        BaseMinionData minion = AssetDatabase.LoadAssetAtPath<BaseMinionData>("Assets/Data/Minions/"+pair.Key+".asset");
                        if (minion != null)
                        {
                            minion.LoadJsonData(pair.Key, (Dictionary<string, object>)pair.Value);
                            r.RegisterMinion(pair.Key, minion);
                        }
                    } else
                    {
                        BaseMinionData asset = ScriptableObject.CreateInstance<BaseMinionData>();                        
                        asset.LoadJsonData(pair.Key, (Dictionary<string, object>)pair.Value);                        
                        AssetDatabase.CreateAsset(asset, "Assets/Data/Minions/"+pair.Key+".asset");
                        r.RegisterMinion(pair.Key, asset);
                    }
                }
            }
        }
        AssetDatabase.StopAssetEditing();
        AssetDatabase.SaveAssets();
    }
#endif

}


[System.Flags]
public enum MinionAttribute
{
    None = 0,
    Flying = 1,
    Assassin = 2,   // One hit kill
    Ranged = 4      // Can hit flying
}