using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformController : RaycastController
{
	public LayerMask passengerMask;

	public Vector3[] localWaypoints;
    private Vector3[] globalWaypoints;

	public float speed;
	public bool cyclic;
	public float waitTime;
	[Range(0,2)]
	public float easeAmount;
    private int fromWaypointIndex;
    private float percentBetweenWaypoints;
    private float nextMoveTime;
    private List<PassengerMovement> passengerMovement;
    private Dictionary<Transform,Controller2D> passengerDictionary = new Dictionary<Transform, Controller2D>();
	
	public override void Start ()
    {
		base.Start ();

        this.globalWaypoints = new Vector3[this.localWaypoints.Length];
		for (int i =0; i < this.localWaypoints.Length; i++) {
            this.globalWaypoints[i] = this.localWaypoints[i] + this.transform.position;
		}
	}

    private void Update ()
    {
        this.UpdateRaycastOrigins ();

		Vector3 velocity = this.CalculatePlatformMovement();

        this.CalculatePassengerMovement(velocity);

        this.MovePassengers (true);
        this.transform.Translate (velocity);
        this.MovePassengers (false);
	}

    private float Ease(float x)
    {
		float a = this.easeAmount + 1;
		return Mathf.Pow(x,a) / (Mathf.Pow(x,a) + Mathf.Pow(1-x,a));
	}

    private Vector3 CalculatePlatformMovement()
    {
		if (Time.time < this.nextMoveTime)
        {
			return Vector3.zero;
		}

        this.fromWaypointIndex %= this.globalWaypoints.Length;
		int toWaypointIndex = (this.fromWaypointIndex + 1) % this.globalWaypoints.Length;
		float distanceBetweenWaypoints = Vector3.Distance (this.globalWaypoints [this.fromWaypointIndex], this.globalWaypoints [toWaypointIndex]);
        this.percentBetweenWaypoints += Time.deltaTime * this.speed /distanceBetweenWaypoints;
        this.percentBetweenWaypoints = Mathf.Clamp01 (this.percentBetweenWaypoints);
		float easedPercentBetweenWaypoints = this.Ease (this.percentBetweenWaypoints);

		Vector3 newPos = Vector3.Lerp (this.globalWaypoints [this.fromWaypointIndex], this.globalWaypoints [toWaypointIndex], easedPercentBetweenWaypoints);

		if (this.percentBetweenWaypoints >= 1)
        {
            this.percentBetweenWaypoints = 0;
            this.fromWaypointIndex ++;

			if (!this.cyclic)
            {
				if (this.fromWaypointIndex >= this.globalWaypoints.Length-1)
                {
                    this.fromWaypointIndex = 0;
					System.Array.Reverse(this.globalWaypoints);
				}
			}

            this.nextMoveTime = Time.time + this.waitTime;
		}

		return newPos - this.transform.position;
	}

    private void MovePassengers(bool beforeMovePlatform)
    {
		foreach (PassengerMovement passenger in this.passengerMovement)
        {
			if (!this.passengerDictionary.ContainsKey(passenger.transform))
            {
                this.passengerDictionary.Add(passenger.transform,passenger.transform.GetComponent<Controller2D>());
			}

			if (passenger.moveBeforePlatform == beforeMovePlatform)
            {
                this.passengerDictionary[passenger.transform].Move(passenger.velocity, passenger.standingOnPlatform);
			}
		}
	}

    private void CalculatePassengerMovement(Vector3 velocity)
    {
		HashSet<Transform> movedPassengers = new HashSet<Transform> ();
        this.passengerMovement = new List<PassengerMovement> ();

		float directionX = Mathf.Sign (velocity.x);
		float directionY = Mathf.Sign (velocity.y);

		// Vertically moving platform
		if (velocity.y != 0)
        {
			float rayLength = Mathf.Abs (velocity.y) + skinWidth;
			
			for (int i = 0; i < this.verticalRayCount; i ++) {
				Vector2 rayOrigin = (directionY == -1)? this.raycastOrigins.bottomLeft: this.raycastOrigins.topLeft;
				rayOrigin += Vector2.right * (this.verticalRaySpacing * i);
				RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, this.passengerMask);

				if (hit && hit.distance != 0) {
					if (!movedPassengers.Contains(hit.transform)) {
						movedPassengers.Add(hit.transform);
						float pushX = (directionY == 1)?velocity.x:0;
						float pushY = velocity.y - (hit.distance - skinWidth) * directionY;

                        this.passengerMovement.Add(new PassengerMovement(hit.transform,new Vector3(pushX,pushY), directionY == 1, true));
					}
				}
			}
		}

		// Horizontally moving platform
		if (velocity.x != 0) {
			float rayLength = Mathf.Abs (velocity.x) + skinWidth;
			
			for (int i = 0; i < this.horizontalRayCount; i ++)
            {
				Vector2 rayOrigin = (directionX == -1)? this.raycastOrigins.bottomLeft: this.raycastOrigins.bottomRight;
				rayOrigin += Vector2.up * (this.horizontalRaySpacing * i);
				RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, this.passengerMask);

				if (hit && hit.distance != 0)
                {
					if (!movedPassengers.Contains(hit.transform))
                    {
						movedPassengers.Add(hit.transform);
						float pushX = velocity.x - (hit.distance - skinWidth) * directionX;
						float pushY = -skinWidth;

                        this.passengerMovement.Add(new PassengerMovement(hit.transform,new Vector3(pushX,pushY), false, true));
					}
				}
			}
		}

		// Passenger on top of a horizontally or downward moving platform
		if (directionY == -1 || velocity.y == 0 && velocity.x != 0)
        {
			float rayLength = skinWidth * 2;
			
			for (int i = 0; i < this.verticalRayCount; i ++)
            {
				Vector2 rayOrigin = this.raycastOrigins.topLeft + Vector2.right * (this.verticalRaySpacing * i);
				RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, this.passengerMask);
				
				if (hit && hit.distance != 0)
                {
					if (!movedPassengers.Contains(hit.transform))
                    {
						movedPassengers.Add(hit.transform);
						float pushX = velocity.x;
						float pushY = velocity.y;

                        this.passengerMovement.Add(new PassengerMovement(hit.transform,new Vector3(pushX,pushY), true, false));
					}
				}
			}
		}
	}

    private struct PassengerMovement
    {
		public Transform transform;
		public Vector3 velocity;
		public bool standingOnPlatform;
		public bool moveBeforePlatform;

		public PassengerMovement(Transform _transform, Vector3 _velocity, bool _standingOnPlatform, bool _moveBeforePlatform)
        {
            this.transform = _transform;
            this.velocity = _velocity;
            this.standingOnPlatform = _standingOnPlatform;
            this.moveBeforePlatform = _moveBeforePlatform;
		}
	}

    private void OnDrawGizmos()
    {
		if (this.localWaypoints != null)
        {
			Gizmos.color = Color.red;
			float size = .3f;

			for (int i =0; i < this.localWaypoints.Length; i ++)
            {
				Vector3 globalWaypointPos = (Application.isPlaying)? this.globalWaypoints[i] : this.localWaypoints[i] + this.transform.position;
				Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
				Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
			}
		}
	}
	
}
