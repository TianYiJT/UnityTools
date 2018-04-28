using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Map2D;
public class ShapeMaker : EditorWindow
{
	static Texture2D defaultTex;
	static string defaultPath="Texture/white";
	static Rect[] RenderRect;
	static Rect[] LineRect;
	static bool[] isUseful;
	static ShapeMaker smwindow;
	static Vector2 deltaSize;
	int tempSizeX=1;
	int tempSizeY=1;
	float defaultSize = 10;
	static TemplateShape templateShape;
	int sizeX=0,sizeY=0;
	string code="";
	string name;
	[MenuItem("Map2DMaker/ShapeMaker")]
	static void Init()
	{
		defaultTex = (Texture2D)Resources.Load (defaultPath, typeof(Texture2D));
		smwindow = (ShapeMaker)EditorWindow.GetWindow (typeof(ShapeMaker),false,"ShapeMaker");
		smwindow.Show ();
		deltaSize = new Vector2 (30,30);
		templateShape = new TemplateShape ();
	}

	void OnGUI()
	{
		if (RenderRect != null)
		{
			float scale = Mathf.Max (defaultSize/tempSizeX,defaultSize/tempSizeY);
			Vector2 tempdeltaSize = deltaSize*scale;
			Vector2 startPos = new Vector2 (20,120);
			for (int i = 0; i < RenderRect.Length; i++)
			{
				RenderRect [i] = new Rect (startPos.x+i/tempSizeY*tempdeltaSize.x,startPos.y+i%tempSizeY*tempdeltaSize.y,tempdeltaSize.x,tempdeltaSize.y);
				if (RenderRect [i].Contains (Event.current.mousePosition))
				{
					if (isUseful [i])
						GUI.DrawTexture (RenderRect[i],defaultTex,ScaleMode.StretchToFill,false,0,Color.red,0,0);
					else
						GUI.DrawTexture (RenderRect[i],defaultTex,ScaleMode.StretchToFill,false,0,Color.green,0,0);
				}
				else
				{
					if (isUseful [i])
						GUI.DrawTexture (RenderRect[i],defaultTex,ScaleMode.StretchToFill,false,0,Color.green,0,0);
					else
						GUI.DrawTexture (RenderRect[i],defaultTex,ScaleMode.StretchToFill,false,0,Color.red,0,0);
				}
			}
			for (int i = 0; i < tempSizeY - 1; i++) 
			{
				LineRect [i] = new Rect (20,(i+1)*tempdeltaSize.y+120,600,2);
				GUI.DrawTexture (LineRect[i],defaultTex,ScaleMode.StretchToFill,false,0,Color.white,0,0);
			}
			for (int i = tempSizeY-1; i < tempSizeX+tempSizeY - 2; i++) 
			{
				LineRect [i] = new Rect (20+(i-(tempSizeY-1)+1)*tempdeltaSize.x,120,2,600);
				GUI.DrawTexture (LineRect[i],defaultTex,ScaleMode.StretchToFill,false,0,Color.white,0,0);
			}
			if (Event.current.type == EventType.MouseDown && Event.current.button == 1 && EditorWindow.focusedWindow == smwindow) 
			{
				for(int i=0;i<RenderRect.Length;i++)
				{
					if (RenderRect [i].Contains (Event.current.mousePosition))
					{
						isUseful [i] = !isUseful [i];
					}
				}
			}
		}
		sizeX=EditorGUILayout.IntField ("sizeX",sizeX);
		sizeY = EditorGUILayout.IntField ("sizeY",sizeY);
		name = EditorGUILayout.TextField ("形状名称",name);
		code = EditorGUILayout.TextField ("doorcode",code);
		if (GUILayout.Button("生成"))
		{
			isUseful = new bool[sizeX*sizeY];
			for (int i = 0; i < sizeX * sizeY; i++)
				isUseful [i] = true;
			RenderRect = new Rect[sizeX*sizeY];
			LineRect = new Rect[sizeX+sizeY-2];
			tempSizeX = sizeX;
			tempSizeY = sizeY;
		}
		if (GUILayout.Button ("保存")) 
		{
			bool[][] ExistArray = new bool[tempSizeX][];
			for (int i = 0; i < ExistArray.Length; i++)
				ExistArray [i] = new bool[tempSizeY];
			for (int i = 0; i < tempSizeX; i++)
				for (int j = 0; j < tempSizeY; j++)
					ExistArray [i] [j] = isUseful [i * tempSizeY + j];
			if (!templateShape.CheckShapeIsValidOrNot (ExistArray))
				Debug.LogError ("非法形状");
			else 
			{
				bool[] door = new bool[4];
				for (int i = 0; i < 4; i++)
				{
					if (code [i] == '1')
						door [i] = true;
					else
						door [i] = false;
				}
				templateShape.TransferShape (ExistArray,door);
				templateShape.output (Application.dataPath+"/Shape/"+name+".xml");
			}	
		}
	}
}
