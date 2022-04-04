using System.Collections.Generic;
using Common;
using UnityEngine;

namespace Colonists.Colonist
{
    public class ColonistRandomizer : MonoBehaviour
    {
        [SerializeField] private CharacterObjectRoots _maleRoots;
        [SerializeField] private CharacterObjectRoots _femaleRoots;

        private readonly CharacterObjectGroups _male = new();
        private readonly CharacterObjectGroups _female = new();
        
        private readonly List<GameObject> _enabledItems = new();

        private enum Gender { Male, Female }

        private void Start()
        {
            BuildLists();
            
            if (_enabledItems.Count != 0)
            {
                foreach (var enabledObject in _enabledItems)
                {
                    enabledObject.SetActive(false);
                }
            }
            
            _enabledItems.Clear();

            ActivateDefaultItems();
        }

        private void BuildLists()
        {
            BuildList(_male.HeadAllElements, _maleRoots.HeadAllElements);
            BuildList(_male.HeadNoElements, _maleRoots.HeadNoElements);
            BuildList(_male.Eyebrow, _maleRoots.Eyebrow);
            BuildList(_male.FacialHair, _maleRoots.FacialHair);
            BuildList(_male.Torso, _maleRoots.Torso);
            BuildList(_male.ArmUpperRight, _maleRoots.ArmUpperRight);
            BuildList(_male.ArmUpperLeft, _maleRoots.ArmUpperLeft);
            BuildList(_male.ArmLowerRight, _maleRoots.ArmLowerRight);
            BuildList(_male.ArmLowerLeft, _maleRoots.ArmLowerLeft);
            BuildList(_male.HandRight, _maleRoots.HandRight);
            BuildList(_male.HandLeft, _maleRoots.HandLeft);
            BuildList(_male.Hips, _maleRoots.Hips);
            BuildList(_male.LegRight, _maleRoots.LegRight);
            BuildList(_male.LegLeft, _maleRoots.LegLeft);
            
            BuildList(_female.HeadAllElements, _femaleRoots.HeadAllElements);
            BuildList(_female.HeadNoElements, _femaleRoots.HeadNoElements);
            BuildList(_female.Eyebrow, _femaleRoots.Eyebrow);
            BuildList(_female.FacialHair, _femaleRoots.FacialHair);
            BuildList(_female.Torso, _femaleRoots.Torso);
            BuildList(_female.ArmUpperRight, _femaleRoots.ArmUpperRight);
            BuildList(_female.ArmUpperLeft, _femaleRoots.ArmUpperLeft);
            BuildList(_female.ArmLowerRight, _femaleRoots.ArmLowerRight);
            BuildList(_female.ArmLowerLeft, _femaleRoots.ArmLowerLeft);
            BuildList(_female.HandRight, _femaleRoots.HandRight);
            BuildList(_female.HandLeft, _femaleRoots.HandLeft);
            BuildList(_female.Hips, _femaleRoots.Hips);
            BuildList(_female.LegRight, _femaleRoots.LegRight);
            BuildList(_female.LegLeft, _femaleRoots.LegLeft);
        }

        private void BuildList(List<GameObject> itemList, Transform root)
        {
            itemList.Clear();
            
            for (var i = 0; i < root.childCount; i++)
            {
                var go = root.GetChild(i).gameObject;
                go.SetActive(false);
                itemList.Add(go);
            }
        }

        private void ActivateDefaultItems()
        {
            var randomGender = EnumUtils.RandomEnumValue<Gender>();

            ActivateDefaultItemsForGender(randomGender);
        }

        private void ActivateDefaultItemsForGender(Gender gender)
        {
            var characterGroups = gender == Gender.Male ? _male : _female;
            
            ActivateItem(characterGroups.HeadAllElements[0]);
            ActivateItem(characterGroups.Eyebrow[0]);
            ActivateItem(characterGroups.FacialHair[0]);
            ActivateItem(characterGroups.Torso[0]);
            ActivateItem(characterGroups.ArmUpperRight[0]);
            ActivateItem(characterGroups.ArmUpperLeft[0]);
            ActivateItem(characterGroups.ArmLowerRight[0]);
            ActivateItem(characterGroups.ArmLowerLeft[0]);
            ActivateItem(characterGroups.HandRight[0]);
            ActivateItem(characterGroups.HandLeft[0]);
            ActivateItem(characterGroups.Hips[0]);
            ActivateItem(characterGroups.LegRight[0]);
            ActivateItem(characterGroups.LegLeft[0]);
        }

        private void ActivateItem(GameObject item)
        {
            item.SetActive(true);
            
            _enabledItems.Add(item);
        }
        
        [System.Serializable]
        private class CharacterObjectRoots
        {
            public Transform HeadAllElements;
            public Transform HeadNoElements;
            public Transform Eyebrow;
            public Transform FacialHair;
            public Transform Torso;
            public Transform ArmUpperRight;
            public Transform ArmUpperLeft;
            public Transform ArmLowerRight;
            public Transform ArmLowerLeft;
            public Transform HandRight;
            public Transform HandLeft;
            public Transform Hips;
            public Transform LegRight;
            public Transform LegLeft;
        }

        [System.Serializable]
        private class CharacterObjectGroups
        {
            public List<GameObject> HeadAllElements = new();
            public List<GameObject> HeadNoElements = new();
            public List<GameObject> Eyebrow = new();
            public List<GameObject> FacialHair = new();
            public List<GameObject> Torso = new();
            public List<GameObject> ArmUpperRight = new();
            public List<GameObject> ArmUpperLeft = new();
            public List<GameObject> ArmLowerRight = new();
            public List<GameObject> ArmLowerLeft = new();
            public List<GameObject> HandRight = new();
            public List<GameObject> HandLeft = new();
            public List<GameObject> Hips = new();
            public List<GameObject> LegRight = new();
            public List<GameObject> LegLeft = new();
        }
    }
}
