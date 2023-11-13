using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Maze : MonoBehaviour {
	public int sizeX,sizeZ;
	public CellCluster cellPrefab;
	private CellCluster[,] cellclusters;
	
	public float generationStepDelay;

	public void Generate () {
		float neighbortriname;
		float wallname;
		int neighbortri;
		float triname;
		sizeX = 16;
		sizeZ = 16;
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
					triname = Mathf.Pow(2, tri);
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
								cellclusters[x, z].transform.Find(""+triname).GetComponent<TriangleCellFloor>().neighbors.Add(cellclusters[x + 1, z].transform.Find(""+neighbortriname).GetComponent<TriangleCellFloor>());
							}
							else {
								cellclusters[x, z].transform.Find(""+triname).GetComponent<TriangleCellFloor>().neighbors.Add(cellclusters[x - 1, z].transform.Find(""+neighbortriname).GetComponent<TriangleCellFloor>());
							}
            			}
            			else // triangle is on x axis
            			{
                			if ((tri & (1U << 0)) == 0) {
								cellclusters[x, z].transform.Find(""+triname).GetComponent<TriangleCellFloor>().neighbors.Add(cellclusters[x, z + 1].transform.Find(""+neighbortriname).GetComponent<TriangleCellFloor>());
							}
							else {
								cellclusters[x, z].transform.Find(""+triname).GetComponent<TriangleCellFloor>().neighbors.Add(cellclusters[x, z - 1].transform.Find(""+neighbortriname).GetComponent<TriangleCellFloor>());
							}
            			}
					} catch (Exception) {

					}
					// WIP
        			List<TriangleCellFloor> temp = new List<TriangleCellFloor>();
        			int j = cellclusters[x, z].transform.Find(""+triname).GetComponent<TriangleCellFloor>().neighbors.Count;
        			for (int i = 0; i < j; i++)
        			{
            			temp.Add(cellclusters[x, z].transform.Find(""+triname).GetComponent<TriangleCellFloor>().neighbors[UnityEngine.Random.Range(0, cellclusters[x, z].transform.Find(""+triname).GetComponent<TriangleCellFloor>().neighbors.Count)]);
            			cellclusters[x, z].transform.Find(""+triname).GetComponent<TriangleCellFloor>().neighbors.Remove(temp.Last());
        			}
        			cellclusters[x, z].transform.Find(""+triname).GetComponent<TriangleCellFloor>().neighbors = temp;
				}
			}
		}
		Stack<TriangleCellFloor> work = new Stack<TriangleCellFloor> ();
		List<TriangleCellFloor> visited = new List<TriangleCellFloor> ();

		TriangleCellFloor start = cellclusters[0, 0].transform.Find("2").GetComponent<TriangleCellFloor>();
		//TriangleCellFloor end = cellclusters[1, 1].transform.Find("2").GetComponent<TriangleCellFloor>();

		work.Push (start);
		Debug.Log(work);
		visited.Add (start);

		while(work.Count > 0){

			TriangleCellFloor current = work.Pop ();
			if (false) { //(current == end) {
				//List<TriangleCellFloor> result = current.history;
				//result.Add (current);
				//Debug.Log(result);
			} else {
				for(int i = 0; i < current.neighbors.Count; i++){

					TriangleCellFloor currentChild = current.neighbors [i];
					if(!visited.Contains(currentChild)){

						work.Push (currentChild);
						visited.Add (currentChild);
						currentChild.history.AddRange (current.history);
						currentChild.history.Add (current);
					}
				}	
			}
		}
		for (int x = 0; x < sizeX; x++)
		{
			for (int z = 0; z < sizeZ; z++)
			{
				for (int tri = 0; tri < 4; tri++)
				{
					triname = Mathf.Pow(2, tri);
					TriangleCellFloor neighbor1 = cellclusters[x, z].transform.Find("" + triname).GetComponent<TriangleCellFloor>();
					if (neighbor1 != start) {
						TriangleCellFloor neighbor2 = cellclusters[x, z].transform.Find("" + triname).GetComponent<TriangleCellFloor>().history.Last();
						wallname = float.Parse(neighbor1.name) + float.Parse(neighbor2.name);
						if (neighbor2.transform.parent.GetComponent<CellCluster>().x == x && neighbor2.transform.parent.GetComponent<CellCluster>().z == z) {
							//Debug.Log("Destroying"+ " at " + neighbor2.transform.parent.GetComponent<CellCluster>().x + ", " + neighbor2.transform.parent.GetComponent<CellCluster>().z);
							Destroy(cellclusters[x, z].transform.Find("" + wallname).gameObject);
						} else if (Mathf.Log(float.Parse(neighbor1.name), 2) % 2 != 0) {
							Debug.Log("n1: " + cellclusters[x, z].transform.Find("" + triname).GetComponent<TriangleCellFloor>().name + " at " + x + ", " + z);
							Debug.Log("n2: " + cellclusters[x, z].transform.Find("" + triname).GetComponent<TriangleCellFloor>().history.Last().name + " at " + neighbor2.transform.parent.GetComponent<CellCluster>().x + ", " + neighbor2.transform.parent.GetComponent<CellCluster>().z);
							Debug.Log("wallname: " + wallname);
							Destroy(cellclusters[x, z].transform.Find("" + wallname).gameObject);
						} else {
							Debug.Log("n1: " + cellclusters[x, z].transform.Find("" + triname).GetComponent<TriangleCellFloor>().name);
							Debug.Log("n2: " + cellclusters[x, z].transform.Find("" + triname).GetComponent<TriangleCellFloor>().history.Last().name);
							Debug.Log("wallname: " + wallname);
							Destroy(cellclusters[neighbor2.transform.parent.GetComponent<CellCluster>().x, neighbor2.transform.parent.GetComponent<CellCluster>().z].transform.Find("" + wallname).gameObject);
						} //greater one holds walls
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
		
		newCell.x = x;
		newCell.z = z;
		cellclusters[x, z] = newCell;
		newCell.name = "Maze Cell " + x + ", " + z;
		newCell.transform.localPosition = new Vector3(x - sizeX * 0.5f + 0.5f, 0f, z - sizeZ * 0.5f + 0.5f);
	}
}