using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Maze : MonoBehaviour {
	public int sizeX,sizeZ;
	public CellCluster cellPrefab;
	private CellCluster[,] cellclusters;
	
	public float generationStepDelay;

	public void Generate () {
		float neighbortriname;
		int neighbortri;
		sizeX = 4;
		sizeZ = 4;
		cellclusters = new CellCluster[sizeX, sizeZ];
		for (int x = 0; x < sizeX; x++) {
			for (int z = 0; z < sizeZ; z++) {
				CreateCell(x, z);
			}
		}
		for (int x = 0; x < sizeX; x++) {
			for (int z = 0; z < sizeZ; z++) {
				for (int tri = 0; tri < 4; tri++) {
					// find target cell
					float triname = Mathf.Pow(2, tri);
					Debug.Log(cellclusters[x, z].transform.Find(""+triname).GetComponent<TriangleCellFloor>()); // original triangle
					
            		Debug.Log(System.Convert.ToString(tri, toBase: 2)); //original triangle in binary
            		neighbortri = tri ^ 2;
					neighbortriname = Mathf.Pow(2, neighbortri);
            		cellclusters[x, z].transform.Find(""+triname).GetComponent<TriangleCellFloor>().neighbors.Add(cellclusters[x, z].transform.Find(""+neighbortriname).GetComponent<TriangleCellFloor>()); // prints out value with xy flag flipped
            		neighbortri = neighbortri ^ 1;
					neighbortriname = Mathf.Pow(2, neighbortri);
            		cellclusters[x, z].transform.Find(""+triname).GetComponent<TriangleCellFloor>().neighbors.Add(cellclusters[x, z].transform.Find(""+neighbortriname).GetComponent<TriangleCellFloor>()); // prints out xy and first/last flags flipped
            		neighbortriname = Mathf.Pow(2, tri ^ 1);
					try {
						if ((tri & (1U << 1)) != 0) // check xz flag
            			{
							if ((tri & (1U << 0)) == 0) {
								cellclusters[x, z].transform.Find(""+triname).GetComponent<TriangleCellFloor>().neighbors.Add(cellclusters[x, z + 1].transform.Find(""+neighbortriname).GetComponent<TriangleCellFloor>());
							}
							else {
								cellclusters[x, z].transform.Find(""+triname).GetComponent<TriangleCellFloor>().neighbors.Add(cellclusters[x, z - 1].transform.Find(""+neighbortriname).GetComponent<TriangleCellFloor>());
							}
            			}
            			else // triangle is on x axis
            			{
                			if ((tri & (1U << 0)) == 0) {
								cellclusters[x, z].transform.Find(""+triname).GetComponent<TriangleCellFloor>().neighbors.Add(cellclusters[x + 1, z].transform.Find(""+neighbortriname).GetComponent<TriangleCellFloor>());
							}
							else {
								cellclusters[x, z].transform.Find(""+triname).GetComponent<TriangleCellFloor>().neighbors.Add(cellclusters[x - 1, z].transform.Find(""+neighbortriname).GetComponent<TriangleCellFloor>());
							}
            			}
					} catch (Exception) {
						Debug.Log("edge cell");
					}
				}
			}
		}
		Stack<TriangleCellFloor> work = new Stack<TriangleCellFloor> ();
		List<TriangleCellFloor> visited = new List<TriangleCellFloor> ();

		TriangleCellFloor start = cellclusters[0, 0].transform.Find("1").GetComponent<TriangleCellFloor>();
		TriangleCellFloor end = cellclusters[1, 1].transform.Find("1").GetComponent<TriangleCellFloor>();

		work.Push (start);
		Debug.Log(work);
		visited.Add (start);
		start.history = new List<TriangleCellFloor> ();

		while(work.Count > 0){

			TriangleCellFloor current = work.Pop ();
			if (current == end) {
				List<TriangleCellFloor> result = current.history;
				result.Add (current);
				Debug.Log(result);
				break;
			} else {
			
				for(int i = 0; i < current.neighbors.Count; i++){

					TriangleCellFloor currentChild = current.neighbors [i];
					if(!visited.Contains(currentChild)){

						work.Push (currentChild);
						visited.Add (currentChild);
						currentChild.history = new List<TriangleCellFloor> (current.history);
						currentChild.history.Add (current);
					}
				}	
			}
		}
	}
	
	public void Deteriorate (ref Maze maze) {
		Destroy(cellclusters[3, 3].gameObject);
	}

	private void CreateCell (int x, int z) {
		CellCluster newCell = Instantiate(cellPrefab) as CellCluster;
		
		cellclusters[x, z] = newCell;
		newCell.name = "Maze Cell " + x + ", " + z;
		newCell.transform.localPosition = new Vector3(x - sizeX * 0.5f + 0.5f, 0f, z - sizeZ * 0.5f + 0.5f);
	}
}