#pragma strict


enum TangentsMode { Cardinal, TCB };
enum PathWrapMode { Clamp, Repeat, PingPong };
enum PositionMode { Segment, Distance };

var mode : TangentsMode;
var a = 0.5;
var t = 0.0;
var c = 0.0;
var b = 0.0;
var closed = false;
var positionMode = PositionMode.Segment;
var wrapMode = PathWrapMode.Clamp;

//var speedControl = false;

var WayPoints : WayPoint[] = new WayPoint[0];

var sceneResolution = 20;
var sceneShowCaptions = true;
var sceneShowProjection = true;
var sceneShowDistances = true;
var sceneShowAbsoluteDistances = false;
var sceneShowHeights = false;
var sceneShowTangents = true;
var sceneTangentProjection = true;
var sceneTangentScale = 0.2;

var Target : Transform;
var position = 0.0;

var autoPlay = false;
var autoPlaySpeed = 0.2;
var autoPlaySetOrientation = true;


private var m_pathLength = 0.0;


class WayPoint extends System.Object
	{
	var position : Vector3;
	var inTangent : Vector3;
	var outTangent : Vector3;
	
	var distanceToNext : float;
	}
	
	
	
function InsertNode(pos : int)
	{
	if (pos < 0) pos = 0;
	
	var wp = new WayPoint();
	
	// New node at the end of the list
	
	if (pos >= WayPoints.Length)
		{
		pos = WayPoints.Length;
		
		if (WayPoints.Length == 0)
			wp.position = transform.position;
		else
		if (WayPoints.Length == 1)
			wp.position = transform.position - transform.forward;
		else
			wp.position = 2 * WayPoints[WayPoints.Length-1].position - WayPoints[WayPoints.Length-2].position;
		}
	else
		{
		if (WayPoints.Length == 1)
			wp.position = transform.position + transform.forward;
		else
		if (pos == 0)
			wp.position = 2 * WayPoints[0].position - WayPoints[1].position;
		else
			wp.position = 0.5 * (WayPoints[pos-1].position + WayPoints[pos].position);
		}
		
	// Simple but inefficient
	
	var tmpList = new System.Collections.Generic.List.<WayPoint>(WayPoints);
	tmpList.Insert(pos, wp);
	WayPoints = tmpList.ToArray();
	
	CalculateTangents();
	}
	
	
	
function MoveNodeUp (node : int)
	{
	if (node > 0 && node < WayPoints.Length)
		{
		var wp = WayPoints[node];
		WayPoints[node] = WayPoints[node-1];
		WayPoints[node-1] = wp;
		
		CalculateTangents();
		}
	}
	
function MoveNodeDown (node : int)
	{
	if (node >= 0 && node < WayPoints.Length-1)
		{
		var wp = WayPoints[node];
		WayPoints[node] = WayPoints[node+1];
		WayPoints[node+1] = wp;
		
		CalculateTangents();
		}
	}
	
function RemoveNode (node : int)
	{
	if (node >= 0 && node < WayPoints.Length)
		{
		// Simple but inefficient
		
		var tmpList = new System.Collections.Generic.List.<WayPoint>(WayPoints);
		tmpList.RemoveAt(node);
		WayPoints = tmpList.ToArray();
		}
	}
	
	


function Hermite (wp0 : WayPoint, wp1 : WayPoint, s : float) : Vector3
	{
	// Brilliant!! by Nils Pipenbrinck aka Submissive/Cubic & $eeN
	// http://www.cubic.org/docs/hermite.htm
	
	var s2 = s*s;
	var s3 = s2*s;
	
	var h1 =  2*s3 - 3*s2 + 1;
	var h2 = -2*s3 + 3*s2;
	var h3 =    s3 - 2*s2 + s;
	var h4 =    s3 -   s2;
	
	return h1*wp0.position + h2*wp1.position + h3*wp0.outTangent + h4*wp1.inTangent;
	}
	
/*	
function HermiteD (wp0 : WayPoint, wp1 : WayPoint, s : float) : Vector3
	{
	// First derivative yields the tangent interpolation
	
	var s2 = s*s;
	var s3 = s2*s;
	
	var h1 =  6*s2 - 6*s;
	var h2 = -6*s2 + 6*s;
	var h3 =  3*s2 - 4*s + 1;
	var h4 =  3*s2 - 2*s;
	
	return h1*wp0.position + h2*wp1.position + h3*wp0.outTangent + h4*wp1.inTangent;
	}
*/	
	
