using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ProceduralWorldGen
{
	//public static DimensionData a => WorldManager.Instance.dimension;

	public static short GetBlock(Vector3 vec ,DimensionData dimension)
	{
		vec.x = Mathf.Floor(vec.x);
		vec.y = Mathf.Floor(vec.y);
		vec.z = Mathf.Floor(vec.z);

		//DimensionData dimension = new DimensionData();

		return 0;
	}
	
}
