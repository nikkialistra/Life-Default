using System.Collections.Generic;
using Common;
using UnityEngine;

namespace Colonists.Colonist
{
    public class ColonistRandomizer : MonoBehaviour
    {
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
            BuildList(_male.HeadAllElements, "Male_Head_All_Elements");
            BuildList(_male.HeadNoElements, "Male_Head_No_Elements");
            BuildList(_male.Eyebrow, "Male_01_Eyebrows");
            BuildList(_male.FacialHair, "Male_02_FacialHair");
            BuildList(_male.Torso, "Male_03_Torso");
            BuildList(_male.ArmUpperRight, "Male_04_Arm_Upper_Right");
            BuildList(_male.ArmUpperLeft, "Male_05_Arm_Upper_Left");
            BuildList(_male.ArmLowerRight, "Male_06_Arm_Lower_Right");
            BuildList(_male.ArmLowerLeft, "Male_07_Arm_Lower_Left");
            BuildList(_male.HandRight, "Male_08_Hand_Right");
            BuildList(_male.HandLeft, "Male_09_Hand_Left");
            BuildList(_male.Hips, "Male_10_Hips");
            BuildList(_male.LegRight, "Male_11_Leg_Right");
            BuildList(_male.LegLeft, "Male_12_Leg_Left");
            
            BuildList(_female.HeadAllElements, "Female_Head_All_Elements");
            BuildList(_female.HeadNoElements, "Female_Head_No_Elements");
            BuildList(_female.Eyebrow, "Female_01_Eyebrows");
            BuildList(_female.FacialHair, "Female_02_FacialHair");
            BuildList(_female.Torso, "Female_03_Torso");
            BuildList(_female.ArmUpperRight, "Female_04_Arm_Upper_Right");
            BuildList(_female.ArmUpperLeft, "Female_05_Arm_Upper_Left");
            BuildList(_female.ArmLowerRight, "Female_06_Arm_Lower_Right");
            BuildList(_female.ArmLowerLeft, "Female_07_Arm_Lower_Left");
            BuildList(_female.HandRight, "Female_08_Hand_Right");
            BuildList(_female.HandLeft, "Female_09_Hand_Left");
            BuildList(_female.Hips, "Female_10_Hips");
            BuildList(_female.LegRight, "Female_11_Leg_Right");
            BuildList(_female.LegLeft, "Female_12_Leg_Left");
        }

        private void BuildList(List<GameObject> itemList, string characterPart)
        {
            var rootTransform = gameObject.GetComponentsInChildren<Transform>();
            
            Transform targetRoot = null;
            
            foreach (var t in rootTransform)
            {
                if (t.gameObject.name == characterPart)
                {
                    targetRoot = t;
                    break;
                }
            }
            
            itemList.Clear();
            
            for (var i = 0; i < targetRoot.childCount; i++)
            {
                var go = targetRoot.GetChild(i).gameObject;
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
