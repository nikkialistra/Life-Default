using UnityEngine;

namespace Controls.CameraControls
{
    public class CameraThresholdMovement : MonoBehaviour
    {
        [Space]
        [Range(0.5f, 1f)]
        [SerializeField] private float _positionMoveXThreshold = 0.97f;
        [Range(0.5f, 1f)]
        [SerializeField] private float _positionMoveYThreshold = 0.97f;

        public Vector2 GetMovementFromPosition(Vector2 position, float moveSpeed)
        {
            var movement = Vector2.zero;
            var normalisedPosition = GetNormalizedPosition(position);

            if (Mathf.Abs(normalisedPosition.x) > _positionMoveXThreshold)
                movement.x = Mathf.Sign(normalisedPosition.x) * moveSpeed * Time.unscaledDeltaTime;

            if (Mathf.Abs(normalisedPosition.y) > _positionMoveYThreshold)
                movement.y = Mathf.Sign(normalisedPosition.y) * moveSpeed * Time.unscaledDeltaTime;

            return movement;
        }

        public bool IsMouseMoved(Vector2 position)
        {
            var normalisedPosition = GetNormalizedPosition(position);

            return Mathf.Abs(normalisedPosition.x) > _positionMoveXThreshold ||
                   Mathf.Abs(normalisedPosition.y) > _positionMoveYThreshold;
        }

        //Result between -1 and 1 is the result between 0 and 1 subtracted with 0.5 and multiplied by 2
        private Vector2 GetNormalizedPosition(Vector2 position)
        {
            var result = new Vector2(((position.x / Screen.width) - 0.5f) * 2f,
                ((position.y / Screen.height) - 0.5f) * 2f);
            return result;
        }
    }
}
