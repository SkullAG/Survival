using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using System.Threading.Tasks;
using System.Threading;
using Unity.Burst;
using Unity.Jobs;
using System;
//using System.Diagnostics;

public class WorldManager : MonoBehaviour
{
	//private Queue<System.Action> actions = new Queue<System.Action>();

	//public Event.Action

	public int maxChuncksLoadedPerUpdate = 5;
	//private Vector3*[] clermin = new Vector3*[0];
	//private Vector3*[] clermax = new Vector3*[0];// new ComputeBuffer(1, UnsafeUtility.SizeOf(typeof(Vector3Int)));

	//public bool* a;

	//ArrayList ar = new ArrayList();

	public List<Chunk> unusedChuncks = new List<Chunk>();
	public List<Chunk> chunksToRerender = new List<Chunk>();
	public int chunksToLoad = 0;
	private int chunksToUnloadLoad = 0;
	public Dictionary<Vector3Int,Chunk> chunks { get; private set; } = new Dictionary<Vector3Int, Chunk>();
	public List<ChunkLoaderEntity> chunkLoaders = new List<ChunkLoaderEntity>();

	//public List<Vector3Int> chunksToActivate = new List<Vector3Int>();
	//public List<Chunk> chunksToDeactivate = new List<Chunk>();
	//private List<Chunk> chunksToDeactivate = new List<Chunk>();

	//List<Vector3Int> toLoadChuncks = new List<Vector3Int>();

	public DimensionData dimension;

	public static WorldManager Instance;

	//static Thread ChunkLoadingThread;
	bool updateChuncks = true;

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

			

			//UpdateChuncks();

			//*clermin = Vector3.up;

