using UnityEditor;
using UnityEngine;

namespace Draft.ScreenRecorder.Editor
{
    [CustomEditor(typeof(ScreenRecorderBase), true)]
    [CanEditMultipleObjects]
    public class ScreenRecorderBaseEditor : UnityEditor.Editor
    {
        // Recording Settings
        private SerializedProperty outputWidth;
        private SerializedProperty outputHeight;
        private SerializedProperty fps;
        private SerializedProperty quality;
        private SerializedProperty bitrate;
        private SerializedProperty useHardwareEncoder;
        private SerializedProperty outputFolder;
        private SerializedProperty filenamePrefix;

        // Audio Settings
        private SerializedProperty enableAudio;
        private SerializedProperty audioSource;
        private SerializedProperty audioSampleRate;
        private SerializedProperty audioChannels;
        private SerializedProperty audioBitrate;

        // Overlay Settings
        private SerializedProperty enableOverlay;
        private SerializedProperty overlayTexture;
        private SerializedProperty overlayRect;
        private SerializedProperty overlayOpacity;

        // Debug
        private SerializedProperty debugMode;

        // Editor-only overlay preset helpers (use properties to auto-save)
        private enum OverlayPivot { Fullscreen, TopLeft, TopRight, BottomLeft, BottomRight }

        private string PrefsKey
        {
            get
            {
                var globalId = GlobalObjectId.GetGlobalObjectIdSlow(target);
                return $"ScreenRecorder_{globalId}_";
            }
        }

        private OverlayPivot overlayPivot
        {
            get => (OverlayPivot)EditorPrefs.GetInt(PrefsKey + "Pivot", 0);
            set => EditorPrefs.SetInt(PrefsKey + "Pivot", (int)value);
        }

        private float overlayPresetWidth
        {
            get => EditorPrefs.GetFloat(PrefsKey + "Width", 150f);
            set => EditorPrefs.SetFloat(PrefsKey + "Width", value);
        }

        private float overlayPresetHeight
        {
            get => EditorPrefs.GetFloat(PrefsKey + "Height", 150f);
            set => EditorPrefs.SetFloat(PrefsKey + "Height", value);
        }

        private float overlayPresetOffsetX
        {
            get => EditorPrefs.GetFloat(PrefsKey + "OffsetX", 20f);
            set => EditorPrefs.SetFloat(PrefsKey + "OffsetX", value);
        }

        private float overlayPresetOffsetY
        {
            get => EditorPrefs.GetFloat(PrefsKey + "OffsetY", 20f);
            set => EditorPrefs.SetFloat(PrefsKey + "OffsetY", value);
        }

        protected virtual void OnEnable()
        {
            // Recording Settings
            outputWidth = serializedObject.FindProperty("outputWidth");
            outputHeight = serializedObject.FindProperty("outputHeight");
            fps = serializedObject.FindProperty("fps");
            quality = serializedObject.FindProperty("quality");
            bitrate = serializedObject.FindProperty("bitrate");
            useHardwareEncoder = serializedObject.FindProperty("useHardwareEncoder");
            outputFolder = serializedObject.FindProperty("outputFolder");
            filenamePrefix = serializedObject.FindProperty("filenamePrefix");

            // Audio Settings
            enableAudio = serializedObject.FindProperty("enableAudio");
            audioSource = serializedObject.FindProperty("audioSource");
            audioSampleRate = serializedObject.FindProperty("audioSampleRate");
            audioChannels = serializedObject.FindProperty("audioChannels");
            audioBitrate = serializedObject.FindProperty("audioBitrate");

            // Overlay Settings
            enableOverlay = serializedObject.FindProperty("enableOverlay");
            overlayTexture = serializedObject.FindProperty("overlayTexture");
            overlayRect = serializedObject.FindProperty("overlayRect");
            overlayOpacity = serializedObject.FindProperty("overlayOpacity");

            // Debug
            debugMode = serializedObject.FindProperty("debugMode");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var recorder = target as ScreenRecorderBase;
            //draw script field
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(recorder), typeof(ScreenRecorderBase), false);
            GUI.enabled = true;

