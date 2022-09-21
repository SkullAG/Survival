using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UtilityEditor;

/*[CreateAssetMenu(fileName = "NewBlock", menuName = "Data/BlockData")]
public class BlockDataHandler : ScriptableObject
{
	public static BlockDataHandler Instance;

	[SerializeField]
	public static Dictionary<short, BlockData> blocks = new Dictionary<short, BlockData>();

    private void Awake()
    {
		if (Instance != null && Instance != this)
		{
			Destroy(this);
		}
		else
		{
			Instance = this;
		}
	}
}*/

[CreateAssetMenu(fileName = "NewBlock", menuName = "Data/BlockData")]
public class BlockData : ScriptableObject
{
	//public static List<BlockData> blocks = new List<BlockData>();
	[SerializeField]
	public static Dictionary<short, BlockData> blocks = new Dictionary<short, BlockData>();

	/*static BlockData()
    {
		BlockData[] b = Resources.FindObjectsOfTypeAll(typeof(BlockData)) as BlockData[];

		blocks = b.ToDictionary(x => x.id, x => x);

	}*/

	//public string dimensionName;
	[UtilityEditor.ReadOnly]
	public short id = 0;
	public string description;

	public bool isVoid = false;

	[HideIf("isVoid")]
	public bool isMarching = false;

	[HideIf("isVoid")]
	public bool canSeeThrough = false;

	[HideIf("isVoid")]
	public bool stopsLiquids = true;

	[HideIf("isVoid")]
	public bool needsInstantiation = false;

	[HideIf("isVoid")]
	public PhysicMaterial physicMaterial;

	[HideIf("isVoid")]
	public Material Material;

	[ShowIf("needsInstantiation")]
	[HideIf("isVoid")]
	public GameObject prefab;

	public short renderGroup;

	//public List<BlockData> dmList => blocks;

	[MenuItem("World/DebugBlockList")]
	public static void DebugBlockList()
    {
		string text = "";

		Debug.Log("Block count " + blocks.Count());

		blocks.Keys.ToList().ForEach(k => text += k + " : " + blocks[k].name + "\n");
		Debug.Log(text);
	}

	//etc further info

	[MenuItem("World/CleanBlockList")]
	private static void Clean()
	{
		blocks = blocks.Where(d => d.Value != null).GroupBy(d => d.Value).Select(group => group.First()).ToDictionary(pair => pair.Key, pair => pair.Value);
		Debug.Log("cleaning block");
		//dmList = blocks;
	}

    private void Awake()
    {
		Debug.Log("BlockAwake");
	}

    private void OnEnable()
    {
		Debug.Log("BlockEnable");
		if (!blocks.Contains(new KeyValuePair<short, BlockData>(this.id, this)))
		{
			//Debug.Log("hell o god no " + id );
			id = GetNewId();
			blocks.Add(id, this);
			//Debug.Log("Block " + id + " added");
		}
	}

#if UNITY_EDITOR
	private void OnValidate()
    {
		if (isVoid) isMarching = false;

		if (isVoid) canSeeThrough = false;

		if (isVoid) stopsLiquids = true;

		if (isVoid) needsInstantiation = false;

		if (isVoid) physicMaterial = null;

		if (isVoid) Material = null;

		if (isVoid && !needsInstantiation) prefab = null;

		if (isVoid) renderGroup = 0;
		else if (canSeeThrough || needsInstantiation) renderGroup = 1;
		else renderGroup = 2;
	}
#endif

	short GetNewId()
	{
		//Debug.Log(id);
		if (!blocks.ContainsKey(id)) return id;
		//else Debug.Log("key " + id + " is used recalculating");

		short num = 0;

		bool found = false;

		BlockData value;

		while (!found)
		{
			found = !blocks.TryGetValue(num, out value);

			if (found) return num;

			if(value == null)
            {
				blocks.Remove(num);
				return num;
			}

			num++;
		}

		return num;
	}
}
