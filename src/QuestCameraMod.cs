using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(
    typeof(QuestCamera.QuestCameraMod),
    QuestCamera.PluginInfo.Name,
    QuestCamera.PluginInfo.Version,
    QuestCamera.PluginInfo.Author)]

namespace QuestCamera;

public sealed class QuestCameraMod : MelonMod
{
    private PhysicalCamera? physicalCamera;
    private CameraControls? controls;
    private CameraScreen? screen;
    private CameraView? view;
    private CameraRecorder? recorder;

    private bool sceneReady;

    private float spawnTimer;

    public override void OnInitializeMelon()
    {
        MelonLogger.Msg(
            "================================");

        MelonLogger.Msg(
            " QuestCamera");

        MelonLogger.Msg(
            $" Version {PluginInfo.Version}");

        MelonLogger.Msg(
            " Touchscreen Camera");

        MelonLogger.Msg(
            " Recording System");

        MelonLogger.Msg(
            "================================");

        physicalCamera =
            new PhysicalCamera();

        controls =
            new CameraControls(
                physicalCamera);

        spawnTimer = 2f;
    }

    public override void OnSceneWasLoaded(
        int buildIndex,
        string sceneName)
    {
        sceneReady = true;

        spawnTimer = 2f;

        MelonLogger.Msg(
            $"QuestCamera: Scene loaded: {sceneName}");
    }

    public override void OnUpdate()
    {
        if (!sceneReady)
            return;

        if (physicalCamera == null)
            return;

        if (!physicalCamera.IsSpawned)
        {
            spawnTimer -= Time.deltaTime;

            if (spawnTimer <= 0f &&
                CameraConfig.AutoSpawn)
            {
                SpawnCamera();
            }

            return;
        }

        physicalCamera.Update();

        view?.Update();

        screen?.Update();
    }

    private void SpawnCamera()
    {
        Camera? playerCamera =
            Camera.main;

        if (playerCamera == null)
        {
            MelonLogger.Warning(
                "QuestCamera: Player camera not found yet.");

            spawnTimer = 1f;

            return;
        }

        Vector3 spawnPosition =
            playerCamera.transform.position +
            playerCamera.transform.forward *
            CameraConfig.SpawnDistance;

        physicalCamera?.Spawn(
            spawnPosition);

        if (physicalCamera == null)
            return;

        if (controls == null)
            return;

        GameObject? cameraObject =
            physicalCamera.GameObject;

        if (cameraObject == null)
            return;

        // =========================
        // CAMERA VIEW
        // =========================

        view =
            cameraObject.AddComponent<CameraView>();

        view.Initialize(
            physicalCamera);

        // =========================
        // RECORDER
        // =========================

        recorder =
            cameraObject.AddComponent<CameraRecorder>();

        recorder.Initialize(
            view);

        controls.SetRecorder(
            recorder);

        // =========================
        // TOUCHSCREEN
        // =========================

        screen =
            cameraObject.AddComponent<CameraScreen>();

        screen.Initialize(
            physicalCamera,
            controls);

        // =========================
        // GRABBABLE CAMERA
        // =========================

        CameraGrabbable? grabbable =
            cameraObject.AddComponent<CameraGrabbable>();

        grabbable.Initialize();

        MelonLogger.Msg(
            "================================");

        MelonLogger.Msg(
            "QuestCamera: CAMERA READY");

        MelonLogger.Msg(
            "Physical camera created.");

        MelonLogger.Msg(
            "Camera view created.");

        MelonLogger.Msg(
            "Recorder created.");

        MelonLogger.Msg(
            "Touchscreen created.");

        MelonLogger.Msg(
            "Grab component created.");

        MelonLogger.Msg(
            "================================");
    }

    // =========================
    // CAMERA CONTROLS
    // =========================

    public void ToggleFreeze()
    {
        controls?.ToggleFreeze();
    }

    public void FlipCamera()
    {
        controls?.Flip();
    }

    public void ToggleRecording()
    {
        controls?.ToggleRecording();
    }

    public void StartRecording()
    {
        controls?.StartRecording();
    }

    public void StopRecording()
    {
        controls?.StopRecording();
    }

    public void TakePhoto()
    {
        controls?.Photo();
    }

    // =========================
    // POV
    // =========================

    public void TogglePOV()
    {
        view?.TogglePov();
    }

    public void CameraPOV()
    {
        view?.SetCameraPov();
    }

    public void PlayerPOV()
    {
        view?.SetPlayerPov();
    }

    // =========================
    // ZOOM
    // =========================

    public void ZoomIn()
    {
        controls?.ZoomIn();
    }

    public void ZoomOut()
    {
        controls?.ZoomOut();
    }

    // =========================
    // CLEANUP
    // =========================

    public override void OnApplicationQuit()
    {
        if (recorder != null)
        {
            recorder.Dispose();
            recorder = null;
        }

        if (controls != null)
        {
            controls.Dispose();
            controls = null;
        }

        if (view != null)
        {
            view.DestroyView();
            view = null;
        }

        if (screen != null)
        {
            screen.DestroyScreen();
            screen = null;
        }

        if (physicalCamera != null)
        {
            physicalCamera.Destroy();
            physicalCamera = null;
        }

        sceneReady = false;
    }
}
