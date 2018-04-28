 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;
namespace Map2D
{
	public abstract class TemplateRL
	{
		public abstract  void output (string path);
		public abstract  void input (string path);
	}
	public class TemplateShape:TemplateRL
	{
		public int sizeX,sizeY;
		public bool[][] ExistArray;
		public bool[] door = new bool[4];
		public void Init(int sizex,int sizey)
		{
			sizeX = sizex;
			sizeY = sizey;
			ExistArray = new bool[sizeX][];
			for(int i=0;i<sizeX;i++)
			{
				ExistArray[i] = new bool[sizeY];
			}
		}
		public bool CheckShapeIsValidOrNot(bool[][] shape)
		{
			for (int i = 1; i < shape.Length-1; i++)
			{
				for (int j = 1; j < shape [i].Length-1; j++)
				{
					if (!shape [i] [j])
					{
						if (shape [i - 1] [j] && shape [i + 1] [j]) {
							Debug.Log (i.ToString()+j.ToString());
							return false;
						}
						if (shape [i] [j - 1] && shape [i] [j + 1]) {
							Debug.Log (i.ToString()+j.ToString());
							return false;
						}
					}
				}
			}
			return true;
		}
		public bool CheckThisPointIsExistOrNot(int x,int y)
		{
			if (!ExistArray[x][y])
				return false;
			return true;
		}
		public void TransferShape(bool[][] shape,bool[] door_)
		{
			sizeY = shape.Length;
			sizeX = shape [0].Length;
			Init (sizeX,sizeY);
			ExistArray = shape;
			door = door_;
		}
		private int valuebounds(int value,int min,int max)
		{
			if (value >= min && value <= max)
				return value;
			else if (value < min)
				return min;
			else
				return max;
		}
		public override void input (string path)
		{
			
			if (!File.Exists (path))
				return;
			XmlDocument xmldoc = new XmlDocument ();
			xmldoc.Load (path);
			XmlElement xe_fir = (XmlElement)xmldoc.FirstChild;
			int sizeX = int.Parse(xe_fir.GetAttribute ("sizeX"));
			int sizeY = int.Parse (xe_fir.GetAttribute("sizeY"));
			string doorcode = xe_fir.GetAttribute ("Door");
			for (int i = 0; i < 4; i++)
			{
				if (doorcode [i] == '1')
					door [i] = true;
				else
					door [i] = false;
			}
			Init (sizeX,sizeY);
			XmlNodeList xnl = xmldoc.SelectSingleNode ("shape").ChildNodes;
			foreach (XmlElement xe in xnl) 
			{
				int Xindex = int.Parse (xe.GetAttribute("XIndex"));
				int Yindex = int.Parse (xe.GetAttribute("YIndex"));
				bool Exist = bool.Parse (xe.GetAttribute("Exist"));
				ExistArray [Xindex] [Yindex] = Exist;
			}
		}
		public override void output (string path)
		{
			if (!File.Exists (path))
				File.Create (path);
			XmlDocument xmldoc = new XmlDocument ();
			XmlElement xe_fir = xmldoc.CreateElement ("shape");
			xe_fir.SetAttribute ("sizeX",ExistArray.Length.ToString());
			xe_fir.SetAttribute ("sizeY",ExistArray[0].Length.ToString());
			string s = "";
			for(int i=0;i<4;i++)
			{
				if(door[i])
					s+="1";
				else
					s+="0";
			}
			xe_fir.SetAttribute ("Door",s);
			xmldoc.AppendChild (xe_fir);
			for (int i = 0; i < ExistArray.Length; i++)
			{
				for (int j = 0; j < ExistArray [0].Length; j++) 
				{
					string _name = "element";
					XmlElement xe= xmldoc.CreateElement (_name);
					xe.SetAttribute ("XIndex",i.ToString());
					xe.SetAttribute ("YIndex",j.ToString());
					xe.SetAttribute ("Exist",ExistArray[i][j].ToString());
					xe_fir.AppendChild (xe);
				}
			}
			xmldoc.Save (path);
		}
	}

	public class TemplateMap:TemplateRL
	{
		public TemplateShape myShape;
		public int[][][] map;
		public int defaultTile;
		public int style;
		public int func;
		public string shapename;
		public void Init(TemplateShape ts)
		{
			myShape = ts;
		}
		public bool CheckIsValidOrNot(bool[][] mapexist)
		{
			for(int i=0;i<mapexist.Length;i++)
			{
				for (int j = 0; j < mapexist [i].Length; j++) 
				{
					if (mapexist [i] [j]&&!myShape.CheckThisPointIsExistOrNot(i,j)) 
					{
						return false;
					}
				}
			}
			return true;
		}

