using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnInCube : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] BoxCollider bc;
    [SerializeField] BoxCollider bc2;
    [SerializeField] BoxCollider bc3;

    Vector3 cubeSize;
    Vector3 cubeCenter;

    Vector3 cube2Size;
    Vector3 cube2Center;

    Vector3 cube3Size;
    Vector3 cube3Center;


    private void Awake()
    {
        Transform cubeTrans = bc.GetComponent<Transform>();
        cubeCenter = cubeTrans.position;
        Transform cube2Trans = bc2.GetComponent<Transform>();
        cube2Center = cube2Trans.position;
        Transform cube3Trans = bc3.GetComponent<Transform>();
        cube3Center = cube3Trans.position;

        // Multiply by scale because it does affect the size of the collider
        cubeSize.x = cubeTrans.localScale.x * bc.size.x;
        cubeSize.y = cubeTrans.localScale.y * bc.size.y;
        cubeSize.z = cubeTrans.localScale.z * bc.size.z;

        cube2Size.x = cube2Trans.localScale.x * bc2.size.x;
        cube2Size.y = cube2Trans.localScale.y * bc2.size.y;
        cube2Size.z = cube2Trans.localScale.z * bc2.size.z;

        cube3Size.x = cube3Trans.localScale.x * bc3.size.x;
        cube3Size.y = cube3Trans.localScale.y * bc3.size.y;
        cube3Size.z = cube3Trans.localScale.z * bc3.size.z;
    }


    public void GameStart()
    {
        StartCoroutine(SpawnLoop());
    }

    private Vector3 GetRandomPosition()
    {
        // You can also take off half the bounds of the thing you want in the box, so it doesn't extend outside.
        // Right now, the center of the prefab could be right on the extents of the box
        int chosenBoxInd = Random.Range(1, 4);
        if (chosenBoxInd == 1)
        {
            Vector3 randomPosition = new Vector3(Random.Range(-cubeSize.x / 2, cubeSize.x / 2), Random.Range(-cubeSize.y / 2, cubeSize.y / 2), Random.Range(-cubeSize.z / 2, cubeSize.z / 2) - 5);
            return cubeCenter + randomPosition;
        }
        else if (chosenBoxInd == 2)
        {
            Vector3 randomPosition = new Vector3(Random.Range(-cube2Size.x / 2, cube2Size.x / 2) + 5, Random.Range(-cube2Size.y / 2, cube2Size.y / 2), Random.Range(-cube2Size.z / 2, cube2Size.z / 2)); //REMEMBER TO ADD DISPLACEMENT
            return cube2Center + randomPosition;
        }
        else
        {
            Vector3 randomPosition = new Vector3(Random.Range(-cube3Size.x / 2, cube3Size.x / 2), Random.Range(-cube3Size.y / 2, cube3Size.y / 2), Random.Range(-cube3Size.z / 2, cube3Size.z / 2) + 5);
            return cube3Center + randomPosition;
        }
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            GameObject go = Instantiate(prefab, GetRandomPosition(), Quaternion.identity);
            yield return new WaitForSeconds(2);
        }
    }
}