private function SpLineLength (wp0 : WayPoint, wp1 : WayPoint) : float
	{
	var dist = 0.0;
	
	var p0 = wp0.position;
	var invRes = 1.0/sceneResolution;
	
	for (var n=1; n<=sceneResolution; n++)
		{
		var p1 = Hermite(wp0, wp1, n*invRes);
		dist += Vector3.Distance(p0, p1);
		p0 = p1;
		}
		
	return dist;
	}

	
	
// Tangent calculation functions	
	
	
private function CalculateTangentsCardinalOpen ()	
	{
	var T : Vector3;
	
	for (var i=0; i<WayPoints.Length; i++)
		{
		if (i == 0)
			T = a * 2 * (WayPoints[i+1].position - WayPoints[i].position);
		else
		if (i == WayPoints.Length-1)
			T = a * 2 * (WayPoints[i].position - WayPoints[i-1].position);
		else
			T = a * (WayPoints[i+1].position - WayPoints[i-1].position);
		
		WayPoints[i].inTangent = T;
		WayPoints[i].outTangent = T;
		}
	}

	
private function CalculateTangentsCardinalClosed ()	
	{
	var P0 : Vector3;
	var P1 : Vector3;
	var T : Vector3;
	
	for (var i=0; i<WayPoints.Length; i++)
		{
		if (i == 0)
			{
			P0 = WayPoints[WayPoints.Length-1].position;
			P1 = WayPoints[i+1].position;
			}
		else
		if (i == WayPoints.Length-1)
			{
			P0 = WayPoints[i-1].position;
			P1 = WayPoints[0].position;
			}
		else
			{
			P0 = WayPoints[i-1].position;
			P1 = WayPoints[i+1].position;
			}
		
		T = a * (P1 - P0);
		
		WayPoints[i].inTangent = T;
		WayPoints[i].outTangent = T;
		}
	}
	
private function CalculateTangentsTCBOpen ()
	{
	var inT : Vector3;	
	var outT : Vector3;
	
	for (var i=0; i<WayPoints.Length; i++)
		{
		if (i == 0)
			{
			outT = (1-t)*(1-c)*(1-b) * (WayPoints[i+1].position - WayPoints[i].position);
			inT = outT;
			}
		else
		if (i == WayPoints.Length-1)
			{
			inT = (1-t)*(1-c)*(1+b) * (WayPoints[i].position - WayPoints[i-1].position);
			outT = inT;
			}
		else
			{
			inT = (1-t)*(1-c)*(1+b)*0.5 * (WayPoints[i].position - WayPoints[i-1].position) +
				  (1-t)*(1+c)*(1-b)*0.5 * (WayPoints[i+1].position - WayPoints[i].position);
				  
			outT = (1-t)*(1+c)*(1+b)*0.5 * (WayPoints[i].position - WayPoints[i-1].position) +
				   (1-t)*(1-c)*(1-b)*0.5 * (WayPoints[i+1].position - WayPoints[i].position);
			}
		
		WayPoints[i].inTangent = inT;
		WayPoints[i].outTangent = outT;
		}		
	}

	
private function CalculateTangentsTCBClosed ()
	{
	var P0 : Vector3;
	var P1 : Vector3;
	var P : Vector3;
	
	var inT : Vector3;	
	var outT : Vector3;
	
	for (var i=0; i<WayPoints.Length; i++)
		{
		if (i == 0)
			{
			P0 = WayPoints[WayPoints.Length-1].position;
			P1 = WayPoints[i+1].position;
			}
		else
		if (i == WayPoints.Length-1)
			{
			P0 = WayPoints[i-1].position;
			P1 = WayPoints[0].position;
			}
		else
			{
			P0 = WayPoints[i-1].position;
			P1 = WayPoints[i+1].position;
			}
			
		P = WayPoints[i].position;
		
		inT = (1-t)*(1-c)*(1+b)*0.5 * (P - P0) + (1-t)*(1+c)*(1-b)*0.5 * (P1 - P);			  
		outT = (1-t)*(1+c)*(1+b)*0.5 * (P - P0) + (1-t)*(1-c)*(1-b)*0.5 * (P1 - P);
		
		WayPoints[i].inTangent = inT;
		WayPoints[i].outTangent = outT;
		}
	}
	
	
