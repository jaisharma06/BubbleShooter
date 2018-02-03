using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleGridModel
{
    public float leftBorder;
    public float rightBorder;
    public float topBorder;
    public float depth;

    public int rows;
    public int columns;
    public float bubbleRadius;

    public BubbleGridModel(float leftBorder, float rightBorder, float topBorder, float depth, int rows, int columns, float bubbleRadius)
    {
        this.leftBorder = leftBorder;
        this.rightBorder = rightBorder;
        this.topBorder = topBorder;
        this.rows = rows;
        this.columns = columns;
        this.bubbleRadius = bubbleRadius;
        this.depth = depth;
    }
}
