using MelonLoader;
using UnityEngine;

namespace QuestCamera;

internal sealed class CameraView : MonoBehaviour
{
    private PhysicalCamera? physicalCamera;

    private Camera? camera;

    private Camera? playerCamera;

    private RenderTexture? renderTexture;

    private GameObject? displayObject;

    private Renderer? displayRenderer;

    private bool initialized;

    private bool playerPov;

    private bool displayEnabled = true;

    private int textureWidth = 1024;

    private int textureHeight = 1024;

    public bool IsInitialized =>
        initialized;

    public bool IsPlayerPov =>
        playerPov;

    public RenderTexture? RenderTexture =>
        renderTexture;

    public Camera? Camera =>
        camera;

    public void Initialize(
        PhysicalCamera physical)
    {
        if (initialized)
            return;

        physicalCamera =
            physical;

        camera =
            physical.UnityCamera;

        if (camera == null)
        {
            MelonLogger.Warning(
                "QuestCamera: CameraView could not find the camera.");

            return;
        }

        FindPlayerCamera();

        CreateRenderTexture();

        CreateDisplay();

        initialized = true;

        MelonLogger.Msg(
            "QuestCamera: Camera view initialized.");
    }

    private void FindPlayerCamera()
    {
        playerCamera =
            Camera.main;

        if (playerCamera == null)
        {
            Camera[] cameras =
                UnityEngine.Object.FindObjectsOfType<Camera>();

            foreach (Camera foundCamera in cameras)
            {
                if (foundCamera == camera)
                    continue;

                playerCamera =
                    foundCamera;

                break;
            }
        }
    }

    private void CreateRenderTexture()
    {
        if (renderTexture != null)
            return;

        renderTexture =
            new RenderTexture(
                textureWidth,
                textureHeight,
                24,
                RenderTextureFormat.ARGB32);

        renderTexture.name =
            "QuestCamera_RenderTexture";

        renderTexture.Create();

        if (camera != null)
        {
            camera.targetTexture =
                renderTexture;
        }
    }

    private void CreateDisplay()
    {
        if (physicalCamera == null)
            return;

        GameObject? cameraObject =
            physicalCamera.GameObject;

        if (cameraObject == null)
            return;

        displayObject =
            GameObject.CreatePrimitive(
                PrimitiveType.Quad);

        displayObject.name =
            "QuestCamera_ViewDisplay";

        displayObject.transform.SetParent(
            cameraObject.transform,
            false);

        displayObject.transform.localPosition =
            new Vector3(
                0f,
                0f,
                -0.18f);

        displayObject.transform.localRotation =
            Quaternion.Euler(
                0f,
                180f,
                0f);

        displayObject.transform.localScale =
            new Vector3(
                0.22f,
                0.16f,
                1f);

        displayRenderer =
            displayObject.GetComponent<Renderer>();

        if (displayRenderer != null &&
            renderTexture != null)
        {
            Material material =
                new Material(
                    Shader.Find(
                        "Unlit/Texture"));

            material.mainTexture =
                renderTexture;

            displayRenderer.material =
                material;
        }
    }

    public void SetPlayerPov()
    {
        playerPov = true;

        MelonLogger.Msg(
            "QuestCamera: Player POV enabled.");
    }

    public void SetCameraPov()
    {
        playerPov = false;

        MelonLogger.Msg(
            "QuestCamera: Camera POV enabled.");
    }

    public void TogglePov()
    {
        playerPov =
            !playerPov;

        MelonLogger.Msg(
            playerPov
                ? "QuestCamera: Player POV."
                : "QuestCamera: Camera POV.");
    }

    public void SetDisplayEnabled(
        bool enabled)
    {
        displayEnabled =
            enabled;

        if (displayObject != null)
        {
            displayObject.SetActive(
                enabled);
        }
    }

    public void ToggleDisplay()
    {
        SetDisplayEnabled(
            !displayEnabled);
    }

    public void SetResolution(
        int width,
        int height)
    {
        if (width <= 0 ||
            height <= 0)
            return;

        textureWidth =
            width;

        textureHeight =
            height;

        RecreateRenderTexture();
    }

    private void RecreateRenderTexture()
    {
        if (camera == null)
            return;

        if (renderTexture != null)
        {
            camera.targetTexture =
                null;

            renderTexture.Release();

            UnityEngine.Object.Destroy(
                renderTexture);
        }

        renderTexture =
            new RenderTexture(
                textureWidth,
                textureHeight,
                24,
                RenderTextureFormat.ARGB32);

        renderTexture.name =
            "QuestCamera_RenderTexture";

        renderTexture.Create();

        camera.targetTexture =
            renderTexture;

        if (displayRenderer != null)
        {
            Material material =
                displayRenderer.material;

            material.mainTexture =
                renderTexture;
        }
    }

    public void SetFov(
        float fov)
    {
        if (camera == null)
            return;

        camera.fieldOfView =
            Mathf.Clamp(
                fov,
                CameraConfig.MinFov,
                CameraConfig.MaxFov);
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

    public void Flip()
    {
        physicalCamera?.Flip();
    }

    public void Update()
    {
        if (!initialized)
            return;

        if (camera == null)
            return;

        if (playerPov)
        {
            UpdatePlayerPov();
        }
        else
        {
            UpdateCameraPov();
        }
    }

    private void UpdateCameraPov()
    {
        if (physicalCamera == null)
            return;

        GameObject? cameraObject =
            physicalCamera.GameObject;

        if (cameraObject == null)
            return;

        /*
         * The physical camera's transform is the
         * source of the camera POV.
         *
         * The Camera component itself follows that
         * transform because it is a child of the
         * physical camera.
         */
    }

    private void UpdatePlayerPov()
    {
        FindPlayerCamera();

        if (playerCamera == null)
            return;

        /*
         * Player POV is captured from the game's
         * active player camera.
         *
         * We do not disable or replace the player's
         * actual VR camera.
         */
    }

    public Texture? GetCurrentTexture()
    {
        if (renderTexture == null)
            return null;

        return renderTexture;
    }

    public void CaptureFrame()
    {
        if (camera == null ||
            renderTexture == null)
            return;

        camera.Render();
    }

    public void DestroyView()
    {
        if (camera != null)
        {
            camera.targetTexture =
                null;
        }

        if (renderTexture != null)
        {
            renderTexture.Release();

            UnityEngine.Object.Destroy(
                renderTexture);
        }

        if (displayObject != null)
        {
            UnityEngine.Object.Destroy(
                displayObject);
        }

        renderTexture = null;

        displayObject = null;

        displayRenderer = null;

        camera = null;

        playerCamera = null;

        physicalCamera = null;

        initialized = false;
    }

    private void OnDestroy()
    {
        DestroyView();
    }
}