			//ChunkLoadingThread = new Thread(UpdateChuncksThread);
		}
	}

	// Start is called before the first frame update
	void Start()
	{
		Debug.Log("WorldManager Start");
		StartCoroutine("ChunckUpdate");

		//Debug.Log(UnsafeUtility.SizeOf(typeof(WorldSaver.BlockFileData)));
		//Debug.Log(typeof(WorldSaver.ChunkFileData));

		//Debug.Log(sizeof(byte) * 3 + sizeof(short));
		Debug.Log(System.GC.GetTotalMemory(true));

		//ChunkLoadingThread.Start();
		//UpdateChuncks();
		//gameObject.SetActive(false);

		//if (!ChunkLoadingThread.IsAlive)
		//{
		//	Debug.Log("started");
		//	ChunkLoadingThread.Start();
		//}
	}

	private void OnEnable()
	{
		//UpdateChuncks();

	}

	public void AddChunkLoader(ChunkLoaderEntity cle)
	{
		chunkLoaders.Add(cle);

		chunksToLoad += cle.RenderBox.volume;

	}

	public void RemoveChunkLoader(ChunkLoaderEntity cle)
	{
		chunkLoaders.Remove(cle);

	}

	// Update is called once per frame
	void FixedUpdate()
	{
		//UpdateChuncksThread();

		if (chunksToLoad > 0)
		{
			InstatiateChunks();
		}

		lock(chunksToRerender)
        {
			while (chunksToRerender.Count > 0)
			{
				setChunkMesh(chunksToRerender[0]);
				chunksToRerender.RemoveAt(0);
			}
        }

		/*lock (chunksToActivate) lock (chunksToDeactivate)
		{
			int index;
			Vector3Int vec;
			for (index = 0; chunksToActivate.Count > 0 && index < maxChuncksLoadedPerUpdate; index++)
			{
				vec = chunksToActivate[0];
				if(chunks.ContainsKey(vec))
				{

					chunks[vec].gameObject.SetActive(true);
					chunks[vec].transform.localPosition = vec * Chunk.size;
				
					chunks[vec].processing = false;

					chunksToActivate.RemoveAt(0);
				}
			}

			for (index = 0; chunksToDeactivate.Count > 0 && index < maxChuncksLoadedPerUpdate; index++)
			{
				chunksToDeactivate[0].gameObject.SetActive(false);

				chunksToDeactivate[0].processing = false;

				chunksToDeactivate.RemoveAt(0);
			}
		}*/
		//}
	}

	/*void UpdateChuncksThread()
	{
		//while (updateChuncks)
		{
			//Debug.Log("running good");

			//System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
			//watch.Start();

			int index;

			for (index = 0; index < chunkLoaders.Count; index++)
			{
				//-Debug.Log(chunkLoaders[index].active);
				if(chunkLoaders[index].active)
				{
					UpdateSingleChunckLoader(chunkLoaders[index]);
				}
			}

			foreach (KeyValuePair<Vector3Int, Chunk> vc in chunks.Reverse())
			{
				if(!vc.Value.verified && !vc.Value.processing)
				{
					//vc.Value.processing = true;
					unusedChuncks.Add(vc.Value);

					vc.Value.processing = true;

					chunksToDeactivate.Add(vc.Value);

					chunks.Remove(vc.Key);
				}
				else
				{
					vc.Value.verified = false;
				}
			}

			//watch.Stop();
			//Debug.Log("All loaded in " + watch.Elapsed + "\nThis was " + usedChunks.Count() + " Chunks");
		}
	}

	void UpdateSingleChunckLoader(ChunkLoaderEntity ch)
	{
		//int boxVolume = ch.RenderBox.volume;
		Vector3Int clpos = Vector3Int.FloorToInt(ch.positionInWorldManager.divide(Chunk.size));
		Vector3Int chunkPos = new Vector3Int();
		Chunk c;

		for (chunkPos.z = clpos.z + ch.RenderBox.z.min; chunkPos.z < clpos.z + ch.RenderBox.z.max; chunkPos.z++)
		{
			for (chunkPos.y = clpos.y + ch.RenderBox.y.min; chunkPos.y < clpos.y + ch.RenderBox.y.max; chunkPos.y++)
			{
				for (chunkPos.x = clpos.x + ch.RenderBox.x.min; chunkPos.x < clpos.x + ch.RenderBox.x.max; chunkPos.x++)
				{
					if (chunks.ContainsKey(chunkPos))
					{
						chunks[chunkPos].verified = true;
					}
					else if(unusedChuncks.Count > 0 && unusedChuncks[0] != null && !unusedChuncks[0].processing)
					{

						c = unusedChuncks[0];

						unusedChuncks.RemoveAt(0);

						//Debug.Log(c != null);
						//c.processing = true;

						c.verified = true;
						c.processing = true;
						chunks.Add(chunkPos, c);

						//Debug.Log(chunkPos);

						chunksToActivate.Add(chunkPos);

						//Debug.Log("a");
					}
				}
			}
		}
	}*/

	void InstatiateChunks()
	{
		Chunk c;

		for(int i = 0; chunksToLoad > 0 && i < maxChuncksLoadedPerUpdate; i++)
		{
			c = new GameObject().AddComponent<Chunk>();
			c.transform.parent = transform;
			c.gameObject.SetActive(false);
			//c.gameObject.AddComponent<BoxCollider>();
			//c.transform.localScale = Chunk.size;
			c.name = "Chunk";
			//c.transform.localPosition = chunksToLoad[0] * Chunk.size;
			chunksToLoad--;

			unusedChuncks.Add(c);
		}
	}

	private IEnumerator ChunckUpdate()
	{
		int index;

		Vector3Int clpos;
		Vector3Int chunkPos = new Vector3Int();
		ChunkLoaderEntity cl;
		Chunk c;

		while (updateChuncks)
		{

			for (index = 0; index < chunkLoaders.Count; index++)
			{
				cl = chunkLoaders[index];

				if (cl.active)// && cl.forceNextChunkUpdate)
				{
					//cl.forceNextChunkUpdate = false;

					clpos = Vector3Int.FloorToInt(cl.positionInWorldManager.divide(Chunk.size));


					for (chunkPos.z = clpos.z + cl.RenderBox.z.min; chunkPos.z < clpos.z + cl.RenderBox.z.max; chunkPos.z++)
					{
						for (chunkPos.y = clpos.y + cl.RenderBox.y.min; chunkPos.y < clpos.y + cl.RenderBox.y.max; chunkPos.y++)
						{
							for (chunkPos.x = clpos.x + cl.RenderBox.x.min; chunkPos.x < clpos.x + cl.RenderBox.x.max; chunkPos.x++)
							{
								if (chunks.ContainsKey(chunkPos))
								{
									chunks[chunkPos].verified = true;
								}
								else if (unusedChuncks.Count > 0 && unusedChuncks[0] != null)
								{

									c = unusedChuncks[0];
									unusedChuncks.RemoveAt(0);

									c.verified = true;

									c.transform.localPosition = chunkPos * Chunk.size;
									c.gameObject.SetActive(true);

									chunks.Add(chunkPos, c);
								}
							}
							yield return null;
						}
					}
				}
			}

			//Debug.Log("full update done");

			foreach (KeyValuePair<Vector3Int, Chunk> vc in chunks.Reverse())
			{
				if (!vc.Value.verified)
				{
					unusedChuncks.Add(vc.Value);

					vc.Value.gameObject.SetActive(false);

					chunks.Remove(vc.Key);
				}
				else
				{
					vc.Value.verified = false;
				}
			}


			yield return null;
		}
	}

	public WorldSaver.BlockFileData[] getSavedInDiskBlocks(Chunk c)
    {
		return WorldSaver.ReadChunckData(c, dimension.name);
    }

	public static short GetBlockFromWorldGen(Vector3 position)
    {
		return (short)(position.y > 4 ? 0 : 1);
    }

	public static short GetBlock(Vector3 position)
    {
		Chunk cr;
		if (!Instance.chunks.TryGetValue(Vector3Int.FloorToInt(position.divide(Chunk.size)), out cr)) return 0;
		return cr.instantitatedblocks[Mathf.FloorToInt(position.x % Chunk.size.x), Mathf.FloorToInt(position.y % Chunk.size.y), Mathf.FloorToInt(position.x % Chunk.size.y)];
    }

	public static Chunk GetChunk(Vector3 position)
	{
		Chunk cr;
		if (!Instance.chunks.TryGetValue(Vector3Int.FloorToInt(position.divide(Chunk.size)), out cr)) return null;
		return cr;
	}

	void setChunkMesh( Chunk c)
    {
		//Debug.Log("Hello");

		lock(c.vertices) lock (c.triangles) lock (c.normals)
		{
			if(c.vertices.Count == 0)
			{
				c.renderer.enabled = false;
				c.collider.enabled = false;
			}
			else
			{
				c.renderer.enabled = true;
				c.collider.enabled = true;

				c.mesh.SetVertices(c.vertices);

				c.mesh.subMeshCount = c.triangles.Count;

				Material[] materials = c.triangles.Keys.ToArray();

				c.renderer.materials = materials;

				for (int i = 0; i < c.triangles.Count; i++)
				{
					c.mesh.SetTriangles(c.triangles[materials[i]], i);
				}

				c.mesh.SetNormals(c.normals);
			}

			c.vertices.Clear();
			c.triangles.Clear();
			c.normals.Clear();
        }
	}

	public static void UpdateChunkMesh(Chunk c)
	{
		Instance.chunksToRerender.Add(c);
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
