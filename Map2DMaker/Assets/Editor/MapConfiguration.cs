using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Map2D;
public class MapConfiguration : EditorWindow 
{
	[MenuItem("Map2DMaker/Save")]
	static void Save()
	{
		TemplatePropTable tpt = new TemplatePropTable ();
		GameObject dt_obj = GameObject.FindGameObjectWithTag ("DefaultTileTableManager");
		GameObject ptall_obj = GameObject.FindGameObjectWithTag ("PropAllTableManager");
		GameObject[] pt_obj = GameObject.FindGameObjectsWithTag ("PropTableManager");
		tpt.allDT = new DefaultTileTableNode[dt_obj.GetComponent<DefaultTileTable>().tempIndex];
		tpt.Groups  = new string[pt_obj.Length];
		tpt.allProp = new PropTableNode[ptall_obj.GetComponent<PropAllTable>().indexall];
		for (int i = 0; i < tpt.allDT.Length; i++) 
		{
			tpt.allDT [i].Index = i;
			tpt.allDT [i].prefabpath = dt_obj.GetComponent<DefaultTileTable> ().allDefaultTile [i].path;
		}
		for (int i = 0; i < tpt.Groups.Length; i++)
			tpt.Groups [i] = pt_obj [i].GetComponent<PropTable> ().name;
		int propIndex = 0;
		for (int i = 0; i < pt_obj.Length; i++)
		{
			for (int j = 0; j < pt_obj [i].GetComponent<PropTable> ().tempIndex; j++) 
			{
				tpt.allProp [propIndex].AllTableIndex = pt_obj [i].GetComponent<PropTable> ().allProp [j].index;
				tpt.allProp [propIndex].GroupName = pt_obj [i].GetComponent<PropTable> ().name;
				tpt.allProp [propIndex].GroupTableIndex = j;
				tpt.allProp [propIndex].prefabPath = pt_obj [i].GetComponent<PropTable> ().allProp [j].path;
				tpt.allProp [propIndex].RangeTable = pt_obj [i].GetComponent<PropTable> ().allProp [j].RangeTable;
				propIndex++;
			}
		}
		tpt.input (Application.dataPath+"/PropTable/data.xml");
	}
	[MenuItem("Map2DMaker/Gen")]
	static void Gen()
	{
		GameObject newPropAll = new GameObject ();
		newPropAll.AddComponent<PropAllTable> ();
		newPropAll.tag = "PropAllTableManager";
		GameObject newPropMakeData = new GameObject ();
		newPropMakeData.AddComponent<PropMakerData> ();
		newPropMakeData.tag="PropMakerData";
		TemplatePropTable tpt = new TemplatePropTable ();
		tpt.output (Application.dataPath+"/PropTable/data.xml");
		for (int i = 0; i < tpt.Groups.Length; i++)
		{
			GameObject temp = new GameObject ();
			temp.tag="PropTableManager";
			temp.AddComponent<PropTable> ();
			temp.GetComponent<PropTable> ().name = tpt.Groups [i];
		}
		GameObject newDT = new GameObject ();
		newDT.AddComponent<DefaultTileTable> ();
		newDT.tag="DefaultTileTableManager";
		newDT.GetComponent<DefaultTileTable>().allDefaultTile = new DefaultTileNode[tpt.allDT.Length];
		newDT.GetComponent<DefaultTileTable> ().tempIndex = tpt.allDT.Length;
		for (int i = 0; i < tpt.allDT.Length; i++)
		{
			GameObject g = Resources.Load (tpt.allDT[i].prefabpath) as GameObject;
			newDT.GetComponent<DefaultTileTable> ().allDefaultTile [tpt.allDT [i].Index].prefab = g;
			newDT.GetComponent<DefaultTileTable> ().allDefaultTile [tpt.allDT [i].Index].path = tpt.allDT [i].prefabpath;
		}
		newPropAll.GetComponent<PropAllTable>().allprop = new PropNode[tpt.allProp.Length];
		newPropAll.GetComponent<PropAllTable> ().indexall = tpt.allProp.Length;
		GameObject[] allPropTablenew = GameObject.FindGameObjectsWithTag ("PropTableManager");
		for (int i = 0; i < tpt.allProp.Length; i++)
		{
			newPropAll.GetComponent<PropAllTable> ().allprop [tpt.allProp [i].AllTableIndex].index = tpt.allProp [i].AllTableIndex;
			newPropAll.GetComponent<PropAllTable> ().allprop [tpt.allProp [i].AllTableIndex].path = tpt.allProp [i].prefabPath;
			newPropAll.GetComponent<PropAllTable> ().allprop [tpt.allProp [i].AllTableIndex].prefab = (GameObject)Resources.Load(tpt.allProp [i].prefabPath);
			newPropAll.GetComponent<PropAllTable> ().allprop [tpt.allProp [i].AllTableIndex].RangeTable = tpt.allProp [i].RangeTable;
			for (int j = 0; j < allPropTablenew.Length; j++) 
			{
				if (allPropTablenew [j].GetComponent<PropTable> ().name == tpt.allProp [i].GroupName)
				{
					allPropTablenew [j].GetComponent<PropTable> ().AddProp ((GameObject)Resources.Load(tpt.allProp [i].prefabPath),
						tpt.allProp [i].RangeTable,tpt.allProp[i].AllTableIndex,tpt.allProp[i].prefabPath);
					break;
				}
			}
		}
	}


}
