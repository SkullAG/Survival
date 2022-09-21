using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkLoaderEntity : MonoBehaviour
{

	public MinMaxVector3D RenderBox;

	public bool active;

	public Vector3 positionInWorldManager { get; private set; }
	//public Vector3Int LastChunkPos { get; private set; }

	public Vector3Int ActualChunkPos { get { return Vector3Int.FloorToInt(positionInWorldManager.divide(Chunk.size)); } }
	//public Vector3 positionInWorldManager { get; private set; }

	public Vector3 positionInWorldManager_ { get { return WorldManager.Instance.transform.InverseTransformPoint(transform.position); } }

	//public bool forceNextChunkUpdate = true;

	private void FixedUpdate()
    {
		positionInWorldManager = WorldManager.Instance.transform.InverseTransformPoint(transform.position);

		//if(LastChunkPos != ActualChunkPos) forceNextChunkUpdate = true;

		//LastChunkPos = ActualChunkPos;
	}

    private void OnEnable()
    {
		active = true;
		//forceNextChunkUpdate = true;
	}
    private void OnDisable()
    {
		active = false;
	}

    public void Start()
	{
		WorldManager.Instance.AddChunkLoader(this);
	}

	public void OnDestroy()
	{
		WorldManager.Instance.RemoveChunkLoader(this);
	}
}
