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
        }

        physicalCamera.Update();
    }

    private void SpawnCamera()
    {
        Camera? playerCamera =
            Camera.main;

        if (playerCamera == null)
        {
            MelonLogger.Warning(
                "QuestCamera: Player camera not found.");
            return;
        }

        Vector3 spawnPosition =
            playerCamera.transform.position +
            playerCamera.transform.forward *
            CameraConfig.SpawnDistance;

        physicalCamera?.Spawn(
            spawnPosition);

        MelonLogger.Msg(
            "QuestCamera: Camera created.");
    }

    public override void OnApplicationQuit()
    {
        physicalCamera?.Destroy();

        sceneReady = false;
    }
}
