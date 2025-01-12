using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public enum PickUpType
    {
        Health,
        Coin
    }
    
    public PickUpType pickUpType;
    public int value = 20;
    public ParticleSystem CollectedVFX;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GameObject().GetComponent<Character>().PickUpItem(this);

            if (CollectedVFX != null)
            {
                Instantiate(CollectedVFX, transform.position, Quaternion.identity);
            }
            
            Destroy(gameObject);
        }
    }
}
