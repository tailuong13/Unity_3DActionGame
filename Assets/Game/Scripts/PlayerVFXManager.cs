using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerVFXManager : MonoBehaviour
{
    public VisualEffect FootStepVFX;
    public ParticleSystem Blade01;
    public VisualEffect SlashVFX;
    public VisualEffect HealthVFX;
    
    public void Update_FootStep(bool state)
    {
        if (state)
        {
            FootStepVFX.Play();
        }
        else
        {
            FootStepVFX.Stop();
        }
    }
    
    public void PlayBlade01()
    {
        Blade01.Play();
    }

    public void PlaySlash(Vector3 pos)
    {
        SlashVFX.transform.position = pos;
        SlashVFX.Play();
    }

    public void PlayHealthVFX()
    {
        HealthVFX.Play();
    }
}
