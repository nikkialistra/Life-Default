using System;
using System.Collections.Generic;
using System.Linq;
using Game.Units.Services;
using Kernel.Types;
using UnityEngine;
using Zenject;

namespace Kernel.Selection
{
    public class UnitSelection : IInitializable
    {
        private ProjectionSelector _selector;
        private SelectionInput _selectionInput;
        private SelectionArea _selectionArea;
        
        public IEnumerable<ISelectable> Selected { get; private set; } = Array.Empty<ISelectable>();
        
        [Inject]
        public void Construct(ProjectionSelector selector, SelectionInput selectionInput, SelectionArea selectionArea)
        {
            _selector = selector;
            _selectionInput = selectionInput;
            _selectionArea = selectionArea;
        }
        
        public void Initialize()
        {
            _selectionInput.Selecting += Draw;
            _selectionInput.SelectingEnd += Select;
        }

        private void Draw(Rect rect)
        {
            _selectionArea.Draw(rect);
        }

        private void Select(Rect rect)
        {
            var newSelected = Enumerable.Empty<ISelectable>();
            if (rect.size != Vector2.zero)
            {
                newSelected = _selector.SelectInScreenSpace(rect);
            }

            var newSelectedArray = newSelected as ISelectable[] ?? newSelected.ToArray();
            foreach (var willDeselect in Selected.Except(newSelectedArray))
            {
                willDeselect.OnDeselect();
            }

            foreach (var selected in newSelectedArray)
            {
                selected.OnSelect();
            }

            Selected = newSelectedArray;

            _selectionArea.StopDrawing();
        }
    }
}