		public void TransferMap(int[][][] map_,int tile,int style_,int func_,string name)
		{
			map = map_;
			defaultTile = tile;
			style = style_;
			func = func_;
			shapename = name;
		}

		public override void input (string path)
		{
			if (!File.Exists (path))
				File.Create (path);
			XmlDocument xmldoc = new XmlDocument ();
			XmlElement xe_fir = xmldoc.CreateElement ("map");
			xe_fir.SetAttribute ("LengthX",map.Length.ToString());
			xe_fir.SetAttribute ("LengthY",map[0].Length.ToString());
			xe_fir.SetAttribute ("LengthZ",map[0][0].Length.ToString());
			xe_fir.SetAttribute ("DefaultTile",defaultTile.ToString());
			xe_fir.SetAttribute ("Style",style.ToString());
			xe_fir.SetAttribute ("Function",func.ToString());
			xe_fir.SetAttribute ("ShapeName",shapename);
			xmldoc.AppendChild (xe_fir);
			for (int i = 0; i < map.Length; i++)
			{
				for (int j = 0; j < map [0].Length; j++)
				{
					for (int k = 0; k < map [0] [0].Length; k++) 
					{
						string _name = "element";
						XmlElement xe = xmldoc.CreateElement (_name);
						xe.SetAttribute ("X", i.ToString ());
						xe.SetAttribute ("Y", j.ToString ());
						xe.SetAttribute ("Z", k.ToString ());
						xe.SetAttribute ("data", map [i] [j][k].ToString ());
						xe_fir.AppendChild (xe);
					}
				}
			}
			xmldoc.Save (path);
		}
		public override void output (string path)
		{
			if (!File.Exists (path))
				return;
			XmlDocument xmldoc = new XmlDocument ();
			xmldoc.Load (path);
			XmlElement xe_fir = (XmlElement)xmldoc.FirstChild;
			int LengthX = int.Parse(xe_fir.GetAttribute ("LengthX"));
			int LengthY = int.Parse(xe_fir.GetAttribute ("LengthY"));
			int LengthZ = int.Parse (xe_fir.GetAttribute("LengthZ"));
			defaultTile = int.Parse(xe_fir.GetAttribute ("DefaultTile"));
			style = int.Parse (xe_fir.GetAttribute("Style"));
			func = int.Parse (xe_fir.GetAttribute("Function"));
			shapename = xe_fir.GetAttribute ("ShapeName");
			map = new int[LengthX][][];
			for (int i = 0; i < LengthX; i++)
			{
				map [i] = new int[LengthY][];
				for (int j = 0; j < LengthY; j++) 
				{
					map[i][j] = new int[LengthZ];
				}
			}
			XmlNodeList xnl = xmldoc.SelectSingleNode ("map").ChildNodes;
			foreach (XmlElement xe in xnl) 
			{
				int indexX = int.Parse(xe.GetAttribute ("X"));
				int indexY = int.Parse(xe.GetAttribute ("Y"));
				int indexZ = int.Parse (xe.GetAttribute ("Z"));
				map [indexX] [indexY][indexZ] = int.Parse(xe.GetAttribute ("data"));
			}
		}
	}
	public struct PropNode
	{
		public int index;
		public GameObject prefab;
		public bool[][] RangeTable;
		public string path;
	};
	public struct PropTableNode
	{
		public int AllTableIndex;
		public int GroupTableIndex;
		public bool[][] RangeTable;
		public string prefabPath;
		public string GroupName;
	};

