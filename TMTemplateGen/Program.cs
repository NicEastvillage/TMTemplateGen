
using GBX.NET;
using GBX.NET.Engines.Game;
using GBX.NET.LZO;
using TMTemplateGen;
using Tomlyn;

const string relativeSettingsPath = "settings.toml";
const string relativeBlankMapPath = "Blank.Map.Gbx";
const string relativeBlocksMapPath = "Blocks.Map.Gbx";

var exeDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!;

var settingsPath = Path.Combine(exeDirectory, relativeSettingsPath);
var blankMapPath = Path.Combine(exeDirectory, relativeBlankMapPath);
var blocksMapPath = Path.Combine(exeDirectory, relativeBlocksMapPath);

var parser = new CommandLine.Parser(s =>
{
    s.HelpWriter = Console.Error;
    s.AllowMultiInstance = true;
});
var parseResult = parser.ParseArguments<CliOptions>(args);
if (parseResult.Errors.ToList().Any())
{
    // Error message is printed automatically
    return;
}
var options = parseResult.Value;
Logger.LogLevel = options.Verbosity;
if (Path.Combine(Environment.CurrentDirectory, options.MapName) == blankMapPath)
{
    Logger.Error($"Error: Map name collides with {blankMapPath}");
    return;
}
if (Path.Combine(Environment.CurrentDirectory, options.MapName) == blocksMapPath)
{
    Logger.Error($"Error: Map name collides with {blocksMapPath}");
    return;
}

if (options.PlacementSeed != null)
{
    Rng.PlacementRng = new(options.PlacementSeed.Value);
    Logger.Debug($"Using fixed placement seed: {options.PlacementSeed}");
}

if (options.BlockTypeSeed != null)
{
    Rng.BlockTypeRng = new(options.BlockTypeSeed.Value);
    Logger.Debug($"Using fixed block type seed: {options.BlockTypeSeed}");
}

Settings settings;
try
{
    var text = File.ReadAllText(settingsPath);
    if (!Toml.TryToModel(text, out settings, out var error))
    {
        Logger.Error( $"Error: Failed to parse {settingsPath}");
        Logger.Error(error.ToString());
        return;
    }

    Logger.Debug($"Loaded settings {settingsPath}");
}
catch (FileNotFoundException e)
{
    Logger.Info($"Creating {settingsPath} since it is missing.");
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
        block.Direction = (Direction)Rng.PlacementRng.Next(4);
        map.Blocks.Add(block);
        Logger.Debug($"Placed {block.Name} at {block.Coord} with rotation {block.Direction}");        
    }
}

Gbx.LZO = new MiniLZO();
var cpMap = Gbx.ParseNode<CGameCtnChallenge>(blocksMapPath);
Logger.Debug($"Loaded map with blocks {blocksMapPath}");

var starts = new BlockSelector(cpMap.Blocks.Where(b => b.Name.Contains("Start")).ToList(), settings.Multipliers);
var finishes = new BlockSelector(cpMap.Blocks.Where(b => b.Name.Contains("Finish")).ToList(), settings.Multipliers);
var checkpoints = new BlockSelector(cpMap.Blocks.Where(b => b.Name.Contains("Checkpoint")).ToList(), settings.Multipliers);

var coords = new CoordSelector(settings.Ranges);

var map = Gbx.ParseNode<CGameCtnChallenge>(blankMapPath);
Logger.Debug($"Loaded blank map {blankMapPath}");

map.MapName = options.MapName;
map.Blocks = new List<CGameCtnBlock>();

var cpCount = Rng.PlacementRng.Next(settings.Ranges.CheckpointsMin, settings.Ranges.CheckpointsMax + 1);
Place(map, 1, starts, coords);
Place(map, 1, finishes, coords);
Place(map, cpCount, checkpoints, coords);

var mapName = $"{options.MapName}.Map.Gbx";
map.Save(mapName);
Logger.Info($"Successfully created '{mapName}' with {cpCount} checkpoints.");
