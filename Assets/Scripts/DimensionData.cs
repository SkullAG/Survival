using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.ComponentModel;

[CreateAssetMenu(fileName = "NewDimension", menuName = "Data/DimensionData")]
public class DimensionData : ScriptableObject
{
	public static List<DimensionData> dimensions = new List<DimensionData>();

	//public string dimensionName;
	[ReadOnly(true)]
	public int id;
	public string description;

	public List<DimensionData> dmList = dimensions;

	//etc further info

	private void OnValidate()
	{
		dimensions = dimensions.Where(d => d != null).Distinct().ToList();
		dmList = dimensions;
	}

	public static int GetDimensionIndexById(int id)
	{
		return dimensions.IndexOf(dimensions.First(d => d.id == id));
	}

	public static int GetDimensionIndexByName(string name)
	{
		return dimensions.IndexOf(dimensions.First(d => d.name == name));
	}

	public static int GetDimensionIndex(DimensionData dimension)
	{
		return dimensions.IndexOf(dimension);
	}

	public static DimensionData GetDimensionByIndex(int index)
	{
		return dimensions[index];
	}

	public static DimensionData GetDimensionById(int id)
	{
		return dimensions.First(d => d.id == id);
	}

	public static DimensionData GetDimensionByName(string name)
	{
		return dimensions.First(d => d.name == name);
	}

	public DimensionData()
	{
		//id = SystemInfo.un
		dimensions.Add(this);
	}

	~DimensionData()
	{
		dimensions.Remove(this);
	}
}
