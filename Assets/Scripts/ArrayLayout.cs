using UnityEngine;

[System.Serializable]
public class ArrayLayout
{
    public int width;
    public int height;

    [System.Serializable]
    public struct rowData
    {
        public bool[] row;
    }

    public rowData[] rows = new rowData[4]; //Grid of 7x7

    public ArrayLayout(int width, int height)
    {
        this.width = width;
        this.height = height;
    }
}