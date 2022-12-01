using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartTriggerScript : MonoBehaviour
{
    public SpawnInCube spawner;
    public GameObject notice;
    public GameObject table;

    private void OnTriggerEnter(Collider other)
    {
        spawner.GameStart();
        Destroy(notice);
        Destroy(table);
        Destroy(transform.parent.gameObject);
    }
}
