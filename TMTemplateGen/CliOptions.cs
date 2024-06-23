using CommandLine;

namespace TMTemplateGen;

public class CliOptions
{
    [Value(0, Default = "Output", HelpText = "Name of generated map/file")]
    public string MapName { get; set; }
    
    [Option('v', "verbose", FlagCounter = true, HelpText = "Increase verbosity level")]
    public int Verbosity { get; set; }
    
    [Option("pseed", HelpText = "Seed for placement randomness")]
    public int? PlacementSeed { get; set; }
    
    [Option("bseed", HelpText = "Seed for block type randomness")]
    public int? BlockTypeSeed { get; set; }
}
