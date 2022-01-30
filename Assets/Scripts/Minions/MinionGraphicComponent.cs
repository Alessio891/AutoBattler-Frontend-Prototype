using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionGraphicComponent : MonoBehaviour
{
    
    public virtual IEnumerator AttackAnimation(MinionGraphicComponent target) {
        iTween.PunchPosition(gameObject, transform.forward * 0.9f, 1.0f);
        yield return new WaitForSeconds(0.5f);
        if (target != null)
            target.GetHitAnimation();
        yield return new WaitForSeconds(1.0f);        
    }

    public void GetHitAnimation() {
        iTween.PunchScale(gameObject, new Vector3(0.3f, 0.3f, 0.3f), 1.0f);
    }

    public void DieAnimation() {
        iTween.ScaleTo(gameObject, Vector3.zero, 0.5f);
    }
}