            // Warning for GameViewRecorder
            if (recorder is GameViewRecorder)
            {
                // EditorGUILayout.HelpBox(
                //     "Recording in Editor may not capture correctly.\n" +
                //     "For accurate results, build and run as standalone application.",
                //     MessageType.Info);
                //EditorGUILayout.Space(5);
            }

            // Recording Settings
            DrawRecordingSettings();
            EditorGUILayout.Space(2);

            // Audio Settings
            DrawAudioSettings();
            EditorGUILayout.Space(2);

            // Overlay Settings
            DrawOverlaySettings(recorder);
            EditorGUILayout.Space(2);

            // Debug Settings
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.PropertyField(debugMode);
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawRecordingSettings()
        {
            EditorGUILayout.BeginVertical("box");

            GUILayout.Label("Recording Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space(1);
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(outputWidth, new GUIContent("Width", "0 = Screen width"));
            EditorGUILayout.PropertyField(outputHeight, new GUIContent("Height", "0 = Screen height"));

            EditorGUILayout.PropertyField(fps, new GUIContent("FPS", "Frames per second (15-60)"));
            EditorGUILayout.PropertyField(quality, new GUIContent("Quality", "0-10 (higher = better)"));
            EditorGUILayout.PropertyField(bitrate, new GUIContent("Bitrate (kbps)", "0 = auto"));
            EditorGUILayout.PropertyField(useHardwareEncoder, new GUIContent("Hardware Encoder"));

            EditorGUILayout.Space(5);
            EditorGUI.indentLevel--;

            EditorGUILayout.LabelField("Output", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(outputFolder, new GUIContent("Folder", "Empty = default folder"));
            if (string.IsNullOrEmpty(outputFolder.stringValue))
            {
#if UNITY_EDITOR_WIN
                string defaultFolder = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyVideos), "ScreenRecorder");
#elif UNITY_EDITOR_OSX
                string defaultFolder = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "Movies", "ScreenRecorder");
#else
                string defaultFolder = "Application.persistentDataPath/ScreenRecorder";
#endif
                EditorGUI.indentLevel++;

                EditorGUILayout.LabelField($"Default: {defaultFolder}", EditorStyles.helpBox);
                EditorGUI.indentLevel--;

            }
            EditorGUILayout.PropertyField(filenamePrefix, new GUIContent("Filename Prefix"));
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }

        private void DrawAudioSettings()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.PropertyField(enableAudio, new GUIContent("Enable Audio"));

            if (enableAudio.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(audioSource, new GUIContent("Source"));
                EditorGUILayout.PropertyField(audioSampleRate, new GUIContent("Sample Rate"));
                EditorGUILayout.PropertyField(audioChannels, new GUIContent("Channels"));
                EditorGUILayout.PropertyField(audioBitrate, new GUIContent("Bitrate (kbps)"));
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();
        }



        private void DrawOverlaySettings(ScreenRecorderBase recorder)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.PropertyField(enableOverlay, new GUIContent("Enable Overlay"));

            if (enableOverlay.boolValue)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(overlayTexture, new GUIContent("Overlay Texture"));

                // Get dimensions for preview
                int outW = recorder.outputWidth;
                int outH = recorder.outputHeight;
                Rect rect = overlayRect.rectValue;

                // Draw preview at top (centered)
                if (outW > 0 && outH > 0)
                {
                    EditorGUILayout.Space(5);
                    DrawOverlayPreview(outW, outH, rect);
                    EditorGUILayout.Space(5);
                }

                // Pivot selection
                EditorGUILayout.LabelField("Position", EditorStyles.boldLabel);

