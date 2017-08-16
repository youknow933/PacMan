using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour {
    //게임기판
    public static GameBoard ins;
    private static int boardWidth = 28; //가로 pellet갯수
    private static int boardHeight = 36; //세로 갯수

    public int totalPellets = 0;
    public int score = 0;
 
    public GameObject[,] board = new GameObject[boardWidth, boardHeight];  //가로 28, 세로 36
	// Use this for initialization
    void Awake()
    {
        ins = this;
    }
	void Start () {
        Object[] objects = GameObject.FindObjectsOfType(typeof(GameObject)); 
                                  //모든 활성화한 로드된 type 타입의 오브젝트 리스트를 반환합니다.
        foreach (GameObject o in objects)
        {
            Vector2 pos = o.transform.position;

            if(o.name != "PacMan" && o.name !="Nodes" && o.name!="NonNodes" && o.name!="Maze" && o.name != "Pellets"
                &&o.tag !="Ghost" && o.tag!= "ghostHome") {//

                if(o.GetComponent<Tile>()!=null)
                {
                    if(o.GetComponent<Tile>().isPellet|| o.GetComponent<Tile>().isSuperPellet)  
                    {
                        totalPellets++;
                    }
                }
                board[(int)pos.x, (int)pos.y] = o;  //팩맨을 제외하고 o를 넣어줌
                //Debug.Log(board[(int)pos.x, (int)pos.y]);
            }else
            {
              //  Debug.Log("Found PackMan at:" + pos);//팩맨을 찾음
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
