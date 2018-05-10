
#pragma strict

var pos : Transform;
var target : Transform;
var damping = 6.0;
var positionZ = -10.0;
var positionY = 2.0;


private var m_rb : Rigidbody;


function LateUpdate () 
	{	
	if (!target) return;

	var targetpos : Vector3;
	
	if (m_rb)
		targetpos = m_rb.worldCenterOfMass;
	else
		targetpos = target.position;

	// Look at and dampen the rotation
	var rotation = Quaternion.LookRotation(targetpos - transform.position);
	transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
		
	if (pos)
		transform.position = pos.transform.position + Vector3.up * positionY + transform.forward * positionZ;
	}

	
function OnEnable () 
	{
	m_rb = GetComponent.<Rigidbody>();
	
	// Make the rigid body not change rotation
	
   	if (m_rb)
		m_rb.freezeRotation = true;
	}