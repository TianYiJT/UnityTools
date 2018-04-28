using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Map2D;
public class PropAllTable:MonoBehaviour
{
	public PropNode[] allprop;
	int size =0 ;
	public int indexall=0;
	public PropNode find(int index)
	{
		return allprop [index];
	}
	public int addprop(GameObject g,bool[][] rangetable,string path)
	{
		if (indexall >= size) 
		{
			PropNode[] newallDefaultTile = new PropNode[(size+1)*2];
			for (int i = 0; i < indexall; i++) 
			{
				newallDefaultTile [i] = allprop [i];
			}
			allprop = newallDefaultTile;
			size = allprop.Length;
		}
		allprop [indexall].prefab = g;
		allprop [indexall].RangeTable = rangetable;
		allprop [indexall].index = indexall;
		allprop [indexall].path = path;
		int thisProp = indexall;
		indexall++;
		return thisProp;
	}
	public void deleteprop(int i_)
	{
		for (int i = i_; i < indexall-1; i++) 
		{
			allprop [i] = allprop [i + 1];
		}
		indexall--;
	}
}
