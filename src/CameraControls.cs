using MelonLoader;
using UnityEngine;

namespace QuestCamera;

internal sealed class CameraControls
{
    private readonly PhysicalCamera camera;

    private bool recording;

    public bool IsRecording => recording;

    public CameraControls(PhysicalCamera camera)
    {
        this.camera = camera;
    }

    // =========================
    // FREEZE / UNFREEZE
    // =========================

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

    // =========================
    // CAMERA FLIP
    // =========================

    public void Flip()
    {
        camera.Flip();

        MelonLogger.Msg(
            camera.IsCharacterFacing
                ? "QuestCamera: Looking toward player."
                : "QuestCamera: Looking forward.");
    }

    // =========================
    // ZOOM
    // =========================

    public void ZoomIn()
    {
        camera.ZoomIn();

        MelonLogger.Msg(
            "QuestCamera: Zoom in.");
    }

    public void ZoomOut()
    {
        camera.ZoomOut();

        MelonLogger.Msg(
            "QuestCamera: Zoom out.");
    }

    // =========================
    // FIELD OF VIEW
    // =========================

    public void SetFov(float fov)
    {
        camera.SetFov(fov);

        MelonLogger.Msg(
            $"QuestCamera: FOV set to {fov}.");
    }

    // =========================
    // PHOTO
    // =========================

    public void Photo()
    {
        MelonLogger.Msg(
            "QuestCamera: PHOTO requested.");

        /*
         * The actual screenshot system will be added
         * in the next stage.
         */
    }

    // =========================
    // RECORDING
    // =========================

    public void StartRecording()
    {
        if (recording)
            return;

        recording = true;

        MelonLogger.Msg(
            "QuestCamera: RECORDING STARTED.");

        /*
         * The actual Quest video recorder will be
         * connected here later.
         */
    }

    public void StopRecording()
    {
        if (!recording)
            return;

        recording = false;

        MelonLogger.Msg(
            "QuestCamera: RECORDING STOPPED.");

        /*
         * The actual video will be finalized and
         * saved to Quest storage/gallery later.
         */
    }

    public void ToggleRecording()
    {
        if (recording)
            StopRecording();
        else
            StartRecording();
    }

    // =========================
    // RECORDING STATE
    // =========================

    public void Update()
    {
        if (!recording)
            return;

        /*
         * Recording-frame processing will be added
         * when we implement the actual recorder.
         */
    }

    // =========================
    // RESET CAMERA
    // =========================

    public void ResetCamera()
    {
        if (camera.GameObject == null)
            return;

        camera.Unfreeze();

        MelonLogger.Msg(
            "QuestCamera: Camera reset.");
    }

    // =========================
    // DESTROY
    // =========================

    public void Dispose()
    {
        if (recording)
            StopRecording();

        MelonLogger.Msg(
            "QuestCamera: Controls disposed.");
    }
}
