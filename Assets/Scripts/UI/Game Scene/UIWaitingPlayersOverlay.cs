using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWaitingPlayersOverlay : ABaseCanvas
{
    public static UIWaitingPlayersOverlay instance;
    protected override void Awake()
    {
        base.Awake();
        instance = this;
    }

    private void Start()
    {
        Show();
    }
}
