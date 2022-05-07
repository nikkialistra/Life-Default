using System.Collections.Generic;
using Units.Enums;
using UnityEngine;

namespace Units.Ancillaries
{
    public class HumanNames : MonoBehaviour
    {
        [SerializeField] private List<string> _maleNames = new()
        {
            "James",
            "Patrick",
            "Kyle",
            "Juan"
        };

        [SerializeField] private List<string> _femaleNames = new()
        {
            "Tiffany",
            "Mary",
            "Sandra",
            "Sean",
            "Dylan",
            "Juan",
            "Roy",
            "Elijah"
        };
        
        [SerializeField] private List<string> _agenderNames = new()
        {
            "Sean",
            "Dylan",
            "Roy",
            "Elijah"
        };

        public string GetRandomNameFor(Gender gender)
        {
            if (gender == Gender.Male)
            {
                var count = _maleNames.Count + _agenderNames.Count;

                var randomNumber = Random.Range(0, count);

                return randomNumber < _maleNames.Count ? _maleNames[randomNumber] : _agenderNames[randomNumber - _maleNames.Count];
            }
            else
            {
                var count = _femaleNames.Count + _agenderNames.Count;

                var randomNumber = Random.Range(0, count);

                return randomNumber < _femaleNames.Count ? _femaleNames[randomNumber] : _agenderNames[randomNumber - _femaleNames.Count];
            }
        }
    }
}
