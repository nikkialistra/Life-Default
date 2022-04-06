﻿using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Common
{
    [RequireComponent(typeof(SkinnedMeshRenderer))]
    public class BonesReplacing : MonoBehaviour
    {
        public SkinnedMeshRenderer _bonesSource;

        [Button(ButtonSizes.Medium)]
        private void ReplaceBones()
        {
            var boneMap = _bonesSource.bones.ToDictionary(bone => bone.gameObject.name);

            var meshRenderer = GetComponent<SkinnedMeshRenderer>();
            var newBones = new Transform[meshRenderer.bones.Length];
            
            for (var i = 0; i < meshRenderer.bones.Length; ++i)
            {
                var bone = meshRenderer.bones[i].gameObject;
                
                if (!boneMap.TryGetValue(bone.name, out newBones[i]))
                {
                    Debug.Log("Unable to map bone \"" + bone.name + "\" to target skeleton.");
                    break;
                }
            }

            meshRenderer.bones = newBones;
        }
    }
}
