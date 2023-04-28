namespace Pixelbyte
{
    public enum Direction : byte
    {
        None = 0,
        //Standards
        Up = 0x1,
        Right = 0x2,
        Down = 0x4,
        Left = 0x8,

        //Diagonals
        UpRight = Up | Right,     //0x03
        DownRight = Down | Right, //0x06
        DownLeft = Down | Left,   //0x0C
        UpLeft = Up | Left,       //0x09

        //Combos
        UpDown = Up | Down,
        LeftRight = Left | Right,
        LeftRightdown = Left | Right | Down,
        LeftRightUp = Left | Right | Up,
        LeftRightUpDown = Left | Right | Up | Down
    }; 
}