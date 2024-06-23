
using GBX.NET;
using GBX.NET.Engines.Game;
using GBX.NET.LZO;
using MapGenTM;
using Tomlyn;

Settings settings;
try
{
    var text = File.ReadAllText("settings.toml");
    if (!Toml.TryToModel(text, out settings, out var error))
    {
        Console.WriteLine("Failed to parse settings.toml");
        Console.WriteLine(error);
        return;
    }
}
catch (FileNotFoundException e)
{
    Console.WriteLine("Did not find 'settings.toml'. Using default settings.");
    settings = new Settings();
    File.WriteAllText("settings.toml", Toml.FromModel(settings));
}

Gbx.LZO = new MiniLZO();
var cpMap = Gbx.ParseNode<CGameCtnChallenge>("AllCPs.Map.Gbx");
var starts = new BlockSelector(cpMap.Blocks.Where(b => b.Name.Contains("Start")).ToList(), settings.Multipliers);
var finishes = new BlockSelector(cpMap.Blocks.Where(b => b.Name.Contains("Finish")).ToList(), settings.Multipliers);
var checkpoints = new BlockSelector(cpMap.Blocks.Where(b => b.Name.Contains("Checkpoint")).ToList(), settings.Multipliers);

var coords = new CoordSelector(settings.Ranges);

var map = Gbx.ParseNode<CGameCtnChallenge>("Blank.Map.Gbx");
map.MapName = "MapGenTM_Output";
map.Blocks = new List<CGameCtnBlock>();

map.Blocks.Add(starts.Next());
map.Blocks.Last().Coord = coords.Next();
map.Blocks.Last().IsGround = map.Blocks.Last().Coord.Y == CoordSelector.Y_FLOOR;
map.Blocks.Last().Direction = (Direction)Random.Shared.Next(4);

map.Blocks.Add(finishes.Next());
map.Blocks.Last().Coord = coords.Next();
map.Blocks.Last().IsGround = map.Blocks.Last().Coord.Y == CoordSelector.Y_FLOOR;
map.Blocks.Last().Direction = (Direction)Random.Shared.Next(4);

var cpCount = Random.Shared.Next(settings.Ranges.CheckpointsMin, settings.Ranges.CheckpointsMax + 1);
for (int i = 0; i < cpCount; i++)
{
    var block = (CGameCtnBlock)checkpoints.Next().DeepClone();
    block.Coord = coords.Next();
    block.IsGround = block.Coord.Y == CoordSelector.Y_FLOOR;
    block.Direction = (Direction)Random.Shared.Next(4);
    map.Blocks.Add(block);
}

var mapName = "Output.Map.Gbx";
map.Save(mapName);
Console.WriteLine($"Successfully created '{mapName} with {cpCount} checkpoints.");
