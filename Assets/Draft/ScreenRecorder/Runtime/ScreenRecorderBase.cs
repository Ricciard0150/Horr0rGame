using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;
using Unity.Collections;


namespace Draft.ScreenRecorder
{
    /// <summary>
    /// Recording state
    /// </summary>
    [UnityEngine.Scripting.Preserve]
    public enum RecordingState
    {
        Idle,
        Recording,
        Stopping
    }

    /// <summary>
    /// Base screen recorder component with AsyncGPUReadback support
    /// </summary>
    [UnityEngine.Scripting.Preserve]
    public abstract class ScreenRecorderBase : MonoBehaviour
    {
        [Tooltip("Output video width. 0 = use source resolution")]
        public int outputWidth = 0;

        [Tooltip("Output video height. 0 = use source resolution")]
        public int outputHeight = 0;

        [Tooltip("Frames per second (15-120)")]
        [Range(15, 60)]
        public int fps = 30;

        [Tooltip("Video quality (0-10, higher = better)")]
        [Range(0, 10)]
        public int quality = 5;

        [Tooltip("Video bitrate in kbps. 0 = auto based on resolution")]
        public int bitrate = 0;

        [Tooltip("Use hardware encoder if available")]
        public bool useHardwareEncoder = true;

        [Tooltip("Output folder path. Empty = default (Videos/ScreenRecorder)")]
        public string outputFolder = "";

        [Tooltip("Output filename prefix")]
        public string filenamePrefix = "recording";

        [Tooltip("Enable audio recording")]
        public bool enableAudio = false;

        [Tooltip("Audio source for recording")]
        public AudioSourceType audioSource = AudioSourceType.Game;

        [Tooltip("Audio sample rate (44100 or 48000)")]
        public int audioSampleRate = 48000;

        [Tooltip("Audio channels (1=mono, 2=stereo)")]
        [Range(1, 2)]
        public int audioChannels = 2;

        [Tooltip("Audio bitrate in kbps")]
        public int audioBitrate = 128;

        [Tooltip("Enable overlay on recording")]
        public bool enableOverlay = false;

        [Tooltip("Overlay texture. Alpha channel used for blending.")]
        public Texture2D overlayTexture;

        [Tooltip("Overlay position and size (x, y, width, height) in pixels. Use (0,0,outputWidth,outputHeight) for fullscreen.")]
        public Rect overlayRect = new Rect(0f, 0f, 0f, 0f);

        [Tooltip("Overlay opacity (0-1)")]
        [Range(0f, 1f)]
        public float overlayOpacity = 1f;

        [Tooltip("Show debug info in console")]
        public bool debugMode = false;

        // Events
        public event Action<string> OnRecordingComplete;
        public event Action<string> OnRecordingError;
        public event Action<int> OnFrameCaptured;

        // State
        public RecordingState State { get; protected set; } = RecordingState.Idle;
        public bool IsRecording => State == RecordingState.Recording;
        public int FrameCount { get; protected set; }
        public string CurrentOutputPath { get; protected set; }

        /// <summary>
        /// Elapsed recording time in seconds
        /// </summary>
        public float ElapsedTime { get; protected set; }

        /// <summary>
        /// Elapsed time formatted as MM:SS
        /// </summary>
        public string ElapsedTimeFormatted => TimeSpan.FromSeconds(ElapsedTime).ToString(@"mm\:ss");

        /// <summary>
        /// Current recording FPS (frames captured per second)
        /// </summary>
        public float RecordingFPS { get; protected set; }

        // Native handle - also expose for AudioCapture
        protected IntPtr recorderHandle = IntPtr.Zero;
        public IntPtr RecorderHandle => recorderHandle;
        protected RenderTexture captureTexture;

        // Audio capture component
        private AudioCapture audioCapture;
        private AudioListener cachedAudioListener;

        // Composite material for overlay/watermark
        private Material compositeMaterial;
        private RenderTexture compositeRT;

        // Keep delegates alive (prevent GC)
        private ScreenRecorderPlugin.RecordingCompleteCallback completeCallback;
        private ScreenRecorderPlugin.RecordingErrorCallback errorCallback;
        
