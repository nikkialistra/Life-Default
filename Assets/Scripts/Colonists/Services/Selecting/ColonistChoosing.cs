using System.Linq;
using Colonists.Colonist;
using UI.Game.GameLook.Components;
using UnityEngine;
using Zenject;

namespace Colonists.Services.Selecting
{
    public class ColonistChoosing : MonoBehaviour
    {
        private UnitsInfoView _unitsInfoView;
        private ColonistRepository _colonistRepository;
        private SelectedColonists _selectedColonists;
        
        private int _indexToTake;

        [Inject]
        public void Construct(UnitsInfoView unitsInfoView, ColonistRepository colonistRepository,
            SelectedColonists selectedColonists)
        {
            _unitsInfoView = unitsInfoView;
            _colonistRepository = colonistRepository;
            _selectedColonists = selectedColonists;
        }

        private void OnEnable()
        {
            _unitsInfoView.SelectColonist += ChooseColonist;
        }

        private void OnDisable()
        {
            _unitsInfoView.SelectColonist -= ChooseColonist;
        }
        
        private void ChooseColonist(ColonistFacade colonist)
        {
            _selectedColonists.Set(colonist);
        }

        public void NextColonistTo(ColonistFacade colonist)
        {
            var colonists = _colonistRepository.GetColonists().ToArray();

            var index = GetUnitIndex(colonists, colonist);
            ChangeToNextUnit(colonists, index);
        }

        private int GetUnitIndex(ColonistFacade[] colonists, ColonistFacade colonist)
        {
            for (var i = 0; i < colonists.Length; i++)
            {
                if (colonists[i] == colonist)
                {
                    return i;
                }
            }

            return -1;
        }

        private void ChangeToNextUnit(ColonistFacade[] colonists, int index)
        {
            index++;
            if (index < colonists.Length)
            {
                var colonist = colonists[index];
                _selectedColonists.Set(colonist);
            }
            else
            {
                var colonist = colonists[0];
                _selectedColonists.Set(colonist);
            }
        }
    }
}
