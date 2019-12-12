//using Leap;
using Leap.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoidType
{
	Autonomous,
	Managed
};

public class Boid : MonoBehaviour 
{
	// Reference to BoidsController object for Managed Boid objects
	[HideInInspector] public BoidsController parentBoidsController;
	public BoidType boidType;
	public float moveSpeed;			// Movement speed in editor units.
	public float rotationPercentage;	// % of move speed for rotation speed.
	public float separationProximity;	// Min distance from neighbough for corrective steering.
	public float neighbourDetectRadius;	// Boid objects within radius will comprise neigbour group

	private Vector3 _separation	= Vector3.zero; // Avoidance vector for separation.
	private Vector3 _alignment 	= Vector3.zero; // Alignment vector for positioning within group.
	private Vector3 _cohesion	= Vector3.zero; // Direction vector for cohesion.
	private Vector3 _groupAverageHeading;
	private Vector3 _groupAveragePosition;		// Averaged position of all neighbours.
	private Vector3	_spawnLocation;			// The spawn location of this Boid.
	
	private int _groupSize 		= 0;		// Total Boids in neighbour group.
	private float _groupSpeed 	= 0f; 		// Total speed of neighbour group.
	private float _distanceToCurrentNeighbour;

	// Delegate used to update trajectory of this Boid.
	delegate void TrajectoryUpdate();
	private TrajectoryUpdate trajectoryUpdate;

    private LeapServiceProvider lp;

	void Awake()
	{
		_spawnLocation = transform.position;
		SetTrajectoryUpdate();
        lp = GameObject.Find("Leap Motion Controller Variant").GetComponent<LeapServiceProvider>();
	}

	void Update() 
	{
        
        
        trajectoryUpdate();
		// Translate along Z-axis in calculated trajectory
		transform.Translate(0, 0, Time.deltaTime * moveSpeed);
	}

	void SetTrajectoryUpdate()
	{
		switch (boidType)
		{
			case BoidType.Autonomous:
				trajectoryUpdate = AutonomousUpdateTrajectory;
				break;
			case BoidType.Managed:
				trajectoryUpdate = ManagedUpdateTrajectory;
				break;
			default:
				break;
		}
	}

	// Leaderless, no consideration for goal position.
	void AutonomousUpdateTrajectory()
	{
		_separation	= Vector3.zero;
		_alignment	= Vector3.zero;

		// Obtain neighbours.
		Collider[] neigbourColliders = Physics.OverlapSphere(transform.position, neighbourDetectRadius);

		List<GameObject> neighboughs = new List<GameObject>();
		for (int i = 0; i < neigbourColliders.Length; i++)
		{
			neighboughs.Add(neigbourColliders[i].gameObject);
		}
		neighboughs.Remove(gameObject); // Remove self from List before iteration.

		foreach (GameObject gameObj in neighboughs)
		{
			_distanceToCurrentNeighbour = Vector3.Distance(gameObj.transform.position, transform.position);
			if (_distanceToCurrentNeighbour <= neighbourDetectRadius)
			{	
				DetectedGameObjectResponse(gameObj);
			}
		}

		if (_groupSize > 0)
		{
			_alignment = _alignment / _groupSize; // Calculate center.
			_cohesion = (_alignment + _separation) - transform.position;
			if (_cohesion != Vector3.zero)
			{
				transform.rotation = Quaternion.Slerp(transform.rotation,
									Quaternion.LookRotation(_cohesion),
									rotationPercentage * moveSpeed * Time.deltaTime);
			}
		}
	}

	// Managed by BoidsManager. Homes in on goal position while accounting for neighbours.
	void ManagedUpdateTrajectory()
	{
		_separation	= Vector3.zero;
		_alignment	= Vector3.zero;

		parentBoidsController.allBoidGameObjects.Remove(gameObject); // Remove self from List before iteration.
		foreach (GameObject gameObj in parentBoidsController.allBoidGameObjects)
		{
			_distanceToCurrentNeighbour = Vector3.Distance(gameObj.transform.position, transform.position);
			if (_distanceToCurrentNeighbour <= neighbourDetectRadius)
			{
				DetectedGameObjectResponse(gameObj);
			}
		}
		parentBoidsController.allBoidGameObjects.Add(gameObject); // Add this back to List

        Leap.Hand left = new Leap.Hand();
        Leap.Hand right = new Leap.Hand();
        float leftIsFist = 1;
        float rightIsFist = 1;
        var frame = lp.GetLeapController().Frame();
        Debug.Log(frame.Hands.Count);
        if(frame.Hands.Count > 0)
        {
            Debug.Log("We have hands!");

            foreach (Leap.Hand hand in frame.Hands)
            {
                if (hand.IsLeft)
                {
                    right = hand;
                    Debug.Log("Right hand found!");
                    
                    
                } else
                {
                    left = hand;
                    Debug.Log("Left hand found!");
                }
            }

            leftIsFist = left.GetFistStrength();
            Debug.Log("Confidence left: " + leftIsFist);
            
            rightIsFist = right.GetFistStrength();
            Debug.Log("Confidence right: " + rightIsFist);


            //Leap.Hand hand = frame.Hands[0];
            //float isFist = hand.GetFistStrength();

        }


        if (_groupSize > 0)
		{
			// Calculate center and add Vector to Goal.
			_alignment = (_alignment / _groupSize) + (parentBoidsController.GetGoalPosition() - transform.position);
            Debug.Log("game object tag" + gameObject.tag);
            switch (gameObject.tag) {
                case "left":
                    if (leftIsFist < .3)
                    {
                        Debug.Log("left hand is making fist!");
                        _cohesion = (_alignment * (-1) + _separation) - transform.position;
                    } else
                    {
                        _cohesion = (_alignment + _separation) - transform.position;
                    }
                    break;
                case "right":
                    if (rightIsFist < .3)
                    {
                        Debug.Log("right hand is making fist!");
                        _cohesion = (_alignment * (-1) + _separation) - transform.position;
                    }
                    else
                    {
                        _cohesion = (_alignment + _separation) - transform.position;
                    }
                    break;
                default:
                    _cohesion = (_alignment + _separation) - transform.position;
                    break;

            }
            
			if (_cohesion != Vector3.zero)
			{
				transform.rotation = Quaternion.Slerp(transform.rotation,
									Quaternion.LookRotation(_cohesion),
									rotationPercentage * moveSpeed * Time.deltaTime);
			}
		}
	}

	// Response to neighbouring GameObject
	void DetectedGameObjectResponse(GameObject gameObj)
	{
		Boid otherBoid = gameObj.GetComponentInParent<Boid>();
		if (otherBoid && otherBoid.boidType == boidType)
		{
			_alignment += gameObj.transform.position; // Add neighbour positions for averaging.
			_groupSpeed += otherBoid.moveSpeed;
			_groupSize++;
		}
		// Account for _separation correction if experiencing proximity intrusion.
		if (_distanceToCurrentNeighbour < separationProximity)
		{
			_separation += transform.position - gameObj.transform.position;
		}
	}
}
