
using GBX.NET;
using GBX.NET.Engines.Game;
using GBX.NET.LZO;
using MapGenTM;
using Tomlyn;

const string settingsPath = "settings.toml";

var parser = new CommandLine.Parser(s =>
{
    s.AutoHelp = true;
    s.AllowMultiInstance = true;
});
var parseResult = parser.ParseArguments<CliOptions>(args);
if (parseResult.Errors.ToList().Any())
{
    // Error message is printed automatically
    return;
}
var options = parseResult.Value;

Settings settings;
try
{
    var text = File.ReadAllText(settingsPath);
    if (!Toml.TryToModel(text, out settings, out var error))
    {
        Console.Error.WriteLine($"Failed to parse {settingsPath}");
        Console.Error.WriteLine(error);
        return;
    }
}
catch (FileNotFoundException e)
{
    if (options.Verbosity > 0)
        Console.WriteLine($"Creating {settingsPath} since it is missing.");
    settings = new Settings();
    File.WriteAllText(settingsPath, Toml.FromModel(settings));
}

void Place(CGameCtnChallenge map, int count, BlockSelector blocks, CoordSelector coords)
{
    for (int i = 0; i < count; i++)
    {
        var block = (CGameCtnBlock)blocks.Next().DeepClone();
        block.Coord = coords.Next();
        block.IsGround = block.Coord.Y == CoordSelector.Y_FLOOR;
        block.Direction = (Direction)Random.Shared.Next(4);
        map.Blocks.Add(block);
        if (options.Verbosity > 1)
            Console.WriteLine($"Placed {block.Name} at {block.Coord} with rotation {block.Direction}");        
    }
}

Gbx.LZO = new MiniLZO();
var cpMap = Gbx.ParseNode<CGameCtnChallenge>("AllCPs.Map.Gbx");
var starts = new BlockSelector(cpMap.Blocks.Where(b => b.Name.Contains("Start")).ToList(), settings.Multipliers);
var finishes = new BlockSelector(cpMap.Blocks.Where(b => b.Name.Contains("Finish")).ToList(), settings.Multipliers);
var checkpoints = new BlockSelector(cpMap.Blocks.Where(b => b.Name.Contains("Checkpoint")).ToList(), settings.Multipliers);

var coords = new CoordSelector(settings.Ranges);

var map = Gbx.ParseNode<CGameCtnChallenge>("Blank.Map.Gbx");
map.MapName = options.MapName;
map.Blocks = new List<CGameCtnBlock>();

var cpCount = Random.Shared.Next(settings.Ranges.CheckpointsMin, settings.Ranges.CheckpointsMax + 1);
Place(map, 1, starts, coords);
Place(map, 1, finishes, coords);
Place(map, cpCount, checkpoints, coords);

var mapName = $"{options.MapName}.Map.Gbx";
map.Save(mapName);
if (options.Verbosity > 0)
    Console.WriteLine($"Successfully created '{mapName} with {cpCount} checkpoints.");
