using UnityEngine;

public class Node 
{
    //현재노드 너비와 폭
    public int Width;
    public int Height;
    //왼쪽아래 꼭짓점 좌표 저장
    public Vector2Int BL;

    //부모노드와 자식노드
    public Node ParentNode;
    public Node LeftNode;
    public Node RightNode;

    //만들어진 방의 너비와 폭
    public int RoomWidth;
    public int RoomHeight;
    //만들어진 방의 왼쪽아래 꼭짓점 좌표 저장
    public Vector2 roomBL;

    //방의 중앙좌표
    public Vector2 Center;
    public Node(int width, int height, Vector2Int bottomLeft)
    {
        Width = width;
        Height = height;

        BL = bottomLeft;
    }
    public bool Split()
    {
        //자식이 있다면 나누지 않음
        if(LeftNode != null || RightNode != null) return false;
        //자식에게 전해줄 너비와 폭을 저장할 변수
        int cWidth;
        int cHeight;
        //0이면 너비를 반으로, 1이면 폭을 반으로 나눔
        int num = Random.Range(0, 2);
        if (num == 0)
        {
            cWidth = Width / 2;
            cHeight = Height;
        }
        else
        {
            cWidth = Width;
            cHeight = Height / 2;
        }
        //나온 값을 왼쪽노드에 전달, 왼쪽노드이므로 BL은 부모의 BL과 같음
        LeftNode = new Node(cWidth, cHeight, BL);

        //오른쪽 노드에 전해줄 BL값을 만드는 과정 위에서 너비와 폭 중 반으로 나눠진
        //값에 따라 x y 좌표에 자식의 너비 or 폭을 더해줌
        Vector2Int newPos = this.BL;
        if (num == 0)
        {
            newPos.x += cWidth;
        }
        else
        {
            newPos.y += cHeight;
        }
        //그리고 오른쪽 노드에 전달
        RightNode = new Node(cWidth, cHeight, newPos);

        return true;
    }

    //방생성
    public void GenerateRoom()
    {
        //자식이 없는노드인 경우
        if (LeftNode == null && RightNode == null)
        {
            //방의 너비와 폭을 정함 위에서 만들어진 사각형 크기의 50%~ 90%
            RoomWidth = Random.Range(Width / 2, (int)(Width * 0.9f));
            RoomHeight = Random.Range(Height / 2, (int)(Height * 0.9f));

            //만들어진 방의 BL좌표를 정하는 과정 
            int x = BL.x + Random.Range(0, Width - RoomWidth);
            int y = BL.y + Random.Range(0, Height - RoomHeight);
            //나온 x y 값을 저장
            roomBL = new Vector2(x,y);
            //방의 중앙 좌표를 정하는 공식
            Center = new Vector2(x + RoomWidth / 2, y + RoomHeight / 2);
        }
        else //자식이 있는 노드의 경우(방이 없는 구역임)
        {
            // 나는 방이 없으므로, 나중에 내 부모(할아버지)가 나와 길을 연결하려 할 때
            // 연결할 좌표가 필요함.
            // 따라서 내 자식 중 하나(왼쪽)의 좌표를 나의 대표 좌표로 삼음.
            Center = LeftNode.Center;
        }

    }
}
