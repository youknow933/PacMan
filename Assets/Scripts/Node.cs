using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {

    public Node[] neighbors;  //이웃 
    public Vector2[] validDirections;  //가용 방향
	// Use this for initialization
	void Start () {
        validDirections = new Vector2[neighbors.Length];

        for(int i = 0; i < neighbors.Length; i++)//이웃 갯수
        {
            Node neighbor = neighbors[i];
            Vector2 tempVector = neighbor.transform.localPosition - transform.localPosition;

            validDirections[i] = tempVector.normalized; //vector가 1인 벡터를 반환한다.
            Debug.Log("validDirections[i]:" + validDirections[i]);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
