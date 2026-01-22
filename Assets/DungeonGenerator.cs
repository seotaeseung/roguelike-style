using UnityEngine;
using UnityEngine.Tilemaps;


public class DungeonGenerator : MonoBehaviour
{
    //시작할때 정하는 맵의 너비와 폭
    public int mapWidth = 100;
    public int mapHeight = 100;
    public int maxDepth = 3; // 몇 번이나 쪼갤지 (트리 깊이)
    private Node rootNode;

    //맵을 기록하기 위한 이차원배열
    int[,] map;

    //생성할 타일맵
    public Tilemap tilemap;
    public TileBase tileBase;

    //맵 생성시 생성될 플레이어
    public GameObject player;
    private void Start()
    {
        //루트노드에 맵의 너비와 폭 시작 BottomLeft좌표를 보내줌
        rootNode = new Node(mapWidth, mapHeight, new Vector2Int(0, 0));
        //기록할 이차원배열의 크기를 정함
        map = new int[mapWidth, mapHeight];
        

        Divide(rootNode, 0);
        GenerateDungeon(rootNode);
        InstantiateMap();
        SpawnPlayer(rootNode);
    }
    

    //노드와 깊이를 전달받는 나누는 함수
    public void Divide(Node node, int depth)
    {
        //만약에 깊이가 최대 깊이보다 크면 종료
        if (depth > maxDepth) return;
        //만약에 나눌 수 있다면
        if (node.Split())
        {
            //왼쪽 오른쪽 노드와 깊이+1 을 전달해서 나눔
            Divide(node.LeftNode, depth + 1);
            Divide(node.RightNode, depth + 1);
        }
    }
    //노드를 전달받는 던전생성 함수
    public void GenerateDungeon(Node node)
    {
        //만약 자식이 없는노드라면
        if(node.LeftNode == null && node.RightNode == null)
        {
            //방을 생성함
            node.GenerateRoom();
            //맵을 기록하는 이차원 배열에 기록 해당 좌표를 1로 변경함
            
            for (int i = 0; i < node.RoomWidth; i++)
            {
                for(int j = 0; j < node.RoomHeight; j++)
                {
                    //방의 시작위치 기준으로 i,j만큼 떨어진 좌표계산
                    map[(int)node.roomBL.x + i, (int)node.roomBL.y + j] = 1;
                }
            }
        }
        else//자식이 있는 노드의 경우
        {
            //왼쪽 오른쪽 노드중 존재하는 노드에 던전 생성
            if (node.LeftNode != null) GenerateDungeon(node.LeftNode);
            if (node.RightNode != null) GenerateDungeon(node.RightNode);
            //그리고 해당노드에도 방을 생성
            node.GenerateRoom();
            //자식노드의 중앙좌표를 가져와 저장
            Vector2 lCenter = node.LeftNode.Center;
            Vector2 rCenter = node.RightNode.Center;

            //좌표에 따라 왼쪽노드가 오른쪽 노드보다 클수도 있으므로 min,max를 정해줌
            float xmin = Mathf.Min(lCenter.x, rCenter.x);
            float xmax = Mathf.Max(lCenter.x, rCenter.x);

            //작은 값부터 큰값까지 길을 이차원 배열에1로 기록함
            for (int i = (int)xmin; i< (int)xmax; i++)
            {
                map[i, (int)lCenter.y] = 1;
            }

            float ymin = Mathf.Min(lCenter.y, rCenter.y);
            float ymax = Mathf.Max(lCenter.y, rCenter.y);

            for (int i = (int)ymin; i < (int)ymax; i++)
            {
                map[(int)rCenter.x, i] = 1;
            }
        }
    } 

    public void InstantiateMap()
    {
        // 맵 전체를 확인
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                //1인 곳에만 큐브를 생성합니다.
                if (map[x, y] == 1)
                {
                    Vector3Int pos = new Vector3Int(x, y, 0);
                    tilemap.SetTile(pos, tileBase);
                }
            }
        }
    }
    //플레이어 생성 스크립트
    public void SpawnPlayer(Node node)
    {
        //만약 왼쪽노드가 있다면 그곳으로 이동하게 하기 
        if (node.LeftNode != null)
        {
            SpawnPlayer(node.LeftNode);
        }
        else //왼쪽 자식이 없는 노드라면 
        {
            Vector3Int spawnpos = new Vector3Int((int)node.Center.x,
                (int)node.Center.y, 0);

            Instantiate(player, tilemap.GetCellCenterWorld(spawnpos), Quaternion.identity);
        }
    }

    /*public void DrawDungeon(Node node)
    {
        if (node == null) return;

        if (node.LeftNode == null && node.RightNode == null)
        {
            Gizmos.color = Color.blue;

            Vector3 center = new Vector3(node.roomBL.x + (node.RoomWidth / 2f),
                                         node.roomBL.y + (node.RoomHeight / 2f), 0);
            Vector3 size = new Vector3(node.RoomWidth, node.RoomHeight, 0);

            Gizmos.DrawWireCube(center, size);
        }
        else
        {
            DrawDungeon(node.LeftNode);
            DrawDungeon(node.RightNode);

            Gizmos.DrawLine(node.LeftNode.Center, node.RightNode.Center);
        }
    }
    public void OnDrawGizmos()
    {
        DrawTree(rootNode);
        DrawDungeon(rootNode);
        if (map != null)
        {
            Gizmos.color = Color.red;
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    
                    if (map[x, y] == 1)
                    {
                        Gizmos.DrawCube(new Vector3(x, y, 0), Vector3.one);
                    }
                }
            }
        }
    }
    public void DrawTree(Node node)
    {
        if (node == null) return;
        Gizmos.color = Color.green;

        Vector3 center = new Vector3(node.BL.x + (node.Width / 2f),
                             node.BL.y + (node.Height / 2f), 0);
        Vector3 size = new Vector3(node.Width, node.Height, 0);
        Gizmos.DrawWireCube(center, size);
        DrawTree(node.LeftNode);
        DrawTree(node.RightNode);
    }*/
}
