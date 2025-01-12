using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Spawner : MonoBehaviour
{
    private List<SpawnPoint> _spawnPointList;
    private List<Character> _spawnedCharacterList;
    private bool _hasSpawned;
    public Collider spawnCollider;
    public UnityEvent OnAllEnemiesDead;
    
    private void Awake()
    {
        var spawnPointArray = transform.parent.GetComponentsInChildren<SpawnPoint>();
        _spawnPointList = new List<SpawnPoint>(spawnPointArray);
        _spawnedCharacterList = new List<Character>();
    }

    private void Update()
    {
        if(!_hasSpawned || _spawnedCharacterList.Count == 0)
        {
            return;
        }
        
        bool allSpawnedAreDead = true;
        
        foreach(Character c in _spawnedCharacterList)
        {
            if (c.CurrentState != Character.CharacterState.Dead)
            {
                allSpawnedAreDead = false;
                break;
            }
        }

        if (allSpawnedAreDead)
        {
            if(OnAllEnemiesDead != null)
            {
                OnAllEnemiesDead.Invoke();
            }
            _spawnedCharacterList.Clear();  
        }
    }

    public void SpawnCharacter()
    {
        if (_hasSpawned)
        {
            return;
        }
        
        _hasSpawned = true;
        
        foreach(SpawnPoint point in _spawnPointList)
        {
            if (point.EnemyToSpawn != null)
            {
                GameObject spawnGameObject = Instantiate(point.EnemyToSpawn, point.transform.position, Quaternion.identity);
                _spawnedCharacterList.Add(spawnGameObject.GetComponent<Character>());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            SpawnCharacter();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position,spawnCollider.bounds.size);
    }
}
