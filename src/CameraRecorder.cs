using System;
using System.IO;
using MelonLoader;
using UnityEngine;

namespace QuestCamera;

internal sealed class CameraRecorder : MonoBehaviour
{
    private CameraView? cameraView;

    private RenderTexture? captureTexture;

    private Texture2D? frameTexture;

    private string? recordingFolder;

    private float frameTimer;

    private int frameNumber;

    private float recordingTime;

    private bool recording;

    private int width = 1024;
    private int height = 1024;

    private float frameRate = 30f;

    public bool IsRecording => recording;

    public float RecordingTime => recordingTime;

    public int FrameCount => frameNumber;

    public void Initialize(CameraView view)
    {
        cameraView = view;

        MelonLogger.Msg(
            "QuestCamera: Recorder initialized.");
    }

    public void StartRecording()
    {
        if (recording)
            return;

        if (cameraView == null)
        {
            MelonLogger.Warning(
                "QuestCamera: Recorder has no CameraView.");

            return;
        }

        captureTexture =
            cameraView.RenderTexture;

        if (captureTexture == null)
        {
            MelonLogger.Warning(
                "QuestCamera: No camera RenderTexture available.");

            return;
        }

        width =
            captureTexture.width;

        height =
            captureTexture.height;

        frameTexture =
            new Texture2D(
                width,
                height,
                TextureFormat.RGB24,
                false);

        recordingFolder =
            CreateRecordingFolder();

        if (recordingFolder == null)
            return;

        frameNumber = 0;
        recordingTime = 0f;
        frameTimer = 0f;

        recording = true;

        MelonLogger.Msg(
            $"QuestCamera: Recording started.");

        MelonLogger.Msg(
            $"QuestCamera: Resolution {width}x{height}");

        MelonLogger.Msg(
            $"QuestCamera: FPS {frameRate}");

        MelonLogger.Msg(
            $"QuestCamera: Folder {recordingFolder}");
    }

    public void StopRecording()
    {
        if (!recording)
            return;

        recording = false;

        MelonLogger.Msg(
            $"QuestCamera: Recording stopped.");

        MelonLogger.Msg(
            $"QuestCamera: Captured {frameNumber} frames.");

        MelonLogger.Msg(
            $"QuestCamera: Duration {recordingTime:0.00} seconds.");

        if (recordingFolder != null)
        {
            MelonLogger.Msg(
                $"QuestCamera: Frames saved to {recordingFolder}");
        }

        recordingFolder = null;

        if (frameTexture != null)
        {
            Destroy(frameTexture);

            frameTexture = null;
        }

        captureTexture = null;
    }

    public void ToggleRecording()
    {
        if (recording)
            StopRecording();
        else
            StartRecording();
    }

    private void Update()
    {
        if (!recording)
            return;

        if (cameraView == null)
            return;

        if (captureTexture == null)
            return;

        if (frameTexture == null)
            return;

        float deltaTime =
            Time.unscaledDeltaTime;

        recordingTime += deltaTime;

        frameTimer += deltaTime;

        float frameInterval =
            1f / frameRate;

        if (frameTimer < frameInterval)
            return;

        frameTimer -= frameInterval;

        CaptureFrame();
    }

    private void CaptureFrame()
    {
        if (captureTexture == null ||
            frameTexture == null ||
            recordingFolder == null)
            return;

        try
        {
            RenderTexture? previous =
                RenderTexture.active;

            RenderTexture.active =
                captureTexture;

            frameTexture.ReadPixels(
                new Rect(
                    0,
                    0,
                    width,
                    height),
                0,
                0,
                false);

            frameTexture.Apply(
                false,
                false);

            RenderTexture.active =
                previous;

            byte[] png =
                frameTexture.EncodeToPNG();

            string filename =
                Path.Combine(
                    recordingFolder,
                    $"frame_{frameNumber:000000}.png");

            File.WriteAllBytes(
                filename,
                png);

            frameNumber++;
        }
        catch (Exception ex)
        {
            MelonLogger.Warning(
                $"QuestCamera: Frame capture failed: {ex.Message}");
        }
    }

    private string? CreateRecordingFolder()
    {
        try
        {
            string root =
                Path.Combine(
                    MelonUtils.UserDataDirectory,
                    "QuestCamera",
                    "Recordings");

            Directory.CreateDirectory(
                root);

            string folder =
                Path.Combine(
                    root,
                    DateTime.Now.ToString(
                        "yyyy-MM-dd_HH-mm-ss"));

            Directory.CreateDirectory(
                folder);

            return folder;
        }
        catch (Exception ex)
        {
            MelonLogger.Error(
                $"QuestCamera: Could not create recording folder: {ex.Message}");

            return null;
        }
    }

    public void SetFrameRate(
        float fps)
    {
        frameRate =
            Mathf.Clamp(
                fps,
                1f,
                60f);
    }

    public void SetResolution(
        int newWidth,
        int newHeight)
    {
        if (recording)
        {
            MelonLogger.Warning(
                "QuestCamera: Stop recording before changing resolution.");

            return;
        }

        if (newWidth <= 0 ||
            newHeight <= 0)
            return;

        width =
            newWidth;

        height =
            newHeight;
    }

    public string GetRecordingFolder()
    {
        return recordingFolder ?? string.Empty;
    }

    public void Dispose()
    {
        if (recording)
            StopRecording();

        if (frameTexture != null)
        {
            Destroy(frameTexture);

            frameTexture = null;
        }

        captureTexture = null;
        cameraView = null;
    }

    private void OnDestroy()
    {
        Dispose();
    }
}
