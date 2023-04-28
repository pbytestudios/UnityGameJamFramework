using UnityEngine;
using System.Collections;

public enum RotationAxis
{
	X,
	Y,
	Z
};

public class OrbitObject : MonoBehaviour 
{
	public float radius = 10.0f;
	public float orbitSpeed = 45.0f; //In degrees per second
	public float radiusTrackSpeed = 5f;
	public Transform orbitTarget;
	public RotationAxis rotationAxis = RotationAxis.X;
	public bool lookAtTarget = false;
	public float axisOffset = 0;

	Vector3 mask = Vector3.one;
	Vector3 invMask = Vector3.one;
	Vector3 axis = Vector3.one;
	// Use this for initialization
	void Start () 
	{
		transform.position = (transform.position - orbitTarget.position).normalized * radius + orbitTarget.position;

		//This mask vector allows us to match the objects rotation axis component to the TargetRotations rotationAxis component of the vector
		switch (rotationAxis) 
		{
		case RotationAxis.X:
			mask = new Vector3(0,1,1);
			invMask = new Vector3(1,0,0);
			axis= Vector3.right;
			break;
		case RotationAxis.Y:
			mask = new Vector3(1, 0, 1);
			invMask = new Vector3(0,1,0);
			axis = Vector3.up;
			break;
		case RotationAxis.Z:
			mask = new Vector3(1, 1, 0);
			invMask = new Vector3(0,0,1);
			axis = Vector3.forward;
			break;
			default:
				break;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.RotateAround(orbitTarget.position, axis, orbitSpeed * Time.deltaTime);

		//Our desired position will be where the orbit object is but we want to zero out along the axis of rotation such that our position
		//matched the orbitTarget's position along the axis of rotation
		var desiredPosition = Vector3.Scale((transform.position - orbitTarget.position), mask).normalized * radius + orbitTarget.position + invMask * axisOffset;

		if(radiusTrackSpeed == 0)
			transform.position = desiredPosition;
		else
			transform.position = Vector3.MoveTowards(transform.position, desiredPosition, radiusTrackSpeed * Time.deltaTime);

		if(lookAtTarget)
			transform.LookAt(orbitTarget);
	}
}
