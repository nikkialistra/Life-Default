﻿using System.Linq;
using Selecting.Selected;
using UI.Game.GameLook.Components.Info;
using UnityEngine;
using Zenject;

namespace Colonists.Services
{
    public class ColonistChoosing : MonoBehaviour
    {
        private ColonistRepository _colonistRepository;
        private SelectedColonists _selectedColonists;

        private int _indexToTake;

        [Inject]
        public void Construct(ColonistsInfoView colonistsInfoView, ColonistRepository colonistRepository,
            SelectedColonists selectedColonists)
        {
            _colonistRepository = colonistRepository;
            _selectedColonists = selectedColonists;
        }

        public void NextColonistTo(Colonist colonist)
        {
            var colonists = _colonistRepository.GetColonists().ToArray();

            var index = GetColonistIndex(colonists, colonist);
            ChangeToNextColonist(colonists, index);
        }

        private int GetColonistIndex(Colonist[] colonists, Colonist colonist)
        {
            for (int i = 0; i < colonists.Length; i++)
                if (colonists[i] == colonist)
                    return i;

            return -1;
        }

        private void ChangeToNextColonist(Colonist[] colonists, int index)
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
