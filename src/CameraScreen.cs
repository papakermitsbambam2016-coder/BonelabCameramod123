using System;
using MelonLoader;
using UnityEngine;

namespace QuestCamera;

internal sealed class CameraScreen : MonoBehaviour
{
    private PhysicalCamera? physicalCamera;
    private CameraControls? controls;

    private GameObject? screenObject;

    private GameObject? recButton;
    private GameObject? photoButton;
    private GameObject? freezeButton;
    private GameObject? flipButton;
    private GameObject? zoomInButton;
    private GameObject? zoomOutButton;

    private bool initialized;

    public bool IsInitialized =>
        initialized;

    public void Initialize(
        PhysicalCamera camera,
        CameraControls cameraControls)
    {
        if (initialized)
            return;

        physicalCamera = camera;
        controls = cameraControls;

        CreateScreen();

        initialized = true;

        MelonLogger.Msg(
            "QuestCamera: Touchscreen initialized.");
    }

    private void CreateScreen()
    {
        if (screenObject != null)
            return;

        screenObject =
            GameObject.CreatePrimitive(
                PrimitiveType.Cube);

        screenObject.name =
            "QuestCamera_Touchscreen";

        screenObject.transform.SetParent(
            transform,
            false);

        screenObject.transform.localPosition =
            new Vector3(
                0f,
                0f,
                -0.16f);

        screenObject.transform.localRotation =
            Quaternion.identity;

        screenObject.transform.localScale =
            new Vector3(
                0.25f,
                0.18f,
                0.015f);

        Renderer? renderer =
            screenObject.GetComponent<Renderer>();

        if (renderer != null)
        {
            renderer.material.color =
                Color.black;
        }

        CreateButtons();
    }

    private void CreateButtons()
    {
        recButton =
            CreateButton(
                "REC",
                new Vector3(
                    -0.08f,
                    0.055f,
                    -0.175f));

        photoButton =
            CreateButton(
                "PHOTO",
                new Vector3(
                    0.08f,
                    0.055f,
                    -0.175f));

        freezeButton =
            CreateButton(
                "FREEZE",
                new Vector3(
                    -0.08f,
                    -0.005f,
                    -0.175f));

        flipButton =
            CreateButton(
                "FLIP",
                new Vector3(
                    0.08f,
                    -0.005f,
                    -0.175f));

        zoomInButton =
            CreateButton(
                "ZOOM+",
                new Vector3(
                    -0.08f,
                    -0.065f,
                    -0.175f));

        zoomOutButton =
            CreateButton(
                "ZOOM-",
                new Vector3(
                    0.08f,
                    -0.065f,
                    -0.175f));
    }

    private GameObject CreateButton(
        string buttonName,
        Vector3 localPosition)
    {
        GameObject button =
            GameObject.CreatePrimitive(
                PrimitiveType.Cube);

        button.name =
            "CameraButton_" +
            buttonName;

        button.transform.SetParent(
            transform,
            false);

        button.transform.localPosition =
            localPosition;

        button.transform.localScale =
            new Vector3(
                0.07f,
                0.035f,
                0.01f);

        Renderer? renderer =
            button.GetComponent<Renderer>();

        if (renderer != null)
        {
            renderer.material.color =
                Color.gray;
        }

        return button;
    }

    public void PressREC()
    {
        if (controls == null)
            return;

        controls.ToggleRecording();

        UpdateScreen();
    }

    public void PressPhoto()
    {
        if (controls == null)
            return;

        controls.Photo();
    }

    public void PressFreeze()
    {
        if (controls == null)
            return;

        controls.ToggleFreeze();

        UpdateScreen();
    }

    public void PressFlip()
    {
        if (controls == null)
            return;

        controls.Flip();
    }

    public void PressZoomIn()
    {
        if (controls == null)
            return;

        controls.ZoomIn();
    }

    public void PressZoomOut()
    {
        if (controls == null)
            return;

        controls.ZoomOut();
    }

    private void UpdateScreen()
    {
        if (physicalCamera == null)
            return;

        if (freezeButton != null)
        {
            Renderer? renderer =
                freezeButton.GetComponent<Renderer>();

            if (renderer != null)
            {
                renderer.material.color =
                    physicalCamera.IsFrozen
                        ? Color.green
                        : Color.gray;
            }
        }

        if (recButton != null &&
            controls != null)
        {
            Renderer? renderer =
                recButton.GetComponent<Renderer>();

            if (renderer != null)
            {
                renderer.material.color =
                    controls.IsRecording
                        ? Color.red
                        : Color.gray;
            }
        }
    }

    public void Update()
    {
        if (!initialized)
            return;

        UpdateScreen();
    }

    public void DestroyScreen()
    {
        if (screenObject != null)
        {
            UnityEngine.Object.Destroy(
                screenObject);
        }

        screenObject = null;

        recButton = null;
        photoButton = null;
        freezeButton = null;
        flipButton = null;
        zoomInButton = null;
        zoomOutButton = null;

        initialized = false;
    }

    private void OnDestroy()
    {
        DestroyScreen();

        physicalCamera = null;
        controls = null;
    }
}