        // Static instance tracking for IL2CPP
        private static readonly Dictionary<IntPtr, ScreenRecorderBase> instanceMap = new Dictionary<IntPtr, ScreenRecorderBase>();

        // Reusable frame buffer — avoids per-frame allocation in GPU readback
        private byte[] _frameBuffer;

        // FIFO of wall-clock capture timestamps (microseconds), one per in-flight
        // readback. Keeps video PTS matched to real time despite async readback delay.
        private readonly Queue<long> _captureTimestamps = new Queue<long>();

        // Pending callbacks to process on main thread
        private volatile bool pendingComplete;
        private volatile bool pendingCompleteSuccess;
        private volatile string pendingCompletePath;
        private volatile string pendingError;

        // Timing for FPS calculation
        private float recordingStartTime;
        private float lastFPSUpdateTime;
        private int lastFrameCountForFPS;

        protected virtual void Awake()
        {
            // Create delegates and keep them alive
            completeCallback = StaticOnNativeComplete;
            errorCallback = StaticOnNativeError;

            // Cache AudioListener to avoid O(n) scan at record start
            cachedAudioListener = FindAnyObjectByType<AudioListener>();
        }

        protected virtual void OnDestroy()
        {
            // Clean up audio capture synchronously — avoids async stop race with DestroyRecorder
            if (audioCapture != null)
            {
                audioCapture.StopCapture();
                Destroy(audioCapture);
                audioCapture = null;
            }

            DestroyRecorder();

            if (captureTexture != null)
            {
                captureTexture.Release();
                Destroy(captureTexture);
            }

            if (compositeRT != null)
            {
                compositeRT.Release();
                Destroy(compositeRT);
            }

            if (compositeMaterial != null)
            {
                Destroy(compositeMaterial);
            }
        }

        protected virtual void Update()
        {
            // Process pending callbacks on main thread
            if (pendingComplete)
            {
                pendingComplete = false;
                HandleRecordingComplete(pendingCompletePath, pendingCompleteSuccess);
            }

            if (!string.IsNullOrEmpty(pendingError))
            {
                string error = pendingError;
                pendingError = null;
                HandleRecordingError(error);
            }

            // Update recording statistics
            if (State == RecordingState.Recording)
            {
                ElapsedTime = Time.realtimeSinceStartup - recordingStartTime;

                // Update FPS every 0.5 seconds
                if (Time.realtimeSinceStartup - lastFPSUpdateTime >= 0.5f)
                {
                    float deltaTime = Time.realtimeSinceStartup - lastFPSUpdateTime;
                    int deltaFrames = FrameCount - lastFrameCountForFPS;
                    RecordingFPS = deltaFrames / deltaTime;

                    lastFPSUpdateTime = Time.realtimeSinceStartup;
                    lastFrameCountForFPS = FrameCount;
                }
            }
        }

