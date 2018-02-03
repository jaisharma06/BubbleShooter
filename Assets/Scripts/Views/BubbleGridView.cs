using System;
using UnityEngine;

[Serializable]
public class BubbleGridView
{
	public GameObject bubblePrefab;
    public float leftBorder = 0.0f;
    public float rightBorder = 10.5f;
    public float topBorder = 10.0f;
    public int rows = 10;
    public int columns = 10;
    public float bubbleRadius = 0.5f;
    public float addRowDuration = 10.0f;

	public Transform bubblesParent;
	public GameObject gun;

}
