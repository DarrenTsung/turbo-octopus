using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor (typeof(VineController))]
public class VineEditor : Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		VineController vineController = target as VineController;
		EditorGUILayout.Separator();
		EditorGUILayout.LabelField("Change Vine Settings", EditorStyles.boldLabel);
		vineController.segmentCount = EditorGUILayout.IntField("Set Segment Count", vineController.segmentCount);
		if (GUILayout.Button("Commit")) {
			EditorUtility.SetDirty (target);
			vineController.UpdateSegmentCount();
		}
	}

	public void OnSceneGUI() {
	}
}
