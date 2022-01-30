using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class MinionDataStructure
{
    public string ID;
    public string Name, Description, Sprite, Graphic, Type;

    public int Attack, MaxHP, Tier;
}
[System.Serializable]
public class GeneralDataStructure
{
    public string ID;
    public string Name, Description, Sprite;    
}
[System.Serializable]
public class ResearchDataStructure
{
    public string ID;
    public string Name, Sprite;
    public int Tier;    
}