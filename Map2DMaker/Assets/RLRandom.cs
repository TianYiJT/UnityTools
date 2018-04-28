using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RLRandomAlgorithm
{
	public class RLRandom 
	{
		static Vector2[][] confirmDoorPos(int[][][] mapTemplate,bool[] door)
		{
			List<Vector2> DoorPosListLeft = new List<Vector2>();
			List<Vector2> DoorPosListRight = new List<Vector2>();
			List<Vector2> DoorPosListDown = new List<Vector2>();
			List<Vector2> DoorPosListUp = new List<Vector2>();
			for(int i=0;i<mapTemplate.Length;i++)
			{
				for (int j = 0; j < mapTemplate [0].Length; j++) 
				{
					bool Exist = false;
					for (int k = 0; k < mapTemplate [0] [0].Length; k++)
					{
						if (mapTemplate [i] [j] [k] == -4)
						{
							Exist = true;
							break;
						}
					}
					if (!Exist)
						continue;
					
					if(i == 0&&door[3]&&j>=1&&j<=mapTemplate[0].Length-2)
					{
						DoorPosListLeft.Add (new Vector2(i,j));	
		
						break;
					}
					if(j == 0&&door[0]&&i>=1&&i<=mapTemplate.Length-2)
					{
						DoorPosListUp.Add (new Vector2(i,j));	
		
						break;
					}
					if((i == mapTemplate.Length - 1)&&door[2]&&j>=1&&j<=mapTemplate[0].Length-2)
					{
						DoorPosListRight.Add (new Vector2(i,j));
				
						break;
					}
					if((j == mapTemplate [0].Length-1)&&door[1]&&i>=1&&i<=mapTemplate.Length-2)
					{
						DoorPosListDown.Add (new Vector2(i,j));	
				
						break;
					}
				}
			}
			Vector2[][] result;
			int resultLength = 0;
			if (DoorPosListDown.Count != 0)
				resultLength++;
			if (DoorPosListUp.Count != 0)
				resultLength++;
			if (DoorPosListLeft.Count != 0)
				resultLength++;
			if (DoorPosListRight.Count != 0)
				resultLength++;
			result = new Vector2[resultLength][];
			int tempIndex = 0;
			if (DoorPosListDown.Count != 0)
				result [tempIndex++] = DoorPosListDown.ToArray ();
			if (DoorPosListUp.Count != 0)
				result [tempIndex++] = DoorPosListUp.ToArray ();
			if (DoorPosListLeft.Count != 0)
				result [tempIndex++] = DoorPosListLeft.ToArray ();
			if (DoorPosListRight.Count != 0)
				result [tempIndex++] = DoorPosListRight.ToArray ();
			return result;
		}
		static int[][][] connectionTest(int[][][] map_,float connectionIntensity,bool LEqual,int style)
		{
			float tempConnection = 0;
			for (int i = 0; i < map_.Length; i++)
			{
				for (int j = 0; j < map_ [0].Length; j++)
				{
					if (map_ [i] [j] [0] >= 0) 
					{
						Vector2 down = new Vector2 (Mathf.Max(0,i-1),j);
						Vector2 up = new Vector2 (Mathf.Min(map_.Length-1,i+1),j);
						Vector2 left = new Vector2 (i,Mathf.Max(0,j-1));
						Vector2 right = new Vector2 (i,Mathf.Min(map_[0].Length-1,j+1));
						if(map_[(int)down.x][(int)down.y][0]>=0||map_[(int)up.x][(int)up.y][0]>=0||map_[(int)left.x][(int)left.y][0]>=0||map_[(int)right.x][(int)right.y][0]>=0)
							tempConnection += 1.0f / ((float)map_.Length * map_ [0].Length);
					}
				}
			}
			if ((tempConnection > connectionIntensity && LEqual) || (tempConnection < connectionIntensity && !LEqual)) 
			{
				float dis = Mathf.Abs (tempConnection - connectionIntensity);
				int randomIteration = Random.Range (1,(int)dis*15);
				for (int i = 0; i < randomIteration; i++)
				{
					int randomX = Random.Range (0,map_.Length-1);
					int randomY = Random.Range (0,map_[0].Length-1);
					if (LEqual) 
					{
						if (map_ [randomX] [randomY] [0] >= 0) 
						{
							Vector2 down = new Vector2 (Mathf.Max(0,randomX-1),randomY);
							Vector2 up = new Vector2 (Mathf.Min(map_.Length-1,randomX+1),randomY);
							Vector2 left = new Vector2 (randomX,Mathf.Max(0,randomY-1));
							Vector2 right = new Vector2 (randomX,Mathf.Min(map_[0].Length-1,randomY+1));
							if (map_ [(int)down.x] [(int)down.y] [0] >= 0 || map_ [(int)up.x] [(int)up.y] [0] >= 0 || map_ [(int)left.x] [(int)left.y] [0] >= 0 || map_ [(int)right.x] [(int)right.y] [0] >= 0)
								map_ [randomX] [randomY] [0] = -2;
						}
					}
					else
					{
						if (map_ [randomX] [randomY] [0] == -2) 
						{
							Vector2 down = new Vector2 (Mathf.Max(0,randomX-1),randomY);
							Vector2 up = new Vector2 (Mathf.Min(map_.Length-1,randomX+1),randomY);
							Vector2 left = new Vector2 (randomX,Mathf.Max(0,randomY-1));
							Vector2 right = new Vector2 (randomX,Mathf.Min(map_[0].Length-1,randomY+1));
							if (map_ [(int)down.x] [(int)down.y] [0] >= 0 || map_ [(int)up.x] [(int)up.y] [0] >= 0 || map_ [(int)left.x] [(int)left.y] [0] >= 0 || map_ [(int)right.x] [(int)right.y] [0] >= 0) 
							{
								int randomIndex = Random.Range(0,RLMap.StyleTileArray [style].Length);
								map_ [randomX] [randomY] [0] = RLMap.StyleTileArray [style] [randomIndex];
							}
						}
					}
				}
			}
			return map_;
		}
		static int[][][] denseTest(int[][][] map_,float denseIntensity,bool LEqual,int style)
		{
			float tempDense=0;
			for (int i = 0; i < map_.Length; i++)
			{
				for (int j = 0; j < map_ [0].Length; j++)
				{
					if (map_ [i] [j] [0] >= 0)
						tempDense += 1.0f / ((float)map_.Length * map_ [0].Length);
				}
			}
			if ((tempDense > denseIntensity && LEqual) || (tempDense < denseIntensity && !LEqual)) 
			{
				float dis = Mathf.Abs (tempDense - denseIntensity);
				int randomIteration = Random.Range (1,(int)dis*15);
				for (int i = 0; i < randomIteration; i++)
				{
					int randomX = Random.Range (0,map_.Length-1);
					int randomY = Random.Range (0,map_[0].Length-1);
					if (LEqual) 
					{
						if (map_ [randomX] [randomY] [0] >= 0)
							map_ [randomX] [randomY] [0] = -2;
					}
					else
					{
						if (map_ [randomX] [randomY] [0] == -2) 
						{
							int randomIndex = Random.Range (0,RLMap.StyleFuncObjectArray[style].Length);
							map_ [randomX] [randomY] [0] = RLMap.StyleFuncObjectArray [style] [randomIndex];
						}
					}
				}
			}
			return map_;
		}
		static int[][][] funcTest(RLFunction func,int[][][] map_)
		{
			if (func != null)
				return func.DoFunc (map_);
			else
				return map_;
		}
		static int[][][] templateTest(int[][][] template,int[][][] map_)
		{
			for (int i = 0; i < map_.Length; i++)
			{
				for (int j = 0; j < map_ [0].Length; j++) 
				{
					for (int k = 0; k < map_ [0] [0].Length; k++)
					{
						if (template [i] [j] [k]!=-6) 
						{
							if (template [i] [j] [k] == -3 || template [i] [j] [k] == -2||template [i] [j] [k] == -1)
							{
								map_ [i] [j] [k] = template [i] [j] [k];
							}
							if (template [i] [j] [k] >= 0) 
							{
								if (RLMap.MapIndexDatabase [template [i] [j] [k]].GetComponent<SpriteRenderer> ())
									map_ [i] [j] [k] = template [i] [j] [k];
								else
								{
									GameObject temp = RLMap.MapIndexDatabase [template [i] [j] [k]].GetComponent<TileGameobject> ().tiles [Random.Range (0, RLMap.MapIndexDatabase [template [i] [j] [k]].GetComponent<TileGameobject> ().tiles.Length)];
									int index = 0;
									for(int l=0;l<RLMap.TileDatabase.Length;l++)
									{
										if (temp == RLMap.TileDatabase [l])
										{
											index = l;
											break;
										}
									}
									map_ [i] [j] [k] = index;
								}
							}
						}
					}
				}
			}
			return map_;
		}
		static int[][][] funcObjectTest(int[][][] map_,int style) 
		{
			int count = Random.Range (0,map_.Length*map_[0].Length/20);
			for(int i=0;i<map_.Length;i++)
			{
				for (int j = 0; j < map_[0].Length; j++) 
				{
					if (map_ [i] [j] [0] == -2&&count>0&&Random.Range(0.0f,0.5f)>0.4f)
					{
						int index = Random.Range (0,RLMap.StyleFuncObjectArray[style].Length);
						int indexMap = RLMap.StyleFuncObjectArray[style][index];
						map_ [i] [j] [0] = indexMap;
						count--;
					}
				}
			}
			return map_;
		}
		static int[][][] AITest(int[][][] map_,int style)
		{
			int count = Random.Range (0,map_.Length*map_[0].Length/20);
			for(int i=0;i<map_.Length;i++)
			{
				for (int j = 0; j < map_[0].Length; j++) 
				{
					if (map_ [i] [j] [0] == -2&&count>0&&Random.Range(0.0f,0.5f)>0.4f)
					{
						int index = Random.Range (0,RLMap.StyleAIArray[style].Length);
						int indexMap = RLMap.StyleAIArray[style][index];
						map_ [i] [j] [0] = indexMap;
						count--;
					}
				}
			}
			return map_;
		}
		static Vector2[] confirmConnectionPoint(int[][][] map,int MIN,int MAX,float Distance)
		{
			int allCount = Random.Range (MIN,MAX);
			int nowCount = 0;
			Vector2[] connectionPoint;
			List<Vector2> connectList = new List<Vector2> ();
			for(int i=0;i<map.Length;i++)
			{
				for (int j = 0; j < map [0].Length; j++) 
				{
					if (map [i] [j] [0] == -5)
					{
						connectList.Add (new Vector2((float)i,(float)j));
					}
				}
			}
			connectionPoint = connectList.ToArray ();
			return connectionPoint;
		}
		static int[][][] GenerateTree(Vector2 start,Vector2 end,int[][][] map,RLMap rm)
		{
			int xstep = (int)Mathf.Abs (start.x-end.x);
			int ystep = (int)Mathf.Abs (start.y - end.y);
			int maxNode = 5;
			int maxNodeTree = map.Length * map [0].Length / 3;
			List<Vector2> nodestart = new List<Vector2> ();
			Vector2 temp = new Vector2(start.x,start.y);
			int tempNode = 0;
			int tempNodeTree = 0;
			Vector2 stepxy = new Vector2 (xstep,ystep);
			while(stepxy.x!=0||stepxy.y!=0)
			{
				if (stepxy.x == 0 && stepxy.y == 0)
					break;
				if (stepxy.x == 0) 
				{
					map [(int)temp.x] [(int)temp.y][0] = -2;
					rm.specialPath.Add (new Vector2(temp.x,temp.y));
					if (tempNode < maxNode)
					{
						if (Random.Range (0.0f, 1.0f) > 0.67f)
						{
							nodestart.Add (new Vector2(temp.x,temp.y));
							tempNode++;
						}
					}
					temp.y += Mathf.Sign (end.y - start.y);
					stepxy.y--;
					continue;
				}
				if (stepxy.y == 0) 
				{
					map [(int)temp.x] [(int)temp.y][0] = -2;
					rm.specialPath.Add (new Vector2(temp.x,temp.y));
					if (tempNode < maxNode)
					{
						if (Random.Range (0.0f, 1.0f) > 0.67f)
						{
							nodestart.Add (new Vector2(temp.x,temp.y));
							tempNode++;
						}
					}
					temp.x += Mathf.Sign (end.x - start.x);
					stepxy.x--;
					continue;
				}
				if (Random.Range (0.0f, 1.0f) > 0.5f)
				{
					map [(int)temp.x] [(int)temp.y][0] = -2;
					rm.specialPath.Add (new Vector2(temp.x,temp.y));
					if (tempNode < maxNode)
					{
						if (Random.Range (0.0f, 1.0f) > 0.67f)
						{
							nodestart.Add (new Vector2(temp.x,temp.y));
							tempNode++;
						}
					}
					temp.x += Mathf.Sign (end.x - start.x);
					stepxy.x--;
				}
				else
				{
					map [(int)temp.x] [(int)temp.y][0] = -2;
					rm.specialPath.Add (new Vector2(temp.x,temp.y));
					if (tempNode < maxNode)
					{
						if (Random.Range (0.0f, 1.0f) > 0.67f)
						{
							nodestart.Add (new Vector2(temp.x,temp.y));
							tempNode++;
						}
					}
					temp.y += Mathf.Sign (end.y - start.y);
					stepxy.y--;
				}
			}
			map [(int)temp.x] [(int)temp.y][0] = -2;
			rm.specialPath.Add (new Vector2(temp.x,temp.y));
			foreach(Vector2 point in nodestart)
			{
				if (tempNodeTree >= maxNodeTree)
					break;
				Stack<Vector2> stack = new Stack<Vector2>();
				stack.Push (point);
				while (stack.Count != 0)
				{
					Vector2 pointnow = stack.Peek ();
					if (tempNodeTree >= maxNodeTree)
						break;
					if (pointnow.x - 1 >= 0 && Random.Range (0.0f, 1.0f) > 0.67f) 
					{
						if (map [(int)pointnow.x - 1] [(int)pointnow.y] [0] != -1) 
						{
							map [(int)pointnow.x - 1] [(int)pointnow.y] [0] = -2;
							tempNodeTree++;
							stack.Push (new Vector2(pointnow.x-1,pointnow.y));
						}
					}
					if (pointnow.x + 1 < map.Length && Random.Range (0.0f, 1.0f) > 0.67f) 
					{
						if (map [(int)pointnow.x + 1] [(int)pointnow.y] [0] != -1) 
						{
							map [(int)pointnow.x + 1] [(int)pointnow.y] [0] = -2;
							tempNodeTree++;
							stack.Push (new Vector2(pointnow.x+1,pointnow.y));
						}
					}
					if (pointnow.y - 1 >= 0 && Random.Range (0.0f, 1.0f) > 0.67f) 
					{
						if (map [(int)pointnow.x] [(int)pointnow.y-1] [0] != -1) 
						{
							map [(int)pointnow.x] [(int)pointnow.y-1] [0] = -2;
							tempNodeTree++;
							stack.Push (new Vector2(pointnow.x,pointnow.y-1));
						}
					}
					if (pointnow.y + 1 <map[0].Length && Random.Range (0.0f, 1.0f) > 0.67f) 
					{
						if (map [(int)pointnow.x] [(int)pointnow.y+1] [0] != -1) 
						{
							map [(int)pointnow.x] [(int)pointnow.y+1] [0] = -2;
							tempNodeTree++;
							stack.Push (new Vector2(pointnow.x,pointnow.y+1));
						}
					}
				}
			}
			return map;
		}
		static int[][][] GenerateTreeTest(Vector2[] door,Vector2[] connect,int[][][] map,int style,RLMap rm)
		{
			if (connect.Length != 0) 
			{
				int index_connect = Random.Range (0, connect.Length - 1);
				for (int i = 0; i < door.Length; i++)
					map = GenerateTree (door [i], connect [index_connect], map,rm);
			}
			else
			{
				for (int i = 1; i < door.Length; i++)
					map = GenerateTree (door [i-1], door [i], map,rm);
				if(door.Length==1)
					map = GenerateTree (door [0],new Vector2(map.Length/2,map[0].Length/2), map,rm);
			}
			for (int i = 0; i < map.Length; i++) 
			{
				for (int j = 0; j < map [0].Length; j++) 
				{
					int index = Random.Range (0,RLMap.StyleTileArray[style].Length);
					int indexMap = RLMap.StyleTileArray [style] [index];
					if (map [i] [j] [0] == -3)
						map [i] [j] [0] = indexMap;
				}
			}
			return map;
		}
		public static RLMap GenerateMap(RLMap rm)
		{
			rm.mapAll = new int[rm.myMap.mapStruct.Length][][];
			for (int i = 0; i < rm.myMap.mapStruct.Length; i++)
			{
				rm.mapAll[i] = new int[rm.myMap.mapStruct[0].Length][];
				for (int j = 0; j < rm.myMap.mapStruct [0].Length; j++) 
				{
					rm.mapAll[i][j] = new int[rm.myMap.mapStruct[0][0].Length];
					for (int k = 0; k < rm.myMap.mapStruct [0] [0].Length; k++) 
					{
						if (rm.myMap.mapStruct [i] [j] [k] == -1)
							rm.mapAll [i] [j] [k] = -1;
						else
							rm.mapAll [i] [j] [k] = -3;
					}
				}
			}
			int randomTileIndex = Random.Range(0,RLMap.StyleDefaultTileArray [rm.Style-2].Length);
			rm.defaultTileIndex = RLMap.StyleDefaultTileArray [rm.Style-2] [randomTileIndex];
			GameObject defaultTileGameobject = RLMap.defaultTileDatabase [rm.myMap.defaultTile];
			if (defaultTileGameobject.GetComponent<SpriteRenderer> ())
				rm.defaultTileIndex = rm.myMap.defaultTile;
			else if (defaultTileGameobject.GetComponent<TileGameobject> ()) 
			{
				if (defaultTileGameobject.GetComponent<TileGameobject> ().tiles.Length != 0) 
				{
					int random2 = Random.Range (0,defaultTileGameobject.GetComponent<TileGameobject> ().tiles.Length);
					GameObject temp = defaultTileGameobject.GetComponent<TileGameobject> ().tiles [random2];
					int index = 0;
					for (int i = 0; i < RLMap.defaultTileDatabase.Length; i++) 
					{
						if(RLMap.defaultTileDatabase[i]==temp)
						{
							index = i;
							break;
						}
					}
					rm.defaultTileIndex = index;
				}
			}
			Vector2[][] allDoor = confirmDoorPos (rm.myMap.mapStruct,rm.DoorDir);
			Vector2[] realDoor = new Vector2[allDoor.Length];
			for (int i = 0; i < allDoor.Length; i++) 
			{
				realDoor[i] = allDoor[i][Random.Range(0,allDoor[i].Length)];
			}
			Vector2[] connect = confirmConnectionPoint (rm.myMap.mapStruct,1,3,rm.myMap.mapStruct.Length/2);
			rm.mapAll=GenerateTreeTest (realDoor,connect,rm.mapAll,rm.Style,rm);
			rm.mapAll = funcObjectTest (rm.mapAll,rm.Style);
		//	rm.mapAll = connectionTest (rm.mapAll,rm.ConnectionIntensity,rm.LEqualOrNot_Connection,rm.Style);
		//	rm.mapAll = denseTest (rm.mapAll, rm.DenseIntensity, rm.LEqualOrNot_Dense,rm.Style);
			foreach (Vector2 v in rm.specialPath)
			{
				rm.mapAll [(int)v.x] [(int)v.y] [0] = -2;
			}
			rm.mapAll = templateTest (rm.myMap.mapStruct,rm.mapAll);
			rm.mapAll = AITest (rm.mapAll,rm.Style);
			rm.mapAll = funcTest (RLMap.allFunc [rm.Function], rm.mapAll);
			return rm;
		}
		public static RLLevel  GenerateLevel(RLLevel level)
		{
			int realCount = Random.Range (level.minCount,level.maxCount);
			int NodeNumber = 1;
			Queue<RLLevelNode> queue_rl;
			queue_rl = new Queue<RLLevelNode> ();
			queue_rl.Enqueue (level.head);
			while (NodeNumber < realCount)
			{
				RLLevelNode temp = queue_rl.Dequeue();
				if(temp.down==null)
				{
					if (Random.Range (0.0f, 1.0f) > 0.5f) 
					{
						NodeNumber++;
						RLLevelNode tempDown = new RLLevelNode ();
						tempDown.up = temp;
						temp.down = tempDown;
						tempDown.father = temp;
						queue_rl.Enqueue (tempDown);
					}
				}
				if (temp.up == null)
				{
					if (Random.Range (0.0f, 1.0f) > 0.5f) 
					{
						NodeNumber++;
						RLLevelNode tempUp = new RLLevelNode ();
						tempUp.down = temp;
						temp.up = tempUp;
						tempUp.father = temp;
						queue_rl.Enqueue (tempUp);
					}
				}
				if (temp.left == null)
				{
					if (Random.Range (0.0f, 1.0f) > 0.5f)
					{
						NodeNumber++;
						RLLevelNode tempLeft = new RLLevelNode ();
						tempLeft.right = temp;
						temp.left = tempLeft;
						tempLeft.father = temp;
						queue_rl.Enqueue (tempLeft);
					}
				}
				if (temp.right == null) 
				{
					if (Random.Range (0.0f, 1.0f) > 0.5f) 
					{
						NodeNumber++;
						RLLevelNode tempRight = new RLLevelNode ();
						tempRight.left = temp;
						tempRight.father = temp;
						temp.right = tempRight;
						queue_rl.Enqueue (tempRight);
					}
				}
				queue_rl.Enqueue (temp);
			}
			level.count = NodeNumber;
			Queue<RLLevelNode> queue_rlnode;
			queue_rlnode = new Queue<RLLevelNode>();
			queue_rlnode.Enqueue (level.head);
			while(queue_rlnode.Count!=0)
			{
				RLLevelNode temp = queue_rlnode.Dequeue ();
				Shape[] correctShapes = FindCorrectShape (temp,RLLevel.allShape);
				int correctIndex = Random.Range (0,correctShapes.Length-1);

				temp.myShape = correctShapes [correctIndex];
				if (temp.down != null&&temp.father!=temp.down)
					queue_rlnode.Enqueue (temp.down);
				if (temp.up!= null&&temp.father!=temp.up)
					queue_rlnode.Enqueue (temp.up);
				if (temp.left != null&&temp.father!=temp.left)
					queue_rlnode.Enqueue (temp.left);
				if (temp.right != null&&temp.father!=temp.right)
					queue_rlnode.Enqueue (temp.right);
			}
			RLLevelNode[] allLeaf = findAllLeaf (level.head);
			int rIndex = Random.Range(0,allLeaf.Length-1);
			level.end = allLeaf [rIndex];
			return level;
		}
		public static Shape[] FindCorrectShape(RLLevelNode rln,Shape[] shapes)
		{
			List<Shape> shapeList = new List<Shape>();
			for(int i=0;i<shapes.Length;i++)
			{
				if (!shapes [i].door [0] && (rln.down != null)) 
				{
					continue;
				}
				if (!shapes [i].door [1] && (rln.up != null)) 
				{
					continue;
				}
				if (!shapes [i].door [2] && (rln.right != null)) 
				{
					continue;
				}
				if (!shapes [i].door [3] && (rln.left != null)) 
				{
					continue;
				}
				shapeList.Add (shapes[i]); 
			}
			return shapeList.ToArray ();
		}

		public static RLLevelNode[] findAllLeaf(RLLevelNode head)
		{
			List<RLLevelNode> allLeafs;
			allLeafs = new List<RLLevelNode> ();
			Queue<RLLevelNode> queue_nodes;
			queue_nodes = new Queue<RLLevelNode> ();
			queue_nodes.Enqueue (head);
			while(queue_nodes.Count!=0)
			{
				RLLevelNode rln = queue_nodes.Dequeue ();
				int neigh = 0;
				if (rln.down != null && rln.father != rln.down) 
				{
					queue_nodes.Enqueue (rln.down);
					neigh++;
				}
				if (rln.up != null && rln.father != rln.up) 
				{
					queue_nodes.Enqueue (rln.up);
					neigh++;
				}
				if (rln.right != null && rln.father != rln.right)
				{
					queue_nodes.Enqueue (rln.right);
					neigh++;
				}
				if (rln.left != null && rln.father != rln.left)
				{
					queue_nodes.Enqueue (rln.left);
					neigh++;
				}
				if (neigh == 0)
					allLeafs.Add (rln);
			}
			return allLeafs.ToArray ();
		}
	}
	public struct Map
	{
		public string ShapeName;
		public int[][][] mapStruct;
		public int func;
		public int style;
		public int defaultTile;
	};
	public class RLFunction
	{
		public virtual int[][][] DoFunc(int[][][] mapInput)
		{
			return mapInput;
		}
	}
	public class RLMap
	{
		public List<Vector2> specialPath = new List<Vector2> ();
		public bool[] DoorDir = new bool[4];
		public int Style;
		public int Function;
		public int[][][] mapAll;
		public Map myMap;
		public float DenseIntensity;
		public float ConnectionIntensity;
		public bool LEqualOrNot_Dense;
		public bool LEqualOrNot_Connection;
		public Vector3 Initpos = new Vector3(0,0,0);
		public static Vector3 standardSize;
		public int defaultTileIndex;
		public void GenerciGame()
		{
			GameObject[] allGameObject = GameObject.FindGameObjectsWithTag ("MapTileObject");
			for (int i = 0; i < allGameObject.Length; i++)
				GameObject.Destroy (allGameObject[i]);
			for (int i = 0; i < mapAll.Length; i++) 
			{
				for (int j = 0; j < mapAll [0].Length; j++)
				{
					if (mapAll [i] [j] [0] != -1) 
					{
						Vector3 temppos_ = new Vector3 (Initpos.x + standardSize.x * i, Initpos.y + standardSize.y * j, Initpos.z + standardSize.z * (1));
						GameObject g = GameObject.Instantiate (defaultTileDatabase [defaultTileIndex], temppos_, Quaternion.identity) as GameObject;
						g.tag = "MapTileObject";
					}

					for (int k = 0; k < mapAll [0] [0].Length; k++)
					{
						if (mapAll [i] [j] [k] >= 0)
						{
							if (TileDatabase [mapAll [i] [j] [k]].GetComponent<SpriteRenderer> ()) 
							{
								Vector3 temppos = new Vector3 (Initpos.x + standardSize.x * i, Initpos.y + standardSize.y * j, Initpos.z + standardSize.z * -k);
								GameObject g_ = GameObject.Instantiate (TileDatabase [mapAll [i] [j] [k]], temppos, Quaternion.identity) as GameObject;
								g_.tag = "MapTileObject";
							}
						}
					}
				}
			}
		}
		public static GameObject[] TileDatabase;
		public static GameObject[] MapIndexDatabase;
		public static int[][] StyleFuncObjectArray;
		public static int[][] StyleAIArray;
		public static int[][] StyleTileArray;
		public  static int[][] StyleDefaultTileArray;
		public static RLFunction[] allFunc;
		public static Map[][][][] MapDataBase_IndexBy_Style_Func_Shape;
		public static GameObject[] defaultTileDatabase;
	};
	public class Shape
	{
		public bool[] door;
		public int index;
		public string name;
		public Shape()
		{
			door = new bool[4];

		}
		public bool[][] shape;
	};
	public class RLLevelNode
	{
		public RLLevelNode father;
		public RLLevelNode left;
		public RLLevelNode right;
		public RLLevelNode up;
		public RLLevelNode down;
		public Shape myShape;
		public RLLevelNode()
		{
			father = null;
			left = null;
			right = null;
			up = null;
			down = null;
			myShape = null;
		}
		public bool[] doorDir()
		{
			bool[] door = new bool[4];
			if (down != null)
				door[0] = true;
			if (up != null)
				door[1] = true;
			if (right != null)
				door[2] = true;
			if (left != null)
				door[3] = true;
			return door;
		}
		public RLMap rMap=new RLMap();
		public bool isput = false;
	};
	public class RLLevel
	{
		public RLLevelNode head = new RLLevelNode();
		public RLLevelNode end;
		public RLLevelNode temp;
		public int[] Function;
		public int style;
		public int maxCount;
		public int minCount;
		public int count;
		public static Shape[] allShape;
		public RLLevelNode[] allNode;
		public void TravelTree()
		{
			Queue<RLLevelNode> queue_rlnode;
			queue_rlnode = new Queue<RLLevelNode>();
			queue_rlnode.Enqueue (head);
			allNode = new RLLevelNode[count-2];
			int tempIndex=0;
			while(queue_rlnode.Count!=0)
			{
				RLLevelNode temp = queue_rlnode.Dequeue ();

				if (temp != head && temp != end) 
				{
					allNode [tempIndex] = temp;
					tempIndex++;
				}
				if (temp.down != null&&temp.father!=temp.down)
					queue_rlnode.Enqueue (temp.down);
				if (temp.up!= null&&temp.father!=temp.up)
					queue_rlnode.Enqueue (temp.up);
				if (temp.left != null&&temp.father!=temp.left)
					queue_rlnode.Enqueue (temp.left);
				if (temp.right != null&&temp.father!=temp.right)
					queue_rlnode.Enqueue (temp.right);
			}
		}
		public void GenerateTemplateMap()
		{
			head.rMap.myMap = GetAMap (style,0,head.myShape.index);
			Conig (ref head,style,0);
			end.rMap.myMap = GetAMap (style,2,end.myShape.index);
			Conig (ref end, style, 2);
			for (int i = 0; i < Function.Length;)
			{
				int randomIndex = Random.Range (0,allNode.Length);
				if (!allNode [randomIndex].isput) 
				{
					allNode [randomIndex].rMap.myMap = GetAMap (style,Function[i],allNode[randomIndex].myShape.index);
					allNode [randomIndex].isput = true;
					Conig (ref allNode [randomIndex], style, Function [i]);
					i++;
				}
			}
			for (int i = 0; i < allNode.Length; i++)
			{
				if (!allNode [i].isput)
				{
					int randomFunc = Random.Range (0, RLMap.allFunc.Length);
					allNode [i].rMap.myMap = GetAMap (style,randomFunc,allNode[i].myShape.index);
					Conig (ref allNode [i], style,randomFunc);
				}
			}
		}
		private void Conig(ref RLLevelNode rln,int style,int func)
		{
			rln.rMap.DoorDir = rln.doorDir ();
			rln.rMap.ConnectionIntensity = Random.Range (0.0f, 1.0f);
			rln.rMap.DenseIntensity = Random.Range (0.0f, 1.0f);
			rln.rMap.Function = func;
			rln.rMap.Style = style;
		}
		private Map GetAMap(int style,int func,int shape)
		{
			if (RLMap.MapDataBase_IndexBy_Style_Func_Shape [style] [func] [shape].Length == 0) 
			{
				Map temp = new Map ();
				temp.ShapeName = RLLevel.allShape [shape].name;
				temp.style = style;
				temp.func = func;
				temp.defaultTile = RLMap.StyleDefaultTileArray [style-1] [0];
				temp.mapStruct = new int[RLLevel.allShape[shape].shape.Length][][];
				for (int i = 0; i < RLLevel.allShape [shape].shape.Length; i++)
				{
					temp.mapStruct[i] = new int[RLLevel.allShape[shape].shape[0].Length][];
					for (int j = 0; j < RLLevel.allShape [shape].shape [0].Length; j++)
					{
						temp.mapStruct[i][j] = new int[1];
						if ((i == temp.mapStruct.Length / 2 && j == 0) || (i == temp.mapStruct.Length / 2 && j == temp.mapStruct [0].Length-1) || (i == 0 && j == temp.mapStruct [0].Length / 2) || (i == temp.mapStruct.Length-1 && j == temp.mapStruct [0].Length / 2)) {
							temp.mapStruct [i] [j] [0] = -4;
						}
						else
							temp.mapStruct [i] [j] [0] = -6;
					}
				}

				return temp;
			} 
			else 
			{
				int randomIndex = Random.Range (0, RLMap.MapDataBase_IndexBy_Style_Func_Shape [style] [func] [shape].Length);
				return RLMap.MapDataBase_IndexBy_Style_Func_Shape [style] [func] [shape] [randomIndex];
			}
		}
	};
};