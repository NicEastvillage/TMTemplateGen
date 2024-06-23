using GBX.NET.Engines.Game;

namespace TMTemplateGen;

public class BlockSelector
{
    private readonly List<CGameCtnBlock> _blocks;
    private readonly List<float> _weights;
    private float _totalWeight;
    
    public BlockSelector(IList<CGameCtnBlock> blocks, Multipliers? multipliers=null)
    {
        _blocks = new List<CGameCtnBlock>(blocks);
        _weights = new List<float>(new float[blocks.Count]);
        for (int i = 0; i < _weights.Count; i++)
        {
            _weights[i] = 1f;
        }
        _totalWeight = _weights.Count;

        if (multipliers != null)
        {
            ApplyMultipliers(multipliers);
        }
    }

    public void ApplyMultipliers(Multipliers multipliers)
    {
        _totalWeight = 0;
        for (int i = 0; i < _weights.Count; i++)
        {
            var w = 1f;
            var name = _blocks[i].Name;
            if (name.Contains("Road")) w *= multipliers.Road;
            if (name.Contains("Platform") || name.Contains("Open")) w *= multipliers.Platform;
            if (name.Contains("Gate")) w *= multipliers.Gate;
            
            if (name.Contains("Tech")) w *= multipliers.Tech;
            if (name.Contains("Dirt")) w *= multipliers.Dirt;
            if (name.Contains("Grass")) w *= multipliers.Grass;
            if (name.Contains("Bump")) w *= multipliers.Bump;
            if (name.Contains("Ice")) w *= multipliers.Ice;
            if (name.Contains("Plastic")) w *= multipliers.Plastic;
            if (name.Contains("Water")) w *= multipliers.Water;

            if (!name.Contains("Slope") && !name.Contains("Tilt") && (!name.Contains("Wall") || name.Contains("WithWall"))) w *= multipliers.Flat;
            if (name.Contains("SlopeUp") || name.Contains("SlopeDown") || name.Contains("Slope2Up") || name.Contains("Slope2Down")) w *= multipliers.Slope;
            if (name.Contains("Tilt") || name.Contains("Slope2Left") || name.Contains("Slope2Right")) w *= multipliers.Tilt;
            if (name.Contains("Wall") && !name.Contains("WithWall")) w *= multipliers.Sideways;
            if (name.Contains("Diag")) w *= multipliers.Diagonal;

            if (name.Contains("WithWall")) w *= multipliers.BobsleighWithWall;
            if (name.Contains("Open")) w *= multipliers.Narrow;

            _weights[i] = w;
            _totalWeight += w;
        }

        if (_blocks.Count > 0 && _totalWeight == 0)
            throw new ArgumentException("No block has a non-zero weight with the given multipliers.");
    }

    public CGameCtnBlock Next()
    {
        if (_blocks.Count == 0 || _totalWeight == 0)
        {
            throw new InvalidOperationException("Unable to pick next block. Either there are no blocks or all of them have weight 0.");
        }
        
        var p = Random.Shared.NextSingle() * _totalWeight;
        for (int i = 0; i < _blocks.Count; i++)
        {
            p -= _weights[i];
            if (p <= 0)
            {
                return _blocks[i];
            }
        }

        return _blocks.Last();
    }
}
