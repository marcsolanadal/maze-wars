using UnityEngine;
using System;
using System.Collections.Generic;

public class MazeBuilder : MonoBehaviour
{
    // Parameters for the maze generator.
    [SerializeField][Range(4, 80)] int cellWidth = 0;
    [SerializeField][Range(4, 80)] int cellHeight = 0;
    [SerializeField][Range(0, 1)] float corridorDirection = 0.5f;
    [SerializeField][Range(0, 1)] float chestSpawnProvability = 0.5f;
    [SerializeField][Range(0, 0.2f)] float trapSpawnProvability = 0.05f;
    [SerializeField] string seed = "lolerpoper";
    [SerializeField] bool useRandomSeed = true;

    // Parameters for the maze theme.
    // TODO: Dinamically get the zones names inside the zones folder.
    [SerializeField] List<string> theme = new List<string>();

    // Parameters for the maze builder.
    [SerializeField] GameObject wallPrefab;
    [SerializeField] GameObject chestPrefab;
    [SerializeField] GameObject roomPrefab;

    private Maze maze;
    private ZoneMeshes zoneMeshes;

    void Start()
    {
        if (useRandomSeed)   
            seed = DateTime.Now.Ticks.ToString();

        GenerateMaze();
        GenerateZoneAssets("Ruins");
        BuildMaze();
        AttachRoomsToMaze(roomPrefab);
    }

    void GenerateMaze()
    {
        Debug.Log("generating maze map...");
        maze = new Maze(cellWidth, cellHeight, seed, corridorDirection);
        maze.BestFirstOrdering();
        maze.AssignCellTypes(chestSpawnProvability, trapSpawnProvability);
    }

    void GenerateZoneAssets(string theme)
    {
        Debug.Log("generating zone meshes...");
        zoneMeshes = new ZoneMeshes(theme, seed);
    }

    void BuildMaze()
    {
        Debug.Log("building maze...");

        int mult = 2;

        for (int x = 0; x < maze.totalCellsWidth; x++)
        {
            for (int y = 0; y < maze.totalCellsHeight; y++)
            {
                switch (maze.map[x, y].type)
                {
                    case CellType.Free:
                        //Gizmos.color = Color.gray;
                        break;
                    case CellType.Wall:
                        switch (maze.map[x, y].wall)
                        {
                            case WallType.Isolate:
                                CreateWall(new Vector3(mult * x, 0, mult * y), zoneMeshes.isolateWall, new Color(0.73f, 0.67f, 0.26f));
                                break;
                            case WallType.Corner:
                                switch (maze.map[x, y].corner)
                                {
                                    case CornerType.None:
                                        //Gizmos.color = new Color(0, 1, 0, 0.2f);
                                        break;
                                    case CornerType.DownLeft:
                                        //Gizmos.color = new Color(0, 50f, 0, 0.5f);
                                        break;
                                    case CornerType.DownRight:
                                        //Gizmos.color = new Color(0, 100f, 0, 0.5f);
                                        break;
                                    case CornerType.UpLeft:
                                        //Gizmos.color = new Color(0, 1, 0, 0.5f);
                                        break;
                                    case CornerType.UpRight:
                                        //Gizmos.color = new Color(0, 1, 0, 0.4f);
                                        break;
                                    case CornerType.End:
                                        //Gizmos.color = new Color(0, 1, 0.5f, 0.7f);
                                        break;
                                    default:
                                        //Gizmos.color = new Color(0, 1, 0, 0.5f);
                                        break;
                                }
                                break;
                            case WallType.Normal:
                                //if (normalWallMaterial != null)
                                //{
                                //    // Create material
                                //}
                                //CreateWall(new Vector3(mult * x, 0, mult * y), zoneMeshes.normalWall, );
                                CreateWall(new Vector3(mult * x, 0, mult * y), zoneMeshes.normalWall, new Color(0, 1, 0, 0.5f));
                                break;
                            case WallType.Joint:
                                CreateWall(new Vector3(mult * x, 0, mult * y), zoneMeshes.jointWall, new Color(0, 1, 0, 0.9f));
                                break;
                            case WallType.invisible:
                                CreateWall(new Vector3(mult * x, 0, mult * y), zoneMeshes.invisibleWall, new Color(0, 1, 1, 0.9f));
                                break;
                            case WallType.outter:
                                CreateWall(new Vector3(mult * x, 0, mult * y), zoneMeshes.outterWall, new Color(0.84f, 0.63f, 0.38f));
                                break;
                            case WallType.None:
                                //Gizmos.color = Color.gray;
                                break;
                        }
                        break;
                    case CellType.Chest:
                        //CreateChest(new Vector3(mult*x, 0, mult *y), zoneMeshes.chest, )
                        //Gizmos.color = Color.yellow;
                        break;
                    case CellType.Visited:
                        //Gizmos.color = Color.gray;
                        break;
                    case CellType.Listed:
                        //Gizmos.color = Color.gray;
                        break;
                    case CellType.Exit:
                        //Gizmos.color = Color.gray;
                        break;
                }
            }
        }

    }

