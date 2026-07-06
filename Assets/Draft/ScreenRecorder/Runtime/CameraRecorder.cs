using UnityEngine;
using UnityEngine.Rendering;

namespace Draft.ScreenRecorder
{
    /// <summary>
    /// Record from a specific camera
    /// </summary>
    [AddComponentMenu("Screen Recorder/Camera Recorder")]
    [UnityEngine.Scripting.Preserve]
    public class CameraRecorder : ScreenRecorderBase
    {
        [Header("Camera Settings")]
        [Tooltip("Camera to record from. If null, uses Camera.main")]
        public Camera targetCamera;

        private float nextCaptureTime;
        private float captureInterval;
        private bool shouldCaptureThisFrame;
        private CaptureHelper captureHelper;

        // URP / SRP capture state. OnRenderImage is never called under a Scriptable Render
        // Pipeline. CameraCaptureBridge taps targetCamera's output without touching its
        // targetTexture, so the camera keeps presenting to the screen normally — no flicker,
        // no UI compositing artifacts.
        private bool usingSRP;
#if SCREENRECORDER_URP
        private System.Action<RenderTargetIdentifier, CommandBuffer> captureAction;
#endif

        /// <summary>
        /// Attached to targetCamera's GameObject so that OnRenderImage fires for that camera.
        /// OnRenderImage always receives a correctly-oriented src RT from Unity regardless of
        /// platform UV conventions or camera depth order, avoiding the back-buffer flip/color
        /// issues that arise when capturing via CommandBuffer + CameraTarget.
        ///
        /// Built-in Render Pipeline only — under URP/HDRP this callback never fires.
        /// </summary>
        private class CaptureHelper : MonoBehaviour
        {
            internal CameraRecorder recorder;

            private void OnRenderImage(RenderTexture src, RenderTexture dst)
            {
                recorder?.OnCameraRenderImage(src);
                Graphics.Blit(src, dst);
            }
        }

        protected override void Awake()
        {
            base.Awake();

            if (targetCamera == null)
            {
                targetCamera = Camera.main;
            }
        }

        public override bool StartRecording()
        {
            if (targetCamera == null)
            {
                targetCamera = Camera.main;
                if (targetCamera == null)
                {
                    LogError("No target camera assigned");
                    return false;
                }
            }

            bool result = base.StartRecording();

            if (result)
            {
                // Setup composite material for overlay/watermark
                SetupCompositeMaterial();

                // Calculate capture interval based on target FPS
                captureInterval = 1.0f / fps;
                nextCaptureTime = Time.unscaledTime;

                // GraphicsSettings.currentRenderPipeline is non-null when URP/HDRP is active.
                usingSRP = GraphicsSettings.currentRenderPipeline != null;

                if (usingSRP)
                {
                    SetupSRPCapture();
                }
                else
                {
                    // Built-in pipeline: OnRenderImage fires on the camera's GameObject.
                    captureHelper = targetCamera.gameObject.AddComponent<CaptureHelper>();
                    captureHelper.recorder = this;
                }
            }

            return result;
        }

        public override void StopRecording()
        {
            if (captureHelper != null)
            {
                Destroy(captureHelper);
                captureHelper = null;
            }

            TeardownSRPCapture();

            base.StopRecording();
        }

        /// <summary>
        /// Register a CameraCaptureBridge action on targetCamera. URP runs it at the end of the
        /// camera's render loop and hands us the camera's color target, which we copy into
        /// captureTexture. targetTexture is left untouched, so on-screen presentation is normal.
        /// </summary>
        private void SetupSRPCapture()
        {
#if SCREENRECORDER_URP
            captureAction = OnCameraCapture;
            CameraCaptureBridge.AddCaptureAction(targetCamera, captureAction);
            CameraCaptureBridge.enabled = true;
#else
            LogError("Recording under a Scriptable Render Pipeline requires the URP package " +
                     "(com.unity.render-pipelines.universal). CameraRecorder cannot capture.");
#endif
        }

        private void TeardownSRPCapture()
        {
            if (!usingSRP)
                return;

            usingSRP = false;
#if SCREENRECORDER_URP
            if (captureAction != null && targetCamera != null)
                CameraCaptureBridge.RemoveCaptureAction(targetCamera, captureAction);
            captureAction = null;
#endif
        }

#if SCREENRECORDER_URP
        /// <summary>
        /// URP capture hook. Records the camera output → captureTexture blit, the optional
        /// overlay composite, and an ordered async readback all into the same CommandBuffer,
        /// so each step runs in order on the GPU. The camera keeps presenting to the screen.
        /// </summary>
        private void OnCameraCapture(RenderTargetIdentifier source, CommandBuffer cb)
        {
            if (!IsRecording || !shouldCaptureThisFrame)
                return;

            shouldCaptureThisFrame = false;

            // Flip Y to match the orientation the encoder expects (same as the Built-in path).
            cb.Blit(source, captureTexture, new Vector2(1f, -1f), new Vector2(0f, 1f));

            // Overlay/watermark composite, recorded into the same buffer so it lands before
            // the readback. compositeRT round-trip keeps captureTexture as the readback source.
            if (TryConfigureOverlay())
            {
                cb.Blit(captureTexture, CompositeRT, CompositeMaterial);
                cb.Blit(CompositeRT, captureTexture);
            }

            cb.RequestAsyncReadback(captureTexture, 0, TextureFormat.RGBA32, OnReadbackComplete);
        }
#endif

        private void LateUpdate()
        {
            if (!IsRecording)
                return;

            // Time-based capture at target FPS
            if (Time.unscaledTime >= nextCaptureTime)
            {
                nextCaptureTime += captureInterval;

                // Prevent accumulation if we fell behind
                if (nextCaptureTime < Time.unscaledTime)
                {
                    nextCaptureTime = Time.unscaledTime + captureInterval;
                }

                // Mark that we should capture after camera renders
                shouldCaptureThisFrame = true;
            }
        }

        /// <summary>
        /// Called from CaptureHelper.OnRenderImage. The src RT is always correctly
        /// oriented by Unity (platform Y-flip is handled internally before OnRenderImage).
        /// </summary>
        private void OnCameraRenderImage(RenderTexture src)
        {
            if (!IsRecording || !shouldCaptureThisFrame)
                return;

            shouldCaptureThisFrame = false;

            Graphics.Blit(src, captureTexture, new Vector2(1f, -1f), new Vector2(0f, 1f));

            // Apply overlay/watermark composite
            RenderTexture sourceRT = ApplyComposite(captureTexture);
            if (sourceRT != captureTexture)
            {
                Graphics.Blit(sourceRT, captureTexture);
            }

            RequestPushFrameCapture();
        }

        protected override void OnDestroy()
        {
            if (captureHelper != null)
            {
                Destroy(captureHelper);
                captureHelper = null;
            }

            TeardownSRPCapture();

            base.OnDestroy();
        }
    }
}
