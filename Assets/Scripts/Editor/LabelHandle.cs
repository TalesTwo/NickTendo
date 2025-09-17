using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DoorTriggerInteraction))]
public class LabelHandle : Editor
{
    
    private static GUIStyle _labelStyle;
    
    private void OnEnable()
    {
        _labelStyle = new GUIStyle();
        _labelStyle.fontSize = 12;
        _labelStyle.normal.textColor = Color.white;
        _labelStyle.alignment = TextAnchor.MiddleCenter;
        _labelStyle.fontStyle = FontStyle.Bold;
    }

    private void OnSceneGUI()
    {
        
        DoorTriggerInteraction door = (DoorTriggerInteraction)target;
        if (!door) { return; }
        
        Handles.BeginGUI();
        Handles.Label(door.transform.position + new Vector3(0f, 1f, 0f), door.CurrentDoorPosition.ToString(), _labelStyle);
        Handles.EndGUI();
    }
}