        /// <summary>
        /// Start recording
        /// </summary>
        public virtual bool StartRecording()
        {
            if (State != RecordingState.Idle)
            {
                LogWarning("Cannot start recording: already recording or stopping");
                return false;
            }




            if (outputWidth <= 0 || outputHeight <= 0)
            {
                outputWidth = Screen.width;
                outputHeight = Screen.height;
            }


            // Ensure even dimensions for H.264
            outputWidth = (outputWidth + 1) & ~1;
            outputHeight = (outputHeight + 1) & ~1;



            // Setup capture texture
            SetupCaptureTexture(outputWidth, outputHeight);

            // Generate output path
            CurrentOutputPath = GenerateOutputPath();

            Log($"Starting recording: {outputWidth}x{outputHeight} @ {fps}fps -> {CurrentOutputPath}");

            // Create settings
            // Use Unity's actual output sample rate for audio to ensure consistency
            int actualAudioSampleRate = enableAudio ? AudioSettings.outputSampleRate : audioSampleRate;

            var settings = new RecordingSettings
            {
                width = outputWidth,
                height = outputHeight,
                fps = fps,
                bitrate = bitrate,
                quality = quality,
                useHardwareEncoder = useHardwareEncoder ? 1 : 0,
                outputPath = CurrentOutputPath,
                // Audio settings - use Unity's actual sample rate
                enableAudio = enableAudio ? 1 : 0,
                audioSampleRate = actualAudioSampleRate,
                audioChannels = audioChannels,
                audioBitrate = audioBitrate,
                audioSource = audioSource
            };

            // Create recorder
            recorderHandle = ScreenRecorderPlugin.ScreenRecorder_Create(
                settings, completeCallback, errorCallback);

            if (recorderHandle == IntPtr.Zero)
            {
                LogError("Failed to create recorder");
                return false;
            }
            
            // Track instance for static callbacks
            lock (instanceMap)
            {
                instanceMap[recorderHandle] = this;
            }

            // Prepare audio BEFORE starting encoder (component setup only, not yet capturing)
            if (enableAudio && (audioSource == AudioSourceType.Game || audioSource == AudioSourceType.Both))
            {
                SetupAudioCapture();
            }

            // Start native encoder
            if (ScreenRecorderPlugin.ScreenRecorder_Start(recorderHandle) == 0)
            {
                string error = ScreenRecorderPlugin.GetErrorString(recorderHandle);
                LogError($"Failed to start recording: {error}");
                if (audioCapture != null)
                {
                    audioCapture.StopCapture();
                    Destroy(audioCapture);
                    audioCapture = null;
                }
                DestroyRecorder();
                return false;
            }

            // Activate video and audio simultaneously
            State = RecordingState.Recording;
            FrameCount = 0;
            ElapsedTime = 0f;
            RecordingFPS = 0f;
            recordingStartTime = Time.realtimeSinceStartup;
            lastFPSUpdateTime = recordingStartTime;
            lastFrameCountForFPS = 0;
            _captureTimestamps.Clear();

            // Activate audio capture — same moment as State=Recording so OnAudioFilterRead starts in sync
            if (audioCapture != null)
            {
                audioCapture.BeginCapture();
            }

            Log($"Recording started (audio: {enableAudio})");
            return true;
        }

        /// <summary>
        /// Setup audio capture from AudioListener
        /// </summary>
        private void SetupAudioCapture()
        {
            if (cachedAudioListener == null)
                cachedAudioListener = FindAnyObjectByType<AudioListener>(); // late fallback

            if (cachedAudioListener == null)
            {
                LogWarning("No AudioListener found - audio recording disabled");
                return;
            }

            audioCapture = cachedAudioListener.GetComponent<AudioCapture>();
            if (audioCapture == null)
                audioCapture = cachedAudioListener.gameObject.AddComponent<AudioCapture>();

            audioCapture.Initialize(this, audioChannels);
            Log("Audio capture initialized");
        }

        /// <summary>
        /// Stop recording
        /// </summary>
        public virtual void StopRecording()
        {
            if (State != RecordingState.Recording)
            {
                LogWarning("Cannot stop recording: not recording");
                return;
            }

            // Stop audio capture first
            if (audioCapture != null)
            {
                audioCapture.StopCapture();
                Destroy(audioCapture);
                audioCapture = null;
            }

            Log($"Stopping recording... ({FrameCount} frames captured)");
            State = RecordingState.Stopping;

            ScreenRecorderPlugin.ScreenRecorder_Stop(recorderHandle);
        }



        /// <summary>
        /// Setup the capture RenderTexture
        /// </summary>
        protected virtual void SetupCaptureTexture(int width, int height)
        {
            if (captureTexture != null)
            {
                if (captureTexture.width == width && captureTexture.height == height)
                    return;

                captureTexture.Release();
                Destroy(captureTexture);
            }

            Log($"Creating capture texture: {width}x{height}");

            // Use explicit R8G8B8A8_SRGB so the RT stores gamma-encoded bytes on all platforms
            // (builds and editor). With RenderTextureFormat.ARGB32 the build resolves to
            // R8G8B8A8_UNorm in Linear color space, causing the shader's GammaToLinearSpace
            // output to be stored as linear bytes that the encoder then misinterprets as gamma.
            captureTexture = new RenderTexture(width, height, 0, GraphicsFormat.R8G8B8A8_SRGB);
            captureTexture.Create();
        }

