using System;
using Kernel.Types;
using UnityEngine;

namespace Game.Cameras
{
    public class CameraFollowing
    {
        public bool Following { get; private set; }
        
        private Transform _followTransform;
        private Vector3 _followOffset;
        
        private readonly Camera _camera;
        private Vector3 _followLastPosition;

        public CameraFollowing(Camera camera)
        {
            _camera = camera;
        }

        public Vector3 GetDeltaFollowPosition()
        {
            if (!Following)
            {
                throw new InvalidOperationException("Trying to get follow position while not following.");
            }

            var delta = _followTransform.position - _followLastPosition;
            _followLastPosition = _followTransform.position;
            return delta;
        }

        public bool TryFollow(Vector2 screenPoint)
        {
            var ray = _camera.ScreenPointToRay(screenPoint);

            if (Physics.Raycast(ray, out var hit))
            {
                if (hit.transform.gameObject.GetComponent<ISelectable>() != null)
                {
                    _followTransform = hit.transform;
                    _followLastPosition = _followTransform.position;
                    Following = true;
                    return true;
                }
            }
            return false;
        }

        public void Reset()
        {
            _followTransform = null;
            Following = false;
        }
    }
}