using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour {

    public float moveSpeed = 3.9f; //몬스터 이동속도 3.9

    public int pinkyReleaseTimer = 5;  //
    public int inkyReleaseTimer = 14;
    public int clydeReleaseTimer = 21;
    public float ghostReleaseTimer = 0;  //

    public bool isInGhostHouse = false;//

    public Node homeNode; //

    public Node startingPosition; //시작위치

    public int scatterModeTimer1 = 7;  //scatter모드
    public int chaseModeTimer1 = 20;    //chase모드
    public int scatterModeTimer2 = 7;
    public int chaseModeTimer2 = 20;
    public int scatterModeTimer3 = 5;
    public int chaseModeTimer3 = 20;
    public int scatterModeTimer4 = 5;
   // public int chaseMdeTimer4 = 20;

    private int modeChangeIteration = 1;
    private float modeChangeTimer = 0;

    public enum Mode   //상태
    {
        Chase,
        Scatter,
        Frightened
    }

    Mode currentMode = Mode.Scatter; //현 상태
    Mode previousMode; //이전상태

    public enum GhostType //
    {
        Red,
        Pink,
        Blue,
        Orange
    }

    public GhostType ghostType = GhostType.Red;//처음 red

    private GameObject pacMan;

    public Node currentNode, targetNode, previousNode;  //현 노드, 다음노드, 이전노드 저장
    private Vector2 direction, nextDirecton; //현 방향, 다음방향





	// Use this for initialization
	void Start () {
        pacMan = GameObject.FindGameObjectWithTag("PacMan");  //PacMan을 찾는다.
    
        Node node = GetNodeAtPosition(transform.localPosition);//현재 node를 node에 넣음
      
        if(node != null)
        {
            currentNode = node; //node가 null이아니면 currentNode에 받아온 node를 넣음
            //Debug.Log("Current1");
        }

        if (isInGhostHouse)//ghosthouse가 true이면 (밖에서 체크) 
        {
            direction = Vector2.up;  //방향 up
            targetNode = currentNode.neighbors[0];
        }else
        {
            direction = Vector2.left;  //red 고스트
            targetNode = ChooseNextNode();
        }
       
        previousNode = currentNode;     //현재 노드를 이전 노드에 줌
       // Debug.Log("currentNode: " + currentNode);


        
        
    }

    // Update is called once per frame
    void Update()
    {

        ModeUpdate();

        Move();

        ReleaseGhosts();



    }
    void Move()
    {
        
        if (targetNode != currentNode && targetNode != null && !isInGhostHouse) //targetnode가 현재 노드가 아니고, null이 아니면
           
        {
            //Debug.Log("OverShot" + OverShotTarget());
            //Debug.Log("OverShotTarget2");
            if (OverShotTarget())  //player가 반대방향으로 이동
            {
                //Debug.Log("OverShot"+OverShotTarget());
                currentNode = targetNode;  //targetnode에 현재노드를 넣는다.

                transform.localPosition = currentNode.transform.position; //위치를 넣는다.
                

                GameObject otherPortal = GetPortal(currentNode.transform.position);

                if (otherPortal != null)
                {
                    transform.localPosition = otherPortal.transform.position;
                  

                    currentNode = otherPortal.GetComponent<Node>();

                }
                targetNode = ChooseNextNode();

                previousNode = currentNode;

                currentNode = null;
            }
            else
            {
                //  Debug.Log(direction);
                transform.localPosition += (Vector3)direction * moveSpeed * Time.deltaTime;
            }
        }
    }
    
    void ModeUpdate()
    {
        if(currentMode != Mode.Frightened) //frightened모드가 아니면
        {
            modeChangeTimer += Time.deltaTime;

            if(modeChangeIteration == 1)  //modeChangeIteration가 1이면
            {
                if(currentMode == Mode.Scatter && modeChangeTimer> scatterModeTimer1) // 현재 모드가 scatter이고, modeChaneTimer가 지나면
                {
                    ChangeMode(Mode.Chase);  //chase 모드로 바꿈
                    modeChangeTimer = 0;    //modeChangeTimer을 0으로 바꿈
                }

                if(currentMode == Mode.Chase && modeChangeTimer > chaseModeTimer1)  //chase모드고 
                {

                    modeChangeIteration = 2;    //modeChangeIteration을 2로바꾸고
                    ChangeMode(Mode.Scatter); //Scatter모드로
                    modeChangeTimer = 0;
                }
            }else if(modeChangeIteration == 2)
            {
                if(currentMode == Mode.Scatter && modeChangeTimer > scatterModeTimer2)
                {
                    ChangeMode(Mode.Chase);
                    modeChangeTimer = 0;
                }
                if (currentMode ==Mode.Chase && modeChangeTimer> chaseModeTimer2)
                {
                    modeChangeIteration = 3;
                    ChangeMode(Mode.Scatter);
                    modeChangeTimer = 0;
                }

            }
            else if(modeChangeIteration == 3)
            {
              if(currentMode ==Mode.Scatter && modeChangeTimer > scatterModeTimer3)
                {
                    ChangeMode(Mode.Chase);
                    modeChangeTimer = 0;
                }
              if(currentMode ==Mode.Chase && modeChangeTimer > chaseModeTimer3)
                {
                    modeChangeIteration = 4;
                    ChangeMode(Mode.Scatter);
                    modeChangeTimer = 0;
                }

            }else if(modeChangeIteration ==4)
            {
                if(currentMode ==Mode.Scatter && modeChangeTimer > scatterModeTimer4) {

                    ChangeMode(Mode.Chase); //chase모드 계속
                    modeChangeTimer = 0;
                }
            }
        }else if(currentMode == Mode.Frightened)
        {

        }
    }
    void ChangeMode (Mode m) //모드 변환
    {
        currentMode = m;
    }

    Vector2 GetRedGhostTargetTile()//레드 일 때
    {
        Vector2 pacManPosition = pacMan.transform.localPosition;
        Vector2 targetTile = new Vector2(Mathf.RoundToInt(pacManPosition.x), Mathf.RoundToInt(pacManPosition.y));
       
        return targetTile;
    }

    Vector2 GetPinkGhostTargetTile()//핑크 일 때 
    {
        //-Four tiles ahead Pac-Man
        //-Taking account Position and Orientation
        // - 4 개의 타일을 앞두고 Pac-Man
        // - 계정 포지션 및 오리엔테이션

        Vector2 pacManPosition = pacMan.transform.localPosition;
        Vector2 pacManOrientation = pacMan.GetComponent<PackMan>().orientation;

        int pacManPositionX = Mathf.RoundToInt(pacManPosition.x);
        int pacManPositionY = Mathf.RoundToInt(pacManPosition.y);

        Vector2 pacManTile = new Vector2(pacManPositionX, pacManPositionY);
        Vector2 targetTile = pacManTile + (4 * pacManOrientation);  //같이 안따라다니게???
        Debug.Log(targetTile);
      
        return targetTile;
    }
    Vector2 GetBlueGhostTargetTile()
    {
        //-Select the position two tiles in front of Pac-Man
        //-Draw Vector from blinky to that position
        //-Double the length of the vector
        // - Pac-Man 앞의 위치 타일 두 개 선택
        // - blinky에서 그 위치로 Vector를 그립니다.
        // - 벡터의 길이를 두배로한다.

        Vector2 pacManPosition = pacMan.transform.localPosition;
        Vector2 pacManOrientation = pacMan.GetComponent<PackMan>().orientation;

        int pacManPositionX = Mathf.RoundToInt(pacManPosition.x);
        int pacManPositionY = Mathf.RoundToInt(pacManPosition.y);

        Vector2 pacManTiler = new Vector2(pacManPositionX, pacManPositionY);

        Vector2 targetTile = pacManTiler + (2 * pacManOrientation);

        //-Temporary Blink Position
        Vector2 tempBlinkyPosition = GameObject.Find("Ghost").transform.localPosition;

        int blinkyPositionX = Mathf.RoundToInt(tempBlinkyPosition.x);
        int blinkyPositionY = Mathf.RoundToInt(tempBlinkyPosition.y);

        tempBlinkyPosition = new Vector2(blinkyPositionX, blinkyPositionY);

        float distance = GetDistance(tempBlinkyPosition, targetTile);   //거리 2배
        distance *= 2;

        targetTile = new Vector2(tempBlinkyPosition.x + distance, tempBlinkyPosition.y + distance);
        return targetTile;
    }

    Vector2 GetOrangeGhostTargetTile()
    {
        //-Calculate the distance from Pac-Man
        //-If the distance is greater than eight tiles. targeting is the same as Blinky
        //-If the distance is less than eight tiles, then target is his home node, so same as scatter mode;
        // - Pac-Man으로부터의 거리 계산
        // - 거리가 여덟 개의 타일보다 큰 경우. 타겟팅은 Blinky와 동일합니다.
        // - 거리가 8 개 타일보다 작 으면 대상은 홈 노드이므로 스 캐터 모드와 동일합니다.


        Vector2 pacManPosition = pacMan.transform.localPosition;

        float distance = GetDistance(transform.localPosition, pacManPosition);
        Vector2 targetTile = Vector2.zero;

        if (distance > 8)
        {
            targetTile = new Vector2(Mathf.RoundToInt(pacManPosition.x), Mathf.RoundToInt(pacManPosition.y));
        }else if (distance < 8)
        {
            targetTile = homeNode.transform.position;
        }
        return targetTile;
    }


    Vector2 GetTargetTile()//
    {
        Vector2 targetTile = Vector2.zero;

        if (ghostType == GhostType.Red)  //레드일때
        {

            targetTile = GetRedGhostTargetTile();

        } else if (ghostType == GhostType.Pink)
        {
            targetTile = GetPinkGhostTargetTile();
        }
        else if(ghostType ==GhostType.Blue)
        {
            targetTile = GetBlueGhostTargetTile();
        }
        else if (ghostType == GhostType.Orange)
        {
            targetTile = GetOrangeGhostTargetTile();
        }
        return targetTile;
    }

    void ReleasePinkGhost()
    {
        if(ghostType == GhostType.Pink && isInGhostHouse)   //핑크고스트, true일 때 
        {
            isInGhostHouse = false;
        }
    }

    void ReleaseBlueGhost()
    {
        if (ghostType == GhostType.Blue && isInGhostHouse)
        {
            isInGhostHouse = false;
        }
    }
    void ReleaseOrangeGhost()
    {
        if (ghostType == GhostType.Orange && isInGhostHouse)
        {
            isInGhostHouse = false;
        }
    }

    void ReleaseGhosts()
    {
        ghostReleaseTimer += Time.deltaTime;
        if (ghostReleaseTimer > pinkyReleaseTimer) 
            ReleasePinkGhost();

        if (ghostReleaseTimer > inkyReleaseTimer) 
            ReleaseBlueGhost();

        if (ghostReleaseTimer > clydeReleaseTimer)
            ReleaseOrangeGhost();

    }
    Node ChooseNextNode()
    {
        Vector2 targetTile = Vector2.zero;  //처음을 zero로 줌

        if(currentMode == Mode.Chase)//
        {
            targetTile = GetTargetTile();
        }else if (currentMode == Mode.Scatter)
        {
            targetTile = homeNode.transform.position;
        }


        targetTile = GetTargetTile(); //

        Node moveToNode = null; 

        Node[] foundNodes = new Node[4];  //최댓값 4
        Vector2[] foundNodesDirection = new Vector2[4];

        int nodeCounter = 0;

        for(int i = 0; i< currentNode.neighbors.Length; i++)
        {
            if(currentNode.validDirections[i] != direction * -1) {  //반대방향이 아닐 때 

                foundNodes[nodeCounter] = currentNode.neighbors[i];  //현재 이웃 노드를 넣음
                foundNodesDirection[nodeCounter] = currentNode.validDirections[i]; //현재 가용방향을 넣음
                nodeCounter++;
            }
        }
        if(foundNodes.Length == 1)  //길이가 1이면 
        {
            moveToNode = foundNodes[0];  //가던 방향
            direction = foundNodesDirection[0];
             
        }
        if(foundNodes.Length > 1)  //길이가 1이상이면
       
        {
            float leastDistance = 100000f;

            for(int i =0; i< foundNodes.Length; i++)
            {
                if(foundNodesDirection[i] != Vector2.zero)  //방향이 0이아니면
                {
                    float distance = GetDistance(foundNodes[i].transform.position, targetTile);//target과 현재 노드의 거리를 반환

                    if(distance < leastDistance)
                    {
                        leastDistance = distance; //거리를 넣어주고
                        moveToNode = foundNodes[i]; //찾을 노드
                        direction = foundNodesDirection[i];

                   //     Debug.Log("MoveToNode: "+moveToNode);
                    }
                }
            }
        }
        return moveToNode;
    }

    Node GetNodeAtPosition (Vector2 pos)
    {
        GameObject tile = GameObject.Find("Game").GetComponent<GameBoard>().board[(int)pos.x, (int)pos.y]; //x,y값으로 tile을 찾음

        Debug.Log("Tile:" + tile +", FOR GHOST :" +ghostType);
        if(tile != null)
        {
            if (tile.GetComponent<Node>() != null)
            {
                return tile.GetComponent<Node>(); //tile이 null이 아니면 그 node를 반환함
            }
        }
        return null;
    }

    GameObject GetPortal (Vector2 pos) //포탈
    {

        GameObject tile = GameObject.Find("Game").GetComponent<GameBoard>().board[(int)pos.x, (int)pos.y];

        if(tile != null)
        {
            if (tile.GetComponent<Tile>().isPortal)//isPortal이면
            {

                GameObject otherPortal = tile.GetComponent<Tile>().portalReceiver;
                return otherPortal; //otherPortal 반환
            }
        }
        return null;
    }
 

    bool OverShotTarget()
    {

        //Debug.Log("OverShotTarget1");
        float nodeToTarget = LengthFromNode(targetNode.transform.position);  //반대방향
        float nodeToSelf = LengthFromNode(transform.localPosition);

        return nodeToSelf > nodeToTarget;
    }

    float LengthFromNode (Vector2 targetPosition)
    {
       // Debug.Log("TargetPosition :" + targetPosition);
       // Debug.Log("PreviousNude.transform.position:" + previousNode.transform.position);
        Vector2 vec = targetPosition - (Vector2)previousNode.transform.position;
        
        return vec.sqrMagnitude;
    }


    float GetDistance(Vector2 posA, Vector2 posB) //포인트 사이 거리
    {

        float dx = posA.x - posB.x;
        float dy = posA.y - posB.y;

        float distance = Mathf.Sqrt(dx * dx + dy * dy);//제곱근 반환

        return distance;
    }

}
