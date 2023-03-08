using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Network;

public class UIShopSlot : UISlotEntry
{
    public MinionType Type;

    public override void Start()
    {
        base.Start();
        Empty();
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        UIBuyPhase.instance.Raycaster.Raycast(eventData, results);
        bool dropSlotFound = false;
        foreach(RaycastResult r in results)
        {
            
            
            if (r.gameObject.tag == "FormationArea")
            {                
                dropSlotFound = true;
                Debug.Log("buying minion");
                ConnectionManager.SendPacketWithCallback<MinionBuyRequest, MinionBuyResponse>(
                        new MinionBuyRequest() { 
                            ClientID = ConnectionManager.instance.clientId, 
                            MinionIndex = UIBuyPhase.instance.ShopSlots.IndexOf(this)
                        },
                        (packet) =>
                        {
                            if (packet.Success)
                            {
                                UIFormationSlot formationSlot = UIBuyPhase.instance.MeleeSlots[packet.SlotIndex];
                                formationSlot.ItemIcon.sprite = ItemIcon.sprite;
                                formationSlot.ItemIcon.enabled = true;
                                UIBuyPhase.instance.GoldText.text = packet.UpdatedGold.ToString() + "/6";
                                UIBuyPhase.instance.UpdateShopList(packet.UpdatedData.CurrentMinionsInShop);
                                UIBuyPhase.instance.UpdateFormation(packet.UpdatedData);
                            }        
                        }
                        );
            }
        }

        
        base.OnEndDrag(eventData);
    }   
}
