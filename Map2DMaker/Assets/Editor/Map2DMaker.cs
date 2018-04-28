using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Map2D;
namespace Map2DMaker
{
	public class PropMaker:EditorWindow
	{
		static GameObject[] ptmanager;
		static GameObject dtmanager;
		static GameObject ptallmanager;
		static Texture2D[] PropRenderTexture;
		static int[] PropIndex;
		static int tempIndex = 0;
		static Rect[] RenderRect;
		static Vector2 windowSize;
		static Vector2 standardSize;
		static PropMaker pmwindow;
		bool IsCreatingTable = false;
		static PropMakerData pmd;
		string TextInput="";
		static GameObject templateGameobject;
		[MenuItem("Map2DMaker/PropMaker")]
		static void Init()
		{
			templateGameobject = GameObject.FindGameObjectWithTag ("TemplateGameObject");
			standardSize = new Vector2(templateGameobject.GetComponent<SpriteRenderer>().bounds.size.x,templateGameobject.GetComponent<SpriteRenderer>().bounds.size.y);
			pmd = GameObject.FindGameObjectWithTag ("PropMakerData").GetComponent<PropMakerData>();
			tempIndex = 0;
			pmwindow = (PropMaker)EditorWindow.GetWindow (typeof(PropMaker),false,"PropMaker");
			windowSize = pmwindow.maxSize;
			pmwindow.Show ();
			ptmanager = GameObject.FindGameObjectsWithTag ("PropTableManager");
			dtmanager = GameObject.FindGameObjectWithTag ("DefaultTileTableManager");
			ptallmanager = GameObject.FindGameObjectWithTag ("PropAllTableManager");
			Vector2 deltaSize = new Vector2 (windowSize.x/50,windowSize.y/50);
		//	Debug.Log (windowSize.ToString());
			if (ptmanager.Length!=0) 
			{
				PropRenderTexture = new Texture2D[ptmanager [tempIndex].GetComponent<PropTable>().tempIndex];
				RenderRect = new Rect[ptmanager [tempIndex].GetComponent<PropTable>().tempIndex];
				PropIndex = new int[ptmanager [tempIndex].GetComponent<PropTable>().tempIndex];
				for (int i = 0; i < ptmanager [tempIndex].GetComponent<PropTable>().tempIndex; i++) 
				{
					GameObject temp = ptmanager [tempIndex].GetComponent<PropTable>().find (i).prefab;
					PropIndex [i] = ptmanager [tempIndex].GetComponent<PropTable>().find (i).index;
					PropRenderTexture [i] = findTexturebyGameobject (ptmanager [tempIndex].GetComponent<PropTable>().find (i).prefab);
					int x1 = i / 10;
					int y1 = i % 10;
					RenderRect [i].center = new Vector2 (deltaSize.x/2+x1*deltaSize.x,deltaSize.y/2+y1*deltaSize.y);
					RenderRect [i].size = deltaSize;
				}
				if (PropRenderTexture.Length != 0) 
				{
					pmd.tempTex = PropRenderTexture [0];
					pmd.tempIndex = PropIndex [0];
					pmd.occlude = ptallmanager.GetComponent<PropAllTable> ().find (PropIndex [0]).RangeTable;
					pmd.selectIndex = 0;
				}
			} 
			else if (dtmanager != null) 
			{
				tempIndex = -1;
				PropRenderTexture = new Texture2D[dtmanager.GetComponent<DefaultTileTable>().tempIndex];
				PropIndex = new int[dtmanager.GetComponent<DefaultTileTable>().tempIndex];
				RenderRect = new Rect[dtmanager.GetComponent<DefaultTileTable>().tempIndex];
				for (int i = 0; i < dtmanager.GetComponent<DefaultTileTable>().tempIndex; i++) 
				{
					PropIndex [i] = i;
					PropRenderTexture [i] = findTexturebyGameobject (dtmanager.GetComponent<DefaultTileTable>().find(i));
					int x1 = i / 10;
					int y1 = i % 10;
					RenderRect [i].center = new Vector2 (deltaSize.x/2+x1*deltaSize.x,deltaSize.y/2+y1*deltaSize.y);
					RenderRect [i].size = deltaSize;
				}
			}
		}
		static Texture2D findTexturebyGameobject(GameObject g)
		{
			Texture2D temp;
			if (g.GetComponent<SpriteRenderer> () != null)
			{
				temp = g.GetComponent<SpriteRenderer> ().sprite.texture;
			}
			else
			{
				temp = g.GetComponent<PropEditorTexture> ().mytex;
			}
			return temp;
		}
		static Vector2 findSizebyGameobject(GameObject g)
		{
			Vector2 result = new Vector2(0,0);
			if (g.GetComponent<SpriteRenderer> () != null) 
			{
				result.x = g.GetComponent<SpriteRenderer> ().bounds.size.x;
				result.y = g.GetComponent<SpriteRenderer> ().bounds.size.y;
			}
			else
			{
				result = g.GetComponent<PropEditorTexture> ().bounds;
				result.x *= standardSize.x;
				result.y *= standardSize.y;
			}
			return result;
		}
		void createTable(object obj)
		{
			int type = (int)obj;
			if (type == 0) 
			{
				IsCreatingTable = true;
			}
			else
			{
				if (dtmanager != null) 
				{
					Destroy (dtmanager);
				}
				dtmanager = new GameObject ();
				dtmanager.AddComponent<DefaultTileTable> ();
				dtmanager.tag = "DefaultTileTableManager";
				tempIndex = -1;
				PropRenderTexture = null;
				RenderRect = null;
				PropIndex = null;
			}
		}
		void selectTable(object obj)
		{
			int type = (int)obj;
			tempIndex = type;
			Vector2 deltaSize = new Vector2 (windowSize.x/50,windowSize.y/50);
			if (type == -1) 
			{
				PropRenderTexture = new Texture2D[dtmanager.GetComponent<DefaultTileTable>().tempIndex];
				PropIndex = new int[dtmanager.GetComponent<DefaultTileTable>().tempIndex];
				RenderRect = new Rect[dtmanager.GetComponent<DefaultTileTable>().tempIndex];
				for (int i = 0; i < dtmanager.GetComponent<DefaultTileTable>().tempIndex; i++) 
				{
					PropIndex [i] = i;
					PropRenderTexture [i] = findTexturebyGameobject (dtmanager.GetComponent<DefaultTileTable>().find(i));
					int x1 = i / 10;
					int y1 = i % 10;
					RenderRect [i].center = new Vector2 (deltaSize.x/2+x1*deltaSize.x,deltaSize.y/2+y1*deltaSize.y);
					RenderRect [i].size = deltaSize;
				}
			}
			else 
			{
				PropRenderTexture = new Texture2D[ptmanager [tempIndex].GetComponent<PropTable>().tempIndex];
				RenderRect = new Rect[ptmanager [tempIndex].GetComponent<PropTable>().tempIndex];
				PropIndex = new int[ptmanager [tempIndex].GetComponent<PropTable>().tempIndex];
				for (int i = 0; i < ptmanager [tempIndex].GetComponent<PropTable>().tempIndex; i++) 
				{
					GameObject temp = ptmanager [tempIndex].GetComponent<PropTable>().find (i).prefab;
					PropIndex [i] = ptmanager [tempIndex].GetComponent<PropTable>().find (i).index;
					PropRenderTexture [i] = findTexturebyGameobject (ptmanager [tempIndex].GetComponent<PropTable>().find (i).prefab);
					int x1 = i / 10;
					int y1 = i % 10;
					RenderRect [i].center = new Vector2 (deltaSize.x/2+x1*deltaSize.x,deltaSize.y/2+y1*deltaSize.y);
					RenderRect [i].size = deltaSize;
				}
			}
		}
		bool InsertProp(object[] g_,string[] paths)
		{
			for (int i = 0; i < paths.Length; i++)
			{
				paths [i] = paths [i].Substring (17);
				int mark=paths[i].Length;
				for (int j = 0; j < paths [i].Length; j++) 
				{
					if (paths [i] [j] == '.')
					{
						mark = j;
						break;
					}
				}
				paths [i] = paths [i].Substring (0,mark);
			}
			Vector2 deltaSize = new Vector2 (windowSize.x/50,windowSize.y/50);
			GameObject[] g = new GameObject[g_.Length];
			for (int i = 0; i < g_.Length; i++)
			{
				g [i] = (GameObject)g_ [i];
			}
			if (tempIndex == -1) 
			{
				for (int i = 0; i < g.Length; i++)
				{
					for (int j = 0; j < dtmanager.GetComponent<DefaultTileTable> ().tempIndex; j++)
					{
						if (g [i] == dtmanager.GetComponent<DefaultTileTable> ().find (j))
							return false;
					}
					dtmanager.GetComponent<DefaultTileTable> ().AddProp (g [i],paths[i]);
					PropRenderTexture = new Texture2D[dtmanager.GetComponent<DefaultTileTable>().tempIndex];
					PropIndex = new int[dtmanager.GetComponent<DefaultTileTable>().tempIndex];
					RenderRect = new Rect[dtmanager.GetComponent<DefaultTileTable>().tempIndex];
					for (int j = 0; j < dtmanager.GetComponent<DefaultTileTable>().tempIndex; j++) 
					{
						PropIndex [j] = j;
						PropRenderTexture [j] = findTexturebyGameobject (dtmanager.GetComponent<DefaultTileTable>().find(j));
						int x1 = j / 10;
						int y1 = j % 10;
						RenderRect [j].center = new Vector2 (deltaSize.x/2+x1*deltaSize.x,deltaSize.y/2+y1*deltaSize.y);
						RenderRect [j].size = deltaSize;
					}
				}
			}
			else
			{
				for (int i = 0; i < g.Length; i++)
				{
					for (int j = 0; j < ptmanager[tempIndex].GetComponent<PropTable> ().tempIndex; j++)
					{
						if (g [i] == ptmanager[tempIndex].GetComponent<PropTable> ().find (j).prefab)
							return false;
					}
					for (int j = 0; j < ptallmanager.GetComponent<PropAllTable> ().indexall; j++)
					{
						if (g [i] == ptallmanager.GetComponent<PropAllTable> ().find (j).prefab)
							return false;
					}
					Vector2 gameobjectSize = findSizebyGameobject (g [i]);
					int sizeX = (int)((gameobjectSize.x / standardSize.x)+0.5f);
					int sizeY = (int)((gameobjectSize.y / standardSize.y)+0.5f);
					sizeX = (sizeX % 2 == 1) ? sizeX : sizeX + 1;
					sizeY = (sizeY % 2 == 1) ? sizeY : sizeY + 1;
					bool[][] b = new bool[sizeX][];
					for (int j = 0; j < sizeX; j++) 
					{
						b [j] = new bool[sizeY];
						for (int k = 0; k < sizeY; k++)
							b [j] [k] = false;
					}
					int indexProp = ptallmanager.GetComponent<PropAllTable> ().addprop (g [i], b,paths[i]);
					Debug.Log (indexProp);
					ptmanager [tempIndex].GetComponent<PropTable> ().AddProp (g [i], b, indexProp,paths[i]);
					PropRenderTexture = new Texture2D[ptmanager [tempIndex].GetComponent<PropTable>().tempIndex];
					RenderRect = new Rect[ptmanager [tempIndex].GetComponent<PropTable>().tempIndex];
					PropIndex = new int[ptmanager [tempIndex].GetComponent<PropTable>().tempIndex];
					for (int j = 0; j < ptmanager [tempIndex].GetComponent<PropTable>().tempIndex; j++) 
					{
						GameObject temp = ptmanager [tempIndex].GetComponent<PropTable>().find (j).prefab;
						PropIndex [j] = ptmanager [tempIndex].GetComponent<PropTable>().find (j).index;
						PropRenderTexture [j] = findTexturebyGameobject (ptmanager [tempIndex].GetComponent<PropTable>().find (j).prefab);
						int x1 = j / 10;
						int y1 = j % 10;
						RenderRect [j].center = new Vector2 (deltaSize.x/2+x1*deltaSize.x,deltaSize.y/2+y1*deltaSize.y);
						RenderRect [j].size = deltaSize;
					}
				//	int indexProp = ptallmanager.GetComponent<PropAllTable>()
				}
			}
			return true;
		}
		void deletetempProp()
		{
			int selectIndex = pmd.selectIndex;
			ptallmanager.GetComponent<PropAllTable> ().deleteprop (PropIndex[selectIndex]);
			for (int i = 0; i < ptmanager.Length; i++) 
			{
				ptmanager [i].GetComponent<PropTable> ().DeleteProp (PropIndex[selectIndex]);
			}
			for(int i=0;i<PropIndex.Length;i++)
			{
				if (PropIndex [i] > PropIndex [selectIndex])
					PropIndex [i]--;
			}
			Texture2D[] newTex = new Texture2D[PropRenderTexture.Length-1];
			Rect[] newRect = new Rect[RenderRect.Length-1];
			int[] newIndex = new int[PropIndex.Length-1];
			for(int i=0;i<PropRenderTexture.Length;i++)
			{
				if (i < selectIndex) 
				{
					newTex [i] = PropRenderTexture [i];
					newRect [i] = RenderRect [i];
					newIndex [i] = PropIndex [i];
				}
				if (i > selectIndex)
				{
					newTex [i-1] = PropRenderTexture [i];
					newRect [i-1] = RenderRect [i];
					newIndex [i-1] = PropIndex [i];
				}
			}
			PropRenderTexture = newTex;
			RenderRect = newRect;
			PropIndex = newIndex;
		}
		void OnGUI()
		{
			if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && EditorWindow.focusedWindow == pmwindow&&!IsCreatingTable) 
			{
				for (int i = 0; i < RenderRect.Length; i++) 
				{
					if (RenderRect [i].Contains (Event.current.mousePosition))
					{
						pmd.tempTex = PropRenderTexture [i];
						pmd.tempIndex = PropIndex [i];
						pmd.occlude = ptallmanager.GetComponent<PropAllTable> ().find (PropIndex [i]).RangeTable;
						pmd.selectIndex = i;
						break;
					}
				}
			}
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.D&&tempIndex>=0) 
			{
				deletetempProp ();
			}
			Rect windowsRect = pmwindow.position;
			if (Event.current.type == EventType.DragUpdated&&windowsRect.Contains(Event.current.mousePosition))
			{

				InsertProp (DragAndDrop.objectReferences, DragAndDrop.paths);
				DragAndDrop.AcceptDrag ();
				Event.current.Use();
			}
			if (IsCreatingTable)
			{
				GUI.BeginGroup (new Rect(300,250,100,100));

				TextInput = EditorGUI.TextField (new Rect(0,0,200,30),TextInput);
				if (GUI.Button (new Rect (50,50,40,50), "确定")) 
				{
					PropRenderTexture = null;
					RenderRect = null;
					PropIndex = null;
					if (ptmanager != null)
					{
						tempIndex = ptmanager.Length;
						GameObject[] temp = new GameObject[ptmanager.Length+1];
						for(int i=0;i<ptmanager.Length;i++)
						{
							temp [i] = ptmanager [i];
						}
						ptmanager = temp;
						ptmanager [tempIndex] = new GameObject ();
						ptmanager [tempIndex].tag = "PropTableManager";
						ptmanager [tempIndex].AddComponent<PropTable> ();
						ptmanager [tempIndex].GetComponent<PropTable> ().name = TextInput;
					}
					IsCreatingTable = false;
				}
				if (GUI.Button (new Rect (0,50,40,50), "取消")) 
				{
					IsCreatingTable = false;
				}
				GUI.EndGroup ();
			}
			if (RenderRect != null&&!IsCreatingTable) 
			{
				for (int i = 0; i < RenderRect.Length; i++)
				{
					GUI.DrawTexture (RenderRect[i],PropRenderTexture[i],ScaleMode.ScaleToFit);
				//	Debug.Log (RenderRect[i].ToString()+" "+i.ToString());
				}
			}
			if (Event.current.type == EventType.MouseDown&&Event.current.button==1&&EditorWindow.focusedWindow==pmwindow) 
			{
				GenericMenu menu = new GenericMenu ();
				menu.AddItem (new GUIContent("create/PropTable"),false,createTable,0);
				menu.AddItem (new GUIContent("create/DefaultTileTable"),false,createTable,1);
				menu.AddSeparator ("");
				if (ptmanager != null) 
				{
					for (int i = 0; i < ptmanager.Length; i++) 
					{
						string ptmname = ptmanager [i].GetComponent<PropTable> ().name;
						string path = "select/";
						path += ptmname;
						menu.AddItem (new GUIContent(path),false,selectTable,i);
					}
				}
				if (dtmanager != null) 
				{
					menu.AddItem (new GUIContent("select/DefaultTileTable"),false,selectTable,-1);
				}
				menu.ShowAsContext ();
				Event.current.Use ();
			}
		}
	}	
};

