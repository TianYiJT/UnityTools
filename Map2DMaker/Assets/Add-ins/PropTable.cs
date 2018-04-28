using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Map2D;
public class PropTable:MonoBehaviour
{
	public string name;
	public PropNode[] allProp;
	int size = 0;
	public int tempIndex = 0;
	public PropNode find(int index)
	{
		return allProp [index];
	}
	public void AddProp(GameObject prefab,bool[][] rangetable,int index,string path)
	{
		if (tempIndex >= size) 
		{
			PropNode[] newallProp = new PropNode[(size+1)*2];
			for (int i = 0; i < tempIndex; i++) 
			{
				newallProp [i] = allProp [i];
			}
			allProp = newallProp;
			size = allProp.Length;
		}
		allProp [tempIndex].prefab = prefab;
		allProp [tempIndex].RangeTable = rangetable;
		allProp [tempIndex].index = index;
		allProp [tempIndex].path = path;
		tempIndex++;
	}
	public void DeleteProp(int index)
	{
		int tempI = -1;
		bool IsExist = false;
		for (int i = 0; i < tempIndex; i++) 
		{
			if (allProp [i].index > index)
				allProp [i].index--;
			if (allProp [i].index == index)
			{
				IsExist = true;
				tempI = i;
			}
		}
		if (IsExist)
		{
			for (int i = tempI; i < tempIndex-1; i++)
			{
				allProp [i] = allProp [i + 1];
			}
			tempIndex--;
		}
	}
}
