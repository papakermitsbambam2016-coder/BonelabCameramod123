using MelonLoader;
using UnityEngine;

namespace QuestCamera;

internal sealed class CameraGrabbable : MonoBehaviour
{
    private Rigidbody? body;
    private Collider[]? colliders;

    private bool initialized;
    private bool held;

    private Transform? grabbingHand;

    private Vector3 grabPosition;
    private Quaternion grabRotation;

    public bool IsHeld => held;

    public Transform? GrabbingHand => grabbingHand;

    public void Initialize()
    {
        if (initialized)
            return;

        initialized = true;

        SetupPhysics();

        MelonLogger.Msg(
            "QuestCamera: Grabbable component initialized.");
    }

    private void SetupPhysics()
    {
        if (gameObject == null)
            return;

        body =
            gameObject.GetComponent<Rigidbody>();

        if (body == null)
        {
            body =
                gameObject.AddComponent<Rigidbody>();
        }

        body.mass = 1.5f;
        body.drag = 0.5f;
        body.angularDrag = 0.5f;
        body.useGravity = true;
        body.isKinematic = false;

        colliders =
            gameObject.GetComponentsInChildren<Collider>();

        if (colliders.Length == 0)
        {
            BoxCollider collider =
                gameObject.AddComponent<BoxCollider>();

            collider.center =
                Vector3.zero;

            collider.size =
                new Vector3(
                    0.35f,
                    0.25f,
                    0.5f);

            colliders =
                new Collider[]
                {
                    collider
                };
        }
    }

    public void Grab(Transform hand)
    {
        if (hand == null)
            return;

        if (!initialized)
            Initialize();

        held = true;

        grabbingHand = hand;

        grabPosition =
            transform.position;

        grabRotation =
            transform.rotation;

        if (body != null)
        {
            body.isKinematic = true;
            body.useGravity = false;
        }

        transform.SetParent(
            hand,
            true);

        MelonLogger.Msg(
            "QuestCamera: Camera grabbed.");
    }

    public void Release()
    {
        if (!held)
            return;

        held = false;

        transform.SetParent(
            null,
            true);

        if (body != null)
        {
            body.isKinematic = false;
            body.useGravity = true;
        }

        grabbingHand = null;

        MelonLogger.Msg(
            "QuestCamera: Camera released.");
    }

    public void Freeze()
    {
        if (body == null)
            return;

        body.velocity =
            Vector3.zero;

        body.angularVelocity =
            Vector3.zero;

        body.isKinematic = true;

        held = false;

        transform.SetParent(
            null,
            true);

        grabbingHand = null;

        MelonLogger.Msg(
            "QuestCamera: Physical camera frozen.");
    }

    public void Unfreeze()
    {
        if (body == null)
            return;

        body.isKinematic = false;

        body.useGravity = true;

        MelonLogger.Msg(
            "QuestCamera: Physical camera unfrozen.");
    }

    public void Throw(
        Vector3 velocity,
        Vector3 angularVelocity)
    {
        if (body == null)
            return;

        body.isKinematic = false;
        body.useGravity = true;

        body.velocity =
            velocity;

        body.angularVelocity =
            angularVelocity;

        MelonLogger.Msg(
            "QuestCamera: Camera thrown.");
    }

    public void StopMotion()
    {
        if (body == null)
            return;

        body.velocity =
            Vector3.zero;

        body.angularVelocity =
            Vector3.zero;
    }

    private void Update()
    {
        if (!held)
            return;

        if (grabbingHand == null)
        {
            Release();
            return;
        }

        /*
         * The actual BONELAB hand-grab system will control
         * the hand transform once BoneLib/BONELAB references
         * are added.
         *
         * For now, keeping the camera parented to the hand
         * gives us the basic physical behavior.
         */
    }

    private void OnDestroy()
    {
        grabbingHand = null;
        body = null;
        colliders = null;
    }
}
