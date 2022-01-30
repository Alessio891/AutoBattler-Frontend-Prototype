using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class ABaseCanvas : MonoBehaviour
{
    CanvasGroup grp;
    protected virtual void Awake()
    {
        grp = GetComponent<CanvasGroup>();
    }

    public virtual void Show()
    {
        grp.alpha = 1.0f;
        grp.interactable = true;
        grp.blocksRaycasts = true;
    }
    public virtual void Hide()
    {
        grp.alpha = 0.0f;
        grp.interactable = false;
        grp.blocksRaycasts = false;
    }
}
