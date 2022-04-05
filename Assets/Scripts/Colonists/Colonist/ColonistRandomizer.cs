using System.Collections.Generic;
using Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Colonists.Colonist
{
    public class ColonistRandomizer : MonoBehaviour
    {
        [SerializeField] private CharacterObjectRoots _maleRoots;
        [SerializeField] private CharacterObjectRoots _femaleRoots;

        [SerializeField] private CharacterObjectGroups _maleBase;
        [SerializeField] private CharacterObjectGroups _femaleBase;
        
        private readonly List<GameObject> _enabledItems = new();

        private enum Gender { Male, Female }

        private void Start()
        {
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

        [Button(ButtonSizes.Medium)]
        private void BuildLists()
        {
            BuildList(_maleBase.HeadAllElements, _maleRoots.HeadAllElements);
            BuildList(_maleBase.HeadNoElements, _maleRoots.HeadNoElements);
            BuildList(_maleBase.Eyebrow, _maleRoots.Eyebrow);
            BuildList(_maleBase.FacialHair, _maleRoots.FacialHair);
            BuildList(_maleBase.Torso, _maleRoots.Torso);
            BuildList(_maleBase.ArmUpperRight, _maleRoots.ArmUpperRight);
            BuildList(_maleBase.ArmUpperLeft, _maleRoots.ArmUpperLeft);
            BuildList(_maleBase.ArmLowerRight, _maleRoots.ArmLowerRight);
            BuildList(_maleBase.ArmLowerLeft, _maleRoots.ArmLowerLeft);
            BuildList(_maleBase.HandRight, _maleRoots.HandRight);
            BuildList(_maleBase.HandLeft, _maleRoots.HandLeft);
            BuildList(_maleBase.Hips, _maleRoots.Hips);
            BuildList(_maleBase.LegRight, _maleRoots.LegRight);
            BuildList(_maleBase.LegLeft, _maleRoots.LegLeft);
            
            BuildList(_femaleBase.HeadAllElements, _femaleRoots.HeadAllElements);
            BuildList(_femaleBase.HeadNoElements, _femaleRoots.HeadNoElements);
            BuildList(_femaleBase.Eyebrow, _femaleRoots.Eyebrow);
            BuildList(_femaleBase.FacialHair, _femaleRoots.FacialHair);
            BuildList(_femaleBase.Torso, _femaleRoots.Torso);
            BuildList(_femaleBase.ArmUpperRight, _femaleRoots.ArmUpperRight);
            BuildList(_femaleBase.ArmUpperLeft, _femaleRoots.ArmUpperLeft);
            BuildList(_femaleBase.ArmLowerRight, _femaleRoots.ArmLowerRight);
            BuildList(_femaleBase.ArmLowerLeft, _femaleRoots.ArmLowerLeft);
            BuildList(_femaleBase.HandRight, _femaleRoots.HandRight);
            BuildList(_femaleBase.HandLeft, _femaleRoots.HandLeft);
            BuildList(_femaleBase.Hips, _femaleRoots.Hips);
            BuildList(_femaleBase.LegRight, _femaleRoots.LegRight);
            BuildList(_femaleBase.LegLeft, _femaleRoots.LegLeft);
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
            var characterGroups = gender == Gender.Male ? _maleBase : _femaleBase;
            
            ActivateItem(characterGroups.HeadAllElements[0]);
            ActivateItem(characterGroups.Eyebrow[0]);
            //ActivateItem(characterGroups.FacialHair[0]);
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
