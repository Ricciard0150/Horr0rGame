using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Draft.ScreenRecorder
{
    /// <summary>
    /// Pixel format for frame data
    /// </summary>
    [UnityEngine.Scripting.Preserve]
    public enum PixelFormat
    {
        RGBA32 = 0,
        BGRA32 = 1,
        RGB24 = 2,
        ARGB32 = 3
    }

    /// <summary>
    /// Audio source type for recording
    /// </summary>
    [UnityEngine.Scripting.Preserve]
    public enum AudioSourceType
    {
        None = 0,       // No audio recording
        Game = 1,       // Game audio from AudioListener
        Microphone = 2, // Microphone input
        Both = 3        // Mixed game audio and microphone
    }

    /// <summary>
    /// Recording settings structure (must match C++ struct layout)
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    [UnityEngine.Scripting.Preserve]
    public struct RecordingSettings
    {
        public int width;
        public int height;
        public int fps;
        public int bitrate;
        public int quality;
        public int useHardwareEncoder;
        [MarshalAs(UnmanagedType.LPStr)]
        public string outputPath;

        // Audio settings
        public int enableAudio;
        public int audioSampleRate;
        public int audioChannels;
        public int audioBitrate;
        public AudioSourceType audioSource;

        public static RecordingSettings Default(int width, int height, string outputPath)
        {
            return new RecordingSettings
            {
                width = width,
                height = height,
                fps = 30,
                bitrate = 0, // Auto
                quality = 5,
                useHardwareEncoder = 1,
                outputPath = outputPath,
                enableAudio = 0,
                audioSampleRate = 48000,
                audioChannels = 2,
                audioBitrate = 128,
                audioSource = AudioSourceType.None
            };
        }

        public static RecordingSettings WithAudio(int width, int height, string outputPath,
                                                   AudioSourceType source = AudioSourceType.Game,
                                                   int sampleRate = 48000, int channels = 2)
        {
            return new RecordingSettings
            {
                width = width,
                height = height,
                fps = 30,
                bitrate = 0,
                quality = 5,
                useHardwareEncoder = 1,
                outputPath = outputPath,
                enableAudio = 1,
                audioSampleRate = sampleRate,
                audioChannels = channels,
                audioBitrate = 128,
                audioSource = source
            };
        }
    }

    /// <summary>
    /// Native plugin bindings for ScreenRecorder
    /// </summary>
    public static class ScreenRecorderPlugin
    {
        private const string LIBRARY_NAME = "ScreenRecorder";

        /// <summary>
        /// Callback when recording is complete
        /// </summary>
        /// <param name="videoPath">Path to the recorded video file</param>
        /// <param name="success">1 if successful, 0 if failed</param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [UnityEngine.Scripting.Preserve]
        public delegate void RecordingCompleteCallback(
            [MarshalAs(UnmanagedType.LPStr)] string videoPath,
            int success);

        /// <summary>
        /// Callback when an error occurs
        /// </summary>
        /// <param name="errorMessage">Error description</param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [UnityEngine.Scripting.Preserve]
        public delegate void RecordingErrorCallback(
            [MarshalAs(UnmanagedType.LPStr)] string errorMessage);

        /// <summary>
        /// Create a new screen recorder instance
        /// </summary>
        [DllImport(LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ScreenRecorder_Create(
            RecordingSettings settings,
            RecordingCompleteCallback onComplete,
            RecordingErrorCallback onError);

        /// <summary>
        /// Start recording
        /// </summary>
        [DllImport(LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ScreenRecorder_Start(IntPtr recorder);

        /// <summary>
        /// Push a frame to the encoder
        /// </summary>
        [DllImport(LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ScreenRecorder_PushFrame(
            IntPtr recorder,
            IntPtr data,
            int dataSize,
            PixelFormat format,
            long timestamp);

        /// <summary>
        /// Push audio samples to the encoder
        /// </summary>
        /// <param name="recorder">Recorder handle</param>
        /// <param name="samples">PCM audio samples (16-bit signed, interleaved)</param>
        /// <param name="sampleCount">Number of samples (total across all channels)</param>
        /// <returns>1 on success, 0 on failure</returns>
        [DllImport(LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ScreenRecorder_PushAudioSamples(
            IntPtr recorder,
            IntPtr samples,
            int sampleCount);

        /// <summary>
        /// Stop recording (async, callback will be invoked when complete)
        /// </summary>
        [DllImport(LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ScreenRecorder_Stop(IntPtr recorder);

        /// <summary>
        /// Check if recording is in progress
        /// </summary>
        [DllImport(LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ScreenRecorder_IsRecording(IntPtr recorder);

        /// <summary>
        /// Get the current frame count
        /// </summary>
        [DllImport(LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ScreenRecorder_GetFrameCount(IntPtr recorder);

        /// <summary>
        /// Get the last error message
        /// </summary>
        [DllImport(LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ScreenRecorder_GetError(IntPtr recorder);

        /// <summary>
        /// Destroy the recorder and free resources
        /// </summary>
        [DllImport(LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ScreenRecorder_Destroy(IntPtr recorder);

        /// <summary>
        /// Get available encoders (comma-separated string)
        /// </summary>
        [DllImport(LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ScreenRecorder_GetAvailableEncoders(
            [MarshalAs(UnmanagedType.LPStr)] System.Text.StringBuilder outEncoders,
            int bufferSize);

        /// <summary>
        /// Check if hardware encoding is available
        /// </summary>
        [DllImport(LIBRARY_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ScreenRecorder_IsHardwareEncodingAvailable();

        // Helper methods

        /// <summary>
        /// Get the last error as a managed string
        /// </summary>
        public static string GetErrorString(IntPtr recorder)
        {
            IntPtr errorPtr = ScreenRecorder_GetError(recorder);
            return errorPtr != IntPtr.Zero ? Marshal.PtrToStringAnsi(errorPtr) : string.Empty;
        }

        /// <summary>
        /// Get available encoders as string array
        /// </summary>
        public static string[] GetAvailableEncodersArray()
        {
            var buffer = new System.Text.StringBuilder(1024);
            int count = ScreenRecorder_GetAvailableEncoders(buffer, 1024);

            if (count <= 0) return Array.Empty<string>();

            return buffer.ToString().Split(',');
        }

        /// <summary>
        /// Check if hardware encoding is available
        /// </summary>
        public static bool IsHardwareEncodingAvailable()
        {
            return ScreenRecorder_IsHardwareEncodingAvailable() != 0;
        }
    }
}
