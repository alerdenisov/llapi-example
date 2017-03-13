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

    public class PlayStatus
    {
        private BehaviorSubject<PlayState> state;

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
        
        public void Set(PlayState next)
        {
            state.OnNext(next);
        }
    }
}