        /// <summary>
        /// Request async GPU readback and send frame to encoder
        /// </summary>
        protected void RequestPushFrameCapture()
        {
            if (State != RecordingState.Recording || captureTexture == null)
                return;

            // Stamp the wall-clock capture time now (at request) so the encoder can
            // keep video in sync with audio. Readback callbacks fire in request order,
            // so a FIFO queue keeps each timestamp matched to its frame even though
            // the readback itself completes asynchronously a few frames later.
            long captureMicros = (long)((Time.realtimeSinceStartup - recordingStartTime) * 1_000_000.0);
            _captureTimestamps.Enqueue(captureMicros);

            // Request async readback using the same sRGB format as captureTexture so no
            // sRGB->linear conversion is applied on platforms that would otherwise linearize
            // when a non-sRGB destination format is requested (e.g. D3D12 on Windows).
            AsyncGPUReadback.Request(captureTexture, 0, GraphicsFormat.R8G8B8A8_SRGB, OnReadbackComplete);
        }

        protected void OnReadbackComplete(AsyncGPUReadbackRequest request)
        {
            // Always dequeue the matching timestamp, even on early-out, so the queue
            // stays aligned with the readback request order.
            long frameTimestamp = _captureTimestamps.Count > 0 ? _captureTimestamps.Dequeue() : -1;

            if (request.hasError)
            {
                LogWarning("GPU readback error");
                return;
            }

            if (State != RecordingState.Recording)
                return;

            // Get pixel data
            var data = request.GetData<byte>();

            if (!data.IsCreated || data.Length == 0)
            {
                LogWarning("Invalid readback data");
                return;
            }

            // Push frame to native encoder — reuse buffer to avoid per-frame heap allocation
            if (_frameBuffer == null || _frameBuffer.Length != data.Length)
                _frameBuffer = new byte[data.Length];
            data.CopyTo(_frameBuffer);

            GCHandle handle = GCHandle.Alloc(_frameBuffer, GCHandleType.Pinned);
            try
            {
                int result = ScreenRecorderPlugin.ScreenRecorder_PushFrame(
                    recorderHandle,
                    handle.AddrOfPinnedObject(),
                    _frameBuffer.Length,
                    PixelFormat.RGBA32,
                    frameTimestamp);

                if (result != 0)
                {
                    FrameCount++;
                    OnFrameCaptured?.Invoke(FrameCount);
                }
            }
            finally
            {
                handle.Free();
            }
        }

        internal void OnNativeComplete(string videoPath, int success)
        {
            // Called from native thread - defer to main thread
            pendingCompletePath = videoPath;
            pendingCompleteSuccess = success != 0;
            pendingComplete = true;
        }

        internal void OnNativeError(string errorMessage)
        {
            // Called from native thread - defer to main thread
            pendingError = errorMessage;
        }

        private void HandleRecordingComplete(string path, bool success)
        {
            Log($"Recording complete: {path} (success: {success})");

            State = RecordingState.Idle;
            DestroyRecorder();

            if (success)
            {
                OnRecordingComplete?.Invoke(path);
            }
            else
            {
                OnRecordingError?.Invoke("Recording failed");
            }
        }

        private void HandleRecordingError(string error)
        {
            LogError($"Recording error: {error}");
            OnRecordingError?.Invoke(error);
        }

        private void DestroyRecorder()
        {
            if (recorderHandle != IntPtr.Zero)
            {
                // Remove from instance map
                lock (instanceMap)
                {
                    instanceMap.Remove(recorderHandle);
                }
                
                ScreenRecorderPlugin.ScreenRecorder_Destroy(recorderHandle);
                recorderHandle = IntPtr.Zero;
            }
        }
        
