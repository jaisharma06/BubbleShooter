using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleGridUtil
{
    public static Vector2 CellForPosition(Vector3 position, BubbleGridModel bubbleModel, bool baselineAlignedLeft)
    {
        int row = bubbleModel.rows - Mathf.FloorToInt(position.y) - 1;
        int column;
        bool rowIsEven = row % 2 == 0;
        if ((rowIsEven && baselineAlignedLeft) || (!rowIsEven && !baselineAlignedLeft))
        {
            column = Mathf.FloorToInt(position.x);
        }
        else
        {
            column = Mathf.FloorToInt(position.x - bubbleModel.bubbleRadius);
        }
        Vector2 result = new Vector2(row, TruncateToInterval(column, 0, bubbleModel.columns - 1));
        return result;
    }
	public static Vector3 PositionForCell(Vector2 cell, BubbleGridModel bubbleModel, bool isBaselineAlignedLeft){				
			bool rowIsEven = cell.x % 2 == 0;
			float y = bubbleModel.rows - cell.x - bubbleModel.bubbleRadius;
			float x;
			if (isBaselineAlignedLeft){
				if (rowIsEven){
					x = cell.y + bubbleModel.bubbleRadius;
				}else{
					x = cell.y +  2 * bubbleModel.bubbleRadius;
				}
			}else{
				if (rowIsEven){
					x = cell.y +  2 * bubbleModel.bubbleRadius;
				}else{
					x = cell.y + bubbleModel.bubbleRadius;
				}
			}
			return new Vector3(x, y, bubbleModel.depth);
		}

    public static int TruncateToInterval(int number, int min, int max)
    {
        int outcome;
        outcome = number;
        if (number < min) outcome = min;
        if (number > max) outcome = max;
        return outcome;
    }
}
