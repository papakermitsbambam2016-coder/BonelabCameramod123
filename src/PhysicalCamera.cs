using System;
using System.Reflection;
using MelonLoader;
using UnityEngine;

namespace QuestCamera;

internal sealed class PhysicalCamera
{
    private GameObject? root;

    private Camera? camera;

    private bool frozen;

    private bool characterFacing;

    private float normalFov = 60f;

    private Vector3 frozenPosition;

    private Quaternion frozenRotation;

    public bool IsSpawned =>
        root != null;

    public bool IsFrozen =>
        frozen;

    public bool IsCharacterFacing =>
        characterFacing;

    public Camera? UnityCamera =>
        camera;

    public GameObject? GameObject =>
        root;

    public void Spawn(Vector3 position)
    {
        if (root != null)
            return;

        root = new GameObject(
            "QuestCamera_Physical");

        root.transform.position =
            position;

        CreateCameraObject();

        CreateVisualBody();

        normalFov =
            CameraConfig.CameraFov;

        if (camera != null)
        {
            camera.fieldOfView =
                normalFov;

            camera.enabled = false;
        }

        if (CameraConfig.StartFrozen)
            Freeze();

        MelonLogger.Msg(
            "QuestCamera: Physical camera spawned.");
    }

    private void CreateCameraObject()
    {
        if (root == null)
            return;

        GameObject cameraObject =
            new GameObject(
                "CameraLens");

        cameraObject.transform.SetParent(
            root.transform,
            false);

        cameraObject.transform.localPosition =
            new Vector3(
                0f,
                0f,
                0.35f);

        cameraObject.transform.localRotation =
            Quaternion.identity;

        camera =
            cameraObject.AddComponent<Camera>();

        camera.enabled = false;

        camera.nearClipPlane =
            0.01f;

        camera.farClipPlane =
            500f;

        camera.fieldOfView =
            CameraConfig.CameraFov;
    }

    private void CreateVisualBody()
    {
        if (root == null)
            return;

        GameObject body =
            GameObject.CreatePrimitive(
                PrimitiveType.Cube);

        body.name =
            "CameraBody";

        body.transform.SetParent(
            root.transform,
            false);

        body.transform.localScale =
            new Vector3(
                0.35f,
                0.25f,
                0.5f);

        body.transform.localPosition =
            Vector3.zero;

        GameObject lens =
            GameObject.CreatePrimitive(
                PrimitiveType.Cylinder);

        lens.name =
            "CameraLensVisual";

        lens.transform.SetParent(
            root.transform,
            false);

        lens.transform.localPosition =
            new Vector3(
                0f,
                0f,
                0.3f);

        lens.transform.localRotation =
            Quaternion.Euler(
                90f,
                0f,
                0f);

        lens.transform.localScale =
            new Vector3(
                0.12f,
                0.06f,
                0.12f);
    }

    public void Freeze()
    {
        if (root == null)
            return;

        frozenPosition =
            root.transform.position;

        frozenRotation =
            root.transform.rotation;

        frozen = true;

        MelonLogger.Msg(
            "QuestCamera: Camera frozen.");
    }

    public void Unfreeze()
    {
        if (root == null)
            return;

        frozen = false;

        MelonLogger.Msg(
            "QuestCamera: Camera unfrozen.");
    }

    public void ToggleFreeze()
    {
        if (frozen)
            Unfreeze();
        else
            Freeze();
    }

    public void Flip()
    {
        if (root == null)
            return;

        characterFacing =
            !characterFacing;

        root.transform.Rotate(
            0f,
            180f,
            0f,
            Space.Self);

        MelonLogger.Msg(
            characterFacing
                ? "QuestCamera: Character-facing."
                : "QuestCamera: Forward-facing.");
    }

    public void ZoomIn()
    {
        if (camera == null)
            return;

        camera.fieldOfView =
            Mathf.Clamp(
                camera.fieldOfView -
                CameraConfig.ZoomAmount,
                CameraConfig.MinFov,
                CameraConfig.MaxFov);
    }

    public void ZoomOut()
    {
        if (camera == null)
            return;

        camera.fieldOfView =
            Mathf.Clamp(
                camera.fieldOfView +
                CameraConfig.ZoomAmount,
                CameraConfig.MinFov,
                CameraConfig.MaxFov);
    }

    public void SetFov(float value)
    {
        if (camera == null)
            return;

        camera.fieldOfView =
            Mathf.Clamp(
                value,
                CameraConfig.MinFov,
                CameraConfig.MaxFov);
    }

    public void Update()
    {
        if (root == null)
            return;

        if (frozen)
        {
            root.transform.position =
                frozenPosition;

            root.transform.rotation =
                frozenRotation;
        }
    }

    public void Destroy()
    {
        if (root != null)
        {
            UnityEngine.Object.Destroy(
                root);
        }

        root = null;
        camera = null;
        frozen = false;
    }
}
