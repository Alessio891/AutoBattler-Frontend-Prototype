using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaitingLobbyOverlay : ABaseCanvas
{

    public Text Text;
    public bool WaitingEnd = false;
    private void Start()
    {
        Hide();
    }

    public void StartAnimation()
    {
        Show();
        StartCoroutine(AnimateWaitText());
    }
    public void StopWaiting()
    {
        Hide();
        WaitingEnd = true;
    }
    IEnumerator AnimateWaitText()
    {
        string text = "Waiting for lobby";
        int count = 1;
        WaitForSeconds wait = new WaitForSeconds(0.5f);
        WaitingEnd = false;
        while(!WaitingEnd)
        {
            Text.text = text;
            text = "Waiting for lobby";
            for (int i = 0; i < count; i++)
                text += ".";
            count++;
            if (count > 3)
                count = 0;
            yield return wait;
        }
    }
}
