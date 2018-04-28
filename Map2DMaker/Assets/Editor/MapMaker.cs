using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Map2D;
using System.IO;
public class MapMaker : EditorWindow 
{
	static MapMaker mmwindow;
	static PropMakerData pmd;
	static string tempName;
	int[][][] PropMap;
	Rect[][][] RenderRect;
	int tempUILayout = 0;
	bool isPreview = false;
	int defaultTile = 0;
	static PropAllTable ptallManager;
	static DefaultTileTable dtManager;
	static TemplateMap templateMap;
	string textShape="",textMap="";
	int lastContainX=-1,lastContainY=-1;
	bool IsDelete  = false;
	int style;
	int func;
	[MenuItem("Map2DMaker/MapMaker")]
	static void Init()
	{
		mmwindow = (MapMaker)EditorWindow.GetWindow (typeof(MapMaker), false,"MapMaker");
		mmwindow.Show ();
		pmd = GameObject.FindGameObjectWithTag ("PropMakerData").GetComponent<PropMakerData>();
		ptallManager = GameObject.FindGameObjectWithTag ("PropAllTableManager").GetComponent<PropAllTable> ();
		dtManager = GameObject.FindGameObjectWithTag ("DefaultTileTableManager").GetComponent<DefaultTileTable> ();
		tempName = "";
		templateMap = new TemplateMap ();
	}
	void ReSizeRenderRect()
	{
		float minX = 20;
		float minY = 20;
		float maxX = 600;
		float maxY = 600;
		float deltaX = (maxX - minX) / PropMap.Length;
		float deltaY = (maxY - minY) / PropMap [0].Length;
		RenderRect = new Rect[PropMap.Length][][];
		for (int i = 0; i < PropMap.Length; i++)
		{
			RenderRect[i] = new Rect[PropMap[0].Length][];
			for (int j = 0; j < PropMap [0].Length; j++)
			{
				RenderRect[i][j] = new Rect[PropMap[0][0].Length];
				for (int k = 0; k < PropMap [0] [0].Length; k++)
				{
					if (PropMap [i] [j] [k] >= 0)
						changeRect (i, j, k, ptallManager.find (PropMap [i] [j] [k]).RangeTable.Length, ptallManager.find (PropMap [i] [j] [k]).RangeTable [0].Length);
					else
						RenderRect [i] [j] [k] = new Rect (minX+i*deltaX,minY+j*deltaY,deltaX,deltaY);
				}
			}
		}
	}
	void changeRect(int x,int y,int k,int scaleX,int scaleY)
	{
		float minX = 20;
		float minY = 20;
		float maxX = 600;
		float maxY = 600;
		float deltaX = (maxX - minX) / PropMap.Length;
		float deltaY = (maxY - minY) / PropMap [0].Length;
		RenderRect [x] [y] [k] = new Rect (minX+x*deltaX-deltaX*(float)(scaleX/2),minY+y*deltaY-deltaY*(float)(scaleY/2),deltaX*scaleX,deltaY*scaleY);
	}
	void ReFreshRect(int x,int y,int k)
	{
		float minX = 20;
		float minY = 20;
		float maxX = 600;
		float maxY = 600;
		float deltaX = (maxX - minX) / PropMap.Length;
		float deltaY = (maxY - minY) / PropMap [0].Length;
		RenderRect [x] [y] [k] = new Rect (minX+x*deltaX,minY+y*deltaY,deltaX,deltaY);
	}
	TemplateShape selectShape(string name)
	{
		TemplateShape ts = new TemplateShape();
		ts.input (Application.dataPath+"/Shape/"+name+".xml");
		return ts;
	}
	bool selectMap(string name)
	{
		templateMap.output (name);
		PropMap = templateMap.map;
		defaultTile = templateMap.defaultTile;
		string realname="";
		int mark=-1;
		for (int i = name.Length - 1; i >= 0; i--) 
		{
			if (name [i] == '/')
			{
				mark = i;
				break;
			}
		}
		realname = name.Substring (mark+1,name.Length-1-mark-4);
		tempName = realname;
		ReSizeRenderRect ();
		return true;
	}
	void saveMap(object name)
	{
		string pathname = (string)name;
		templateMap.TransferMap (PropMap,defaultTile,style,func,textShape);
		templateMap.input (Application.dataPath+"/Map/"+pathname+".xml");
	}
	bool createMap(string name,TemplateShape templateshape)
	{
		tempName = name;
		if (File.Exists (Application.dataPath + "/Map/" + name+".xml"))
			return false;
		else
		{
			int lengthX = templateshape.sizeX;
			int lengthY = templateshape.sizeY;
			PropMap = new int[lengthX][][];
			for (int i = 0; i < lengthX; i++) 
			{
				PropMap[i] = new int[lengthY][];
				for (int j = 0; j < lengthY; j++)
				{
					PropMap [i] [j] = new int[1];
					PropMap [i] [j] [0] = -1;
				}
				for (int j = 0; j < lengthY; j++)
				{
					if (templateshape.ExistArray [i] [j])
						PropMap [i] [j] [0] = -2;
				}
			}
			ReSizeRenderRect ();
			return true;
		}
	}
	void deleteMap(object name)
	{
		tempName = "";
		tempUILayout = 0;
		defaultTile = 0;
		isPreview = false;
		PropMap = null;
		RenderRect = null;
	}
	void pushAProp(int x,int y,bool[][] exist,int propindex)
	{
		int minX = x - exist.Length / 2;
		int maxX = x + exist.Length / 2;
		int minY = y - exist [0].Length / 2;
		int maxY = y + exist [0].Length / 2;
		changeRect (x, y, tempUILayout, exist.Length, exist [0].Length);
		minX = Mathf.Max (minX,0);
		minY = Mathf.Max (minY,0);
		maxX = Mathf.Min (maxX,PropMap.Length-1);
		maxY = Mathf.Min(maxY,PropMap[0].Length-1);
		for (int i = minX; i <= maxX; i++)
		{
			for (int j = minY; j <= maxY; j++)
			{
				PropMap [i] [j] [tempUILayout] = -3;
			}
		}
		PropMap [x] [y] [tempUILayout] = propindex;
	}
	void pickAProp(int x,int y)
	{
		if (PropMap [x] [y] [tempUILayout] >= 0)
		{
			ReFreshRect (x, y, tempUILayout);
			int tempPropIndex = PropMap [x] [y] [tempUILayout];
			bool[][] tempOcculde = ptallManager.find (tempPropIndex).RangeTable;
			int minX = x - tempOcculde.Length / 2;
			int maxX = x + tempOcculde.Length / 2;
			int minY = y - tempOcculde [0].Length / 2;
			int maxY = y + tempOcculde [0].Length / 2;
			minX = Mathf.Max (minX,0);
			minY = Mathf.Max (minY,0);
			maxX = Mathf.Min (maxX,PropMap.Length-1);
			maxY = Mathf.Min(maxY,PropMap[0].Length-1);
			for (int i = minX; i <= maxX; i++)
			{
				for (int j = minY; j <= maxY; j++)
				{
					PropMap [i] [j] [tempUILayout] = -2;
				}
			}
		}
	}
	bool checkPushValid(int x,int y,bool[][] exist)
	{
		int minX = x - exist.Length / 2;
		int maxX = x + exist.Length / 2;
		int minY = y - exist [0].Length / 2;
		int maxY = y + exist [0].Length / 2;
		if (minX < 0 || minY < 0 || maxX > PropMap.Length - 1 || maxY > PropMap [0].Length - 1)
			return false;
		minX = Mathf.Max (minX,0);
		minY = Mathf.Max (minY,0);
		maxX = Mathf.Min (maxX,PropMap.Length-1);
		maxY = Mathf.Min(maxY,PropMap[0].Length-1);
		for (int i = minX; i <= maxX; i++) 
		{
			for (int j = minY; j <= maxY; j++) 
			{
				if (!exist [i - minX] [j - minY]&&PropMap[i][j][tempUILayout]!=-2) 
				{
					return false;
				}
			}
		}
		return true;
	}
	void changeLayout(object index)
	{
		if (lastContainX != -1 && lastContainY != -1 && PropMap [lastContainX] [lastContainY] [tempUILayout] < 0) 
		{
			ReFreshRect (lastContainX,lastContainY,tempUILayout);
			lastContainX = -1;
			lastContainY = -1;
		}
		tempUILayout = (int)index;
	}
	void changeDefaultTile(object index)
	{
		defaultTile = (int)index;
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
	void createLayout(object o)
	{
		int tempMaxLayout = PropMap [0] [0].Length;
		int[][][] newPropMap = new int[PropMap.Length][][];
		for(int i=0;i<PropMap.Length;i++)
		{
			newPropMap[i] = new int[PropMap[0].Length][];
			for(int j=0;j<PropMap[0].Length;j++)
				newPropMap[i][j] = new int[PropMap[0][0].Length+1];
		}
		for(int i=0;i<newPropMap.Length;i++)
		{
			for (int j = 0; j < newPropMap [0].Length; j++)
			{
				for (int k = 0; k < newPropMap [0] [0].Length; k++)
				{
					if (k < newPropMap [0] [0].Length - 1)
						newPropMap [i] [j] [k] = PropMap [i] [j] [k];
					else
					{
						if (PropMap [i] [j] [0] == -1)
							newPropMap [i] [j] [k] = -1;
						else
							newPropMap [i] [j] [k] = -2;
					}
				}
			}
		}
		float minX = 20;
		float minY = 20;
		float maxX = 600;
		float maxY = 600;
		float deltaX = (maxX - minX) / PropMap.Length;
		float deltaY = (maxY - minY) / PropMap [0].Length;
		PropMap = newPropMap;
		Rect[][][] newRect = new Rect[PropMap.Length][][];
		for (int i = 0; i < PropMap.Length; i++)
		{
			newRect[i] = new Rect[PropMap[0].Length][];
			for (int j = 0; j < PropMap [0].Length; j++)
			{
				newRect[i][j] = new Rect[PropMap[0][0].Length];
				for (int k = 0; k < newRect [i] [j].Length; k++) 
				{
					if (k < newPropMap [0] [0].Length - 1)
						newRect [i] [j] [k] = RenderRect [i] [j] [k];
					else
						newRect [i] [j] [k] = new Rect (minX+deltaX*i,minY+deltaY*j,deltaX,deltaY);
				}
			}
		}
		RenderRect = newRect;
	}
	void changePreviewState(object o)
	{
		isPreview = !isPreview;
	}
	void changeDeleteState(object o)
	{
		IsDelete = !IsDelete;
	}
	void OnGUI()
	{
		if (tempName != "") 
		{
			if (lastContainX != -1 && lastContainY != -1 && PropMap [lastContainX] [lastContainY] [tempUILayout] < 0) 
			{
				ReFreshRect (lastContainX,lastContainY,tempUILayout);
				lastContainX = -1;
				lastContainY = -1;
			}
			if (Event.current.type == EventType.MouseDown && EditorWindow.focusedWindow == mmwindow && Event.current.button == 1) 
			{
				GenericMenu menu = new GenericMenu ();
				menu.AddItem (new GUIContent ("save"), false, saveMap, tempName);
				menu.AddItem (new GUIContent ("delete"), false, deleteMap, tempName);
				menu.AddItem (new GUIContent("createLayout"),false,createLayout,null);
				menu.AddItem (new GUIContent("changeUILayout"),false,changeLayout,(tempUILayout+1)%PropMap[0][0].Length);
				menu.AddItem (new GUIContent ("changeDefaultTile"), false, changeDefaultTile, (defaultTile + 1) % dtManager.tempIndex);
				menu.AddItem (new GUIContent ("changePreviewState"), false, changePreviewState, null);
				menu.AddItem (new GUIContent("changeDeleteState"),false,changeDeleteState,null);
				menu.ShowAsContext ();
				Event.current.Use ();
			}
			float minX = 20;
			float minY = 20;
			float maxX = 600;
			float maxY = 600;
			float deltaX = (maxX - minX) / PropMap.Length;
			float deltaY = (maxY - minY) / PropMap [0].Length;
			if (!isPreview) 
			{
				if (IsDelete)
				{
					int containX=-1, containY=-1;
					for (int k = -1; k < RenderRect [0] [0].Length; k++) 
					{
						for (int i = 0; i < RenderRect.Length; i++)
						{
							for (int j = 0; j < RenderRect [0].Length; j++) 
							{

								if (k == -1)
								{
									if (PropMap [i] [j] [0] != -1)
									{
										Texture2D tempdefaultTileTex = dtManager.find (defaultTile).GetComponent<SpriteRenderer> ().sprite.texture;
										Rect r = new Rect (minX + i * deltaX, minY + j * deltaY, deltaX, deltaY);
										GUI.DrawTexture (r, tempdefaultTileTex);
									}
									continue;
								}

								if (PropMap [i] [j] [k] >= 0) 
								{
									Texture2D tempTex = findTexturebyGameobject (ptallManager.find (PropMap [i] [j] [k]).prefab);
									if (RenderRect [i] [j] [k].Contains (Event.current.mousePosition) && k == tempUILayout)
									{
										containX = i;
										containY = j;
										GUI.DrawTexture (RenderRect [i] [j] [k], tempTex, ScaleMode.StretchToFill, true, 0, Color.red, 0, 0);
									}
									if (k != tempUILayout)
										GUI.DrawTexture (RenderRect [i] [j] [k], tempTex, ScaleMode.StretchToFill, true, 0, Color.gray, 0, 0);
									else
										GUI.DrawTexture (RenderRect [i] [j] [k], tempTex, ScaleMode.StretchToFill, true, 0, Color.white, 0, 0);
								}
							
							}
						}
					}
					if (Event.current.type == EventType.MouseDown && Event.current.button == 0&&containX!=-1&&containY!=-1)
						pickAProp (containX, containY);
				} 
				else
				{
					int containX=-1, containY=-1;
					for (int k = -1; k < RenderRect [0] [0].Length; k++) 
					{
						for (int i = 0; i < RenderRect.Length; i++) 
						{
							for (int j = 0; j < RenderRect [0].Length; j++) 
							{

								if (k == -1) 
								{
									if (PropMap [i] [j] [0] != -1) 
									{
										Texture2D tempdefaultTileTex = dtManager.find (defaultTile).GetComponent<SpriteRenderer> ().sprite.texture;
										Rect r = new Rect (minX + i * deltaX, minY + j * deltaY, deltaX, deltaY);
										GUI.DrawTexture (r, tempdefaultTileTex);
									}
									continue;
								}
								if (PropMap [i] [j] [k] >= 0) 
								{
									Texture2D tempTex = findTexturebyGameobject (ptallManager.find (PropMap [i] [j] [k]).prefab);
									if (k != tempUILayout)
										GUI.DrawTexture (RenderRect [i] [j] [k], tempTex, ScaleMode.StretchToFill, true, 0, Color.gray, 0, 0);
									else
										GUI.DrawTexture (RenderRect [i] [j] [k], tempTex, ScaleMode.StretchToFill, true, 0, Color.white, 0, 0);
								}
								if (RenderRect [i] [j] [k].Contains (Event.current.mousePosition) && k == tempUILayout && PropMap [i] [j] [k] != -1) 
								{
									containX = i;
									containY = j;
								}
							
							}
						}
					}
					if (containX != -1 && containY != -1) 
					{
						changeRect (containX,containY,tempUILayout,pmd.occlude.Length,pmd.occlude[0].Length);
						if (checkPushValid (containX, containY, pmd.occlude))
							GUI.DrawTexture (RenderRect [containX] [containY] [tempUILayout], pmd.tempTex,ScaleMode.StretchToFill,true,0,Color.green,0,0);
						else
							GUI.DrawTexture (RenderRect [containX] [containY] [tempUILayout], pmd.tempTex,ScaleMode.StretchToFill,true,0,Color.red,0,0);
						if (Event.current.type == EventType.MouseDown && Event.current.button == 0&&checkPushValid(containX,containY,pmd.occlude))
							pushAProp (containX, containY, pmd.occlude, pmd.tempIndex);
					}
					lastContainX = containX;
					lastContainY = containY;
				}
			}
			else
			{
				for (int k = -1; k < RenderRect [0] [0].Length; k++)
				{
					for (int i = 0; i < RenderRect.Length; i++) 
					{
						for (int j = 0; j < RenderRect [0].Length; j++)
						{

							if (k == -1)
							{
								Texture2D tempdefaultTileTex = dtManager.find (defaultTile).GetComponent<SpriteRenderer> ().sprite.texture;
								Rect r = new Rect (minX + i * deltaX, minY + j * deltaY, deltaX, deltaY);
								if (PropMap [i] [j] [0] != -1)
									GUI.DrawTexture (r, tempdefaultTileTex);
							} 
							else if (PropMap [i] [j] [k] >= 0) 
							{
								Texture2D tempTex = findTexturebyGameobject (ptallManager.find (PropMap [i] [j] [k]).prefab);
								GUI.DrawTexture (RenderRect [i] [j] [k], tempTex, ScaleMode.StretchToFill, true);
							}
						
						}
					}
				}
			}
		}
		else 
		{
			textMap = EditorGUILayout.TextField("MapName",textMap);
			textShape = EditorGUILayout.TextField ("ShapeName",textShape);
			style = EditorGUILayout.IntField ("Style",style);
			func = EditorGUILayout.IntField ("Function",func);
			if (GUILayout.Button ("create")) 
			{
				TemplateShape tempshape= selectShape (textShape);
				if (tempshape != null) 
				{
					if (!createMap (textMap, tempshape)) 
					{
						Debug.LogError ("创建失败");
					}
				}
			}
			if(Event.current.type==EventType.DragUpdated&&mmwindow.position.Contains(Event.current.mousePosition))
			{

				string path = DragAndDrop.paths [0];
				if (!selectMap (path))
				{
					Debug.LogError ("创建失败");
				}
				DragAndDrop.AcceptDrag ();
				Event.current.Use();
			}
		}
	}

}
