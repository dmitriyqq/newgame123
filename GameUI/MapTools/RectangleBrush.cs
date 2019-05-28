using System;
using System.Collections.Generic;
using System.Numerics;
using GameModel.GameObjects;

namespace GameUI
{
    public class RectangleBrush : Brush
    {
        public override void Select(Map map, Vector3 mapCoords, Tool tool)
        {
            var (si, sj) = map.WorldToMapCoords(mapCoords.X, mapCoords.Z);

            var top = si - Size;
            var left = sj - Size;
            
            var bottom = si + Size;
            var right = sj + Size;

            var list = new List<Tuple<int, int>>(); 
            for (var i = top; i < bottom; i++)
            {
                for (var j = left; j < right; j++)
                {
                    if (i >= 0 && j >= 0 && i < map.Size && j < map.Size)
                    {
                        list.Add(new Tuple<int, int>(i, j));
                    }
                }
            }

            tool.UpdatePoints(map, list, mapCoords);
        }

        public override void UpdateCursorObject(CursorObject cursorObject, Vector3 mapCoords)
        {
            
        }
    }
}