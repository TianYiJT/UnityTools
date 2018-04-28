using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RLRandomAlgorithm;
using Map2D;
using System.IO;
public class RLRandomTranfer 
{
//	RLMap rlMap;
//	RLLevel rlLevel;
	RLFunction[] rlfunc;
	private string getName(string str)
	{
		string des = "";
		int index = 0;
		for (int i = str.Length - 5; i >= 0; i--) 
		{
			if (str [i] == '/')
			{
				index = i;	
				break;
			}
		}
		des = str.Substring (index+1,str.Length-5-index);
		des = des.Substring (6,des.Length-6);
		return des;
	}
	public void Init()
	{
		Vector3 stdSize = GameObject.FindGameObjectWithTag ("TemplateGameObject").GetComponent<SpriteRenderer>().bounds.size;
		RLMap.standardSize = stdSize;
		rlfunc = new RLFunction[3];
		RLMap.allFunc = rlfunc;
		TemplateShape ts = new TemplateShape();
		string[] shapeNames = Directory.GetFiles (Application.dataPath+"/Shape","*.xml");
		RLLevel.allShape = new Shape[shapeNames.Length];
		for (int i = 0; i < RLLevel.allShape.Length; i++)
			RLLevel.allShape [i] = new Shape ();
		for (int i = 0; i < shapeNames.Length; i++) 
		{
			ts.input (shapeNames[i]);
			for (int j = 0; j < ts.door.Length; j++)
			{
				RLLevel.allShape [i].door[j] = ts.door[j];
			}
			RLLevel.allShape[i].shape = new bool[ts.ExistArray.Length][];
			for (int j = 0; j < ts.ExistArray.Length; j++)
			{
				RLLevel.allShape[i].shape[j] = new bool[ts.ExistArray[0].Length];
				for (int k = 0; k < ts.ExistArray [0].Length; k++)
				{
					RLLevel.allShape [i].shape [j] [k] = ts.ExistArray [j] [k];
 				}
			}
			RLLevel.allShape [i].index = i;
			RLLevel.allShape [i].name = getName (shapeNames [i]);
		}
		TemplatePropTable tpt = new TemplatePropTable();
		tpt.output (Application.dataPath+"/PropTable/data.xml");
		RLMap.defaultTileDatabase = new GameObject[tpt.allDT.Length];
		RLMap.StyleDefaultTileArray = new int[tpt.allDT.Length][];
		for (int i = 0; i < tpt.allDT.Length; i++)
		{
			RLMap.defaultTileDatabase [i] = Resources.Load (tpt.allDT[i].prefabpath) as GameObject;
			RLMap.StyleDefaultTileArray[i] = new int[1];
			RLMap.StyleDefaultTileArray [i] [0] = i;
		}
		RLMap.MapIndexDatabase = new GameObject[tpt.allProp.Length];
		RLMap.TileDatabase = new GameObject[tpt.allProp.Length];
		RLMap.StyleAIArray = new int[tpt.Groups.Length][];
		RLMap.StyleFuncObjectArray = new int[tpt.Groups.Length][];
		RLMap.StyleTileArray = new int[tpt.Groups.Length][];
		List<int>[] ai_list = new List<int>[tpt.Groups.Length]; 
		List<int>[] funcobject_list = new List<int>[tpt.Groups.Length]; 
		List<int>[] tile_list = new List<int>[tpt.Groups.Length]; 
		for (int i = 0; i < tpt.Groups.Length; i++) 
		{
			ai_list [i] = new List<int> ();
			funcobject_list [i] = new List<int> ();
			tile_list [i] = new List<int> ();
		}
		for (int i = 0; i < tpt.allProp.Length; i++)
		{
			GameObject temp = Resources.Load (tpt.allProp [i].prefabPath) as GameObject;
			if (temp.GetComponent<SpriteRenderer> ())
			{
				RLMap.MapIndexDatabase [i] = temp;
				RLMap.TileDatabase [i] = temp;
				int index =0 ;
				for (int j = 0; j < tpt.Groups.Length; j++) 
				{
					if (tpt.Groups [j] == tpt.allProp [i].GroupName)
					{
						index = j;
						break;
					}
				}
				if (temp.GetComponent<PropEditorTexture> ().type == 5)
					ai_list [index].Add (i);
				if (temp.GetComponent<PropEditorTexture> ().type == 6)
					tile_list[index].Add (i);
				if (temp.GetComponent<PropEditorTexture> ().type == 8)
					funcobject_list [index].Add (i);
			}
			else
			{
				RLMap.MapIndexDatabase [i] = temp;	
			}
		}

		for (int i = 0; i < tpt.Groups.Length; i++) 
		{
			RLMap.StyleAIArray [i] = ai_list [i].ToArray ();
			RLMap.StyleFuncObjectArray [i] = funcobject_list [i].ToArray ();
			RLMap.StyleTileArray [i] = tile_list [i].ToArray ();
		}

		List<Map>[][][] maplist = new List<Map>[tpt.Groups.Length][][];
		RLMap.MapDataBase_IndexBy_Style_Func_Shape = new Map[tpt.Groups.Length][][][];
		for (int i = 0; i < tpt.Groups.Length; i++)
		{
			maplist[i] = new List<Map>[rlfunc.Length][];
			RLMap.MapDataBase_IndexBy_Style_Func_Shape[i] = new Map[rlfunc.Length][][];
			for (int j = 0; j < rlfunc.Length; j++) 
			{
				maplist[i][j] = new List<Map>[shapeNames.Length];
				for (int k = 0; k < shapeNames.Length; k++)
					maplist [i] [j] [k] = new List<Map> ();
				RLMap.MapDataBase_IndexBy_Style_Func_Shape[i][j] = new Map[shapeNames.Length][];
			}
		}
		TemplateMap tm = new TemplateMap();
		string[] mapNames = Directory.GetFiles (Application.dataPath+"/Map","*.xml");
		for (int i = 0; i < mapNames.Length; i++) 
		{
			tm.output (mapNames[i]);
			Map tempMap = new Map();
			tempMap.func = tm.func;
			tempMap.defaultTile = tm.defaultTile;
			tempMap.ShapeName = tm.shapename;
			tempMap.style = tm.style;
			tempMap.mapStruct = tm.map;
			int index = 0;
			for (int j = 0; j < shapeNames.Length; j++) 
			{
				if (RLLevel.allShape[j].name == tempMap.ShapeName)
				{
					index = j;
					break;
				}
			}
			for(int j=0;j<tempMap.mapStruct.Length;j++)
			{
				for (int k = 0; k < tempMap.mapStruct [0].Length; k++) 
				{
					for (int l = 0; l < tempMap.mapStruct [0] [0].Length; l++)
					{
						if (tempMap.mapStruct [j] [k] [l] >= 0&&tempMap.mapStruct[j][k][l]<=RLMap.MapIndexDatabase.Length-1)
						{
							if (RLMap.MapIndexDatabase [tempMap.mapStruct[j][k][l]].GetComponent<PropEditorTexture> ().type <= -4) 
							{
								tempMap.mapStruct[j][k][l] = RLMap.MapIndexDatabase [tempMap.mapStruct[j][k][l]].GetComponent<PropEditorTexture> ().type;
							}

						}
					}
				}
			}
			maplist [tempMap.style] [tempMap.func] [index].Add (tempMap);	
		}
		for (int i = 0; i < tpt.Groups.Length; i++)
			for (int j = 0; j < rlfunc.Length; j++)
				for (int k = 0; k < RLLevel.allShape.Length; k++)
					RLMap.MapDataBase_IndexBy_Style_Func_Shape [i] [j] [k] = maplist [i] [j] [k].ToArray ();
	}
}
