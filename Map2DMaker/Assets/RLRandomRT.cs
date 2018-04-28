using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RLRandomAlgorithm;

public class RLRandomRT : MonoBehaviour
{
	public bool DEBUG;
	private RLRandomTranfer rlrt;
	private RLLevel temprll;
	// Use this for initialization
	void Start ()
	{
		rlrt = new RLRandomTranfer ();
		rlrt.Init ();
		if (DEBUG)
		{
			int[] func = {1,1,1};
			int style = 2;
			int min = 20;
			int max = 30;
			GenerateLevel (func,style,min,max);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (DEBUG)
		{
			if (Input.GetKeyDown (KeyCode.UpArrow))
				InDoor (1);
			if (Input.GetKeyDown (KeyCode.DownArrow))
				InDoor (0);
			if (Input.GetKeyDown (KeyCode.RightArrow))
				InDoor (2);
			if (Input.GetKeyDown (KeyCode.LeftArrow))
				InDoor (3);
		}
	}

	public void GenerateLevel(int[] func,int style,int minMap,int maxMap)
	{
		temprll = new RLLevel ();
		temprll.Function = func;
		temprll.style = style;
		temprll.minCount = minMap;
		temprll.maxCount = maxMap;
		temprll = RLRandom.GenerateLevel (temprll);
		temprll.TravelTree ();
		temprll.GenerateTemplateMap ();
		temprll.head.rMap = RLRandom.GenerateMap (temprll.head.rMap);
		temprll.end.rMap = RLRandom.GenerateMap (temprll.end.rMap);
		for (int i = 0; i < temprll.count-2; i++)
		{
			temprll.allNode [i].rMap = RLRandom.GenerateMap (temprll.allNode[i].rMap);
		}
		temprll.temp = temprll.head;
		temprll.temp.rMap.GenerciGame ();
	}

	public void InDoor(int dir)
	{
		bool generate = false;
		switch (dir)
		{
		case 0:
			if (temprll.temp.down != null) 
			{
				temprll.temp = temprll.temp.down;
				generate = true;
			}
			break;
		case 1:
			if (temprll.temp.up != null) 
			{
				temprll.temp = temprll.temp.up;
				generate = true;
			}
			break;
		case 2:
			if (temprll.temp.right != null)
			{
				temprll.temp = temprll.temp.right;
				generate = true;
			}
			break;
		case 3:
			if (temprll.temp.left != null) 
			{
				temprll.temp = temprll.temp.left;
				generate = true;
			}
			break;
		}
		if(generate)
			temprll.temp.rMap.GenerciGame ();
	}

	public void InNextLevel(int[] func,int style,int minMap,int maxMap)
	{
		GenerateLevel (func,style,minMap,maxMap);
	}
}
