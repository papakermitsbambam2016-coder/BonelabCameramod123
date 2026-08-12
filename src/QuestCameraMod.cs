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

    private bool sceneReady;
    private bool systemsInitialized;

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
            " Touchscreen Camera Foundation");

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
        systemsInitialized = false;

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

        /*
         * Create the camera view system.
         */
        GameObject? cameraObject =
            physicalCamera.GameObject;

        if (cameraObject == null)
            return;

        view =
            cameraObject.AddComponent<CameraView>();

        view.Initialize(
            physicalCamera);

        /*
         * Create the touchscreen system.
         */
        screen =
            cameraObject.AddComponent<CameraScreen>();

        screen.Initialize(
            physicalCamera,
            controls);

        /*
         * Create the physical grab component.
         */
        CameraGrabbable? grabbable =
            cameraObject.AddComponent<CameraGrabbable>();

        grabbable.Initialize();

        systemsInitialized = true;

        MelonLogger.Msg(
            "================================");

        MelonLogger.Msg(
            "QuestCamera: CAMERA READY");

        MelonLogger.Msg(
            "Physical camera created.");

        MelonLogger.Msg(
            "Touchscreen created.");

        MelonLogger.Msg(
            "Camera view created.");

        MelonLogger.Msg(
            "Grab system initialized.");

        MelonLogger.Msg(
            "================================");
    }

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

    public void TakePhoto()
    {
        controls?.Photo();
    }

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

    public void ZoomIn()
    {
        controls?.ZoomIn();
    }

    public void ZoomOut()
    {
        controls?.ZoomOut();
    }

    public override void OnApplicationQuit()
    {
        if (controls != null)
            controls.Dispose();

        if (view != null)
            view.DestroyView();

        if (screen != null)
            screen.DestroyScreen();

        if (physicalCamera != null)
            physicalCamera.Destroy();

        physicalCamera = null;
        controls = null;
        screen = null;
        view = null;

        sceneReady = false;
        systemsInitialized = false;
    }
}
