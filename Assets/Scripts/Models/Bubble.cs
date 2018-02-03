public enum BubbleColor { Red, Blue, Green, Pink, Yellow, Cyan, White}

public class Bubble{
    private BubbleColor m_color;

    public Bubble(BubbleColor color)
    {
        m_color = color;
    }

    public BubbleColor color
    {
        get { return m_color; }
        set { m_color = value; }
    }
}
