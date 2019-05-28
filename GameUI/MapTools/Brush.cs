using System;
using System.Collections.Generic;
using System.Numerics;
using GameModel.GameObjects;

namespace GameUI
{
    public abstract class Brush
    {
        public int Size { get; set; }
        public abstract void Select(Map map, Vector3 mapCoords, Tool tool);

        public abstract void UpdateCursorObject(CursorObject cursorObject, Vector3 mapCoords);
    }
}