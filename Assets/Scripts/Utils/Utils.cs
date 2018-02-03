using System;
using UnityEngine;

public static class Utils
{
    public static T Random<T>()
    {
        Array D = Enum.GetValues(typeof(T));
        T R = (T)D.GetValue(UnityEngine.Random.Range(0, D.Length));
        return R;
    }

    public static Color BubbleColorToColor(BubbleColor bubbleColor)
    {
        switch (bubbleColor)
        {
            case BubbleColor.Blue:
                return Color.blue;
            case BubbleColor.Red:
                return Color.red;
            case BubbleColor.Green:
                return Color.green;
            case BubbleColor.Pink:
                return Color.magenta;
            case BubbleColor.Yellow:
                return Color.yellow;
            case BubbleColor.White:
                return Color.white;
            case BubbleColor.Cyan:
                return Color.cyan;
            default: return Color.clear;
        }
    }
}