	public class TemplatePropTable:TemplateRL
	{
		public PropTableNode[] allProp;
		public string[] Groups;
		public DefaultTileTableNode[] allDT;
		public override void input (string path)
		{
			if (!File.Exists (path))
				File.Create (path);
			XmlDocument xmldoc = new XmlDocument ();
			XmlElement xml_fir = xmldoc.CreateElement ("Prop");
			XmlElement xml_gruop = xmldoc.CreateElement ("GroupName");
			XmlElement xml_prop = xmldoc.CreateElement ("PropTable");
			XmlElement xml_DT = xmldoc.CreateElement ("DefaultTile");
			xmldoc.AppendChild (xml_fir);
			xml_fir.AppendChild (xml_gruop);
			xml_fir.AppendChild (xml_prop);
			xml_fir.AppendChild (xml_DT);
			xml_fir.SetAttribute ("allPropSize",allProp.Length.ToString());
			xml_fir.SetAttribute ("GroupSize",Groups.Length.ToString());
			xml_fir.SetAttribute ("DefaultTileSize",allDT.Length.ToString());
			for (int i = 0; i < Groups.Length; i++) 
			{
				XmlElement tempElement = xmldoc.CreateElement ("Group");
				tempElement.SetAttribute ("GroupName",Groups[i]);
				xml_gruop.AppendChild (tempElement);
			}
			for (int i = 0; i < allDT.Length; i++)
			{
				XmlElement tempElement = xmldoc.CreateElement ("DT");
				tempElement.SetAttribute ("DT_PATH",allDT[i].prefabpath);
				tempElement.SetAttribute ("DT_INDEX",allDT[i].Index.ToString());
				xml_DT.AppendChild (tempElement);
			}
			for (int i = 0; i < allProp.Length; i++)
			{
				XmlElement tempElement = xmldoc.CreateElement ("Prop");
				tempElement.SetAttribute ("AllIndex",allProp[i].AllTableIndex.ToString());
				tempElement.SetAttribute ("GroupIndex",allProp[i].GroupTableIndex.ToString());
				tempElement.SetAttribute ("GroupName",allProp[i].GroupName);
				tempElement.SetAttribute ("prefabPath",allProp[i].prefabPath);
				tempElement.SetAttribute ("RangeTableX",allProp[i].RangeTable.Length.ToString());
				tempElement.SetAttribute ("RangeTableY",allProp[i].RangeTable[0].Length.ToString());
				xml_prop.AppendChild (tempElement);
			}
			xmldoc.Save (path);
		}
		public override void output(string path)
		{
			if (!File.Exists (path))
				return;
			XmlDocument xmldoc = new XmlDocument ();
			xmldoc.Load (path);
			XmlElement xml_fir = (XmlElement)xmldoc.FirstChild;
			int PropTableSize = int.Parse(xml_fir.GetAttribute ("allPropSize"));
			int GroupSize = int.Parse (xml_fir.GetAttribute("GroupSize"));
			int DTSize = int.Parse (xml_fir.GetAttribute("DefaultTileSize"));
			allProp = new PropTableNode[PropTableSize];
			Groups = new string[GroupSize];
			allDT = new DefaultTileTableNode[DTSize];
			XmlNodeList xnl_group = xmldoc.SelectSingleNode("Prop/GroupName").ChildNodes;
			XmlNodeList xnl_prop = xmldoc.SelectSingleNode("Prop/PropTable").ChildNodes;
			XmlNodeList xnl_dt = xmldoc.SelectSingleNode("Prop/DefaultTile").ChildNodes;
			int tempgroupIndex = 0;
			int temppropIndex = 0;
			int tempdtIndex = 0;
			foreach(XmlElement xe in xnl_group)
			{
				Groups [tempgroupIndex] = xe.GetAttribute ("GroupName");
				tempgroupIndex++;
			}
			foreach (XmlElement xe in xnl_dt) 
			{
				allDT [tempdtIndex].Index = int.Parse (xe.GetAttribute ("DT_INDEX"));
				allDT [tempdtIndex].prefabpath = xe.GetAttribute ("DT_PATH");
				tempdtIndex++;
			}
			foreach (XmlElement xe in xnl_prop)
			{
				allProp [temppropIndex].AllTableIndex = int.Parse(xe.GetAttribute ("AllIndex"));
				allProp [temppropIndex].GroupTableIndex = int.Parse(xe.GetAttribute ("GroupIndex"));
				allProp [temppropIndex].GroupName = (xe.GetAttribute ("GroupName"));
				allProp [temppropIndex].prefabPath = (xe.GetAttribute ("prefabPath"));
				int rangeX = int.Parse(xe.GetAttribute ("RangeTableX"));
				int rangeY = int.Parse (xe.GetAttribute("RangeTableY"));
				allProp[temppropIndex].RangeTable = new bool[rangeX][];
				for(int i=0;i<rangeX;i++)
				{
					allProp[temppropIndex].RangeTable[i] = new bool[rangeY];
					for (int j = 0; j < rangeY; j++)
						allProp [temppropIndex].RangeTable [i] [j] = false;
				}
				temppropIndex++;
			}
		}
	}
	public struct DefaultTileNode
	{
		public GameObject prefab;
		public string path;
	};
	public struct DefaultTileTableNode
	{
		public string prefabpath;
		public int Index;
	};
};
