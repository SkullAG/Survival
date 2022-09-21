using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
public class Chunk : MonoBehaviour
{
	public static Vector3Int size = new Vector3Int(16, 16, 16);
	//public BlockData blockPrefab;

	public short[,,] instantitatedblocks;

	public MeshRenderer renderer;

	public MeshCollider collider;

	public Mesh mesh;

	//public BlockData[,,] blocks = new BlockData[size.x, size.y, size.z];

	//private List<WorldSaver.BlockFileData> primitiveBlockData;
	private Dictionary<short, short> primitiveBlockData;

	private Vector3 position;

	//public WorldManager worldManager;
	//public Mesh mesh;

	public static int VecToInt(Vector3Int v) { return VecToInt(v.x, v.y, v.z); }
	public static int VecToInt(int x, int y, int z) { return x + (y * size.x) + (z * size.x * size.y); }

	public static Vector3Int IntToVec(int i) { return new Vector3Int(i % size.x, i / size.x % size.y, i / (size.x * size.y) % size.z); }

	//public bool loading = false;
	//public bool processing = false;
	public bool verified = false;

	public bool updating = false;

	//public bool hasToSetMeshData = false;

	public List<Vector3> vertices;
	public Dictionary<Material, List<int>> triangles;
	public List<Vector3> normals;

	private void Awake()
	{
		primitiveBlockData = new Dictionary<short, short>();

		instantitatedblocks = new short[size.x,size.y,size.z];

		mesh = new Mesh();

		renderer = this.GetComponent<MeshRenderer>(); 

		collider = this.GetComponent<MeshCollider>();

		this.GetComponent<MeshFilter>().mesh = mesh;

		collider.sharedMesh = mesh;
	}

	// Start is called before the first frame update
	void Start()
	{

	}

