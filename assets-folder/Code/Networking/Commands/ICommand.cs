using UnityEngine;
using System.Collections;

namespace LlapiExample
{
    public interface ICommand
    {
        int Connection { set; get; }
        byte CommandId { get; }
        void Read(object[] buffer, ref int position);
        object[] Data();
    }

    public static class CommandIds
    {
        // Game state commands (0xX)
        public const byte Game_Start        = 0x1;
        public const byte Game_Pause        = 0x2;
        public const byte Game_End          = 0x3;

        /// Character commands (0x0X)
        public const byte Character_Spawn   = 0x10;
        public const byte Character_Die     = 0x11;
        public const byte Character_Move    = 0x12;
        public const byte Character_Shoot   = 0x13;
        public const byte Character_Built   = 0x14;
        public const byte Character_Look    = 0x15;
        public const byte Character_Bullet  = 0x16;

        // Entities commands (0x1X)
        public const byte Entity_Damage     = 0x21;
        public const byte Entity_Create     = 0x22;
        public const byte Entity_Remove     = 0x23;


    }
}