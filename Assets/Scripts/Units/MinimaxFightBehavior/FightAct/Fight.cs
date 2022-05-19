﻿using System;
using System.Collections.Generic;
using static Units.MinimaxFightBehavior.FightAct.Player.PlayerSide;

namespace Units.MinimaxFightBehavior.FightAct
{
    public class Fight
    {
        private readonly Player _firstPlayer;
        private readonly Player _secondPlayer;

        private readonly PlayerFightStatuses _playerFightStatuses;
        
        private readonly Stack<HistoryFightState> _history = new();

        public Fight(Player firstPlayer, Player secondPlayer)
        {
            _firstPlayer = firstPlayer;
            _secondPlayer = secondPlayer;

            _playerFightStatuses = new PlayerFightStatuses(firstPlayer.StartHealth, secondPlayer.StartHealth);

            State = FightState.FirstPlayerTurn;
        }
        
        public enum FightState
        {
            FirstPlayerTurn,
            SecondPlayerTurn,
            
            FirstPlayerVictory,
            SecondPlayerVictory,

            Draw
        }

        public Player ActivePlayer => State switch
        {
            FightState.FirstPlayerTurn => _firstPlayer,
            FightState.SecondPlayerTurn => _secondPlayer,
            _ => null
        };

        public FightState State { get; private set; }

        public bool IsTerminal => State is FightState.FirstPlayerVictory or FightState.SecondPlayerVictory or FightState.Draw;

        public void ShowCurrentFightStatus()
        {
            _playerFightStatuses.ShowCurrentFightStatus();
        }

        public List<FightMove> GetPossibleMoves()
        {
            if (ActivePlayer == null)
            {
                throw new InvalidOperationException("Cannot make move when there is no turns");
            }
            
            if (IsTerminal)
            {
                return new List<FightMove>();
            }

            return ActivePlayer.GetPossibleMoves();
        }

        public void MakeMove(FightMove fightMove)
        {
            if (ActivePlayer == null)
            {
                throw new InvalidOperationException("Cannot make move when there is no turns");
            }
            
            if (IsTerminal)
            {
                throw new InvalidOperationException("The fight is finished");
            }
            
            _history.Push(new HistoryFightState(State, ActivePlayer.Side, fightMove));

            UpdatePlayerFightStatuses(ActivePlayer.Side, fightMove);

            State = DeduceState(ActivePlayer.Side);
        }

        public void UndoMove()
        {
            if (_history.Count == 0)
            {
                throw new InvalidOperationException("Cannot undo when no moves were done");
            }

            var fightState = _history.Pop();
            
            UpdatePlayerFightStatuses(fightState.WhichMove, fightState.ReverseFightMove);

            State = fightState.State;
        }

        private void UpdatePlayerFightStatuses(Player.PlayerSide side, FightMove fightMove)
        {
            if (side == First)
            {
                _playerFightStatuses.ChangeSecondPlayerHealth(fightMove.HitDamage);
                _playerFightStatuses.ChangeFirstPlayerHealth(fightMove.TakeDamage);
            }
            else
            {
                _playerFightStatuses.ChangeFirstPlayerHealth(fightMove.HitDamage);
                _playerFightStatuses.ChangeSecondPlayerHealth(fightMove.TakeDamage);
            }
        }

        private FightState DeduceState(Player.PlayerSide side)
        {
            if (IsSomeoneDefeated(out var fightState))
            {
                return fightState;
            }

            return side switch {
                First => FightState.SecondPlayerTurn,
                Second => FightState.FirstPlayerTurn,
                _ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
            };
        }

        private bool IsSomeoneDefeated(out FightState fightState)
        {
            if (_playerFightStatuses.IsBothPlayersDefeated)
            {
                fightState = FightState.Draw;
                return true;
            }

            if (_playerFightStatuses.IsFirstPlayerDefeated)
            {
                fightState = FightState.SecondPlayerVictory;
                return true;
            }

            if (_playerFightStatuses.IsSecondPlayerDefeated)
            {
                fightState = FightState.FirstPlayerVictory;
                return true;
            }

            fightState = default;
            return false;
        }
    }
}