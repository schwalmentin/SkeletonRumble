using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class Controller2D : RaycastController
{
	public float maxSlopeAngle = 80;

	public CollisionInfo collisions;
	[HideInInspector]
	public Vector2 playerInput;
    public bool showControllerGizmos;
    [HideInInspector] public bool canGoThroughPlatform;

	public override void Start()
    {
		base.Start ();
        this.collisions.faceDir = 1;
	}

	public void Move(Vector2 moveAmount, bool standingOnPlatform)
    {
        this.Move (moveAmount, Vector2.zero, standingOnPlatform);
	}

	public void Move(Vector2 moveAmount, Vector2 input, bool standingOnPlatform = false)
    {
        this.UpdateRaycastOrigins ();

        this.collisions.Reset ();
        this.collisions.moveAmountOld = moveAmount;
        this.playerInput = input;

		if (moveAmount.y < 0)
        {
            this.DescendSlope(ref moveAmount);
		}

		if (moveAmount.x != 0)
        {
            this.collisions.faceDir = (int)Mathf.Sign(moveAmount.x);
		}

        this.HorizontalCollisions (ref moveAmount);

		if (moveAmount.y != 0)
        {
            this.VerticalCollisions (ref moveAmount);
		}

        this.transform.Translate (moveAmount);

		if (standingOnPlatform)
        {
            this.collisions.below = true;
		}

        this.canGoThroughPlatform = false;
    }

    private void HorizontalCollisions(ref Vector2 moveAmount)
    {
		float directionX = this.collisions.faceDir;
		float rayLength = Mathf.Abs (moveAmount.x) + skinWidth;

		if (Mathf.Abs(moveAmount.x) < skinWidth)
        {
			rayLength = 2*skinWidth;
		}

		for (int i = 0; i < this.horizontalRayCount; i ++)
        {
			Vector2 rayOrigin = (directionX == -1)? this.raycastOrigins.bottomLeft: this.raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (this.horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, this.collisionMask);

            if (this.showControllerGizmos)
            {
                Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red);
            }

            if (hit)
            {
				if (hit.distance == 0 || hit.collider.gameObject.layer == 14)
                {
					continue;
				}

                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

				if (i == 0 && slopeAngle <= this.maxSlopeAngle)
                {
					if (this.collisions.descendingSlope) {
                        this.collisions.descendingSlope = false;
						moveAmount = this.collisions.moveAmountOld;
					}

					float distanceToSlopeStart = 0;

					if (slopeAngle != this.collisions.slopeAngleOld)
                    {
						distanceToSlopeStart = hit.distance-skinWidth;
						moveAmount.x -= distanceToSlopeStart * directionX;
					}

                    this.ClimbSlope(ref moveAmount, slopeAngle, hit.normal);
					moveAmount.x += distanceToSlopeStart * directionX;
				}

				if (!this.collisions.climbingSlope || slopeAngle > this.maxSlopeAngle)
                {
					moveAmount.x = (hit.distance - skinWidth) * directionX;
					rayLength = hit.distance;

					if (this.collisions.climbingSlope)
                    {
						moveAmount.y = Mathf.Tan(this.collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x);
					}

                    this.collisions.left = directionX == -1;
                    this.collisions.right = directionX == 1;
				}
			}
		}
	}

    private void VerticalCollisions(ref Vector2 moveAmount)
    {
		float directionY = Mathf.Sign (moveAmount.y);
		float rayLength = Mathf.Abs (moveAmount.y) + skinWidth;

		for (int i = 0; i < this.verticalRayCount; i ++)
        {
			Vector2 rayOrigin = (directionY == -1)? this.raycastOrigins.bottomLeft: this.raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (this.verticalRaySpacing * i + moveAmount.x);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, this.collisionMask);

            if (this.showControllerGizmos)
            {
                Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red);
            }

            if (hit)
            {
				if (hit.collider.gameObject.layer == 14)
                {
					if (directionY == 1f || hit.distance == 0)
                    {
						continue;
					}

					if (this.collisions.fallingThroughPlatform)
                    {
						continue;
					}

					if (this.playerInput.y <= -0.7 && this.canGoThroughPlatform) 
                    {
                        this.collisions.fallingThroughPlatform = true;
                        this.Invoke("ResetFallingThroughPlatform",.1f);
						continue;
					}
				}

				moveAmount.y = (hit.distance - skinWidth) * directionY;
				rayLength = hit.distance;

				if (this.collisions.climbingSlope)
                {
					moveAmount.x = moveAmount.y / Mathf.Tan(this.collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(moveAmount.x);
				}

                this.collisions.below = directionY == -1;
                this.collisions.above = directionY == 1;
			}
		}

		if (this.collisions.climbingSlope)
        {
			float directionX = Mathf.Sign(moveAmount.x);
			rayLength = Mathf.Abs(moveAmount.x) + skinWidth;
			Vector2 rayOrigin = ((directionX == -1)? this.raycastOrigins.bottomLeft: this.raycastOrigins.bottomRight) + Vector2.up * moveAmount.y;
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin,Vector2.right * directionX,rayLength, this.collisionMask);

			if (hit)
            {
				float slopeAngle = Vector2.Angle(hit.normal,Vector2.up);

				if (slopeAngle != this.collisions.slopeAngle)
                {
					moveAmount.x = (hit.distance - skinWidth) * directionX;
                    this.collisions.slopeAngle = slopeAngle;
                    this.collisions.slopeNormal = hit.normal;
				}
			}
		}
	}

    private void ClimbSlope(ref Vector2 moveAmount, float slopeAngle, Vector2 slopeNormal)
    {
		float moveDistance = Mathf.Abs (moveAmount.x);
		float climbmoveAmountY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;

		if (moveAmount.y <= climbmoveAmountY) {
			moveAmount.y = climbmoveAmountY;
			moveAmount.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (moveAmount.x);
            this.collisions.below = true;
            this.collisions.climbingSlope = true;
            this.collisions.slopeAngle = slopeAngle;
            this.collisions.slopeNormal = slopeNormal;
		}
	}

    private void DescendSlope(ref Vector2 moveAmount)
    {

		RaycastHit2D maxSlopeHitLeft = Physics2D.Raycast (this.raycastOrigins.bottomLeft, Vector2.down, Mathf.Abs (moveAmount.y) + skinWidth, this.collisionMask);
		RaycastHit2D maxSlopeHitRight = Physics2D.Raycast (this.raycastOrigins.bottomRight, Vector2.down, Mathf.Abs (moveAmount.y) + skinWidth, this.collisionMask);
		if (maxSlopeHitLeft ^ maxSlopeHitRight) {
            this.SlideDownMaxSlope (maxSlopeHitLeft, ref moveAmount);
            this.SlideDownMaxSlope (maxSlopeHitRight, ref moveAmount);
		}

		if (!this.collisions.slidingDownMaxSlope)
        {
			float directionX = Mathf.Sign (moveAmount.x);
			Vector2 rayOrigin = (directionX == -1) ? this.raycastOrigins.bottomRight : this.raycastOrigins.bottomLeft;
			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, -Vector2.up, Mathf.Infinity, this.collisionMask);

			if (hit)
            {
				float slopeAngle = Vector2.Angle (hit.normal, Vector2.up);

				if (slopeAngle != 0 && slopeAngle <= this.maxSlopeAngle)
                {
					if (Mathf.Sign (hit.normal.x) == directionX)
                    {
						if (hit.distance - skinWidth <= Mathf.Tan (slopeAngle * Mathf.Deg2Rad) * Mathf.Abs (moveAmount.x))
                        {
							float moveDistance = Mathf.Abs (moveAmount.x);
							float descendmoveAmountY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;
							moveAmount.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (moveAmount.x);
							moveAmount.y -= descendmoveAmountY;

                            this.collisions.slopeAngle = slopeAngle;
                            this.collisions.descendingSlope = true;
                            this.collisions.below = true;
                            this.collisions.slopeNormal = hit.normal;
						}
					}
				}
			}
		}
	}

    private void SlideDownMaxSlope(RaycastHit2D hit, ref Vector2 moveAmount)
    {
		if (hit)
        {
			float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

			if (slopeAngle > this.maxSlopeAngle)
            {
				moveAmount.x = Mathf.Sign(hit.normal.x) * (Mathf.Abs (moveAmount.y) - hit.distance) / Mathf.Tan (slopeAngle * Mathf.Deg2Rad);

                this.collisions.slopeAngle = slopeAngle;
                this.collisions.slidingDownMaxSlope = true;
                this.collisions.slopeNormal = hit.normal;
			}
		}

	}

    private void ResetFallingThroughPlatform()
    {
        this.collisions.fallingThroughPlatform = false;
	}

	public struct CollisionInfo
    {
		public bool above, below;
		public bool left, right;

		public bool climbingSlope;
		public bool descendingSlope;
		public bool slidingDownMaxSlope;

		public float slopeAngle, slopeAngleOld;
		public Vector2 slopeNormal;
		public Vector2 moveAmountOld;
		public int faceDir;
		public bool fallingThroughPlatform;

		public void Reset()
        {
            this.above = this.below = false;
            this.left = this.right = false;
            this.climbingSlope = false;
            this.descendingSlope = false;
            this.slidingDownMaxSlope = false;
            this.slopeNormal = Vector2.zero;

            this.slopeAngleOld = this.slopeAngle;
            this.slopeAngle = 0;
		}
	}

    public void GoTroughPlatform(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            this.canGoThroughPlatform = true;
        }
    }
}