private function ApplySpeedControl ()
	{
	// http://www.cubic.org/docs/hermite.htm
	//
	// Note!! The Speed Control part of the document is taken as opposite here:
	// Outgoing tangent uses Ni, and incoming tangent uses Ni-1.
	// Not sure if the document is wrong or my implementation
	// requires it that way, but it works correctly now :)
	
	if (WayPoints.Length < 3) return;
	
	// All intermediate points
	
	for (var i=1; i<WayPoints.Length-1; i++)
		{
		var n0 = WayPoints[i-1].distanceToNext;
		var n1 = WayPoints[i].distanceToNext;
		
		WayPoints[i].inTangent *= 2*n0 / (n0+n1);
		WayPoints[i].outTangent *= 2*n1 / (n0+n1);
		}
	
	// First and last points in closed paths.
	// Open paths don't require adjustment at the ending points.
	
	if (closed)
		{
		n0 = WayPoints[WayPoints.Length-1].distanceToNext;
		n1 = WayPoints[0].distanceToNext;
		
		WayPoints[0].inTangent *= 2*n0 / (n0+n1);
		WayPoints[0].outTangent *= 2*n1 / (n0+n1);
		
		n0 = WayPoints[WayPoints.Length-2].distanceToNext;
		n1 = WayPoints[WayPoints.Length-1].distanceToNext;
		
		WayPoints[WayPoints.Length-1].inTangent *= 2*n0 / (n0+n1);
		WayPoints[WayPoints.Length-1].outTangent *= 2*n1 / (n0+n1);
		}
	}	
	
	
function CalculateTangents ()
	{
	var i : int;
	
	if (WayPoints.Length < 2) return;
	
	// Calculate tangents according to the type of the curve
	
	if (mode == TangentsMode.Cardinal)
		{
		if (closed && WayPoints.Length > 2)
			CalculateTangentsCardinalClosed();
		else
			CalculateTangentsCardinalOpen();
		}
	else
		{
		if (closed && WayPoints.Length > 2)
			CalculateTangentsTCBClosed();
		else
			CalculateTangentsTCBOpen();
		}
		
	// Calculate distances between segments
	
	var dist : float;
	m_pathLength = 0.0;
		
	for (i=0; i<WayPoints.Length-1; i++)
		{
		dist = SpLineLength(WayPoints[i], WayPoints[i+1]);
		WayPoints[i].distanceToNext = dist;
		m_pathLength += dist;
		}
	
	if (closed && WayPoints.Length > 2)
		dist = SpLineLength(WayPoints[WayPoints.Length-1], WayPoints[0]);
	else
		dist = 0.0;
		
	WayPoints[WayPoints.Length-1].distanceToNext = dist;
	m_pathLength += dist;
		
	// Apply speed control if position will be specified in distance units
	
	if (positionMode == PositionMode.Distance)
		ApplySpeedControl();
	}



	
// d is the waypoint-relative position:
// 	- 0 is the first waypoint
//	- 1 is the second waypoint
//	- 1.5 is half the way between the second and third position
//
// if the path has 4 nodes:
//  - open path: values are from 0 to 3
//	- closed path: values are from 0 to 4



private function ClampPosition (d : float, l : float, w : PathWrapMode) : float
	{
	switch (w)
		{
		case PathWrapMode.Repeat:
			d = Mathf.Repeat(d, l);
			break;
			
		case PathWrapMode.PingPong:
			d = Mathf.PingPong(d, l);
			break;
			
		default:
			d = Mathf.Clamp(d, 0, l);
			break;
		}
		
	return d;
	}
	

private function GetPositionClosed (d : float) : Vector3
	{
	// Closed paths use Repeat mode only
	
	d = Mathf.Repeat(d, WayPoints.Length);	
	var wp0 = Mathf.FloorToInt(d);
	
	if (wp0 == WayPoints.Length-1)
		return Hermite(WayPoints[wp0], WayPoints[0], d % 1.0);
	else
		return Hermite(WayPoints[wp0], WayPoints[wp0+1], d % 1.0);
	}
	
	
private function GetPositionOpen (d : float, wrap : PathWrapMode) : Vector3
	{
	var lastNode = WayPoints.Length-1;
	
	d = ClampPosition(d, lastNode, wrap);
	
	var wp0 = Mathf.FloorToInt(d);
	if (wp0 == lastNode)
		return WayPoints[lastNode].position;
	else
		return Hermite(WayPoints[wp0], WayPoints[wp0+1], d % 1.0);
	}
	
