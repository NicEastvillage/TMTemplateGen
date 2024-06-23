using GBX.NET;

namespace TMTemplateGen;

public class CoordSelector
{
    public const int XZ_SIZE = 48;
    public const int Y_FLOOR = 9;
    public const int Y_SKY = 39;
    
    private readonly int _xMin, _xMax, _yMin, _yMax, _zMin, _zMax;

    private readonly List<Int3> _used = new();
    
    public CoordSelector(Ranges ranges)
    {
        (_xMin, _xMax) = SortAndClamp( ranges.XMin + XZ_SIZE / 2, ranges.XMax + XZ_SIZE / 2, 0, XZ_SIZE - 1);
        (_yMin, _yMax) = SortAndClamp( Y_FLOOR + ranges.YMin, Y_FLOOR + ranges.YMax, Y_FLOOR, Y_SKY - 1);
        (_zMin, _zMax) = SortAndClamp( ranges.ZMin + XZ_SIZE / 2, ranges.ZMax + XZ_SIZE / 2, 0, XZ_SIZE - 1);
    }

    private (int, int) SortAndClamp(int a, int b, int min, int max)
    {
        (a, b) = a < b ? (a, b) : (b, a);
        a = Math.Max(a, min);
        b = Math.Min(b, max);
        return (a, b);
    } 

    public Int3 Next()
    {
        var limit = 10000;
        do
        {
            var coord = new Int3(
                Rng.PlacementRng.Next(_xMin, _xMax + 1),
                Rng.PlacementRng.Next(_yMin, _yMax + 1),
                Rng.PlacementRng.Next(_zMin, _zMax + 1)
            );
            if (!_used.Contains(coord))
            {
                _used.Add(coord);
                return coord;                
            }
        } while (limit-- > 0);

        throw new InvalidOperationException("Ran out of tries attempting to find used coord.");
    }
}
