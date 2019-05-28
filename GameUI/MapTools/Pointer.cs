using System;
using System.Collections.Generic;
using System.Numerics;
using GameModel.GameObjects;

namespace GameUI
{
    public class Pointer : Brush
    {
        public override void Select(Map map, Vector3 mapCoords, Tool tool)
        {
            var (si, sj) = map.WorldToMapCoords(mapCoords.X, mapCoords.Z);
            var list = new List<Tuple<int, int>> {new Tuple<int, int>(si, sj)};
            tool.UpdatePoints(map, list, mapCoords);
        }

        public override void UpdateCursorObject(CursorObject cursorObject, Vector3 mapCoords)
        {
        }
    }
}