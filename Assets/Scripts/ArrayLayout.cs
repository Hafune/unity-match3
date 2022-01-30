﻿using UnityEngine;

[System.Serializable]
public class ArrayLayout  {

	[System.Serializable]
	public struct rowData{
		public bool[] row;
	}

    public Grid grid = null!;
    public rowData[] rows = new rowData[14]; //Grid of 7x7
}
