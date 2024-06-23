using System.Runtime.Serialization;

namespace TMTemplateGen;

public class Settings
{
    public Ranges Ranges { get; set; } = new();
    public Multipliers Multipliers { get; set; } = new();
}

public class Ranges
{
    public int CheckpointsMin { get; set; } = 4;
    public int CheckpointsMax { get; set; } = 9;
    
    // Assuming (0,0,0) is map center at floor level. Ranges are inclusive, Y is up.
    [DataMember(Name = "x_min")]
    public int XMin { get; set; } = -11;
    [DataMember(Name = "x_max")]
    public int XMax { get; set; } = 11;
    [DataMember(Name = "y_min")]
    public int YMin { get; set; } = 0;
    [DataMember(Name = "y_max")]
    public int YMax { get; set; } = 10;
    [DataMember(Name = "z_min")]
    public int ZMin { get; set; } = -11;
    [DataMember(Name = "z_max")]
    public int ZMax { get; set; } = 11;
}

public class Multipliers
{
    // Types
    public float Road { get; set; } = 1f;
    public float Platform { get; set; } = 1f;
    public float Gate { get; set; } = 1f;

    // Surface
    public float Tech { get; set; } = 1f;
    public float Dirt { get; set; } = 1f;
    public float Grass { get; set; } = 1f;
    public float Bump { get; set; } = 1f;
    public float Ice { get; set; } = 1f;
    public float Plastic { get; set; } = 1f;
    public float Water { get; set; } = 1f;

    // Slope
    public float Flat { get; set; } = 1f; // Name does not contain "Slope" or "Tilt" or "Wall" (but "WithWall" is ok)
    public float Slope { get; set; } = 1f;
    public float Tilt { get; set; } = 1f; // Name contains "Tilt" or "Slope2Left" or "Slope2Right"
    public float Sideways { get; set; } = 1f; // Name contains "Wall" but not "WithWall"
    public float Diagonal { get; set; } = 1f; // Name contains "Diag"

    // Bobsleigh wall
    public float BobsleighWithWall { get; set; } = 1f; // Name contains "WithWall"
    
    // Platform with penalty surface along edges
    public float Narrow { get; set; } = 1f; // Name contains "Open"
}
