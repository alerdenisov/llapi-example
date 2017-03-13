using UnityEngine;
using System.Collections;
using System;

namespace LlapiExample
{
    public class StartGameView : BaseView
    {
        public Action OnStartGame;
        public void StartButton()
        {
            OnStartGame();
        }
    }
}