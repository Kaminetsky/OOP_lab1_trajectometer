#pragma strict
#pragma downcast
@CustomEditor (EdyPath)



class EdyPathEditor extends Editor
	{
	private static var m_nodeFoldout = true;	
	private static var m_optionsFoldout = false;
	
	function OnInspectorGUI ()
		{
		var Target = target as EdyPath;
		
		Target.mode = EditorGUILayout.EnumPopup(GUIContent("Curve type", "Tangent calculation method"), Target.mode);
		
		if (Target.mode == TangentsMode.Cardinal)
			{		
			Target.a = EditorGUILayout.Slider(GUIContent("Bias (A)", "Tightness of the curve"), Target.a, 0.0, 1.0);
			}
		else
			{
			Target.t = EditorGUILayout.Slider(GUIContent("Tension (T)", "How sharply does the curve bend?"), Target.t, -1.0, 1.0);
			Target.c = EditorGUILayout.Slider(GUIContent("Continuity (C)", "How rapid is the change in the speed and direction?"), Target.c, -1.0, 1.0);
			Target.b = EditorGUILayout.Slider(GUIContent("Bias (B)", "What is the direction of the curve as it passes through the keypoint?"), Target.b, -1.0, 1.0);
			}
			
		Target.closed = EditorGUILayout.Toggle(GUIContent("Closed", "Connect the last and first points"), Target.closed);
		Target.positionMode = EditorGUILayout.EnumPopup(GUIContent("Position mode", "How to interpret the position value along the curve"), Target.positionMode);

		if (!Target.closed)
			Target.wrapMode = EditorGUILayout.EnumPopup(GUIContent("Wrap mode", "How to handle positions outside limits"), Target.wrapMode);
		else
			EditorGUILayout.LabelField(GUIContent("Wrap mode", "Closed paths use Repeat mode only"), GUIContent("Repeat", "Closed paths use Repeat mode only"));
			
		m_nodeFoldout = EditorGUILayout.Foldout(m_nodeFoldout, "Path nodes");
		
		EditorGUI.indentLevel = 2;
		if (m_nodeFoldout)
			{		
			EditorGUILayout.Space();		
			EditorGUILayout.HelpBox("Use Unity's standard tools for manipulating nodes (Pivot mode and rotation, keys Z and X)", MessageType.Info);		
			EditorGUILayout.Separator();
			}
	
		// ------- NODE LIST
		
		var pathLen = 0.0;
		
		for (var i=0; i<Target.WayPoints.Length; i++)
			{
			if (Target.closed || i < Target.WayPoints.Length-1)
				pathLen += Target.WayPoints[i].distanceToNext;
			
			if (m_nodeFoldout)
				{
				EditorGUILayout.BeginHorizontal();
				
				GUILayout.Label(String.Format("   Node {0}", i));

				GUI.backgroundColor = Color.green*0.8;
				if (GUILayout.Button(GUIContent("Ins", "Insert a new node before this one"), GUILayout.Width(30))) 
					{					
					Undo.RecordObject(target, "New path node inserted");
					Target.InsertNode(i);				
					GUI.changed = true;
					}
				GUI.backgroundColor = Color.white;
				
				/* Moving nodes is a very bad idea. It must be enough to add / remove nodes at any position of the path.
				
				if (i > 0)
					{
					if (GUILayout.Button(GUIContent("▲", "Move this node up"), GUILayout.Width(30)))
						{
						Undo.RecordObject(target, "Path node moved up");
						Target.MoveNodeUp(i);
						GUI.changed = true;
						}
					}
				else 
					GUILayout.Space(30+4);
					
				if (i < Target.WayPoints.Length-1)
					{
					if (GUILayout.Button(GUIContent("▼", "Move this node down"), GUILayout.Width(30)))
						{
						Undo.RecordObject(target, "Path node moved down");
						Target.MoveNodeDown(i); 
						GUI.changed = true;
						}
					}
				else
					GUILayout.Space(30+4);
				*/
				GUILayout.Space(5);
				GUI.backgroundColor = Color.red*0.8;
				if (GUILayout.Button(GUIContent("X", "Remove this node"), GUILayout.Width(30))) 
					{
					Undo.RecordObject(target, "Path node removed");
					Target.RemoveNode(i);
					GUI.changed = true;
					}
				GUI.backgroundColor = Color.white;
				
				EditorGUILayout.EndHorizontal();
				}
			}
		
		if (m_nodeFoldout)
			{		
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("");
			GUI.backgroundColor = Color.green*0.8;
			if (GUILayout.Button(GUIContent("New node", "Insert a new node at the end of the list"), GUILayout.Width(90))) 
				{
				Undo.RecordObject(target, "New path node inserted");
				Target.InsertNode(Target.WayPoints.Length);
				GUI.changed = true;
				}
			GUI.backgroundColor = Color.white;
			GUILayout.Space(18+30);			
			EditorGUILayout.EndHorizontal();
			}
		
		EditorGUILayout.HelpBox(String.Format("{0} nodes, path length: {1,4:0.000}", Target.WayPoints.Length, pathLen), MessageType.None);
		EditorGUI.indentLevel = 0;
		
		// ------- END OF NODE LIST
		
		EditorGUILayout.Separator();		
		EditorGUILayout.Space();
		
		// ------- TARGET / AUTOPLAY
		
		var allowSceneObjects = !EditorUtility.IsPersistent(target);
		Target.Target = EditorGUILayout.ObjectField(GUIContent("Target", "Target transform for path positioning. Uses current GameObject by default"), Target.Target, Transform, allowSceneObjects);
		Target.position = EditorGUILayout.FloatField(GUIContent("Position", "Position in the path. Can be segments or distance units acording to the Position Mode"), Target.position);
		
		EditorGUILayout.Space();
		
		Target.autoPlay = EditorGUILayout.Toggle(GUIContent("Autoplay", "Automatically moves the target along the path"), Target.autoPlay);
		Target.autoPlaySpeed = EditorGUILayout.FloatField(GUIContent("Autoplay speed", "Speed for autoplay in units / sec (units can be segments or distance according to Position Mode)"), Target.autoPlaySpeed);
		Target.autoPlaySetOrientation = EditorGUILayout.Toggle(GUIContent("Autoplay orientation", "Keeps the target oriented through the path"), Target.autoPlaySetOrientation);
		
		// ------- DEBUG / SCENE 
		
		m_optionsFoldout = EditorGUILayout.Foldout(m_optionsFoldout, "Scene display options");
		if (m_optionsFoldout)
			{
			EditorGUILayout.Separator();		
			EditorGUILayout.Space();
			
			EditorGUI.indentLevel = 2;
			Target.sceneShowCaptions = EditorGUILayout.Toggle(GUIContent("Show captions", "Show the node numbers"), Target.sceneShowCaptions);
			Target.sceneShowProjection = EditorGUILayout.Toggle(GUIContent("Show projection", "Show the spline projection in the plane Y=0"), Target.sceneShowProjection);
			Target.sceneShowDistances = EditorGUILayout.Toggle(GUIContent("Show distances", "Show the distance between each node"), Target.sceneShowDistances);
			Target.sceneShowAbsoluteDistances = EditorGUILayout.Toggle(GUIContent("  Absolute distances", "Show absolute distances in the path"), Target.sceneShowAbsoluteDistances);
			Target.sceneShowHeights = EditorGUILayout.Toggle(GUIContent("Show heights", "Show the height of each node (Y coordinate)"), Target.sceneShowHeights);
			Target.sceneShowTangents = EditorGUILayout.Toggle(GUIContent("Show tangents", "Show the input and output tangents for each node"), Target.sceneShowTangents);
			Target.sceneTangentProjection = EditorGUILayout.Toggle(GUIContent("Tangent projection", "Also project the tangents in the plane Y=0"), Target.sceneTangentProjection);
			Target.sceneTangentScale = EditorGUILayout.Slider(GUIContent("Tangent scale", "Scale factor for showing tangents"), Target.sceneTangentScale, 0.0, 1.0);		
			EditorGUI.indentLevel = 0;
			}
			
		if (GUI.changed)
			{
			Target.CalculateTangents();
			EditorUtility.SetDirty(target);
			}
		}


	function OnSceneGUI ()
		{
		var Target = target as EdyPath;
		var textPos : Vector3;
		
		var pathDist = 0.0;
		
		
		for (var i=0; i<Target.WayPoints.Length; i++)
			{
			var P = Target.WayPoints[i].position;
			pathDist += Target.WayPoints[i].distanceToNext;
			
			// Show position manipulator
			
			if (Tools.current == Tool.Move)
				{
				if (Tools.pivotMode == PivotMode.Pivot)
					{
					EditorGUI.BeginChangeCheck();
					
					var pos = Handles.PositionHandle(P, Tools.pivotRotation == PivotRotation.Global || Target.WayPoints.Length < 2 || Target.WayPoints[i].outTangent == Vector3.zero? Quaternion.identity : Quaternion.LookRotation(Target.WayPoints[i].outTangent));
					
					if (EditorGUI.EndChangeCheck())
						{
						Undo.RecordObject(target, "Path node moved");
						Target.WayPoints[i].position = pos;
						EditorUtility.SetDirty(target);
						}
					}
				}
			
			// Show distance labels

			if (Target.sceneShowDistances)
				{
				var textRelativePos = Target.sceneShowAbsoluteDistances? 0.85 : 0.5;
				var distanceValue = Target.sceneShowAbsoluteDistances? pathDist : Target.WayPoints[i].distanceToNext;
				
				if (i < Target.WayPoints.Length-1)
					{
					textPos = Target.Hermite(Target.WayPoints[i], Target.WayPoints[i+1], textRelativePos);					
					Handles.Label(textPos, String.Format("{0,5:0.000}", distanceValue));
					}
				else
					if (Target.closed && Target.WayPoints.Length > 1)
						{
						textPos = Target.Hermite(Target.WayPoints[i], Target.WayPoints[0], textRelativePos);					
						Handles.Label(textPos, String.Format("{0,5:0.000}", distanceValue));
						}
				}
				
			// Show node height labels
			
			if (Target.sceneShowHeights)
				{
				textPos = P - Vector3.up * HandleUtility.GetHandleSize(P) * 0.1;
				Handles.Label(textPos, String.Format("{0,5:0.000}", P.y));
				}
				
			// Show the node captions
			
			if (Target.sceneShowCaptions)
				{
				textPos = P + Vector3.up * HandleUtility.GetHandleSize(P) * 0.25;
				Handles.Label(textPos, String.Format("{0}", i));
				}
			}
			
		if (Tools.current == Tool.Move && Tools.pivotMode == PivotMode.Center)
			{
			EditorGUI.BeginChangeCheck();
			
			var median = Target.GetMedianPoint();
			var newPos = Handles.PositionHandle(median, Quaternion.identity);			
			
			if (EditorGUI.EndChangeCheck() && newPos != median)
				{
				Undo.RecordObject(target, "Path moved");
				Target.MoveAllPoints(newPos - median);
				EditorUtility.SetDirty(target);
				}
			}
			
		if (GUI.changed)
			{
			Target.CalculateTangents();
			// EditorUtility.SetDirty(target);
			}
			
		// if (Input.GetMouseButtonDown(0))
			// {
			// Undo.CreateSnapshot();
			// Undo.RegisterSnapshot();
			// }
		}
	}
