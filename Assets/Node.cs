using UnityEngine;

public class Node 
{
    public int Width;
    public int Height;
    public Vector2Int TR;
    public Vector2Int TL;
    public Vector2Int BR;
    public Vector2Int BL;

    public Node ParentNode;
    public Node LeftNode;
    public Node RightNode;

    public int RoomWidth;
    public int RoomHeight;
    public Vector2 roomBL;

    public Vector2 Center;
    public Node(int width, int height, Vector2Int bottomLeft)
    {
        Width = width;
        Height = height;

        BL = bottomLeft;
        
    }
    public bool Split()
    {
        if(LeftNode != null || RightNode != null) return false;

        int cWidth;
        int cHeight;
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

        LeftNode = new Node(cWidth, cHeight, BL);

        Vector2Int newPos = this.BL;
        if (num == 0)
        {
            newPos.x += cWidth;
        }
        else
        {
            newPos.y += cHeight;
        }
        RightNode = new Node(cWidth, cHeight, newPos);

        return true;
    }
    public void GenerateRoom()
    {
        if (LeftNode == null && RightNode == null)
        {
            RoomWidth = Random.Range(Width / 2, (int)(Width * 0.9f));
            RoomHeight = Random.Range(Height / 2, (int)(Height * 0.9f));

            int x = BL.x + Random.Range(0, Width - RoomWidth);
            int y = BL.y + Random.Range(0, Height - RoomHeight);

            roomBL = new Vector2(x,y);

            Center = new Vector2(x + RoomWidth / 2, y + RoomHeight / 2);
        }
        else
        {
            Center = LeftNode.Center;
        }

    }
}
