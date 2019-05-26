using System.Numerics;

namespace GameModel
{
    public interface IRayCaster
    {
        (Vector3 start, Vector3 dir) CastRay(int x1, int y1);
    }
}