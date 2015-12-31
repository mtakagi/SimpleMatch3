using UnityEngine;
using System.Collections;

public class YWBlock : MonoBehaviour 
{
	public GameObject mark;

	public int Column = -1;
	public int Row = -1;
	public Color Color = Color.white;


	public bool IsItSame(YWBlock block)
	{
		return this.Color.Equals (block.Color);
	}

	public void Swap(YWBlock block)
	{
		/*
		Vector3 tempPos = this.transform.localPosition;
		this.transform.localPosition = block.transform.localPosition;
		block.transform.localPosition = tempPos;
		*/

		int tempColumn = this.Column;
		this.Column = block.Column;
		block.Column = tempColumn;

		int tempRow = this.Row;
		this.Row = block.Row;
		block.Row = tempRow;
	}
}
