#pragma strict


class GUIUtils
	{
	// Non-dirty foldout
	
	static function EditorFoldout(foldout : boolean, content : String) : boolean
		{
		return EditorFoldout(foldout, GUIContent(content), EditorStyles.foldout);
		}	
	
	static function EditorFoldout(foldout : boolean, content : GUIContent) : boolean
		{
		return EditorFoldout(foldout, content, EditorStyles.foldout);
		}	

	static function EditorFoldout(foldout : boolean, content : String, style : GUIStyle) : boolean
		{
		return EditorFoldout(foldout, GUIContent(content), style);
		}	
	
	static function EditorFoldout(foldout : boolean, content : GUIContent, style : GUIStyle) : boolean
		{
		var tmp = GUI.changed;
		
		var result = EditorGUILayout.Foldout(foldout, content, style);
		if (!tmp && result != foldout) GUI.changed = false; 
		return result;
		}
	}
