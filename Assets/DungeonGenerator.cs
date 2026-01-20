using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;


public class DungeonGenerator : MonoBehaviour
{
    [Header("맵 설정")]
    public int mapWidth = 100;
    public int mapHeight = 100;
    public int maxDepth = 3; // 몇 번이나 쪼갤지 (트리 깊이)
    private Node rootNode;

    int[,] map;
    private void Start()
    {
        rootNode = new Node(mapWidth, mapHeight, new Vector2Int(0, 0));
        map = new int[mapWidth, mapHeight];

        Divide(rootNode, 0);
        GenerateDungeon(rootNode);
        InstantiateMap();
    }
    public void Divide(Node node, int depth)
    {
        if (depth > maxDepth) return;
        if (node.Split())
        {
            Divide(node.LeftNode, depth + 1);
            Divide(node.RightNode, depth + 1);
        }
    }
    public void GenerateDungeon(Node node)
    {
        if(node.LeftNode == null && node.RightNode == null)
        {
            node.GenerateRoom();
            for(int i = 0; i < node.RoomWidth; i++)
            {
                for(int j = 0; j < node.RoomHeight; j++)
                {
                    map[(int)node.roomBL.x + i, (int)node.roomBL.y + j] = 1;
                }
            }
        }
        else
        {
            if (node.LeftNode != null) GenerateDungeon(node.LeftNode);
            if (node.RightNode != null) GenerateDungeon(node.RightNode);
            node.GenerateRoom();
            Vector2 lCenter = node.LeftNode.Center;
            Vector2 rCenter = node.RightNode.Center;

            float xmin = Mathf.Min(lCenter.x, rCenter.x);
            float xmax = Mathf.Max(lCenter.x, rCenter.x);

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
    public void DrawDungeon(Node node)
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
    /*public void OnDrawGizmos()
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
                    // 1(바닥)인 곳에만 빨간 큐브 그리기
                    if (map[x, y] == 1)
                    {
                        Gizmos.DrawCube(new Vector3(x, y, 0), Vector3.one);
                    }
                }
            }
        }
    }*/
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
    }
    [Header("프리팹 설정")]
    public GameObject floorPrefab; // 여기에 큐브 프리팹 연결

    public void InstantiateMap()
    {
        // 맵 전체를 훑습니다.
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                // 바닥(1)인 곳에만 큐브를 생성합니다.
                if (map[x, y] == 1)
                {
                    // 위치는 (x, 0, y) 또는 (x, y, 0) 등 원하는 축으로 설정
                    // 보통 3D 게임은 바닥이 X, Z 평면이므로 (x, 0, y)를 씁니다.
                    Vector2 pos = new Vector2(x, y);

                    Instantiate(floorPrefab, pos, Quaternion.identity, transform);
                }
            }
        }
    }
}
