using System.Linq;
using Colonists.Colonist;
using UI.Game.GameLook.Components;
using UnityEngine;
using Zenject;

namespace Colonists.Services.Selecting
{
    public class ColonistChoosing : MonoBehaviour
    {
        private ColonistRepository _colonistRepository;
        private SelectedColonists _selectedColonists;
        
        private int _indexToTake;

        [Inject]
        public void Construct(ColonistsInfoView colonistsInfoView, ColonistRepository colonistRepository, SelectedColonists selectedColonists)
        {
            _colonistRepository = colonistRepository;
            _selectedColonists = selectedColonists;
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
