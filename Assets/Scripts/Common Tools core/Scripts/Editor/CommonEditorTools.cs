
using UnityEngine;
using UnityEditor;


// Common tools for using from the Editor


public class CommonEditorTools
	{
	// Useful for PropertyDrawer.OnGui

	public static bool IsUndoRedoPerformed ()
		{
		return Event.current.type == EventType.ExecuteCommand &&
			Event.current.commandName == "UndoRedoPerformed";
		}

	// Generic GUI.changed including Undo

	public static bool GUIChanged ()
		{
		return GUI.changed || IsUndoRedoPerformed();
		}

	// Convenience methods for a Editor Layout Foldout that respond to clicks on the text also,
	// not only at the fold arrow.

	public static bool LayoutFoldout(bool foldout, string content, string hint)
		{
		Rect rect = EditorGUILayout.GetControlRect();
		return EditorGUI.Foldout(rect, foldout, new GUIContent(content, hint), true);
		}


	public static bool LayoutFoldout(bool foldout, string content)
		{
		Rect rect = EditorGUILayout.GetControlRect();
		return EditorGUI.Foldout(rect, foldout, content, true);
		}


	// Persistent foldout
	//
	// NOTE! This is not required for PropertyDrawers.
	// Instead, use the read/write field property.isExpanded with the regular EditorGUI.Foldout.

	public static bool PersistentFoldout(Rect position, GUIContent label, string persistentId)
		{
		bool isExpanded = EditorPrefs.GetBool(persistentId);
		isExpanded = EditorGUI.Foldout(position, isExpanded, label, true);
		EditorPrefs.SetBool(persistentId, isExpanded);

		return isExpanded;
		}

	public static bool PersistentFoldout(Rect position, string label, string persistentId)
		{
		return PersistentFoldout(position, new GUIContent(label), persistentId);
		}


	// Detect whether a componen is enabled and belongs to an active object

	public static bool IsActiveAndPlaying (MonoBehaviour target)
		{
		return Application.isPlaying && target.enabled && target.gameObject.activeInHierarchy;
		}
	}

