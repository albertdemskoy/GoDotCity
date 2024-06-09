using Godot;
using Godot.Collections;

public partial class DataMap : Resource {

    public DataMap() {
        Cash = 10000;
    }

    [Export]
    public int Cash {get; set;}

    [Export]
    public Array<DataStructure> Structures {get; set;}
}
