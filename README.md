# TMTemplateGen

This is a small tool generate Trackmania 2020 maps consisting of randomly placed checkpoints including a random start and finish.

## Usage
Simply run the following command to generate a map `Output.Map.Gbx`:
```
./TMTemplateGen.exe
```

The binary can be provided arguments and options as follows:
```
Usage: ./TMTemplateGen.exe <mapname=Output> [options]

Positional arguments:
    mapname         Name of generate map and file (default: "Output")

Options:
    -v, --verbose   Increase verbosity level
    --help          Display this help message
    --version       Display version information
```

### Settings
In `settings.toml` you will find additional settings such as:
- `checkpoints_min`/`checkpoints_max` - Specifies the number of checkpoints places as a range.
- `x_min`/`x_max`, `y_min`/`y_max`, `z_min`/`z_max` - Specifies the possible coordinates of place blocks as ranges where (0, 0, 0) is the middle of the map at ground level. Min/max X/Z values are -24/24. Max Y value is 30. 
- Multipliers which determines the likelihood of certain types of blocks to appear. A value of 1.0 is default. A value of 2.0 makes these blocks twice as likely to occur. Modifiers include: `road`, `platform`, `gate`, `tech`, `dirt`, `grass`, `bump` (sausage), `ice`, `plastic`, `water`, `flat`, `slope`, `tilt` (slanted left/right), `sideways` (wall), `diagonal`, `bobsleigh_with_walls`, `narrow` (penalty surface along edges)

Note that all range bounds are inclusive.

### `Blank.Map.Gbx` and `Blocks.Map.Gbx`
The generator creates maps by creating a copy of `Blank.Map.Gbx` and inserted blocks from `Blocks.Map.Gbx` at random places according to the `settings.toml`.
It will always place 1 start block, 1 finish block, and a random number of checkpoints.
Unless custom multipliers are used, evey start/finish/checkpoint block of `Blocks.Map.Gbx` is equally likely to be picked.
By default `Blocks.Map.Gbx` contains one of each type of start, finish, and checkpoint, but you can add or remove blocks from this map if you want to change the likelihood of individual blocks.
