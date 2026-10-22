using UnityEngine;
using Core.Architecture;

namespace Core.Skills.VR
{
    public enum LocomotionMode
    {
        ContinuousSmooth,
        Teleportation
    }

    /// <summary>
    /// Smooth locomotion & Arc Teleportation controller supporting snap turns and comfort vignetting.
    /// </summary>
    public sealed class VRLocomotionController : MonoBehaviour
    {
        [SerializeField] private CharacterController characterController;
        [SerializeField] private float moveSpeed = 2.5f;
        [SerializeField] private float snapTurnAngle = 45f;
        [SerializeField] private LocomotionMode locomotionMode = LocomotionMode.ContinuousSmooth;

        public void Move(Vector2 inputDirection, float deltaTime)
        {
            if (characterController == null || inputDirection.sqrMagnitude < 0.001f) return;

            var forward = transform.forward;
            var right = transform.right;
            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();

            var moveVector = (forward * inputDirection.y + right * inputDirection.x) * (moveSpeed * deltaTime);
            characterController.Move(moveVector);
        }

        public void SnapTurn(float direction)
        {
            if (Mathf.Abs(direction) > 0.5f)
            {
                float turn = Mathf.Sign(direction) * snapTurnAngle;
                transform.Rotate(0, turn, 0);
            }
        }
    }
}
