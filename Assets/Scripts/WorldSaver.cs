using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.Collections.LowLevel.Unsafe;
using System.Runtime.InteropServices;
using UtilityEditor;

public static class WorldSaver
{
	public static string worldName { get; private set; }

	[Serializable]
	public struct BlockFileData
	{
		public byte x, y, z;
		public short id;
	}

	//[Serializable]

	[MenuItem("World/CreateSaveFile")]
	private static void createRoughSaveFile()
	{
		EditorWindow window = new UtilsEditor.TextPopup(CreateSaveFile, "World Name:");
		window.title = "Create World Files";
		window.ShowUtility();
	}

	public static void CreateSaveFile(string name)
	{
		string path = Application.dataPath + "/Saves";

		if(!Directory.Exists(path)) Directory.CreateDirectory(path);

		path += "/" + name;

		if (!Directory.Exists(path)) Directory.CreateDirectory(path);
		else { Debug.LogError("Path: \"" + path + "\" already exist, overwrite instead"); return; }

		DimensionData dimension;

		//path += "/" + name;
		for (short i = 0; i < DimensionData.dimensions.Count; i++)
		{
			if(DimensionData.dimensions.TryGetValue(i, out dimension)) CreateDimensionFile(name, dimension.name);
		}
	}

	public static void CreateDimensionFile(string _worldName, string dimensionName)
	{
		worldName = _worldName;

		string path = Application.dataPath + "/Saves/" + worldName + "/" + dimensionName + "/map";

		if (!Directory.Exists(path)) Directory.CreateDirectory(path);

		//if (!Directory.Exists(path)) { Debug.LogError("Path: " + path + "doesn't exist"); return; }

		path += "/0_0_0.chunck";
		FileStream file;

		if (File.Exists(path)) file = File.OpenWrite(path);
		else file = File.Create(path);
		//int[4096] i;
		BlockFileData[] bd = new BlockFileData[4096];

		for(int i = 0; i < bd.Length; i++)
		{
			bd[i].x = 10;
			bd[i].y = 20;
			bd[i].z = 30;
			bd[i].id = 40;
        }
		//Debug.Log(UnsafeUtility.SizeOf(typeof(BlockFileData)) * 4096);

		//BinaryFormatter formatter = new BinaryFormatter();

		//formatter.Serialize(file, bd);
		WriteChunckData(file, bd);
		file.Close();

		//Debug.Log(ReadChunckData(file)[0]);//.Deserialize()[0].x;


		//file.Write(bd, 0, UnsafeUtility.SizeOf(typeof(BlockFileData)) * bd.Length);

		//File.re
		//file.
	}

	public static void WriteChunckData(Chunk chunk, string dimensionName, BlockFileData[] blocks)
	{
		Vector3Int pos = Vector3Int.FloorToInt(chunk.transform.position / 16);
		string path = Application.dataPath + "/Saves/" + worldName + "/" + dimensionName + "/map/" + pos.x + "_" + pos.y + "_" + pos.z + ".chunck";

		if (!File.Exists(path)) { Debug.LogError("Path: " + path + "doesn't exist"); return; };

		FileStream file;
		WriteChunckData(file = File.OpenWrite(path), blocks);

		file.Close();
	}

	public static void WriteChunckData(FileStream file, BlockFileData[] blocks)
    {
		file.Write(blocks.Serialize());
	}

	public static BlockFileData[] ReadChunckData(Chunk chunk, string dimensionName)
	{
		Vector3Int pos = Vector3Int.FloorToInt(chunk.transform.position);
		string path = Application.dataPath + "/Saves/" + worldName + "/" + dimensionName + "/map/" + pos.x + "_" + pos.y + "_" + pos.z + ".chunck";

		if (!File.Exists(path)) { Debug.LogError("Path: " + path + "doesn't exist"); return null; };

		return File.ReadAllBytes(path).DeserializeToBlockFileData();
	}

	public static BlockFileData[] ReadChunckData(FileStream file)
	{
		return File.ReadAllBytes(file.Name).DeserializeToBlockFileData();
	}

	/*public static  IEnumerator<BlockFileData> Serialize(this BlockFileData[] bfdArray)
    {
		foreach(BlockFileData bd in bfdArray)
        {
			yield return bd.Serialize();
        }
    }*/

	public static Span<byte> Serialize(this BlockFileData[] bfdArray)
	{
		return MemoryMarshal.AsBytes(bfdArray.AsSpan());
		
	}

	public static BlockFileData[] DeserializeToBlockFileData(this byte[] byteArray)
	{
		int BlockFileDataSize = UnsafeUtility.SizeOf(typeof(BlockFileData));

		BlockFileData[] blocks = new BlockFileData[byteArray.Length / BlockFileDataSize];
		Debug.Log(blocks.Length + " Blocks Readed");

		/*Span<byte> buffer = new Span<byte>(byteArray);

		foreach(BlockFileData bd in blocks)
        {
			MemoryMarshal.Read()
        }*/
		for(int i = 0; i < blocks.Length;)
        {
			IntPtr intPtr = Marshal.AllocHGlobal(byteArray.Length);
			Marshal.Copy(byteArray, BlockFileDataSize * i, intPtr, byteArray.Length);
			//TEST_RECORD record2 = Marshal.PtrToStructure(intPtr, typeof(TEST_RECORD));

			Marshal.PtrToStructure(intPtr, blocks[i]);
			Marshal.FreeHGlobal(intPtr);
        }


		//return MemoryMarshal.AsBytes(bfdArray.AsSpan());
		return blocks;
	}

	public static Dictionary<short, short> ToDictionary(this BlockFileData[] bas)
    {
		Dictionary<short, short> d = new Dictionary<short, short>();
		BlockFileData ba;
		for (int i = 0; i < bas.Length; i++)
        {
			ba = bas[i];
			d.Add((short)(ba.x + (ba.y * Chunk.size.x) + (ba.z * Chunk.size.x * Chunk.size.y)), ba.id);

		}

		return d;
	}
}
