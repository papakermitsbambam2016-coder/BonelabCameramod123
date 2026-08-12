using MelonLoader;
using UnityEngine;

namespace QuestCamera;

internal sealed class CameraControls
{
    private readonly PhysicalCamera camera;

    private CameraRecorder? recorder;

    public bool IsRecording =>
        recorder != null &&
        recorder.IsRecording;

    public CameraControls(
        PhysicalCamera camera)
    {
        this.camera = camera;
    }

    public void SetRecorder(
        CameraRecorder cameraRecorder)
    {
        recorder =
            cameraRecorder;
    }

    public void Freeze()
    {
        camera.Freeze();

        MelonLogger.Msg(
            "QuestCamera: Camera frozen.");
    }

    public void Unfreeze()
    {
        camera.Unfreeze();

        MelonLogger.Msg(
            "QuestCamera: Camera unfrozen.");
    }

    public void ToggleFreeze()
    {
        camera.ToggleFreeze();

        MelonLogger.Msg(
            camera.IsFrozen
                ? "QuestCamera: Camera frozen."
                : "QuestCamera: Camera unfrozen.");
    }

    public void Flip()
    {
        camera.Flip();

        MelonLogger.Msg(
            camera.IsCharacterFacing
                ? "QuestCamera: Looking toward player."
                : "QuestCamera: Looking forward.");
    }

    public void ZoomIn()
    {
        camera.ZoomIn();
    }

    public void ZoomOut()
    {
        camera.ZoomOut();
    }

    public void SetFov(
        float fov)
    {
        camera.SetFov(fov);
    }

    public void Photo()
    {
        MelonLogger.Msg(
            "QuestCamera: PHOTO requested.");

        // Photo system will use the same RenderTexture
        // as the recorder.
    }

    public void StartRecording()
    {
        if (recorder == null)
        {
            MelonLogger.Warning(
                "QuestCamera: Recorder is not connected.");

            return;
        }

        recorder.StartRecording();
    }

    public void StopRecording()
    {
        if (recorder == null)
            return;

        recorder.StopRecording();
    }

    public void ToggleRecording()
    {
        if (recorder == null)
        {
            MelonLogger.Warning(
                "QuestCamera: Recorder is not connected.");

            return;
        }

        recorder.ToggleRecording();
    }

    public void Dispose()
    {
        if (recorder != null)
            recorder.Dispose();

        recorder = null;
    }
}
