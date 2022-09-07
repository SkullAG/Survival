using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UtilityEditor;

[CreateAssetMenu(fileName = "NewBlock", menuName = "Data/BiomeData")]
public class BiomeData : ScriptableObject
{
	[SerializeField]
	public static Dictionary<short, BiomeData> biomes = new Dictionary<short, BiomeData>();
	[UtilityEditor.ReadOnly]
	public short id = 0;
	public string description;

	public Color Tint;

	[MenuItem("World/DebugBiomeList")]
	public static void DebugBiomeList()
	{
		string text = "";

		Debug.Log("Block count " + biomes.Count());

		biomes.Keys.ToList().ForEach(k => text += k + " : " + biomes[k].name + "\n");
		Debug.Log(text);
	}

	[MenuItem("World/CleanBiomeList")]
	private static void Clean()
	{
		biomes = biomes.Where(d => d.Value != null).GroupBy(d => d.Value).Select(group => group.First()).ToDictionary(pair => pair.Key, pair => pair.Value);
		Debug.Log("cleaning Biome");
	}

	private void OnEnable()
	{
		//Debug.Log("BlockAwake");

		if (!biomes.Contains(new KeyValuePair<short, BiomeData>(this.id, this)))
		{
			//Debug.Log("hell o god no " + id );
			id = GetNewId();
			biomes.Add(id, this);
			//Debug.Log("Block " + id + " added");
		}
	}

	short GetNewId()
	{
		//Debug.Log(id);
		if (!biomes.ContainsKey(id)) return id;
		//else Debug.Log("key " + id + " is used recalculating");

		short num = 0;

		bool found = false;

		BiomeData value;

		while (!found)
		{
			found = !biomes.TryGetValue(num, out value);

			if (found) return num;

			if (value == null)
			{
				biomes.Remove(num);
				return num;
			}

			num++;
		}

		return num;
	}
}
