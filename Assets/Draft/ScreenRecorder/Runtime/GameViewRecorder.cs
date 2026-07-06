using System.Collections;
using UnityEngine;

namespace Draft.ScreenRecorder
{
    /// <summary>
    /// Record the game view (entire screen output)
    /// </summary>
    [AddComponentMenu("Screen Recorder/Game View Recorder")]
    [UnityEngine.Scripting.Preserve]
    public class GameViewRecorder : ScreenRecorderBase
    {
        private float nextCaptureTime;
        private float captureInterval;
        private Material screenCaptureMaterial;

        public override bool StartRecording()
        {
            bool result = base.StartRecording();
            if (result)
            {
                // Setup material for color space correct blit
                Shader blitShader = Shader.Find("Hidden/ScreenRecorder/ScreenCaptureBlit");
                if (blitShader != null)
                {
                    screenCaptureMaterial = new Material(blitShader);
                    screenCaptureMaterial.hideFlags = HideFlags.HideAndDontSave;
                }
                else
                {
                    LogError("ScreenCaptureBlit shader not found! Color space issues may occur.");
                }

                // Setup composite material for overlay/watermark
                SetupCompositeMaterial();

                captureInterval = 1.0f / fps;
                nextCaptureTime = Time.unscaledTime;
            }
            return result;
        }

        public override void StopRecording()
        {
            if (screenCaptureMaterial != null)
            {
                Destroy(screenCaptureMaterial);
                screenCaptureMaterial = null;
            }

            base.StopRecording();
        }


        private IEnumerator CaptureRoutine()
        {
            // Wait for the frame to finish rendering
            yield return new WaitForEndOfFrame();

            // Capture the screen as a Texture2D
            Texture2D screenTex = ScreenCapture.CaptureScreenshotAsTexture();

            if (captureTexture != null && screenTex != null)
            {
                // Blit with color space correction
                if (screenCaptureMaterial != null)
                {
                    Graphics.Blit(screenTex, captureTexture, screenCaptureMaterial);
                }
                else
                {
                    // Fallback if shader not found
                    Graphics.Blit(screenTex, captureTexture,new Vector2(1, -1), new Vector2(0, 1)); // Flip Y

                }

                // Then apply overlay/watermark composite
                RenderTexture sourceRT = ApplyComposite(captureTexture);

                if (sourceRT != captureTexture)
                {
                    // Copy composited result back to capture texture
                    Graphics.Blit(sourceRT, captureTexture);
                }

                // Finally, request the frame for encoding
                RequestPushFrameCapture();
            }

            // Cleanup temporary texture
            Destroy(screenTex);
        }

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


                // Start capture coroutine
                StartCoroutine(CaptureRoutine());
            }
        }

        protected override void OnDestroy()
        {
            if (screenCaptureMaterial != null)
            {
                Destroy(screenCaptureMaterial);
                screenCaptureMaterial = null;
            }

            base.OnDestroy();
        }
    }
}