/* TO-DO: Revisar. Parece como que va bien en horizontal pero no en vertical.
	Podría ser algo con la función derivada de Hermite? (HermiteD)
	
private function GetTangentClosed (d : float) : Vector3
	{
	// Closed paths use Repeat mode only
	
	d = Mathf.Repeat(d, WayPoints.Length);	
	var wp0 = Mathf.FloorToInt(d);
	
	if (wp0 == WayPoints.Length-1)
		return HermiteD(WayPoints[wp0], WayPoints[0], d % 1.0);
	else
		return HermiteD(WayPoints[wp0], WayPoints[wp0+1], d % 1.0);
	}
	
	
private function GetTangentOpen (d : float, wrap : PathWrapMode) : Vector3
	{
	var lastNode = WayPoints.Length-1;
	
	d = ClampPosition(d, lastNode, wrap);
	
	var wp0 = Mathf.FloorToInt(d);
	if (wp0 == lastNode)
		return WayPoints[lastNode].inTangent;
	else
		return HermiteD(WayPoints[wp0], WayPoints[wp0+1], d % 1.0);
	}
*/	
	

function GetPosition (d : float) : Vector3
	{
	// Degenerated paths
	
	if (WayPoints.Length == 0) return Vector3.zero;
	if (WayPoints.Length == 1) return WayPoints[0].position;
	
	// Distance mode
	
	if (positionMode == PositionMode.Distance)
		d = DistanceToPosition(d);
	
	// Closed paths use Repeat mode only
	
	if (closed && WayPoints.Length > 2) 
		return GetPositionClosed(d);
	else
		return GetPositionOpen(d, wrapMode);
	}
	
	
function GetTangent (d : float) : Vector3
	{
	// Degenerated paths
	
	if (WayPoints.Length == 0) return Vector3.forward;
	if (WayPoints.Length == 1) return transform.forward;
	
	// Distance mode
	
	if (positionMode == PositionMode.Distance)
		d = DistanceToPosition(d);
	
	// Get the tangent
	
	var epsilon = 0.02;
	
	if (closed && WayPoints.Length > 2)
		{
		var d0 = Mathf.Repeat(d - epsilon, WayPoints.Length);
		var d1 = Mathf.Repeat(d + epsilon, WayPoints.Length);
		
		return (GetPositionClosed(d1) - GetPositionClosed(d0)).normalized;
		
		// return GetTangentClosed(d).normalized;
		}
	else
		{
		d = ClampPosition(d, WayPoints.Length-1, wrapMode);
		return (GetPositionOpen(d + epsilon, PathWrapMode.Clamp) - GetPositionOpen(d - epsilon, PathWrapMode.Clamp)).normalized;
		
		// return GetTangentOpen(d, PathWrapMode.Clamp);
		}
	}


function SetPosition (d : float)
	{
	if (!Target) Target = transform;
	
	Target.position = GetPosition(d);
	}
	
	
function SetOrientation (d : float, offset : float)
	{
	if (!Target) Target = transform;
	
	var Dir : Vector3;
	
	if (Mathf.Approximately(offset, 0.0))
		Dir = GetTangent(d);
	else
		Dir = GetPosition(d+offset) - GetPosition(d);
	
	Target.rotation = Quaternion.LookRotation(Dir, Vector3.up);
	}


private function DistanceToPosition (l : float) : float
	{
	var lastNode : int;
	
	if (WayPoints.Length < 2) return 0.0;
	
	if (closed && WayPoints.Length > 2)
		{
		l = Mathf.Repeat(l, m_pathLength);
		lastNode = WayPoints.Length-1;
		}
	else
		{
		l = ClampPosition(l, m_pathLength, wrapMode);
		lastNode = WayPoints.Length-2;
		}
		
	// Now l is in the range 0..m_pathLength.
	// Find the node the distance belongs to.

	var dSum = 0.0;
	var dNext : float;
	
	for (var i=0; i<=lastNode; i++)
		{
		dNext = WayPoints[i].distanceToNext;
		if (dSum + dNext >= l)
			break;
			
		dSum += dNext;
		}		
		
	// Now:
	//	- i is the node that contains our distance
	//  - dNext is the length of the node i
	// 	- dSum is the distance along the previous nodes.
	
	return i + (l - dSum) / dNext;
	}
	
	
	
private function DrawGizmoSpLine (wp0 : WayPoint, wp1 : WayPoint, col : Color, yFactor : float)
	{
	Gizmos.color = col;
	
	var p0 = wp0.position;
	p0.y *= yFactor;
	
	var invRes = 1.0/sceneResolution;
	
	for (var n=1; n<=sceneResolution; n++)
		{
		var p1 = Hermite(wp0, wp1, n*invRes);
		p1.y *= yFactor;
		
		Gizmos.DrawLine(p0, p1);
		p0 = p1;
		}
	}