    private GameObject CreateWall(Vector3 position, Mesh mesh, Color color)
    {
        // TODO: This is really not optimal. We should create only one material for each type of wall.
        Material material = new Material(Shader.Find("Standard"));
        material.color = color;

        wallPrefab.GetComponent<Transform>().position = position;
        wallPrefab.GetComponent<MeshFilter>().mesh = mesh;
        wallPrefab.GetComponent<MeshCollider>().sharedMesh = mesh;
        wallPrefab.GetComponent<MeshRenderer>().material = material;

        return Instantiate(wallPrefab);
    }
     
    private GameObject CreateChest(Vector3 position, float rotation, GameObject prefab, Material material)
    {

        // TODO: Generating Blender parameters to randomize the chest.

        // TODO: Depending on the size of the chest we choose the item inside.

        // Position and orientation.
        chestPrefab.GetComponent<Transform>().position = position;
        chestPrefab.GetComponent<Transform>().rotation = new Quaternion(0, rotation, 0, 0);

        // Assigning the mesh to the renderer and collider.
        //chestPrefab.GetComponent<MeshFilter>().mesh = mesh;
        //chestPrefab.GetComponent<MeshCollider>().sharedMesh = mesh;

        // Assigning material with randomly chosen texture.
        chestPrefab.GetComponent<MeshRenderer>().material = material;

        // Assigning animations to the chest.

        return Instantiate(chestPrefab);
    }

    private void AttachRoomsToMaze(GameObject prefab)
    {
        // Creating starting room.
        GameObject startRoom = Instantiate(prefab);

        //GameObject entrance = startRoom.
        Vector3 mazeEntrance = new Vector3(maze.Entrance.x * 2, 0, 0);
        startRoom.transform.position = mazeEntrance;

        // Creating end room.
        //GameObject endRoom = Instantiate(prefab);
        //Vector3 mazeExit = new Vector3(maze.Exit.position.x, maze.Exit.position.y);
        //endRoom.transform.Rotate(Vector3.up, 180);
        //startRoom.transform.position = mazeExit;
    }

    // Helper function for visualization
    // TODO: It must be a way to improve this shitty code.
    void OnDrawGizmos()
    {
        for (int x = 0; x < maze.totalCellsWidth; x++)
        {
            for (int y = 0; y < maze.totalCellsHeight; y++)
            {
                switch (maze.map[x, y].type)
                {
                    case CellType.Free:
                        Gizmos.color = Color.gray;
                        break;
                    case CellType.Wall:
                        switch (maze.map[x, y].wall)
                        {
                            case WallType.Isolate:
                                Gizmos.color = new Color(0.73f, 0.67f, 0.26f);
                                break;
                            case WallType.Corner:
                                switch (maze.map[x, y].corner)
                                {
                                    case CornerType.None:
                                        Gizmos.color = new Color(0, 1, 0, 0.2f);
                                        break;
                                    case CornerType.DownLeft:
                                        Gizmos.color = new Color(0, 50f, 0, 0.5f);
                                        break;
                                    case CornerType.DownRight:
                                        Gizmos.color = new Color(0, 100f, 0, 0.5f);
                                        break;
                                    case CornerType.UpLeft:
                                        Gizmos.color = new Color(0, 1, 0, 0.5f);
                                        break;
                                    case CornerType.UpRight:
                                        Gizmos.color = new Color(0, 1, 0, 0.4f);
                                        break;
                                    case CornerType.End:
                                        Gizmos.color = new Color(0, 1, 0.5f, 0.7f);
                                        break;
                                    default:
                                        Gizmos.color = new Color(0, 1, 0, 0.5f);
                                        break;
                                }
                                break;
                            case WallType.Normal:
                                Gizmos.color = new Color(0, 1, 0, 0.5f);
                                break;
                            case WallType.Joint:
                                Gizmos.color = new Color(0, 1, 0, 0.9f);
                                break;
                            case WallType.invisible:
                                Gizmos.color = new Color(0, 1, 1, 0.9f);
                                break;
                            case WallType.outter:
                                Gizmos.color = new Color(0.84f, 0.63f, 0.38f);
                                break;
                            case WallType.None:
                                Gizmos.color = Color.gray;
                                break;
                        }
                        break;
                    case CellType.Chest:
                        Gizmos.color = Color.yellow;
                        break;
                    case CellType.Trap:
                        Gizmos.color = Color.red;
                        break;
                    case CellType.Visited:
                        Gizmos.color = Color.gray;
                        break;
                    case CellType.Listed:
                        Gizmos.color = Color.gray;
                        break;
                    case CellType.Exit:
                        Gizmos.color = Color.gray;
                        break;
                }

                Vector3 pos = new Vector3(-maze.totalCellsWidth / 2 + x + .5f, 10, -maze.totalCellsHeight / 2 + y + .5f);
                Gizmos.DrawCube(pos, Vector3.one);
            }
        }
    }

}