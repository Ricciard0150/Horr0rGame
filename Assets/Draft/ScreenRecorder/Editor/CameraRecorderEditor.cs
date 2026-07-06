using UnityEditor;
using UnityEngine;

namespace Draft.ScreenRecorder.Editor
{

    [CustomEditor(typeof(CameraRecorder), true)]
    [CanEditMultipleObjects]
    public class CameraRecorderEditor : ScreenRecorderBaseEditor
    {
        private SerializedProperty targetCameraProp;

        protected override void OnEnable()
        {
            base.OnEnable();
            targetCameraProp = serializedObject.FindProperty("targetCamera");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.PropertyField(targetCameraProp, new GUIContent("Target Camera"));
            EditorGUILayout.HelpBox("Camera to record from. If null, uses Camera.main", MessageType.Info);
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
