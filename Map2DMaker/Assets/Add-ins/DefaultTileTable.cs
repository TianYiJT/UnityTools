using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Map2D;
public class DefaultTileTable:MonoBehaviour
{
	public DefaultTileNode[] allDefaultTile;
	int size = 0;
	public int tempIndex = 0;
	public GameObject find(int index)
	{
		return allDefaultTile[index].prefab;
	}
	public void AddProp(GameObject prefab,string path)
	{
		if (tempIndex >= size) 
		{
			DefaultTileNode[] newallDefaultTile = new DefaultTileNode[(size+1)*2];
			for (int i = 0; i < tempIndex; i++) 
			{
				newallDefaultTile [i] = allDefaultTile [i];
			}
			allDefaultTile = newallDefaultTile;
			size = allDefaultTile.Length;
		}
		allDefaultTile [tempIndex].prefab = prefab;
		allDefaultTile [tempIndex].path = path;
		tempIndex++;
	}
	public void DeleteProp(int index)
	{
		for (int i = index; i < tempIndex-1; i++) 
		{
			allDefaultTile [i] = allDefaultTile [i + 1];
		}
		tempIndex--;
	}
}
