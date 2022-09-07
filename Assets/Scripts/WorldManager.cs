using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;

public class WorldManager : MonoBehaviour
{
	public Dictionary<Vector3Int,Chunk> chunks = new Dictionary<Vector3Int, Chunk>();
	public List<ChunkLoaderEntity> chunkLoaders = new List<ChunkLoaderEntity>();

	List<Vector3Int> toLoadChuncks = new List<Vector3Int>();

	public DimensionData dimension;

	public static WorldManager Instance;

	private void Awake()
	{
		// If there is an instance, and it's not me, delete myself.

		if (Instance != null && Instance != this)
		{
			Destroy(this);
		}
		else
		{
			Instance = this;
		}
	}

	// Start is called before the first frame update
	void Start()
	{
		//Debug.Log(UnsafeUtility.SizeOf(typeof(WorldSaver.BlockFileData)));
		//Debug.Log(typeof(WorldSaver.ChunkFileData));

		//Debug.Log(sizeof(byte) * 3 + sizeof(short));
		Debug.Log(System.GC.GetTotalMemory(true));
	}

	public void AddChunkLoader(ChunkLoaderEntity cle)
    {
		chunkLoaders.Add(cle);

	}

	public void RemoveChunkLoader(ChunkLoaderEntity cle)
	{
		chunkLoaders.Remove(cle);

	}

	// Update is called once per frame
	void FixedUpdate()
	{
		
	}

	void CheckChunkLoaders()
    {
		for (int index = 0; index < chunkLoaders.Count; index++)
        {
			if(chunkLoaders[index].isActiveAndEnabled)
            {
				/*for(chunkLoaders[index])
				{
					chunks.
				}
				toLoadChuncks*/
            }
        }
    }

	/*public WorldSaver.BlockFileData GetPrimitiveBlockData(Vector3Int blockPos)
    {

    }*/

	/*public BlockData getBlockAt(Vector3 blockPos)
	{
		chunks.
	}

	public bool hasTransparentNeighbours(Vector3Int blockPos)
	{

	}*/

	/*private void OnRenderObject()
	{
		Debug.Log("hello");

		//Graphics.DrawMesh
	}*/
}
