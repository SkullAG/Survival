using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;

public class WorldManager : MonoBehaviour
{
	public List<Chunk> chunks = new List<Chunk>();

	// Start is called before the first frame update
	void Start()
	{
		Debug.Log(UnsafeUtility.SizeOf(typeof(WorldSaver.BlockFileData)));
		//Debug.Log(typeof(WorldSaver.ChunkFileData));

		Debug.Log(sizeof(byte) * 3 + sizeof(short));
	}

	// Update is called once per frame
	void Update()
	{
		
	}

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
