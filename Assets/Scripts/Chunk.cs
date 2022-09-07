using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class Chunk : MonoBehaviour
{
	public static Vector3Int size = new Vector3Int(16, 16, 16);
	//public BlockData blockPrefab;

	public short[] instantitatedblocks;

	//public BlockData[,,] blocks = new BlockData[size.x, size.y, size.z];

	//private List<WorldSaver.BlockFileData> primitiveBlockData;
	private Dictionary<short, short> primitiveBlockData;

	public WorldManager worldManager;
	//public Mesh mesh;

	public static int VecToInt(Vector3Int v) { return VecToInt(v.x, v.y, v.z); }
	public static int VecToInt(int x, int y, int z) { return x + (y * size.x) + (z * size.x * size.y); }

	public static Vector3Int IntToVec(int i) { return new Vector3Int(i % size.x, i / size.x % size.y, i / (size.x * size.y) % size.z); }

	private void Awake()
	{
		
		
	}

	// Start is called before the first frame update
	void Start()
	{
		worldManager = WorldManager.Instance;

		instantitatedblocks = new short[size.x * size.y * size.z];

		//UpdateAllBlocks();
		//UpdateAllBlocksAsync(20*20*20);
		UpdateAllBlocksAsync(1);



		//StartCoroutine(repeatTest(20 * 20 * 20));
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	/*public BlockData GetBlock(Vector3Int coord)
	{
		//Vector3Int coord = Vector3Int.FloorToInt(_coord);

		// falta hacer

		return blocks[coord.x, coord.y, coord.z];
	}*/

	public void LoadChunk()
	{
		primitiveBlockData = WorldSaver.ReadChunckData(this, WorldManager.Instance.dimension.name).ToDictionary();
	}

	public float UpdateAllBlocks()
	{
		Stopwatch watch = Stopwatch.StartNew();
		watch.Start();

		short bid = -1;
		short index = 0;
		Vector3Int blockPos = Vector3Int.zero;

		//separar carga de chuncks en distintos frames hara que cargen sin problemas

		for (blockPos.x = 0; blockPos.x < size.x; blockPos.x++)
		{
			for (blockPos.z = 0; blockPos.z < size.z; blockPos.z++)
			{
				for (blockPos.y = 0; blockPos.y < size.y; blockPos.y++)
				{
					index = (short)VecToInt(blockPos);
					//Stopwatch watch = Stopwatch.StartNew();
					//watch.Start();
					//index = -1;

					if (!primitiveBlockData.TryGetValue(index, out bid))
					{

					}

                    //UnityEngine.Debug.Log(index + ", " + instantitatedblocks.Length);
					instantitatedblocks[index] = bid;

					//watch.Stop();
					//UnityEngine.Debug.Log("block:" + (transform.position + blockPos) + " load took: " + watch.ElapsedMilliseconds);
				}
			}
		}


		watch.Stop();
		//UnityEngine.Debug.Log("Chunk:" + (transform.position / 16) + " load took: " + watch.ElapsedMilliseconds);
		return watch.ElapsedMilliseconds;
	}

	public async void UpdateAllBlocksAsync(int times)
	{
		float time = Time.time;

		WorldSaver.BlockFileData a = new WorldSaver.BlockFileData();

		primitiveBlockData = new Dictionary<short, short>();

		for (short num = 0; num < (size.x * size.z * size.y); num++)
		{
			primitiveBlockData.Add(num, a.id);
		}

		for (int i = 0; i < times; i++)
		{
			float b = await Task.Run(UpdateAllBlocks);

			//UnityEngine.Debug.Log("Chunk:" + (transform.position / 16) + " load took: " + a + " total: " + (Time.time - time));
			//Task.
			//yield return new WaitForEndOfFrame();
		}

        UnityEngine.Debug.Log(times + " chuncks loaded in " + (Time.time - time) + "s");
	}

	IEnumerator repeatTest(int times)
    {
		for(int i = 0; i < times; i++)
        {
			UpdateAllBlocks();
			yield return new WaitForEndOfFrame();
        }
    }

	/*public int GetBlockPrimitiveDataIndex(Vector3 pos)
	{
		//UnityEngine.Debug.Log("hola?");

		for (int i = 0; i < primitiveBlockData.Count; i++)
		{
			//UnityEngine.Debug.Log("hola?");
			if (pos.x == primitiveBlockData[i].x && pos.y == primitiveBlockData[i].y && pos.z == primitiveBlockData[i].z)
				return i;
		}

		return -1;
	}*/

	public void RewriteMesh()
	{
		
	}
}
