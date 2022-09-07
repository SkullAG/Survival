using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.ComponentModel;
using UtilityEditor;
using UnityEditor;

[CreateAssetMenu(fileName = "NewDimension", menuName = "Data/DimensionData")]
public class DimensionData : ScriptableObject
{
	[SerializeField]
	public static Dictionary<short, DimensionData> dimensions = new Dictionary<short, DimensionData>();

	[UtilityEditor.ReadOnly]
	public short id = 0;
	public string description;

	[System.Serializable]
	public class BiomeClimatologicalData
	{
		public BiomeData biome;

		public Vector2 heatRange = new Vector2();
		public Vector2 temperatureRange = new Vector2();
		public Vector2 heightRange = new Vector2();
	}

	public List<BiomeClimatologicalData> surfaceBiomes;
	public List<BiomeClimatologicalData> undergroundBiomes;

	//public List<BlockData> dmList => blocks;

	[MenuItem("World/DebugBlockList")]
	public static void DebugBlockList()
	{
		string text = "";

		Debug.Log("Block count " + dimensions.Count());

		dimensions.Keys.ToList().ForEach(k => text += k + " : " + dimensions[k].name + "\n");
		Debug.Log(text);
	}

	//etc further info

	[MenuItem("World/CleanBlockList")]
	private static void Clean()
	{
		dimensions = dimensions.Where(d => d.Value != null).GroupBy(d => d.Value).Select(group => group.First()).ToDictionary(pair => pair.Key, pair => pair.Value);
		Debug.Log("cleaning block");
		//dmList = blocks;
	}

	private void OnEnable()
	{
		Debug.Log("BlockAwake");

		if (!dimensions.Contains(new KeyValuePair<short, DimensionData>(this.id, this)))
		{
			//Debug.Log("hell o god no " + id );
			id = GetNewId();
			dimensions.Add(id, this);
			//Debug.Log("Block " + id + " added");
		}
	}

	short GetNewId()
	{
		//Debug.Log(id);
		if (!dimensions.ContainsKey(id)) return id;
		//else Debug.Log("key " + id + " is used recalculating");

		short num = 0;

		bool found = false;

		DimensionData value;

		while (!found)
		{
			found = !dimensions.TryGetValue(num, out value);

			if (found) return num;

			if (value == null)
			{
				dimensions.Remove(num);
				return num;
			}

			num++;
		}

		return num;
	}
}
