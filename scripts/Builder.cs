using System;
using Godot;
using Godot.Collections;

public partial class Builder : Node3D {

	[Export]
	public Array<Structure> Structures {get; set;}

	[Export]
	public Node3D Selector {get; set;}

	[Export]
	public Node3D SelectorContainer {get; set;}

	[Export]
	public Camera3D ViewCamera {get; set;}

	[Export]
	public GridMap GridMap {get; set;}

	[Export]
	public Label CashDisplay {get; set;}


	DataMap map;
	int structureId = 0;
	Plane plane;

	public override void _Ready()
	{
		map = new DataMap();
		plane = new Plane(Vector3.Up, Vector3.Zero); 

		// 	# Create new MeshLibrary dynamically, can also be done in the editor
		// 	# See: https://docs.godotengine.org/en/stable/tutorials/3d/using_gridmaps.html

		var meshLibrary = new MeshLibrary();
		foreach (var structure in Structures) {		
			var id = meshLibrary.GetLastUnusedItemId();		
			meshLibrary.CreateItem(id);
			meshLibrary.SetItemMesh(id, GetMesh(structure.Model));
			meshLibrary.SetItemMeshTransform(id, new Transform3D());
		}

	   GridMap.MeshLibrary = meshLibrary;
	   UpdateStructure();
	   UpdateCash();

	}

	public override void _Process(double delta) {
		ActionRotate(); // Rotates selection 90 degrees
		ActionStructureToggle(); // Toggles between structures

		ActionSave(); // Saving
		ActionLoad(); // Loading
		
		// Map position based on mouse
		var worldPosition = (Vector3) plane.IntersectsRay(
			ViewCamera.ProjectRayOrigin(GetViewport().GetMousePosition()),
			ViewCamera.ProjectRayNormal(GetViewport().GetMousePosition()));
		
		var gridmapPosition = new Vector3I((int)Math.Round(worldPosition.X), 0, (int)Math.Round(worldPosition.Z));

		Selector.Position = VectorOps.Lerp(Selector.Position, gridmapPosition, delta * 40);
		
		ActionBuild(gridmapPosition);
		ActionDemolish(gridmapPosition);
	}

	 // # Retrieve the mesh from a PackedScene, used for dynamically creating a MeshLibrary

	Mesh GetMesh(PackedScene packedScene){
		var sceneState = packedScene.GetState();
		for (int nodeIdx = 0; nodeIdx < sceneState.GetNodeCount(); nodeIdx++) {
			if (sceneState.GetNodeType(nodeIdx) == "MeshInstance3D") {
				for (int propertyIdx = 0; propertyIdx < sceneState.GetNodePropertyCount(nodeIdx); propertyIdx++) {
					var propName = sceneState.GetNodePropertyName(nodeIdx, propertyIdx);
					if (propName == "mesh") {
						var propValue = sceneState.GetNodePropertyValue(nodeIdx, propertyIdx);
						return propValue.As<Mesh>();
					}
				}
			}
		}  
		return null;
	}

	public void ActionBuild(Vector3I gridmapPosition) 
	{
		if (Input.IsActionJustPressed("build")) 
		{
			var currTileStructure = GridMap.GetCellItem(gridmapPosition);
			GridMap.SetCellItem(gridmapPosition, structureId, GridMap.GetOrthogonalIndexFromBasis(Selector.Basis));

			if (currTileStructure != structureId) 
			{
				map.Cash -= Structures[structureId].Price;
				UpdateCash();
			}
		}
	}


	public void ActionDemolish(Vector3I gridmapPosition) {
		if (Input.IsActionJustPressed("demolish")) 
		{
			GridMap.SetCellItem(gridmapPosition, -1);
		}
	}

	public void ActionRotate() {
		if (Input.IsActionJustPressed("rotate"))
		{
			Selector.RotateY((float)Math.PI/2);
		}
	}

	int Wrap(int next) 
	{
		return next >= Structures.Count 
			? 0
			: next < 0 
				? Structures.Count - 1 
				: next;
	}

	public void ActionStructureToggle() 
	{

		if (Input.IsActionJustPressed("structure_next"))
		{
			structureId = Wrap(structureId + 1);
		}

		if (Input.IsActionJustPressed("structure_previous")) 
		{
			structureId = Wrap(structureId - 1);
		}

		UpdateStructure();
	}


	public void UpdateStructure() 
	{
		foreach (Node n in SelectorContainer.GetChildren())
		{
			SelectorContainer.RemoveChild(n);
		}
		var _model = (Node3D) Structures[structureId].Model.Instantiate();
		SelectorContainer.AddChild(_model);
		_model.Transform.Translated(new Vector3((float)0.25, 0, 0));
	}

	public void UpdateCash()
	{
		CashDisplay.Text = "$" + map.Cash.ToString();
	}       
	
	public void ActionSave()
	{
		if (Input.IsActionJustPressed("save"))
		{
			map.Structures.Clear();

			foreach (var cell in GridMap.GetUsedCells())
			{
				var structureDetails = new DataStructure
				{
					Position = new Vector2I(cell.X, cell.Y),
					Orientation = GridMap.GetCellItemOrientation(cell),
					Structure = GridMap.GetCellItem(cell),
				};

				map.Structures.Add(structureDetails);
			}

			ResourceSaver.Save(map, "user://map.res");
		}
	}

	public void ActionLoad()
	{
		if (Input.IsActionJustPressed("load"))
		{
			GridMap.Clear();
		
			var loadedResource =  ResourceLoader.Load("user://map.res") ?? new DataMap();
			if (loadedResource is DataMap map){		
				foreach (var cell in map.Structures)
				{
					GD.Print($"{cell.Position.X}, {cell.Structure}");
					GridMap.SetCellItem(new Vector3I(cell.Position.X, 0, cell.Position.Y), cell.Structure, cell.Orientation);
				}    
			} else {
				GD.Print("Not a datamap??");
			}
		}
	}
}
