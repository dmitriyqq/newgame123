using System;

namespace GameModel.GameObjects
{
    public class Map : GameObject
    {
        public Voxel[,] Data { get; private set; }
        public int Size  { get; set; } 
        public float Resolution  { get; set; } = 1.0f;
        public int Octaves  { get; set; } = 3;
        public float Persistence { get; set; } = 3.83f;
        public event Action OnUpdate;
        public event Action OnBatchUpdate;

        
        public Map(int size)
        {
            Size = size;
            Static = true;
            CreateMap();
        }

        public override void Update(float deltaTime)
        {
        }

        public (float x, float z) MapToWorldCords(int i, int j)
        {
            var x = (i - Size / 2) * Resolution;
            var z = -(j - Size / 2) * Resolution;

            return (x, z);
        }

        public (int i, int j) WorldToMapCoords(float x, float y)
        {
            var i = (int) (x / Resolution + Size / 2);
            var j = (int) (Size / 2 - y / Resolution);
            return (j, i);
        }

        public void CreateMap()
        {
            Data = new Voxel[Size, Size];

            var averageHeight = 0.0f;
            
            for (var i = 0; i < Size; i++)
            {
                for (var j = 0; j < Size; j++)
                {
                    var x = ((double) i ) / Size;
                    var y = ((double) j ) / Size;

                    Data[i, j].Height = (float) Perlin.OctavePerlin(x, y, 1.0f, Octaves, Persistence);

                    averageHeight += Data[i, j].Height;
                }
            }

            averageHeight /= (Size * Size);
            
            for (var i = 0; i < Size; i++)
            {
                for (var j = 0; j < Size; j++)
                {
                    Data[i, j].Height -= averageHeight;
                }
            }

            OnUpdate?.Invoke();
        }

        public void Update()
        {
            OnUpdate?.Invoke();
        }
        
        public void BatchUpdate()
        {
            OnBatchUpdate?.Invoke();
        }
    }
}