                // Pivot selection buttons
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Toggle(overlayPivot == OverlayPivot.Fullscreen, "Full", EditorStyles.miniButtonLeft))
                {
                    overlayPivot = OverlayPivot.Fullscreen;
                }
                if (GUILayout.Toggle(overlayPivot == OverlayPivot.TopLeft, "TL", EditorStyles.miniButtonMid))
                {
                    overlayPivot = OverlayPivot.TopLeft;
                }
                if (GUILayout.Toggle(overlayPivot == OverlayPivot.TopRight, "TR", EditorStyles.miniButtonMid))
                {
                    overlayPivot = OverlayPivot.TopRight;
                }
                if (GUILayout.Toggle(overlayPivot == OverlayPivot.BottomLeft, "BL", EditorStyles.miniButtonMid))
                {
                    overlayPivot = OverlayPivot.BottomLeft;
                }
                if (GUILayout.Toggle(overlayPivot == OverlayPivot.BottomRight, "BR", EditorStyles.miniButtonRight))
                {
                    overlayPivot = OverlayPivot.BottomRight;
                }
                EditorGUILayout.EndHorizontal();

                // Only show size/offset for non-fullscreen pivots
                if (overlayPivot != OverlayPivot.Fullscreen)
                {
                    EditorGUILayout.Space(5);

                    bool canCalculate = outW > 0 && outH > 0;

                    if (!canCalculate)
                    {
                        EditorGUILayout.HelpBox("Set Output Width/Height to calculate position.", MessageType.Info);
                    }

                    GUI.enabled = canCalculate;

                    // Offset fields
                    EditorGUILayout.BeginHorizontal();
                    overlayPresetOffsetX = EditorGUILayout.FloatField("Offset X", overlayPresetOffsetX, GUILayout.MinWidth(40));
                    overlayPresetOffsetY = EditorGUILayout.FloatField("Offset Y", overlayPresetOffsetY, GUILayout.MinWidth(40));
                    EditorGUILayout.EndHorizontal();

                    // Size fields
                    EditorGUILayout.BeginHorizontal();
                    overlayPresetWidth = EditorGUILayout.FloatField("Width", overlayPresetWidth, GUILayout.MinWidth(40));
                    overlayPresetHeight = EditorGUILayout.FloatField("Height", overlayPresetHeight, GUILayout.MinWidth(40));
                    EditorGUILayout.EndHorizontal();

                    GUI.enabled = true;

                    // Always calculate rect based on pivot
                    if (canCalculate)
                    {
                        rect = CalculateRectFromPivot(overlayPivot, outW, outH,
                            overlayPresetWidth, overlayPresetHeight,
                            overlayPresetOffsetX, overlayPresetOffsetY);
                        overlayRect.rectValue = rect;
                    }
                }
                else
                {
                    // Fullscreen mode
                    rect = new Rect(0, 0, 0, 0);
                    overlayRect.rectValue = rect;
                }

                EditorGUILayout.Space(3);
                EditorGUILayout.PropertyField(overlayOpacity, new GUIContent("Opacity"));

                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();
        }

        private Rect CalculateRectFromPivot(OverlayPivot pivot, int outW, int outH,
            float width, float height, float offsetX, float offsetY)
        {
            float x = 0, y = 0;

            switch (pivot)
            {
                case OverlayPivot.Fullscreen:
                    return new Rect(0, 0, 0, 0);

                case OverlayPivot.TopLeft:
                    x = offsetX;
                    y = offsetY;
                    break;

                case OverlayPivot.TopRight:
                    x = outW - width - offsetX;
                    y = offsetY;
                    break;

                case OverlayPivot.BottomLeft:
                    x = offsetX;
                    y = outH - height - offsetY;
                    break;

                case OverlayPivot.BottomRight:
                    x = outW - width - offsetX;
                    y = outH - height - offsetY;
                    break;
            }

            return new Rect(x, y, width, height);
        }

        private void DrawOverlayPreview(int outputW, int outputH, Rect overlayRect)
        {
            // Calculate preview size maintaining aspect ratio
            float maxWidth = EditorGUIUtility.currentViewWidth - 60;
            float aspectRatio = (float)outputW / outputH;
            float previewHeight = 120;
            float previewWidth = previewHeight * aspectRatio;

            // Limit max width
            if (previewWidth > maxWidth)
            {
                previewWidth = maxWidth;
                previewHeight = previewWidth / aspectRatio;
            }

            // Reserve space and center
            Rect layoutRect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth - 40, previewHeight + 10);
            float centerX = layoutRect.x + (layoutRect.width - previewWidth) / 2;

            Rect previewArea = new Rect(centerX, layoutRect.y, previewWidth, previewHeight);

            // Draw background (video area)
            EditorGUI.DrawRect(previewArea, new Color(0.2f, 0.2f, 0.2f, 1f));

            // Draw border
            Handles.BeginGUI();
            Handles.color = new Color(0.4f, 0.4f, 0.4f, 1f);
            Handles.DrawWireDisc(previewArea.center, Vector3.forward, 0); // Force handles init
            var borderPoints = new Vector3[]
            {
                new Vector3(previewArea.xMin, previewArea.yMin, 0),
                new Vector3(previewArea.xMax, previewArea.yMin, 0),
                new Vector3(previewArea.xMax, previewArea.yMax, 0),
                new Vector3(previewArea.xMin, previewArea.yMax, 0),
                new Vector3(previewArea.xMin, previewArea.yMin, 0)
            };
            Handles.DrawPolyLine(borderPoints);
            Handles.EndGUI();

            // Calculate scale factor
            float scaleX = previewWidth / outputW;
            float scaleY = previewHeight / outputH;

            // Calculate overlay rect in preview space
            float overlayW = overlayRect.width > 0 ? overlayRect.width : outputW;
            float overlayH = overlayRect.height > 0 ? overlayRect.height : outputH;

            // overlayRect uses top-origin Y (y=0 = top of video), same as Unity UI — no flip needed.
            Rect overlayPreviewRect = new Rect(
                previewArea.x + overlayRect.x * scaleX,
                previewArea.y + overlayRect.y * scaleY,
                overlayW * scaleX,
                overlayH * scaleY
            );

            // Clamp to preview bounds
            overlayPreviewRect = ClampRectToArea(overlayPreviewRect, previewArea);

            // Draw overlay rectangle
            Color overlayColor = new Color(0.3f, 0.7f, 1f, 0.5f);
            EditorGUI.DrawRect(overlayPreviewRect, overlayColor);

            // Draw overlay border
            Handles.BeginGUI();
            Handles.color = new Color(0.3f, 0.7f, 1f, 1f);
            var overlayBorderPoints = new Vector3[]
            {
                new Vector3(overlayPreviewRect.xMin, overlayPreviewRect.yMin, 0),
                new Vector3(overlayPreviewRect.xMax, overlayPreviewRect.yMin, 0),
                new Vector3(overlayPreviewRect.xMax, overlayPreviewRect.yMax, 0),
                new Vector3(overlayPreviewRect.xMin, overlayPreviewRect.yMax, 0),
                new Vector3(overlayPreviewRect.xMin, overlayPreviewRect.yMin, 0)
            };
            Handles.DrawPolyLine(overlayBorderPoints);
            Handles.EndGUI();

            // Draw size label
            string sizeLabel = $"{outputW}x{outputH}";
            var labelStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                alignment = TextAnchor.LowerRight,
                normal = { textColor = new Color(0.6f, 0.6f, 0.6f) }
            };
            GUI.Label(new Rect(previewArea.xMax - 80, previewArea.yMax - 16, 75, 15), sizeLabel, labelStyle);
        }

        private Rect ClampRectToArea(Rect rect, Rect area)
        {
            float xMin = Mathf.Max(rect.xMin, area.xMin);
            float yMin = Mathf.Max(rect.yMin, area.yMin);
            float xMax = Mathf.Min(rect.xMax, area.xMax);
            float yMax = Mathf.Min(rect.yMax, area.yMax);

            return new Rect(xMin, yMin, Mathf.Max(0, xMax - xMin), Mathf.Max(0, yMax - yMin));
        }
    }
}
