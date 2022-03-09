using UnityEngine;
using System.Collections;

[RequireComponent (typeof (BoxCollider2D))]
public class RaycastController : MonoBehaviour
{
	public LayerMask collisionMask;
	
	public const float skinWidth = .015f;
    private const float dstBetweenRays = .25f;
	[HideInInspector]
	public int horizontalRayCount;
	[HideInInspector]
	public int verticalRayCount;

	[HideInInspector]
	public float horizontalRaySpacing;
	[HideInInspector]
	public float verticalRaySpacing;

	[HideInInspector]
	public BoxCollider2D collider;
	public RaycastOrigins raycastOrigins;

	public virtual void Awake()
    {
        this.collider = this.GetComponent<BoxCollider2D> ();
	}

	public virtual void Start()
    {
        this.CalculateRaySpacing ();
	}

	public void UpdateRaycastOrigins()
    {
		Bounds bounds = this.collider.bounds;
		bounds.Expand (skinWidth * -2);

        this.raycastOrigins.bottomLeft = new Vector2 (bounds.min.x, bounds.min.y);
        this.raycastOrigins.bottomRight = new Vector2 (bounds.max.x, bounds.min.y);
        this.raycastOrigins.topLeft = new Vector2 (bounds.min.x, bounds.max.y);
        this.raycastOrigins.topRight = new Vector2 (bounds.max.x, bounds.max.y);
	}
	
	public void CalculateRaySpacing()
    {
		Bounds bounds = this.collider.bounds;
		bounds.Expand (skinWidth * -2);

		float boundsWidth = bounds.size.x;
		float boundsHeight = bounds.size.y;

        this.horizontalRayCount = 5;
        this.verticalRayCount = 5;

        this.horizontalRaySpacing = bounds.size.y / (this.horizontalRayCount - 1);
        this.verticalRaySpacing = bounds.size.x / (this.verticalRayCount - 1);
	}
	
	public struct RaycastOrigins
    {
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}
}
