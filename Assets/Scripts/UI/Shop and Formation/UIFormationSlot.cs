using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFormationSlot : UISlotEntry
{
    public MinionType Type;

    public override void Start()
    {
        base.Start();
        Empty();
    }

    public void Sell() { }

    public void SetMinion(MinionMatchData data)
    {
        ItemIcon.enabled = true;
        ItemIcon.sprite = Registry.instance.GetMinion(data.MinionID).Portrait;
    }
}
