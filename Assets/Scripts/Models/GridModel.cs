using UnityEngine;
using System.Collections;

public class GridModel
{
    public bool baseLineAlignedLeft;
    private int rows;
    private int columns;
    private BubbleModel[,] bubbleGrid;

    public GridModel(int rows, int columns)
    {
        baseLineAlignedLeft = true;
        this.rows = rows;
        this.columns = columns;
        bubbleGrid = new BubbleModel[rows, columns];
    }

    public void Insert(BubbleModel bubble, int x, int y)
    {
        if (x < 0 || x > rows - 1 || y < 0 || y > columns - 1)
        {
            return;
        }

        bubbleGrid[x, y] = bubble;
    }

    public void Remove(int x, int y)
    {
        if (x < 0 || x > rows - 1 || y < 0 || y > columns - 1)
        {
            return;
        }

        bubbleGrid[x, y] = null;
    }

    public void Remove(BubbleModel bubble)
    {
        Vector2 position = Position(bubble);

        if (position.x > -1 || position.y > -1)
        {
            Remove((int)position.x, (int)position.y);
        }
    }

    public Vector2 Position(BubbleModel bubble)
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if (bubbleGrid[i, j] == bubble)
                {
                    return new Vector2(i, j);
                }

            }
        }
        return -1 * Vector2.one;
    }

    private void AddNotNull(ArrayList arrayList, System.Object token)
    {
        if (token != null)
            arrayList.Add(token);
    }

    private void Exclusive(ArrayList arrayList, ArrayList sourceArrayList)
    {
        ArrayList exclusives = new ArrayList();
        foreach (object anObject in arrayList)
        {
            if (!sourceArrayList.Contains(anObject))
            {
                exclusives.Add(anObject);
            }
        }
        arrayList.RemoveRange(0, arrayList.Count);
        arrayList.AddRange(exclusives);
    }

    private ArrayList Distinct(ArrayList arrayList)
    {
        ArrayList returnArray = new ArrayList();
        foreach (object someObject in arrayList)
        {
            if (!returnArray.Contains(someObject))
            {
                returnArray.Add(someObject);
            }
        }
        return returnArray;
    }

    private ArrayList ColorClusterRecursive(BubbleModel bubble, ArrayList visited)
    {
        ArrayList similarColorNeighbours = Utils.FilterByColor(GetNeighbours(bubble), bubble.color);
        Exclusive(similarColorNeighbours, visited);
        visited.Add(bubble);
        ArrayList returnArray = new ArrayList();
        returnArray.Add(bubble);
        foreach (BubbleModel aBubble in similarColorNeighbours)
        {
            if (bubble != aBubble)
                returnArray.AddRange(ColorClusterRecursive(aBubble, visited));
        }
        return returnArray;
    }

    private ArrayList GetConnectedBubblesRecursive(BubbleModel bubble, ArrayList visited, bool isBasedAlignedLeft)
    {
        ArrayList neighboursNotVisited = GetNeighbours(bubble);
        Exclusive(neighboursNotVisited, visited);
        //neighboursNotVisited.Remove(bubble);
        visited.Add(bubble);
        ArrayList returnArray = new ArrayList();
        returnArray.Add(bubble);
        foreach (BubbleModel someBubble in neighboursNotVisited)
        {
            if (bubble != someBubble)
                returnArray.AddRange(GetConnectedBubblesRecursive(someBubble, visited, isBasedAlignedLeft));
        }
        return returnArray;
    }

    private ArrayList GetConnectedBubbles(BubbleModel bubble)
    {
        return GetConnectedBubblesRecursive(bubble, new ArrayList(), baseLineAlignedLeft);
    }

    private ArrayList GetAnchoredBubbles()
    {
        ArrayList anchoredBubbles = new ArrayList();
        for (int j = 0; j < columns; j++)
        {
            if (bubbleGrid[0, j] != null)
            {
                anchoredBubbles.Add(bubbleGrid[0, j]);
            }
        }
        return anchoredBubbles;
    }

    public ArrayList GetBubbles()
    {
        ArrayList bubbles = new ArrayList();
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if (bubbleGrid[i, j] != null)
                {
                    bubbles.Add(bubbleGrid[i, j]);
                }
            }
        }
        return bubbles;
    }

    public bool HasBubble(int x, int y)
    {
        return bubbleGrid[x, y] != null;
    }

    public BubbleModel Bubble(int x, int y)
    {
        return bubbleGrid[x, y];
    }

    public ArrayList GetNeighbours(int x, int y)
    {
        ArrayList neighbours = new ArrayList();
        if (x < 0 || x > rows - 1 || y < 0 || y > columns - 1)
        {
            return null;
        }

        if (bubbleGrid[x, y] != null)
        {
            AddNotNull(neighbours, bubbleGrid[x, y]);

            // Left and right neighbours
            if (y > 0) AddNotNull(neighbours, bubbleGrid[x, y - 1]);
            if (y < columns - 1) AddNotNull(neighbours, bubbleGrid[x, y + 1]);

            // higher and lower neighbours
            bool isRowEven = x % 2 == 0;
            if ((baseLineAlignedLeft && isRowEven) || (!baseLineAlignedLeft && !isRowEven))
            {
                if (x > 0)
                {
                    if (y > 0) AddNotNull(neighbours, bubbleGrid[x - 1, y - 1]);
                    AddNotNull(neighbours, bubbleGrid[x - 1, y]);
                }
                if (x < rows - 1)
                {
                    if (y > 0) AddNotNull(neighbours, bubbleGrid[x + 1, y - 1]);
                    AddNotNull(neighbours, bubbleGrid[x + 1, y]);
                }
            }
            else
            {
                if (x > 0)
                {
                    AddNotNull(neighbours, bubbleGrid[x - 1, y]);
                    if (y < columns - 1) AddNotNull(neighbours, bubbleGrid[x - 1, y + 1]);
                }
                if (x < rows - 1)
                {
                    AddNotNull(neighbours, bubbleGrid[x + 1, y]);
                    if (y < columns - 1) AddNotNull(neighbours, bubbleGrid[x + 1, y + 1]);
                }
            }
            return neighbours;
        }
        return null;
    }

    public ArrayList GetNeighbours(BubbleModel bubble)
    {
        Vector2 position = Position(bubble);
        return GetNeighbours((int)position.x, (int)position.y);
    }

    public ArrayList ColorCluster(BubbleModel bubble)
    {
        return Distinct(ColorClusterRecursive(bubble, new ArrayList()));
    }

    public ArrayList LooseBubbles()
    {
        ArrayList anchoredBubbles = GetAnchoredBubbles();
        ArrayList connectedBubbles = new ArrayList();

        foreach (BubbleModel anchoredBubble in anchoredBubbles)
        {
            ArrayList connected = GetConnectedBubbles(anchoredBubble);
            connectedBubbles.AddRange(connected);
            connectedBubbles = Distinct(connectedBubbles);
        }
        ArrayList theBubbles = GetBubbles();
        Exclusive(theBubbles, connectedBubbles);
        return theBubbles;
    }

    public bool ShiftOneRow()
    {
        bool overflows = false;
        for (int i = rows - 1; i >= 0; i--)
        {
            for (int j = 0; j < columns; j++)
            {
                if (bubbleGrid[i, j] != null)
                {
                    if (i >= rows - 1)
                    {
                        overflows = true;
                    }
                    else
                    {
                        bubbleGrid[i + 1, j] = bubbleGrid[i, j];
                        bubbleGrid[i, j] = null;
                    }
                }
                else
                {
                }
            }
        }
        baseLineAlignedLeft = !baseLineAlignedLeft;
        return overflows;
    }

}
