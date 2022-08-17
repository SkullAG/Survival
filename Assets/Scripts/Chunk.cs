using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public Vector3 dimensions = new Vector3(16, 384, 16);
    public BlockData blockPrefab;

    public List<BlockData> blocks = new List<BlockData>();

    private void Awake()
    {
        generate();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public BlockData getBlock(Vector3 _coord)
    {
        Vector3Int coord = Vector3Int.FloorToInt(_coord);

        // falta hacer

        return blocks[blocks.Count - 1];
    }

    public void generate()
    {
        for(int num = 0; num <= (dimensions.x * dimensions.z * dimensions.y); num ++)
        {
            int x = Mathf.FloorToInt(num % dimensions.x);
            int y = Mathf.FloorToInt(num / dimensions.x % dimensions.y);
            int z = Mathf.FloorToInt(num / dimensions.x / dimensions.y);

            blocks.Add(Instantiate(blockPrefab.gameObject, new Vector3(x, y, z), Quaternion.identity, transform).GetComponent<BlockData>());
        }
    }
}
