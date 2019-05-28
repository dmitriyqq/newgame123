using System;
using System.Collections.Generic;
using System.Numerics;
using GameModel.GameObjects;

namespace GameUI
{
    public class CircleBrush : Brush
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
                    var diff = Size * Size > (Math.Pow(si - i, 2) + Math.Pow(sj - j, 2));
                    if (i >= 0 && j >= 0 && i < map.Size && j < map.Size && diff)
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