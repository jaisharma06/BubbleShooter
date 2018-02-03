using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleGridController : MonoBehaviour
{
    public BubbleGridView view;
    public BubbleGridModel model;

    #region PrivateFields
    private float bubbleSpeed = 12.0f;
    private int defaultRowsCount = 5;

    private GridModel grid;
    private BubbleController currentBubble;
    private ArrayList bubbleControllers;
    private bool pendingToAddRow;
    private bool isPlaying;
    #endregion

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        Initialize();
    }

	/// <summary>
	/// Start is called on the frame when a script is enabled just before
	/// any of the Update methods is called the first time.
	/// </summary>
	void Start()
	{
		StartGame();
	}

    private void Initialize()
    {
        isPlaying = false;
        pendingToAddRow = false;
        grid = new GridModel(view.rows, view.columns);
        bubbleControllers = new ArrayList();
    }

    public void StartGame()
    {
        model = new BubbleGridModel(view.leftBorder, view.rightBorder, view.topBorder, 0.0f, view.rows, view.columns, view.bubbleRadius);
        currentBubble = CreateBubble();
        isPlaying = true;
        StartCoroutine(AddRowScheduler());

        for (int i = 0; i < defaultRowsCount; i++)
        {
            AddRow();
        }
    }

    private BubbleController CreateBubble()
    {
        GameObject bubblePrefab = Instantiate(view.bubblePrefab) as GameObject;
        bubblePrefab.transform.parent = view.bubblesParent;
        bubblePrefab.transform.position = new Vector3((view.rightBorder - view.leftBorder) / 2.0f - model.bubbleRadius / 2.0f, -0.65f, 0);
        BubbleController bubbleController = bubblePrefab.GetComponent<BubbleController>();
        bubbleController.view.leftBorder = model.leftBorder;
        bubbleController.view.rightBorder = model.rightBorder;
        bubbleController.view.topBorder = model.topBorder;
        bubbleController.view.radius = model.bubbleRadius;
        bubbleController.view.speed = bubbleSpeed;
        bubbleController.view.angle = 90.0f;
        bubbleController.view.moving = false;
        bubbleController.CollisionDelegate = OnBubbleCollision;
        bubbleController.MotionDelegate = CanMoveToPosition;
        bubbleControllers.Add(bubbleController);
        return bubbleController;
    }

    private void OnBubbleCollision(GameObject bubble)
    {

        // If the ball falls under the amoun of rows, the game is over
        Vector2 bubblePos = BubbleGridUtil.CellForPosition(bubble.transform.position, model, grid.baseLineAlignedLeft);
        if ((int)bubblePos.x >= model.rows)
        {
            FinishGame(GameState.Lost);
            return;
        }

        // Create the new bubble
        BubbleController bubbleController = bubble.GetComponent<BubbleController>();
        Vector2 matrixPosition = BubbleGridUtil.CellForPosition(bubble.transform.position, model, grid.baseLineAlignedLeft);

        // Update the model
        grid.Insert(bubbleController.model, (int)matrixPosition.x, (int)matrixPosition.y);

        // if we don't have to add a new row (because of the timer), move the bubble smoothly to its snapping point			
        if (!pendingToAddRow)
        {
            bubbleController.MoveTo(BubbleGridUtil.PositionForCell(matrixPosition, model, grid.baseLineAlignedLeft), 0.1f);
        }
        else
        {
            // otherwise move it rapidly
            bubbleController.transform.position = BubbleGridUtil.PositionForCell(matrixPosition, model, grid.baseLineAlignedLeft);
        }

        // Explode the bubbles that need to explode
        // The the cluster of bubbles with a similar color as the colliding one
        ArrayList cluster = grid.ColorCluster(bubbleController.model);

        if (cluster.Count > 2)
        {
            // Explode the cluster
            bubbleController.transform.position = BubbleGridUtil.PositionForCell(matrixPosition, model, grid.baseLineAlignedLeft);
            DestroyCluster(cluster, true);
            // Notify that bubbles have been removed
            EventManger.BubblesRemoved(cluster.Count, true);
        }

        // Drop the bubbles that fall
        cluster = grid.LooseBubbles();
        DestroyCluster(cluster, false);
        if (cluster.Count > 0)
            EventManger.BubblesRemoved(cluster.Count, false);

        // Add a new Row of random bubbles if required
        if (pendingToAddRow)
        {
            AddRow();
            StartCoroutine(AddRowScheduler());
        }

        // If there are no bubble lefts, win the game
        if (grid.GetBubbles().Count == 0)
        {
            FinishGame(GameState.Won);
            return;
        }

        // Prepare the new bubble to shoot it
        currentBubble = CreateBubble();
    }

    private void FinishGame(GameState state)
    {
        //BubbleShooterController shooterController = this._bubbleShooter.GetComponent<BubbleShooterController>();
        //shooterController.isAiming = false;
        EventManger.GameFinished(state);
        isPlaying = false;
    }

    private void DestroyCluster(ArrayList cluster, bool explodes)
    {
        foreach (BubbleModel bubble in cluster)
        {
            DestroyBubble(GetBubbleController(bubble), explodes);
        }
    }

    private void DestroyBubble(BubbleController bubbleController, bool explodes)
    {
        grid.Remove(bubbleController.model);
        bubbleControllers.Remove(bubbleController);
        bubbleController.CollisionDelegate = null;
        bubbleController.Destroy(explodes);
    }

    private BubbleController GetBubbleController(BubbleModel bubble)
    {
        foreach (BubbleController bubbleController in bubbleControllers)
        {
            if (bubbleController.model == bubble)
                return bubbleController;
        }
        return null;
    }

    private void AddRow()
    {
        pendingToAddRow = false;
        bool overflows = grid.ShiftOneRow();


        for (int i = 0; i < model.columns; i++)
        {
            BubbleController bubbleController = CreateBubble();
            bubbleController.view.moving = false;
            grid.Insert(bubbleController.model, 0, i);
        }

        foreach (BubbleController bubbleController in bubbleControllers)
        {
            if (bubbleController != currentBubble)
            {
                Vector3 position = BubbleGridUtil.PositionForCell(grid.Position(bubbleController.model), model, grid.baseLineAlignedLeft);
                //bubbleController.MoveTo(position, shiftAnimationDuration);				
                bubbleController.transform.position = position;
            }

        }

        if (overflows)
        {
            FinishGame(GameState.Lost);
            return;
        }
    }

    bool CanMoveToPosition(Vector3 position)
    {
        Vector2 location = BubbleGridUtil.CellForPosition(position, model, grid.baseLineAlignedLeft);
        if ((int)location.x <= model.rows - 1)
        {
            return !grid.HasBubble((int)location.x, (int)location.y);
        }
        return true;
    }

    #region Coroutines
    private IEnumerator AddRowScheduler()
    {
        yield return new WaitForSeconds(view.addRowDuration);
        pendingToAddRow = true;
    }
    #endregion
}