function OnDrawGizmos ()
	{
	if (WayPoints.Length > 1)
		{		
		if (Mathf.Approximately(m_pathLength, 0.0))
			CalculateTangents();
		
		for (var i=0; i<WayPoints.Length; i++)
			{
			var point = WayPoints[i].position;
			var pointFlat = Vector3(point.x, 0.0, point.z);
			
			// Draw current waypoint and its projection
			
			Gizmos.color = Color.Lerp(Color.blue, Color.white, 0.25);
			
			//Usar #if editor?
			//Gizmos.DrawSphere (point, HandleUtility.GetHandleSize(point) * 0.05);
			Gizmos.DrawSphere (point, 0.05);
			
			if (sceneShowProjection)
				{
				Gizmos.color = Color(0.5,0.5,0.5,0.75);
				//Gizmos.DrawSphere (pointFlat, HandleUtility.GetHandleSize(pointFlat) * 0.04);
				Gizmos.DrawSphere (point, 0.04);
				Gizmos.DrawLine(point, pointFlat);
				}
			
			// Draw tangents
			
			if (sceneShowTangents && WayPoints.Length > 2)
				{
				var T : Vector3;
				
				// Outcoming tangent: all points except last one in open paths

				if (closed || i != WayPoints.Length-1)
					{
					T = WayPoints[i].outTangent * sceneTangentScale;
					
					Gizmos.color = Color.Lerp(Color.red, Color.white, 0.25);				
					Gizmos.DrawLine(point, point + T);
					
					if (sceneShowProjection && sceneTangentProjection)
						{
						Gizmos.color = Color(0.5,0.5,0.5,0.75);
						Gizmos.DrawLine(pointFlat, pointFlat + Vector3(T.x, 0.0, T.z));
						}
					}
					
				// Incoming tangent: all points except first one in open paths
					
				if (closed || i != 0)
					{
					T = WayPoints[i].inTangent * sceneTangentScale;
					
					Gizmos.color = Color.Lerp(Color.red, Color.white, 0.75);
					Gizmos.DrawLine(point, point - T);
					
					if (sceneShowProjection && sceneTangentProjection)
						{
						Gizmos.color = Color(0.5,0.5,0.5,0.75);
						Gizmos.DrawLine(pointFlat, pointFlat - Vector3(T.x, 0.0, T.z));
						}
					}
				}
			
			// Draw spline with next waypoint
			
			if (i < WayPoints.Length-1)
				{
				DrawGizmoSpLine(WayPoints[i], WayPoints[i+1], Color.green, 1.0);
				if (sceneShowProjection) DrawGizmoSpLine(WayPoints[i], WayPoints[i+1], Color(0.5,0.5,0.5,0.75), 0.0);				
				}
				
			// Spline from last to first in closed paths
				
			if (closed && WayPoints.Length > 2 && i == WayPoints.Length-1)
				{
				DrawGizmoSpLine(WayPoints[i], WayPoints[0], Color.green, 1.0);
				if (sceneShowProjection) DrawGizmoSpLine(WayPoints[i], WayPoints[0], Color(0.5,0.5,0.5,0.75), 0.0);
				}
			}
			
		// Dibujar posición actual
		
		Gizmos.color = Color.Lerp(Color.green, Color.white, 0.25);
		Gizmos.DrawWireCube (GetPosition(position), 0.05*Vector3.one);
		}
	}


function OnEnable () 
	{
	CalculateTangents();
	}
	

function Update () 
	{
	if ((!closed || WayPoints.Length == 2) && wrapMode == PathWrapMode.Clamp)
		position = Mathf.Clamp(position, 0, positionMode == PositionMode.Distance? m_pathLength : WayPoints.Length-1);
	
	SetPosition(position);
	
	if (autoPlay)
		{
		if (autoPlaySetOrientation) SetOrientation(position, 0.0);
		position += autoPlaySpeed * Time.deltaTime;
		}
	}
	
	
	
// Additional utility functions

function GetMedianPoint () : Vector3
    {
	var P = Vector3.zero;
	
	if (WayPoints.Length > 0)
		{
		for (var wp : WayPoint in WayPoints)
			P += wp.position;
			
		P /= WayPoints.Length;
		}
	
	return P;
	}
	
function MoveAllPoints (delta : Vector3)
	{
	for (var wp : WayPoint in WayPoints)
		wp.position += delta;
	}
	