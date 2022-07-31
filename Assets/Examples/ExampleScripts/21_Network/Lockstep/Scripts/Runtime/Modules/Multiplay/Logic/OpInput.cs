using System;
using MessagePack;
using UnityEngine;
namespace Protocol
{
    [Serializable]
    [MessagePackObject]
    public struct OpInput : IEquatable<OpInput>
    {
        [Key(0)]
        public int Conv { get; set; }
        [Key(1)]
        public bool LeftShiftDown { get; set; }
        [Key(2)]
        public bool MouseLeftDown { get; set; }
        [Key(3)]
        public Vector3 Position { get; set; }
        [Key(4)]
        public Vector3 Rotation { get; set; }
        [Key(5)]
        public Vector3 Input { get; set; }
        [Key(6)]
        public Vector3 Forward{ get; set; }
        public OpInput(int conv, bool leftShiftDown, bool mouseLeftDown, Vector3 position, Vector3 rotation, Vector3 input,Vector3 forward)
        {
            Conv = conv;
            LeftShiftDown = leftShiftDown;
            MouseLeftDown = mouseLeftDown;
            Position = position;
            Rotation = rotation;
            Input = input;
            Forward = forward;
        }
        public readonly static OpInput Default = new OpInput();
        public bool Equals(OpInput other)
        {
            return LeftShiftDown == other.LeftShiftDown && MouseLeftDown == other.MouseLeftDown
                && Position == other.Position && Rotation == other.Rotation
                && Input == other.Input&&Forward==other.Forward;
        }
        public override string ToString()
        {
            return $"Conv:{Conv} ; LeftShiftDown:{LeftShiftDown} ;  MouseLeftDown:{MouseLeftDown}";
        }
    }
}
