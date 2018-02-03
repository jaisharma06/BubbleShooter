using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BubbleController : MonoBehaviour
{
    public Bubble model;
    public BubbleView view;

    private float m_destroySpeed = 10.0f;
    private SpriteRenderer m_sprtiteRenderer;
    private Rigidbody2D m_rigidbody;
    private Collider2D m_collider;

    #region Delegates
    MotionDetectionDelegate motionDelegate;
    public delegate bool MotionDetectionDelegate(Vector3 position);

    CollisionDetectionDelegate collisionDetectionDelegate;
    public delegate void CollisionDetectionDelegate(GameObject bubble);

    #region Properties
    public CollisionDetectionDelegate CollisionDelegate
    {
        set { collisionDetectionDelegate = value; }
    }

    public MotionDetectionDelegate MotionDelegate
    {
        set { motionDelegate = value; }
    }
    #endregion
    #endregion

    #region UnityBuiltIns
    private void Start()
    {
        Initialize();
        SetColor();
    }

    private void Update()
    {
        if (view.moving)
        {
            transform.Translate(Vector3.right * view.speed * Mathf.Cos(Mathf.Deg2Rad * view.angle) * Time.deltaTime);
            transform.Translate(Vector3.up * view.speed * Mathf.Sin(Mathf.Deg2Rad * view.angle) * Time.deltaTime);

            if (motionDelegate != null)
            {
                if (!motionDelegate(transform.position))
                {
                    transform.Translate(Vector3.left * view.speed * Mathf.Cos(Mathf.Deg2Rad * view.angle) * Time.deltaTime);
                    transform.Translate(Vector3.down * view.speed * Mathf.Cos(Mathf.Deg2Rad * view.angle) * Time.deltaTime);
                    view.moving = false;
                    if (collisionDetectionDelegate != null)
                    {
                        collisionDetectionDelegate(gameObject);
                    }
                    else
                    {
                        //Update direction of the bubble
                        UpdateDirection();
                    }
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (view.moving)
        {
            view.moving = false;
            if (collisionDetectionDelegate != null)
            {
                collisionDetectionDelegate(gameObject);
            }
        }
    }
    #endregion

    #region PrivateMethods
    private void Initialize()
    {
        m_sprtiteRenderer = GetComponent<SpriteRenderer>();
        m_collider = GetComponent<Collider2D>();
        m_rigidbody = GetComponent<Rigidbody2D>();
    }

    private void SetColor()
    {
        model = new Bubble(Utils.Random<BubbleColor>());
        m_sprtiteRenderer.color = Utils.BubbleColorToColor(model.color);
    }

    private void UpdateDirection()
    {
        if (transform.position.x + view.radius >= view.rightBorder || transform.position.x - view.radius <= view.leftBorder)
        {
            view.angle = 180.0f - view.angle;
            if (transform.position.x + view.radius >= view.rightBorder)
                transform.position = new Vector3(view.rightBorder - view.radius, transform.position.y, transform.position.z);
            if (transform.position.x - view.radius <= view.leftBorder)
                transform.position = new Vector3(view.leftBorder + view.radius, transform.position.y, transform.position.z);
        }

        if (transform.position.y + view.radius >= view.topBorder)
        {
            view.moving = false;
            if (collisionDetectionDelegate != null)
            {
                collisionDetectionDelegate(gameObject);
            }
        }
    }
    #endregion

    #region PublicMethods
    public void Destroy(bool explode)
    {
        StopAllCoroutines();
        Destroy(m_rigidbody);
        Destroy(m_collider);
        if (explode)
        {
            iTween.ScaleTo(gameObject, Vector3.zero, 0.2f);
        }
        else
        {
            var destroyPos = new Vector3(transform.position.x, 0, 0);
            var distance = Vector3.Distance(transform.position, destroyPos);
            iTween.MoveTo(gameObject, destroyPos, distance / view.speed);
        }
    }
    #endregion
}