        // Static callbacks for IL2CPP compatibility
        [MonoPInvokeCallback(typeof(ScreenRecorderPlugin.RecordingCompleteCallback))]
        private static void StaticOnNativeComplete(string videoPath, int success)
        {
            var targets = new List<ScreenRecorderBase>();
            lock (instanceMap)
            {
                foreach (var kvp in instanceMap)
                {
                    if (kvp.Value.State == RecordingState.Stopping)
                        targets.Add(kvp.Value);
                }
                // Fallback: notify all if none in Stopping state
                if (targets.Count == 0)
                {
                    foreach (var kvp in instanceMap)
                        targets.Add(kvp.Value);
                }
            }
            foreach (var target in targets)
                target.OnNativeComplete(videoPath, success);
        }

        [MonoPInvokeCallback(typeof(ScreenRecorderPlugin.RecordingErrorCallback))]
        private static void StaticOnNativeError(string errorMessage)
        {
            var targets = new List<ScreenRecorderBase>();
            lock (instanceMap)
            {
                foreach (var kvp in instanceMap)
                    targets.Add(kvp.Value);
            }
            foreach (var target in targets)
                target.OnNativeError(errorMessage);
        }

        protected string GenerateOutputPath()
        {
            string folder = string.IsNullOrEmpty(outputFolder)
                ? GetDefaultOutputFolder()
                : outputFolder;

            // Ensure directory exists
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string filename = $"{filenamePrefix}_{timestamp}.mp4";

            return Path.Combine(folder, filename);
        }

