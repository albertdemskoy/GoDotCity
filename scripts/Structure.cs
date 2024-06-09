using Godot;

public partial class Structure : Resource
{
    [ExportSubgroup("Model")]
    [Export]
    public PackedScene Model {get; set;}

    [ExportSubgroup("Gameplay")]
    [Export]
    public int Price {get; set;}
}