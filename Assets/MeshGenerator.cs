using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

public class MeshGenerator : MonoBehaviour {

    public SquareGrid squareGrid;

    public void GenerateMesh(int[,] map, float squareSize)
    {
        squareGrid = new SquareGrid(map, squareSize);
    }

    void OnDrawGizmos()
    {
        if(squareGrid != null)
        {
            foreach(Square square in squareGrid)
            {
                foreach(ControlNode control in (IEnumerable<ControlNode>)square)
                {
                    Gizmos.color = control.active ? Color.black : Color.white;
                    Gizmos.DrawCube(control.position, Vector3.one * .4f);
                }

                Gizmos.color = Color.grey;

                foreach (Node node in (IEnumerable<Node>)square)
                {
                    Gizmos.DrawCube(node.position, Vector3.one * .15f);
                }
            }
        }
    }

    public class SquareGrid : IEnumerable<Square>
    {
        public Square[,] squares;

        public SquareGrid(int[,] map, float squareSize)
        {
            int nodeCountX = map.GetLength(0);
            int nodeCountY = map.GetLength(1);
            float mapWidth = nodeCountX * squareSize;
            float mapHeight = nodeCountY * squareSize;

            ControlNode[,] controlNodes = new ControlNode[nodeCountX, nodeCountY];

            for(int x = 0; x < nodeCountX; x++)
            {
                for(int y = 0; y < nodeCountY; y++)
                {
                    Vector3 pos = new Vector3(-mapWidth / 2f + x * squareSize + squareSize / 2f, 0, -mapHeight / 2f + y / squareSize + squareSize / 2f);
                    controlNodes[x, y] = new ControlNode(pos, map[x, y] == 1, squareSize);
                }
            }

            squares = new Square[nodeCountX-1, nodeCountY-1];
            for (int x = 0; x < nodeCountX-1; x++)
            {
                for (int y = 0; y < nodeCountY-1; y++)
                {
                    squares[x, y] = new Square(controlNodes[x, y + 1], controlNodes[x + 1, y + 1], controlNodes[x + 1, y], controlNodes[x, y]);
                }
            }
        }

        private IEnumerable<Square> Squares()
        {
            for(int x = 0; x < squares.GetLength(0); x++)
            {
                for(int y = 0; y < squares.GetLength(1); y++)
                {
                    yield return squares[x, y];
                }
            }
        }

        public IEnumerator<Square> GetEnumerator()
        {
            return Squares().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Squares().GetEnumerator();
        }
    }

    public class Square : IEnumerable<ControlNode>, IEnumerable<Node>
    {
        public ControlNode topLeft, topRight, bottomRight, bottomLeft;
        public Node centerTop, centerRight, centerBottom, centerLeft;

        public Square(ControlNode _topLeft, ControlNode _topRight, ControlNode _bottomRight, ControlNode _bottomLeft)
        {
            topLeft = _topLeft;
            topRight = _topRight;
            bottomRight = _bottomRight;
            bottomLeft = _bottomLeft;

            centerTop = _topLeft.rightNode;
            centerRight = _bottomRight.aboveNode;
            centerBottom = _bottomLeft.rightNode;
            centerLeft = bottomLeft.aboveNode;
        }

        private IEnumerable<ControlNode> ControlNodes()
        {
            yield return topLeft;
            yield return topRight;
            yield return bottomRight;
            yield return bottomLeft;
        }

        private IEnumerable<Node> Nodes()
        {
            yield return centerTop;
            yield return centerRight;
            yield return centerBottom;
            yield return centerLeft;
        }

        public IEnumerator<ControlNode> GetEnumerator()
        {
            return ControlNodes().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ControlNodes().GetEnumerator();
        }

        IEnumerator<Node> IEnumerable<Node>.GetEnumerator()
        {
            return Nodes().GetEnumerator();
        }
    }

	public class Node
    {
        public Vector3 position;
        public int vertexIndex = -1;

        public Node(Vector3 pos)
        {
            position = pos;
        }
    }

    public class ControlNode : Node
    {
        public bool active;
        public Node aboveNode;
        public Node rightNode;

        public ControlNode(Vector3 _pos, bool _active, float _squareSize) : base(_pos)
        {
            active = _active;
            aboveNode = new Node(position + Vector3.forward * _squareSize / 2f);
            rightNode = new Node(position + Vector3.right * _squareSize / 2f);

            //Debug.Log(string.Format("Pos: [{0},{1},{2}], Above: [{3},{4},{5}], Right: [{6},{7},{8}]", _pos.x, _pos.y, _pos.z, aboveNode.position.x, aboveNode.position.y, aboveNode.position.z, rightNode.position.x, rightNode.position.y, rightNode.position.z));
        }
    }
}
