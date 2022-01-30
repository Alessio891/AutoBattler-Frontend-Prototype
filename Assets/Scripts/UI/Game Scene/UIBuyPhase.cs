using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBuyPhase : ABaseCanvas
{
    public static UIBuyPhase instance;

    public GraphicRaycaster Raycaster;

    public List<UIShopSlot> ShopSlots;

    public List<UIFormationSlot> MeleeSlots, RangedSlots;
    public UIFormationSlot RightFlyingSlot, LeftFlyingSlot;

    public Image FillBar;
    public Text SecondsText, GoldText, HPText;
    public int startTime = 5;
    int currentTime = 5;

    protected override void Awake()
    {
        base.Awake();
        instance = this;
    }

    public override void Show()
    {
        base.Show();

        StartCoroutine(CountDown());
    }

    IEnumerator CountDown()
    {
        currentTime = startTime;
        FillBar.fillAmount = 1.0f;
        SecondsText.text = currentTime.ToString();
        while(currentTime > 0)
        {
            yield return new WaitForSeconds(1.0f);
            currentTime--;
            SecondsText.text = currentTime.ToString();
            float amount = (float)((float)currentTime / (float)startTime);
            FillBar.fillAmount = amount;
        }
        yield return null;
    }

    public void UpdateShopList(MinionMatchData[] minions)
    {
        for (int i = 0; i < ShopSlots.Count; i++)
        {
            if (i < minions.Length)
            {
                MinionMatchData s = minions[i];
                Debug.Log("Received minion " + s);
                BaseMinionData minion = Registry.instance.GetMinion(s.MinionID);
                if (minion != null)
                {
                    ShopSlots[i].ItemIcon.enabled = true;
                    ShopSlots[i].ItemIcon.sprite = minion.Portrait;
                }
            } else
            {
                ShopSlots[i].ItemIcon.enabled = false;
                ShopSlots[i].ItemIcon.sprite = null;
            }
        }
    }

    public void UpdateFormation(PlayerMatchData data)
    {
        HPText.text = "HP: " + data.HP.ToString();
        for(int i = 0; i < data.MeleeMinions.Length; i++)
        {
            if (!string.IsNullOrEmpty(data.MeleeMinions[i].MinionID))
            {
                MeleeSlots[i].SetMinion(data.MeleeMinions[i]);
            } else
            {
                MeleeSlots[i].Empty();
            }
        }

        for (int i = 0; i < data.RangedMinions.Length; i++)
        {
            if (!string.IsNullOrEmpty(data.RangedMinions[i].MinionID))
            {
                RangedSlots[i].SetMinion(data.RangedMinions[i]);
            }
            else
            {
                RangedSlots[i].Empty();
            }
        }

        if (!string.IsNullOrEmpty(data.RightFlyingMinion.MinionID))
        {
            RightFlyingSlot.SetMinion(data.RightFlyingMinion);
        }
        else
            RightFlyingSlot.Empty();

        if (!string.IsNullOrEmpty(data.LeftFlyingMinion.MinionID))
        {
            LeftFlyingSlot.SetMinion(data.LeftFlyingMinion);
        }
        else
            LeftFlyingSlot.Empty();
    }

    private void Start()
    {
        Hide();
    }
}
