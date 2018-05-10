
#pragma strict

// The target we are following
var target : Transform;
// The distance in the x-z plane to the target
var distance = 10.0;
// the height we want the camera to be above the target
var height = 5.0;
// Look above the target (height * this ratio)
var targetHeightRatio = 0.5;
// How fast we reach the target values
var heightDamping = 2.0;
var rotationDamping = 3.0;
var velocityDamping = 5.0;

private var lastPos = Vector3.zero;
private var currentVelocity = Vector3.zero;
private var wantedRotationAngle = 0.0;

private var firstTime = true;


function LateUpdate ()
	{
	// Early out if we don't have a target
	if (!target) return;

	if (firstTime)
		{
		lastPos = target.position;
		wantedRotationAngle = target.eulerAngles.y;
		currentVelocity = target.forward;
		firstTime = false;
		}

	var updatedVelocity = (target.position - lastPos) / Time.deltaTime;
	updatedVelocity.y = 0.0;

	if (updatedVelocity.magnitude > 1.0)
		{
		currentVelocity = Vector3.Lerp(currentVelocity, updatedVelocity, velocityDamping * Time.deltaTime);
		wantedRotationAngle = Mathf.Atan2(currentVelocity.x, currentVelocity.z) * Mathf.Rad2Deg;
		}
	lastPos = target.position;

	var wantedHeight = target.position.y + height;

	var currentRotationAngle = transform.eulerAngles.y;
	var currentHeight = transform.position.y;

	// Damp the rotation around the y-axis
	currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);

	// Damp the height
	currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightDamping * Time.deltaTime);

	// Convert the angle into a rotation
	var currentRotation = Quaternion.Euler (0, currentRotationAngle, 0);

	// Set the position of the camera on the x-z plane to:
	// distance meters behind the target
	transform.position = target.position;
	transform.position -= currentRotation * Vector3.forward * distance;

	// Set the height of the camera
	transform.position.y = currentHeight;

	transform.LookAt (target.position + Vector3.up*height*targetHeightRatio);
	}













