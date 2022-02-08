﻿using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UnitManagement.Targeting.Formations
{
    public class FormationPreviewDrawing : MonoBehaviour
    {
        [Required]
        [SerializeField] private PositionPreview _positionPreviewPrefab;
        [Required]
        [SerializeField] private Transform _positionPreviewsParent;

        [Title("Special Previews")]
        [Required]
        [SerializeField] private PositionPreview _directionArrow;
        [Required]
        [SerializeField] private PositionPreview _noFormationMark;

        private readonly List<PositionPreview> _positionPreviews = new();

        private Coroutine _flashFinishCoroutine;

        private int _nextIndex = 0;

        private bool _showNoFormationMark;

        public bool ShowDirectionArrow { get; set; }

        public void Show(Vector3[] formationPositions, float rotation)
        {
            Reset();

            int startIndex;
            if (ShowDirectionArrow)
            {
                PlaceDirectionArrow(formationPositions, rotation);
                startIndex = 1;
            }
            else
            {
                startIndex = 0;
            }

            while (startIndex < formationPositions.Length)
            {
                var positionPreview = GetOrCreatePositionPreview();
                positionPreview.transform.position = formationPositions[startIndex];

                startIndex++;
            }
        }

        public void ShowNoFormationMark(Vector3 position)
        {
            Reset();

            _showNoFormationMark = true;

            _noFormationMark.gameObject.SetActive(true);
            _noFormationMark.Activate();
            _noFormationMark.transform.position = position;
        }

        public void UpdatePositions(Vector3[] formationPositions, float rotation)
        {
            if (ShowDirectionArrow)
            {
                UpdateDirectionArrow(formationPositions, rotation);

                for (var i = 1; i < formationPositions.Length; i++)
                {
                    _positionPreviews[i - 1].transform.position = formationPositions[i];
                }
            }
            else
            {
                for (var i = 0; i < formationPositions.Length; i++)
                {
                    _positionPreviews[i].transform.position = formationPositions[i];
                }
            }
        }

        public void Flash()
        {
            if (_showNoFormationMark)
            {
                _noFormationMark.StartFlash();
            }
            else
            {
                FlashFormation();
            }

            _flashFinishCoroutine = StartCoroutine(FlashFinish(_positionPreviewPrefab.FadeTime));
        }

        private void FlashFormation()
        {
            if (ShowDirectionArrow)
            {
                _directionArrow.StartFlash();
            }

            for (var i = 0; i < _nextIndex; i++)
            {
                _positionPreviews[i].StartFlash();
            }
        }

        private void Reset()
        {
            _showNoFormationMark = false;
            _noFormationMark.gameObject.SetActive(false);

            if (_flashFinishCoroutine != null)
            {
                StopCoroutine(_flashFinishCoroutine);
            }

            if (_directionArrow.Activated)
            {
                _directionArrow.Deactivate();
                _directionArrow.gameObject.SetActive(false);
            }

            foreach (var positionPreview in _positionPreviews)
            {
                if (positionPreview.Activated)
                {
                    positionPreview.Deactivate();
                    positionPreview.gameObject.SetActive(false);
                }
            }

            _nextIndex = 0;
        }

        private void PlaceDirectionArrow(Vector3[] formationPositions, float rotation)
        {
            _directionArrow.gameObject.SetActive(true);
            _directionArrow.Activate();
            _directionArrow.transform.position = formationPositions[0];
            _directionArrow.transform.rotation = Quaternion.Euler(0, rotation, 0);
        }

        private PositionPreview GetOrCreatePositionPreview()
        {
            PositionPreview positionPreview;

            if (_positionPreviews.Count <= _nextIndex)
            {
                positionPreview = Instantiate(_positionPreviewPrefab, _positionPreviewsParent);
                _positionPreviews.Add(positionPreview);
            }
            else
            {
                positionPreview = _positionPreviews[_nextIndex];
                positionPreview.gameObject.SetActive(true);
                positionPreview.Activate();
            }

            _nextIndex++;
            return positionPreview;
        }

        private void UpdateDirectionArrow(Vector3[] formationPositions, float rotation)
        {
            _directionArrow.transform.position = formationPositions[0];
            _directionArrow.transform.rotation = Quaternion.Euler(0, rotation, 0);
        }

        private IEnumerator FlashFinish(float fadeTime)
        {
            yield return new WaitForSeconds(fadeTime);

            _noFormationMark.gameObject.SetActive(false);
            _directionArrow.gameObject.SetActive(false);

            foreach (var positionPreview in _positionPreviews)
            {
                positionPreview.gameObject.SetActive(false);
            }
        }
    }
}