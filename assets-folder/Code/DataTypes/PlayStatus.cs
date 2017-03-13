using UnityEngine;
using System.Collections;
using UniRx;

namespace LlapiExample
{
    public enum PlayState : byte
    {
        Await = 1 << 0,         // 0000 0100
        Alive = 1 << 1,
        Dead = 1 << 2,
        Respawn = 1 << 3
    }

    public enum Team : byte
    {
        Unknown,
        TeamA,
        TeamB
    }

    public class PlayStatus
    {
        private BehaviorSubject<PlayState> state;
        private Team team;

        public PlayStatus(PlayState initial = PlayState.Await)
        {
            state = new BehaviorSubject<PlayState>(initial);
        }

        public IObservable<PlayState> Observable
        {
            get { return state; }
        }

        public PlayState Current
        {
            get { return state.Value; }
        }

        public Team Team { get { return team; } }
        
        public void Set(PlayState next)
        {
            state.OnNext(next);
        }

        public void SetTeam(Team team)
        {
            this.team = team;
        }
    }
}