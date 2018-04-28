using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RLRandomAlgorithm
{
	public class RLRandom 
	{
		static Vector2[][] confirmDoorPos(int[][][] mapTemplate,Shape shapeTemplate)
		{
			List<Vector2> DoorPosListLeft = new List<Vector2>();
			List<Vector2> DoorPosListRight = new List<Vector2>();
			List<Vector2> DoorPosListDown = new List<Vector2>();
			List<Vector2> DoorPosListUp = new List<Vector2>();
			for(int i=0;i<mapTemplate.Length;i++)
			{
				for (int j = 0; j < mapTemplate [0].Length; j++) 
				{
					bool Exist = true;
					for (int k = 0; k < mapTemplate [0] [0].Length; k++)
					{
						if (mapTemplate [i] [j] [k] != -4)
						{
							Exist = false;
							break;
						}
					}
					if (!Exist)
						continue;
					if(i == 0&&shapeTemplate.door[3]&&j>=1&&j<=mapTemplate[0].Length-2)
					{
						DoorPosListLeft.Add (new Vector2(i,j));	
						break;
					}
					if(j == 0&&shapeTemplate.door[1]&&i>=1&&i<=mapTemplate.Length-2)
					{
						DoorPosListUp.Add (new Vector2(i,j));	
						break;
					}
					if((i == mapTemplate.Length - 2)&&shapeTemplate.door[2]&&j>=1&&j<=mapTemplate[0].Length-2)
					{
						DoorPosListRight.Add (new Vector2(i,j));
						break;
					}
					if((j == mapTemplate [0].Length-2)&&shapeTemplate.door[0]&&i>=1&&i<=mapTemplate.Length-2)
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
						Vector2 up = new Vector2 (Mathf.Min(map_.Length,i+1),j);
						Vector2 left = new Vector2 (i,Mathf.Max(0,j-1));
						Vector2 right = new Vector2 (i,Mathf.Min(map_[0].Length,j+1));
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
							Vector2 up = new Vector2 (Mathf.Min(map_.Length,randomX+1),randomY);
							Vector2 left = new Vector2 (randomX,Mathf.Max(0,randomY-1));
							Vector2 right = new Vector2 (randomX,Mathf.Min(map_[0].Length,randomY+1));
							if (map_ [(int)down.x] [(int)down.y] [0] >= 0 || map_ [(int)up.x] [(int)up.y] [0] >= 0 || map_ [(int)left.x] [(int)left.y] [0] >= 0 || map_ [(int)right.x] [(int)right.y] [0] >= 0)
								map_ [randomX] [randomY] [0] = -2;
						}
					}
					else
					{
						if (map_ [randomX] [randomY] [0] == -2) 
						{
							Vector2 down = new Vector2 (Mathf.Max(0,randomX-1),randomY);
							Vector2 up = new Vector2 (Mathf.Min(map_.Length,randomX+1),randomY);
							Vector2 left = new Vector2 (randomX,Mathf.Max(0,randomY-1));
							Vector2 right = new Vector2 (randomX,Mathf.Min(map_[0].Length,randomY+1));
							if (map_ [(int)down.x] [(int)down.y] [0] >= 0 || map_ [(int)up.x] [(int)up.y] [0] >= 0 || map_ [(int)left.x] [(int)left.y] [0] >= 0 || map_ [(int)right.x] [(int)right.y] [0] >= 0) 
							{
								int randomIndex = Random.Range(0,RLMap.StyleTileArray [style].Length - 1);
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
							int randomIndex = Random.Range (0,RLMap.StyleFuncObjectArray[style].Length-1);
							map_ [randomX] [randomY] [0] = RLMap.StyleFuncObjectArray [style] [randomIndex];
						}
					}
				}
			}
			return map_;
		}
		static int[][][] funcTest(RLFunction func,int[][][] map_)
		{
			return func.DoFunc (map_);
		}
		static int[][][] templateTest(int[][][] template,int[][][] map_)
		{
			for (int i = 0; i < map_.Length; i++)
			{
				for (int j = 0; j < map_ [0].Length; j++) 
				{
					for (int k = 0; k < map_ [0] [0].Length; k++)
					{
						if (template [i] [j] [k]!=0) 
						{
							if (template [i] [j] [k] == -3 || template [i] [j] [k] == -2)
							{
								map_ [i] [j] [k] = template [i] [j] [k];
							}
							if (template [i] [j] [k] >= 1) 
							{
								if (RLMap.MapIndexDatabase [template [i] [j] [k]].GetComponent<SpriteRenderer> () != null) 
								{
									map_ [i] [j] [k] = template [i] [j] [k];
								}
								else
								{
									TileGameobject tg = RLMap.MapIndexDatabase [template [i] [j] [k]].GetComponent<TileGameobject> ();
									int randomIndex = Random.Range (0, tg.tiles.Length - 1);
									map_ [i] [j] [k] = tg.tiles [randomIndex];
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
				for (int j = 0; j < map_.Length; j++) 
				{
					if (map_ [i] [j] [0] == -2&&count>0&&Random.Range(0.0f,0.5f)>0.4f)
					{
						int index = Random.Range (0,RLMap.StyleFuncObjectArray[style].Length-1);
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
				for (int j = 0; j < map_.Length; j++) 
				{
					if (map_ [i] [j] [0] == -2&&count>0&&Random.Range(0.0f,0.5f)>0.4f)
					{
						int index = Random.Range (0,RLMap.StyleAIArray[style].Length-1);
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
			Vector2[] connectionPoint = new Vector2[allCount];
			while (nowCount < allCount)
			{
				Vector2 temp = new Vector2 (Random.Range(0,map.Length-1),Random.Range(0,map[0].Length-1));
				if (map [(int)temp.x] [(int)temp.y][0] != -5) 
				{
					continue;
				}
				bool isCanbeUse = true;
				for (int i = 0; i < nowCount; i++)
					if (Vector2.Distance (temp, connectionPoint [i]) < Distance)
					{
						isCanbeUse = false;
						break;
					}
				if (isCanbeUse)
				{
					connectionPoint [nowCount] = temp;
					nowCount++;
				}
			}
			return connectionPoint;
		}
		static int[][][] GenerateTree(Vector2 start,Vector2 end,int[][][] map)
		{
			Vector2 dis = end - start;
			int xDis = (int)dis.x;
			int yDis = (int)dis.y;
			int right=-1, left=-1;
			int up=-1, down=-1;
			if (xDis >= 0) 
			{
				right = Random.Range (xDis, map.Length - 1);
				left = right - xDis;
			}
			else
			{
				left = Random.Range (xDis, map.Length - 1);
				right = left - xDis;
			}
			if (yDis >= 0)
			{
				up = Random.Range (yDis, map [0].Length - 1);
				down = up - yDis;
			}
			else
			{
				down = Random.Range (yDis, map [0].Length - 1);
				up = down - yDis;
			}
			Queue<Vector2> path = new Queue<Vector2> ();
			Vector2 last = Vector2.zero;
			path.Enqueue (start);
			Vector2 temp = start;
			map [(int)temp.x] [(int)temp.y] [0] = -2;
			int branch = Random.Range ((left+right+up+down)/4,((left+right+up+down)/3));
			while (left != 0 && right != 0 && up != 0 && down!=0)
			{
				if(last!=Vector2.right&&left>0&&temp.x-1>=0)
				{
					if (map [(int)temp.x - 1] [(int)temp.y][0] != -3 && Random.Range (0.0f, 1.0f) > 0.5f)
					{
						left--;
						path.Enqueue (new Vector2(temp.x-1,temp.y));
						temp = new Vector2 (temp.x-1,temp.y);
						map [(int)temp.x] [(int)temp.y] [0] = -2;
						last = Vector2.left;
					}
				}
				if(last!=Vector2.left&&right>0&&temp.x+1<=map.Length-1)
				{
					if (map [(int)temp.x +1] [(int)temp.y][0] != -3 && Random.Range (0.0f, 1.0f) > 0.5f)
					{
						right--;
						path.Enqueue (new Vector2(temp.x+1,temp.y));
						temp = new Vector2 (temp.x+1,temp.y);
						map [(int)temp.x] [(int)temp.y] [0] = -2;
						last = Vector2.right;
					}
				}
				if(last!=Vector2.up&&down>0&&temp.y-1>=0)
				{
					if (map [(int)temp.x] [(int)temp.y-1][0] != -3 && Random.Range (0.0f, 1.0f) > 0.5f)
					{
						down--;
						path.Enqueue (new Vector2(temp.x,temp.y-1));
						temp = new Vector2 (temp.x,temp.y-1);
						map [(int)temp.x] [(int)temp.y] [0] = -2;
						last = Vector2.down;
					}
				}
				if(last!=Vector2.down&&up>0&&temp.y+1<=map[0].Length-1)
				{
					if (map [(int)temp.x] [(int)temp.y+1][0] != -3 && Random.Range (0.0f, 1.0f) > 0.5f)
					{
						up--;
						path.Enqueue (new Vector2(temp.x,temp.y+1));
						temp = new Vector2 (temp.x,temp.y+1);
						map [(int)temp.x] [(int)temp.y] [0] = -2;
						last = Vector2.up;
					}
				}
			}
			int nowBranch = 0;
			while (nowBranch < branch) 
			{
				Vector2 pathnode = path.Dequeue ();
				if (pathnode.x - 1 >= 0) 
				{
					if (map [(int)pathnode.x - 1] [(int)pathnode.y][0] != -3 && Random.Range (0.0f, 1.0f) > 0.7f) 
					{
						map [(int)pathnode.x - 1] [(int)pathnode.y] [0] = -2;
						nowBranch++;
						path.Enqueue (new Vector2(pathnode.x-1,pathnode.y));
					}
				}
				if (pathnode.x + 1 <= map.Length-1) 
				{
					if (map [(int)pathnode.x + 1] [(int)pathnode.y][0] != -3 && Random.Range (0.0f, 1.0f) > 0.7f) 
					{
						map [(int)pathnode.x + 1] [(int)pathnode.y] [0] = -2;
						nowBranch++;
						path.Enqueue (new Vector2(pathnode.x+1,pathnode.y));
					}
				}
				if (pathnode.y - 1 >= 0) 
				{
					if (map [(int)pathnode.x] [(int)pathnode.y-1][0] != -3 && Random.Range (0.0f, 1.0f) > 0.7f) 
					{
						map [(int)pathnode.x] [(int)pathnode.y-1] [0] = -2;
						nowBranch++;
						path.Enqueue (new Vector2(pathnode.x,pathnode.y-1));
					}
				}
				if (pathnode.y + 1 <= map[0].Length-1) 
				{
					if (map [(int)pathnode.x] [(int)pathnode.y+1][0] != -3 && Random.Range (0.0f, 1.0f) > 0.7f) 
					{
						map [(int)pathnode.x] [(int)pathnode.y+1] [0] = -2;
						nowBranch++;
						path.Enqueue (new Vector2(pathnode.x,pathnode.y+1));
					}
				}
			}
			return map;
		}
		static int[][][] GenerateTreeTest(Vector2[] door,Vector2[] connect,int[][][] map,int style)
		{
			for (int i = 0; i < door.Length; i++)
				for (int j = 0; j < connect.Length; j++)
					map=GenerateTree (door[i],connect[j],map);
			for (int i = 0; i < connect.Length; i++)
				for (int j = i+1; j < connect.Length; j++)
					map=GenerateTree (connect[i],connect[j],map);
			for (int i = 0; i < map.Length; i++) 
			{
				for (int j = 0; j < map [0].Length; j++) 
				{
					int index = Random.Range (0,RLMap.StyleTileArray[style].Length-1);
					int indexMap = RLMap.StyleTileArray [style] [index];
					if (map [i] [j] [0] == -1)
						map [i] [j] [0] = indexMap;
				}
			}
			return map;
		}
		static RLMap GenerateMap(RLMap rm)
		{
			Vector2[][] allDoor = confirmDoorPos (rm.myMap.mapStruct,RLLevel.allShape[rm.myMap.ShapeIndex]);
			Vector2[] realDoor = new Vector2[allDoor.Length];
			for (int i = 0; i < allDoor.Length; i++) 
			{
				realDoor[i] = allDoor[i][Random.Range(0,allDoor[i].Length-1)];
			}
			Vector2[] connect = confirmConnectionPoint (rm.myMap.mapStruct,1,3,rm.myMap.mapStruct.Length/2);
			rm.mapAll=GenerateTreeTest (realDoor,connect,rm.mapAll,rm.Style);
			rm.mapAll = funcObjectTest (rm.mapAll,rm.Style);
			rm.mapAll = connectionTest (rm.mapAll,rm.ConnectionIntensity,rm.LEqualOrNot_Connection,rm.Style);
			rm.mapAll = denseTest (rm.mapAll, rm.DenseIntensity, rm.LEqualOrNot_Dense,rm.Style);
			rm.mapAll = templateTest (rm.myMap.mapStruct,rm.mapAll);
			rm.mapAll = AITest (rm.mapAll,rm.Style);
			rm.mapAll = funcTest (RLMap.allFunc [rm.Function], rm.mapAll);
			return rm;
		}
		static RLLevel  GenerateLevel(RLLevel level)
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
				if (shapes [i].door [0] ^ (rln.down != null)) 
				{
					continue;
				}
				if (shapes [i].door [1] ^ (rln.up != null)) 
				{
					continue;
				}
				if (shapes [i].door [2] ^ (rln.right != null)) 
				{
					continue;
				}
				if (shapes [i].door [3] ^ (rln.left != null)) 
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
		public int ShapeIndex;
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
		public int DoorDir;
		public int Style;
		public int Function;
		public int[][][] mapAll;
		public Map myMap;
		public float DenseIntensity;
		public float ConnectionIntensity;
		public bool LEqualOrNot_Dense;
		public bool LEqualOrNot_Connection;
		public Vector3 Initpos;
		public Vector3 standardSize;
		public void GenerciGame()
		{
			for (int i = 0; i < mapAll.Length; i++) 
			{
				for (int j = 0; j < mapAll [0].Length; j++)
				{
					Vector3 temppos_ = new Vector3 (Initpos.x + standardSize.x * i, Initpos.y + standardSize.y * j, Initpos.z + standardSize.z * (-1));
					GameObject.Instantiate (defaultTileDatabase[myMap.defaultTile],temppos_,Quaternion.identity);
					for (int k = 0; k < mapAll [0] [0].Length; k++)
					{
						if (mapAll [i] [j] [k] >= 0)
						{
							Vector3 temppos = new Vector3 (Initpos.x + standardSize.x * i, Initpos.y + standardSize.y * j, Initpos.z + standardSize.z * k);
							GameObject.Instantiate (TileDatabase [mapAll [i] [j] [k]],temppos,Quaternion.identity);
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
		public  static int[] PropIndexArray;
		public static RLFunction[] allFunc;
		public static Map[] MapDataBase;
		public static GameObject[] defaultTileDatabase;
	};
	public class Shape
	{
		public bool[] door;
		public int index;
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
	};
	public class RLLevel
	{
		public RLLevelNode head;
		public RLLevelNode end;
		public int[][] Function;
		public int style;
		public int maxCount;
		public int minCount;
		public static Shape[] allShape;
	};
};