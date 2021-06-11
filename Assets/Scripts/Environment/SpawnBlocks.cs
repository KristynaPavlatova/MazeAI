using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBlocks : MonoBehaviour
{
    public GameObject block;
    public int gridX;
    public int gridZ;
    public float offset = 1;
    public Vector3 origin = Vector3.zero;
    public Material[] materials = new Material[4];
    // Start is called before the first frame update
    void Start()
    {
        SpawnGrid();
    }
    private void SpawnGrid()
    {
        for (int x = 0; x < gridX; x++)
        {
            for (int z = 0; z < gridZ; z++)
            {
                Vector3 spawnPos = new Vector3(x * offset, 0, z * offset) + origin;
                GameObject clone = Instantiate(block, spawnPos, Quaternion.identity);
                clone.transform.SetParent(this.transform);
            }
        }
    }
}
