using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EnemyVFXManager : MonoBehaviour
{
    public VisualEffect FootStepVFX;
    public VisualEffect AttackVFX;
    public ParticleSystem BeingHitVFX;
    public VisualEffect BeingHitSplashVFX;
    
    public void PlayAttackVFX()
    {
        AttackVFX.Play();
    }
    
    public void BurstFootStep()
    {
        FootStepVFX.Play();
    }
    
    public void PlayBeingHitVFX(Vector3 attackerPos)
    {
        Vector3 forceForward = transform.position - attackerPos;
        forceForward.Normalize();
        forceForward.y = 0;
        BeingHitVFX.transform.rotation = Quaternion.LookRotation(forceForward);
        BeingHitVFX.Play();

        Vector3 splashPos = transform.position;
        splashPos.y += 2f;
        VisualEffect newSplash = Instantiate(BeingHitSplashVFX, splashPos, Quaternion.identity);
        newSplash.Play(); 
        Destroy(newSplash.gameObject, 10f);
    }
    
}