        protected string GetDefaultOutputFolder()
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "ScreenRecorder");
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Movies", "ScreenRecorder");
#else
            return Path.Combine(Application.persistentDataPath, "ScreenRecorder");
#endif
        }

        protected void Log(string message)
        {
            if (debugMode)
            {
                Debug.Log($"[ScreenRecorder] {message}");
            }
        }

        protected void LogWarning(string message)
        {
            Debug.LogWarning($"[ScreenRecorder] {message}");
        }

        protected void LogError(string message)
        {
            Debug.LogError($"[ScreenRecorder] {message}");
        }

        /// <summary>
        /// Setup the composite material for overlay rendering
        /// </summary>
        protected void SetupCompositeMaterial()
        {
            if (!enableOverlay)
            {
                return;
            }

            // Load shader
            Shader compositeShader = Shader.Find("Hidden/ScreenRecorder/Composite");
            if (compositeShader == null)
            {
                LogError("Composite shader not found! Make sure RecordingComposite.shader is in project.");
                return;
            }

            // Release any existing resources before creating new ones (guard against restart)
            if (compositeRT != null)
            {
                compositeRT.Release();
                Destroy(compositeRT);
                compositeRT = null;
            }
            if (compositeMaterial != null)
            {
                Destroy(compositeMaterial);
                compositeMaterial = null;
            }

            compositeMaterial = new Material(compositeShader);
            compositeMaterial.hideFlags = HideFlags.HideAndDontSave;

            // Create composite RT
            compositeRT = new RenderTexture(outputWidth, outputHeight, 0, RenderTextureFormat.ARGB32);
            compositeRT.Create();

            Log("Composite material initialized");
        }

        /// <summary>
        /// Configure the composite material's overlay properties for the current frame.
        /// Returns true if an overlay should be drawn (caller does the actual blit).
        /// Shared by the Built-in (immediate Graphics.Blit) and URP (CommandBuffer) paths.
        /// </summary>
        protected bool TryConfigureOverlay()
        {
            if (compositeMaterial == null || !enableOverlay || overlayTexture == null)
            {
                return false;
            }

            // Calculate actual rect (default to fullscreen if size is 0)
            float rectX = overlayRect.x;
            float rectY = overlayRect.y;
            float rectW = overlayRect.width > 0 ? overlayRect.width : outputWidth;
            float rectH = overlayRect.height > 0 ? overlayRect.height : outputHeight;

            // Convert pixel coordinates to normalized (0-1) for shader.
            // UV y=0 maps to video row 0 (top) because captureTexture is pre-flipped;
            // rectY is already in top-origin video space, so no flip needed here.
            float normalizedX = rectX / outputWidth;
            float normalizedY = rectY / outputHeight;
            float normalizedW = rectW / outputWidth;
            float normalizedH = rectH / outputHeight;

            compositeMaterial.SetFloat("_OverlayEnabled", 1f);
            compositeMaterial.SetTexture("_OverlayTex", overlayTexture);
            compositeMaterial.SetVector("_OverlayRect", new Vector4(
                normalizedX, normalizedY,
                normalizedW, normalizedH
            ));
            compositeMaterial.SetFloat("_OverlayOpacity", overlayOpacity);

            return true;
        }

        /// <summary>Composite RenderTexture / material for use by the URP CommandBuffer path.</summary>
        protected RenderTexture CompositeRT => compositeRT;
        protected Material CompositeMaterial => compositeMaterial;

        /// <summary>
        /// Apply composite overlay to the source texture (Built-in immediate path).
        /// </summary>
        /// <param name="source">Source RenderTexture to composite</param>
        /// <returns>Composited RenderTexture (or source if no compositing needed)</returns>
        protected RenderTexture ApplyComposite(RenderTexture source)
        {
            if (!TryConfigureOverlay())
            {
                return source;
            }

            // Blit with composite shader
            Graphics.Blit(source, compositeRT, compositeMaterial);

            return compositeRT;
        }
    }

    /// <summary>
    /// Captures Unity audio from AudioListener and sends to native encoder.
    /// Attach this to a GameObject with an AudioListener component.
    /// </summary>
    [RequireComponent(typeof(AudioListener))]
    [UnityEngine.Scripting.Preserve]
    public class AudioCapture : MonoBehaviour
    {
        private ScreenRecorderBase recorder;
        private short[] convertedSamples;
        private GCHandle samplesHandle;
        private IntPtr pinnedSamplesPtr;
        private volatile bool isCapturing;
        private int channels;

        /// <summary>
        /// Initialize audio capture with a recorder
        /// </summary>
        public void Initialize(ScreenRecorderBase targetRecorder, int audioChannels)
        {
            recorder = targetRecorder;
            channels = audioChannels;

            // Pre-allocate with large fixed size — covers any DSP buffer size, avoids per-callback allocation
            int bufferSize = 8192 * channels;
            if (samplesHandle.IsAllocated)
                samplesHandle.Free();
            convertedSamples = new short[bufferSize];
            samplesHandle = GCHandle.Alloc(convertedSamples, GCHandleType.Pinned);
            pinnedSamplesPtr = samplesHandle.AddrOfPinnedObject();
            // isCapturing stays false — call BeginCapture() to activate
        }

        /// <summary>
        /// Begin audio capture (activate after Initialize)
        /// </summary>
        public void BeginCapture()
        {
            isCapturing = true;
        }

        /// <summary>
        /// Stop audio capture
        /// </summary>
        public void StopCapture()
        {
            isCapturing = false;

            pinnedSamplesPtr = IntPtr.Zero;  // zero before freeing — eliminates TOCTOU race

            if (samplesHandle.IsAllocated)
            {
                samplesHandle.Free();
            }

            recorder = null;
        }

        /// <summary>
        /// Unity audio callback - called on the audio thread
        /// </summary>
        private void OnAudioFilterRead(float[] data, int channelCount)
        {
            if (!isCapturing || recorder == null || !recorder.IsRecording)
                return;

            int count = Mathf.Min(data.Length, convertedSamples.Length);

            // Convert float samples (-1.0 to 1.0) to 16-bit PCM
            ConvertFloatToInt16(data, convertedSamples, count);

            // Send to recorder
            PushAudioToRecorder(count);
        }

        private void ConvertFloatToInt16(float[] input, short[] output, int count)
        {
            for (int i = 0; i < count; i++)
            {
                // Clamp and convert
                float sample = Mathf.Clamp(input[i], -1f, 1f);
                output[i] = (short)(sample * 32767f);
            }
        }

        private void PushAudioToRecorder(int count)
        {
            if (recorder == null || recorder.RecorderHandle == IntPtr.Zero || pinnedSamplesPtr == IntPtr.Zero)
                return;

            ScreenRecorderPlugin.ScreenRecorder_PushAudioSamples(
                recorder.RecorderHandle, pinnedSamplesPtr, count);
        }

        private void OnDestroy()
        {
            StopCapture();
        }
    }
}
