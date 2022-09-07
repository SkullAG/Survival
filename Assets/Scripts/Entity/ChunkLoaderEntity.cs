using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkLoaderEntity : MonoBehaviour
{

	public MinMaxVector3D RenderBox;

	public void Start()
	{
		WorldManager.Instance.AddChunkLoader(this);
	}

	public void OnDestroy()
	{
		WorldManager.Instance.RemoveChunkLoader(this);
	}
}
