using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class MapGenerator : MonoBehaviour {

	[SerializeField] private int cellWidth;
	[SerializeField] private int cellHeight;
	[SerializeField] private int totalWidth;
	[SerializeField] private int totalHeight;
	[SerializeField] private string seed = "lolerpoper";
	[SerializeField] private bool useRandomSeed = true;
	
	private struct IntVector2 {
		public int x;
		public int y;
		public IntVector2(int x, int y) {
			this.x = x;
			this.y = y;
		}
	}
	
	public static bool Even (int num) {
		return (num % 2 == 0) ? true : false; 
	}
	
	public static bool Odd (int num) {
		return (num % 2 != 0) ? true : false; 
	}
	
    private System.Random pseudoRNG;
    private byte[,] maze;
	private IntVector2 exitCell;

	void Start() {
	
		if (useRandomSeed)
			seed = Time.time.ToString();
		
		// Getting random seed
		pseudoRNG = new System.Random(seed.GetHashCode());
        
		cellWidth = 10;
		cellHeight = 10;
		totalWidth = 2 * cellWidth + 1;
		totalHeight = 2 * cellHeight + 1;

		GenerateMaze();
    }
	
	void setExitCell(IntVector2 exitCell) {
		maze[exitCell.x,exitCell.y] = 3;
	}
	
	void GenerateMaze () {
		maze = new byte[totalWidth,totalHeight];
		generateMazeSkeleton();
		setExitCell(new IntVector2(1, 1));
		bestFirstGeneration();
	}
	
	void generateMazeSkeleton() {
		for(int x = 0; x < totalWidth; x++) {
			for(int y = 0; y < totalHeight; y++) {
				// Outer maze walls.
				if(x == 0 || x == totalWidth-1 || y == 0 || y == totalHeight-1) {
					maze[x,y] = 1;
				}
				// Even numbers will be walls.
				else if((x % 2 != 0) && (y % 2 != 0)) {
					maze[x,y] = 0;
				}
				// The other cases will be cells.
				else {
					maze[x,y] = 1;
				}
			}
		}
	}
		
		/* Best-First Search 
		 *
		 * 1 - Start at a particular cell and call it the "exit."
		 * 2 - Mark the current cell as visited, and get a list of its neighbors.
		 * 3 - For each neighbor, starting with a randomly selected neighbor:
		 *     If that neighbor hasn't been visited, remove the wall between this cell and that neighbor, 
		 *	   and then recur with that neighbor as the current cell.
		 */
		
	void bestFirstGeneration() {
		//IntVector2 currentCell = exitCell;
		Debug.Log("best first generation...");

	}
	
	void InspectNeighbors(IntVector2 currentCell) {
	
		// TODO: find a better way to implement all this functionality.
	 
//		// Upper neighbor
//		if (maze[currentCell.x, currentCell.y + 2] == 2) {
//		
//		}
//		
//		// Lower neighbor
//		if (maze[currentCell.x, currentCell.y - 2] == 2) {
//			
//		}
//		
//		// Left neighbor
//		if (maze[currentCell.x - 2, currentCell.y] == 2) {
//			
//		}
//		
//		// Right neighbor
//		if (maze[currentCell.x + 2, currentCell.y] == 2) {
//			
//		}
	
	}
	
//	IntVector2 getNeighbor(IntVector2 currentCell) {
//
//		
//		IntVector2 nextPossibleCell = new IntVector2(currentCellPos.x + x, currentCellPos.y + y);
//	}
//	
	
	int generateRandomMovement() {
		return (pseudoRNG.Next() <= 0.5) ? -2 : 2;
	}
	
	IntVector2 getRandomNeighbor(IntVector2 currentCellPos) {
	
		int x = generateRandomMovement();
		int y = generateRandomMovement();
		
		IntVector2 nextPossibleCell = new IntVector2(currentCellPos.x + x, currentCellPos.y + y);
		return nextPossibleCell;
		
//		// Checking next possible cell is inbunds if not we call the function recursivly until we find a suitable cell.
//		if((nextPossibleCell.x > 0 || nextPossibleCell < totalWidth) && 
//		   (nextPossibleCell.y > 0 || nextPossibleCell.y < totalWidth)) {
//			return nextPossibleCell;
//		}
//		else {
//			getRandomNeighbor(currentCellPos);
//		}
	
		
	}
	
	
	void OnDrawGizmos() {
		for(int x = 0; x < totalWidth; x++) {
			for(int y = 0; y < totalHeight; y++) {
				switch(maze[x,y]) {
					case 0: 
						Gizmos.color = Color.white;
						break;
					case 1:
						Gizmos.color = Color.black;
						break;
					case 2:
						Gizmos.color = Color.blue;
						break;
					case 3:
						Gizmos.color = Color.red;
						break;
				}
				//Gizmos.color = (maze[x,y] == 1) ? Color.black : Color.white;
				Vector3 pos = new Vector3(-totalWidth/2 + x + .5f, 0, -totalHeight/2 + y + .5f);
                Gizmos.DrawCube(pos, Vector3.one);
            }
        }
    }
    
	//		if (useRandomSeed)
	//			seed = Time.time.ToString();
	
	// Getting random seed
	//pseudoRNG = new System.Random(seed.GetHashCode());
    
//	[SerializeField] private int mazeCellsX;
//	[SerializeField] private int mazeCellsY;
//	
//	[SerializeField] private float cellWidth;
//	[SerializeField] private float cellHeight;
//	
//	[SerializeField] private float wallDepth;
//	[SerializeField] private float wallHeight;
//	
//	[SerializeField] private string seed = "lolerpoper";
//	[SerializeField] private bool useRandomSeed = true;
//	    
//	private System.Random pseudoRNG;
//	private Cell[,] maze;
//	//private Vector3 exit = new Vector3(0,0);
//    
//	public class Wall
//	{
//		public Vector3 position;
//		public float width;
//		//public bool active = true;
//		
//		public Wall(Vector3 _position, float _width) {
//			position = _position;
//			width = _width;
//		}
//	}
//	 
//	public class Cell : Wall
//	{
//		public Wall bottom, right;
//		public float height;
//		//public bool visited;
//		
//		public Cell(Vector3 _pos, float _height, float _width, float _wallWidth) : base(_pos, _wallWidth) {
//			height = _height;
//			width = _width;
//		}
//	}
//	
////	void Start() {
////		GenerateRandomMaze();
////	}
//	 
//	void GenerateRandomMaze() {
//		//maze = new Cell[mazeCellsX,mazeCellsY];
//		CreateMazeSkeleton();
//		//BestFirstMaze();
//	}
//	
//	void CreateMazeSkeleton() {
//		for(int x = 0; x < mazeCellsX; x++) {
//			for(int y = 0; y < mazeCellsY; y++) {
//				maze[x,y] = new Cell(new Vector3(x,y), cellHeight, cellWidth, wallDepth);
//			}
//		}
//	}
//	
//	/* Best-First Search 
//	 *
//	 * 1 - Start at a particular cell and call it the "exit."
//	 * 2 - Mark the current cell as visited, and get a list of its neighbors.
//	 * 3 - For each neighbor, starting with a randomly selected neighbor:
//	 *     If that neighbor hasn't been visited, remove the wall between this cell and that neighbor, 
//	 *	   and then recur with that neighbor as the current cell.
//	 */
//	void BestFirstMaze() {
//		
////		// start position
////		Vector3 currentCell = exit;
////		
////		Cell nextCell = getRandomNeighbor(currentCell);
////		
////		// Iterates through the current cell until a next cell is found
////		
////		// Sets the current cell depending on weather the neigbor is visited or not.
////		currentCell = (!nextCell.visited) ? nextCell : currentCell;
////		
////		if (!nextCell.visited) {
////			currentCell = nextCell;
////		}
//		
//		Debug.Log("Generating maze with best-first search...");
//		
//		// visited?
//		
//	}
//	
////	Cell getRandomNeighbor(Cell cell) {
////	
////	}
//	
////	void markAsVisited(int x, int y) {
////		maze[x,y].visited = true;
////	}
//	
//	void OnDrawGizmos() 
//	{
//		for(int x = 0; x < mazeCellsX; x++) {
//			for(int y = 0; y < mazeCellsY; y++) {
//				
//				// Drawing ground
//				Gizmos.color = Color.white;
//				Vector3 pos = new Vector3(x - mazeCellsX/2 + 0.5f, y - mazeCellsY/2 + 0.5f, 0);
//				Gizmos.DrawCube(pos, new Vector3(cellWidth, cellHeight, wallDepth));
//				
//				// Drawing right walls only if they are active
//				//if (maze[x,y].right.active) {
//					Gizmos.color = Color.red;
//					Vector3 rightWallPos = new Vector3(x - mazeCellsX/2, y - mazeCellsY/2 + 0.5f, wallHeight/2);
//					Gizmos.DrawCube(rightWallPos, new Vector3(wallDepth, cellHeight, wallHeight));
//				//}
//				
//				// Drawing bottom walls only if active
//				//if (maze[x,y].bottom.active == true) {
//					Gizmos.color = Color.blue;
//					Vector3 bottomWallPos = new Vector3(x - mazeCellsX/2+ 0.5f, y - mazeCellsY/2, wallHeight/2);
//					Gizmos.DrawCube(bottomWallPos, new Vector3(cellWidth, wallDepth, wallHeight));  
//				//}
//				
//			}
//		}
//	}
	
	
}
