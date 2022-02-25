using System;
using UI.Game.GameLook.Components;
using UnityEngine;
using Zenject;

namespace Environment.TimeCycle.Seasons
{
    public class SeasonCycle : MonoBehaviour
    {
        [SerializeField] private int _year = 2114;
        [SerializeField] private Season _season = Season.Spring;
        [Range(1, 7)]
        [SerializeField] private int _day = 1;
        
        private TimeWeatherView _timeWeatherView;

        [Inject]
        public void Construct(TimeWeatherView timeWeatherView)
        {
            _timeWeatherView = timeWeatherView;
        }

        public event Action SeasonDayChange;
        public event Action<Season> SeasonChange; 

        private void Start()
        {
            UpdateView();
        }

        public void NextDay()
        {
            _day++;

            if (_day == 8)
            {
                _day = 1;
                NextSeason();
                SeasonChange?.Invoke(_season);
            }
            else
            {
                SeasonDayChange?.Invoke();
            }

            UpdateView();
        }

        private void NextSeason()
        {
            _season = _season switch {
                Season.Spring => Season.Summer,
                Season.Summer => Season.Autumn,
                Season.Autumn => Season.Winter,
                Season.Winter => Season.Spring,
                _ => throw new ArgumentOutOfRangeException()
            };

            if (_season == Season.Spring)
            {
                _year++;
            }
        }

        private void UpdateView()
        {
            _timeWeatherView.ChangeSeasonInfo(_season, _day, _year);
        }
    }
}
