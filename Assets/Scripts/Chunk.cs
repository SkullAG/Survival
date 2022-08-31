using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public Vector3 dimensions = new Vector3(16, 384, 16);
    public BlockData blockPrefab;

    public List<BlockData> blocks = new List<BlockData>();

    public WorldManager worldManager;
    //public Mesh mesh;


    private void Awake()
    {
        worldManager = GetComponentInParent<WorldManager>();
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
        Vector3Int blockPos = Vector3Int.zero;
        for(int num = 0; num <= (dimensions.x * dimensions.z * dimensions.y); num ++)
        {
            blockPos.x = Mathf.FloorToInt(transform.position.x + num % dimensions.x);
            blockPos.y = Mathf.FloorToInt(transform.position.y + num / dimensions.x % dimensions.y);
            blockPos.z = Mathf.FloorToInt(transform.position.z + num / dimensions.x / dimensions.y);

            //blocks.Add(Instantiate(blockPrefab.gameObject, blockPos, Quaternion.identity, transform).GetComponent<BlockData>());
        }
    }

    

    

    public void rewriteMesh()
    {

    }
}