    private void OnEnable()
    {
		position = transform.position;
		UpdateAllBlocksAsync();
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

	public void UpdateAllBlocks()
	{
		//Stopwatch watch = Stopwatch.StartNew();
		//watch.Start();

		short bid = -1;
		short index = 0;
		Vector3Int blockPos = Vector3Int.zero;

		//separar carga de chuncks en distintos frames hara que cargen sin problemas

		lock (instantitatedblocks)
        {
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
							//when there's no info saved about the block

							//UnityEngine.Debug.Log(instantitatedblocks.Length);

							bid = WorldManager.GetBlockFromWorldGen(position + blockPos);
						}

						//UnityEngine.Debug.Log(index + ", " + instantitatedblocks.Length);
						instantitatedblocks[blockPos.x, blockPos.y, blockPos.z] = bid;

						//watch.Stop();
						//UnityEngine.Debug.Log("block:" + (transform.position + blockPos) + " load took: " + watch.ElapsedMilliseconds);
					}
				}
			}
        }

		

		//watch.Stop();
		//UnityEngine.Debug.Log("Chunk:" + (transform.position / 16) + " load took: " + watch.ElapsedMilliseconds);
		//return watch.ElapsedMilliseconds;
	}

	public async void UpdateAllBlocksAsync()
	{
		updating = true;

		float time = Time.time;

		//primitiveBlockData.Clear();

		//UnityEngine.Debug.Log(WorldManager.Instance.getSavedInDiskBlocks(this).Count());
		WorldManager.Instance.getSavedInDiskBlocks(this).ToDictionary(ref primitiveBlockData); 

		//foreach (WorldSaver.BlockFileData a in WorldManager.Instance.getSavedInDiskBlocks(this))
		//{
		//	primitiveBlockData.Add((short)VecToInt(a.x,a.y,a.z), a.id);
		//}
		
		await Task.Run(UpdateAllBlocks);

		updating = false;

		await Task.Run(RefreshMesh);

		//UnityEngine.Debug.Log(times + " chuncks loaded in " + (Time.time - time) + "s");
	}

	public void RefreshMesh()
    {
		//mesh.Clear();

		//bool refreshingMesh = true;

		int x, y, z;
		//Vector3Int blockPos = Vector3Int.zero;

		vertices = new List<Vector3>();
		int vertIndex = 0;
		triangles = new Dictionary<Material, List<int>>();
		normals = new List<Vector3>();

		BlockData[] blocks = new BlockData[4];
		Chunk[] neigbours = new Chunk[3]
		{
			WorldManager.GetChunk(new Vector3(position.x - Chunk.size.x, position.y, position.z)),
			WorldManager.GetChunk(new Vector3(position.x, position.y - Chunk.size.y, position.z)),
			WorldManager.GetChunk(new Vector3(position.x, position.y, position.z - Chunk.size.z)),
		};
		//List<Material> materials = new List<Material>();

		//BlockData mainBlock;

		for (x = 0; x < size.x; x++)
		{
			for (z = 0; z < size.z; z++)
			{
				for (y = 0; y < size.y; y++)
				{
					blocks[0] = BlockData.blocks[instantitatedblocks[x, y, z]];

					if (x > 0) blocks[1] = BlockData.blocks[instantitatedblocks[x - 1, y, z]];
					else if (neigbours[0] != null) blocks[1] = BlockData.blocks[neigbours[0].instantitatedblocks[x + size.x - 1, y, z]];
					else blocks[1] = BlockData.blocks[0];

					if (y > 0) blocks[2] = BlockData.blocks[instantitatedblocks[x, y - 1, z]];
					else if (neigbours[1] != null) blocks[2] = BlockData.blocks[neigbours[1].instantitatedblocks[x, y + size.y - 1, z]];
					else blocks[2] = BlockData.blocks[0];

					if (z > 0) blocks[3] = BlockData.blocks[instantitatedblocks[x, y, z - 1]];
					else if (neigbours[2] != null) blocks[3] = BlockData.blocks[neigbours[2].instantitatedblocks[x, y, z + size.z - 1]];
					else blocks[3] = BlockData.blocks[0];

					if(blocks[0].renderGroup > blocks[1].renderGroup)
                    {
						vertices.Add(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
						vertices.Add(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));
						vertices.Add(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
						vertices.Add(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));

						normals.Add(Vector3.right);
						normals.Add(Vector3.right);
						normals.Add(Vector3.right);
						normals.Add(Vector3.right);

						if (!triangles.ContainsKey(blocks[0].Material))
						{
							triangles.Add(blocks[0].Material, new List<int>());
						}

                        triangles[blocks[0].Material].Add(vertIndex);
						triangles[blocks[0].Material].Add(vertIndex + 1);
						triangles[blocks[0].Material].Add(vertIndex + 2);
						triangles[blocks[0].Material].Add(vertIndex + 1);
						triangles[blocks[0].Material].Add(vertIndex + 2);
						triangles[blocks[0].Material].Add(vertIndex + 3);

						vertIndex = vertices.Count;
					}
					else if(blocks[0].renderGroup < blocks[1].renderGroup)
					{
						vertices.Add(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
						vertices.Add(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));
						vertices.Add(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
						vertices.Add(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));

						normals.Add(Vector3.left);
						normals.Add(Vector3.left);
						normals.Add(Vector3.left);
						normals.Add(Vector3.left);

						if (!triangles.ContainsKey(blocks[1].Material))
						{
							triangles.Add(blocks[1].Material, new List<int>());
						}

						triangles[blocks[1].Material].Add(vertIndex);
						triangles[blocks[1].Material].Add(vertIndex + 1);
						triangles[blocks[1].Material].Add(vertIndex + 2);
						triangles[blocks[1].Material].Add(vertIndex + 1);
						triangles[blocks[1].Material].Add(vertIndex + 2);
						triangles[blocks[1].Material].Add(vertIndex + 3);

						vertIndex = vertices.Count;
					}

					if (blocks[0].renderGroup > blocks[2].renderGroup)
					{
						vertices.Add(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
						vertices.Add(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));
						vertices.Add(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
						vertices.Add(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));

						normals.Add(Vector3.up);
						normals.Add(Vector3.up);
						normals.Add(Vector3.up);
						normals.Add(Vector3.up);

						if (!triangles.ContainsKey(blocks[0].Material))
						{
							triangles.Add(blocks[0].Material, new List<int>());
						}

						triangles[blocks[0].Material].Add(vertIndex);
						triangles[blocks[0].Material].Add(vertIndex + 1);
						triangles[blocks[0].Material].Add(vertIndex + 2);
						triangles[blocks[0].Material].Add(vertIndex + 1);
						triangles[blocks[0].Material].Add(vertIndex + 2);
						triangles[blocks[0].Material].Add(vertIndex + 3);

						vertIndex = vertices.Count;
					}
					else if (blocks[0].renderGroup < blocks[2].renderGroup)
					{
						vertices.Add(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
						vertices.Add(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));
						vertices.Add(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
						vertices.Add(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));

						normals.Add(Vector3.down);
						normals.Add(Vector3.down);
						normals.Add(Vector3.down);
						normals.Add(Vector3.down);

						if (!triangles.ContainsKey(blocks[2].Material))
						{
							triangles.Add(blocks[2].Material, new List<int>());
						}

						triangles[blocks[2].Material].Add(vertIndex);
						triangles[blocks[2].Material].Add(vertIndex + 1);
						triangles[blocks[2].Material].Add(vertIndex + 2);
						triangles[blocks[2].Material].Add(vertIndex + 1);
						triangles[blocks[2].Material].Add(vertIndex + 2);
						triangles[blocks[2].Material].Add(vertIndex + 3);

						vertIndex = vertices.Count;
					}

					if (blocks[0].renderGroup > blocks[3].renderGroup)
					{
						vertices.Add(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
						vertices.Add(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
						vertices.Add(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
						vertices.Add(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));

						normals.Add(Vector3.forward);
						normals.Add(Vector3.forward);
						normals.Add(Vector3.forward);
						normals.Add(Vector3.forward);

						if (!triangles.ContainsKey(blocks[0].Material))
						{
							triangles.Add(blocks[0].Material, new List<int>());
						}

						triangles[blocks[0].Material].Add(vertIndex);
						triangles[blocks[0].Material].Add(vertIndex + 1);
						triangles[blocks[0].Material].Add(vertIndex + 2);
						triangles[blocks[0].Material].Add(vertIndex + 1);
						triangles[blocks[0].Material].Add(vertIndex + 2);
						triangles[blocks[0].Material].Add(vertIndex + 3);

						vertIndex = vertices.Count;
					}
					else if (blocks[0].renderGroup < blocks[3].renderGroup)
					{
						vertices.Add(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
						vertices.Add(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
						vertices.Add(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
						vertices.Add(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));

						normals.Add(Vector3.back);
						normals.Add(Vector3.back);
						normals.Add(Vector3.back);
						normals.Add(Vector3.back);

						if (!triangles.ContainsKey(blocks[3].Material))
						{
							triangles.Add(blocks[3].Material, new List<int>());
						}

						triangles[blocks[3].Material].Add(vertIndex);
						triangles[blocks[3].Material].Add(vertIndex + 1);
						triangles[blocks[3].Material].Add(vertIndex + 2);
						triangles[blocks[3].Material].Add(vertIndex + 1);
						triangles[blocks[3].Material].Add(vertIndex + 2);
						triangles[blocks[3].Material].Add(vertIndex + 3);

						vertIndex = vertices.Count;
					}
				}
			}
		}

		WorldManager.Instance.chunksToRerender.Add(this);

		/*mesh.SetVertices(vertices);

		mesh.subMeshCount = triangles.Count;

		renderer.materials = triangles.Keys.ToArray();

		for (int i = 0; i < triangles.Count; i++)
        {
			mesh.SetTriangles(triangles[renderer.materials[i]], i);
        }

		mesh.SetNormals(normals);*/
	}

	/*IEnumerator repeatTest(int times)
    {
		for(int i = 0; i < times; i++)
        {
			UpdateAllBlocks();
			yield return new WaitForEndOfFrame();
        }
    }*/

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
