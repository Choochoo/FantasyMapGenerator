using System;
using System.Collections.Generic;
using System.Drawing;

namespace Delaunay
{

    /**
    *  Kruskal's spanning tree algorithm with union-find
     * Skiena: The Algorithm Design Manual, p. 196ff
     * Note: the sites are implied: they consist of the end PointFs of the line segments
    */
    public class Kruskal
    {
        public static List<LineSegment> GetKruskal(List<LineSegment> lineSegments)
        {
            return GetKruskal(lineSegments, "minimum");
        }
        public static List<LineSegment> GetKruskal(List<LineSegment> lineSegments, string type)
        {
            Dictionary<PointF, Node> nodes = new Dictionary<PointF, Node>();
            List<LineSegment> mst = new List<LineSegment>();
            List<Node> nodePool = Node.pool;

            switch (type)
            {
                // note that the compare voids are the reverse of what you'd expect
                // because (see below) we traverse the lineSegments in reverse order for speed
                case "maximum":
                    lineSegments.SortFunc(LineSegment.CompareLengths);
                    break;
                default:
                    lineSegments.SortFunc(LineSegment.CompareLengthsMax);
                    break;
            }

            for (int i = lineSegments.Count; --i > -1; )
            {
                LineSegment lineSegment = lineSegments[i];

                Node node0 = nodes[lineSegment.p0];
                Node rootOfSet0;
                if (node0 == null)
                {
                    node0 = nodePool.Count > 0 ? nodePool.Pop() : new Node();
                    // intialize the node:
                    rootOfSet0 = node0.parent = node0;
                    node0.treeSize = 1;

                    nodes[lineSegment.p0] = node0;
                }
                else
                {
                    rootOfSet0 = Find(node0);
                }

                Node node1 = nodes[lineSegment.p1];
                Node rootOfSet1;
                if (node1 == null)
                {
                    node1 = nodePool.Count > 0 ? nodePool.Pop() : new Node();
                    // intialize the node:
                    rootOfSet1 = node1.parent = node1;
                    node1.treeSize = 1;

                    nodes[lineSegment.p1] = node1;
                }
                else
                {
                    rootOfSet1 = Find(node1);
                }

                if (rootOfSet0 != rootOfSet1)	// nodes not in same set
                {
                    mst.Add(lineSegment);

                    // merge the two sets:
                    int treeSize0 = rootOfSet0.treeSize;
                    int treeSize1 = rootOfSet1.treeSize;
                    if (treeSize0 >= treeSize1)
                    {
                        // set0 absorbs set1:
                        rootOfSet1.parent = rootOfSet0;
                        rootOfSet0.treeSize += treeSize1;
                    }
                    else
                    {
                        // set1 absorbs set0:
                        rootOfSet0.parent = rootOfSet1;
                        rootOfSet1.treeSize += treeSize0;
                    }
                }
            }
            throw new NotImplementedException("This was modified, may not work anymore");
            foreach (KeyValuePair<PointF, Node> node in nodes)
            {
                nodePool.Add(node.Value);
            }

            return mst;
        }

        static Node Find(Node node)
        {
            if (node.parent == node)
            {
                return node;
            }
            else
            {
                Node root = Find(node.parent);
                // this line is just to speed up subsequent finds by keeping the tree depth low:
                node.parent = root;
                return root;
            }
        }
    }

    class Node
    {
        public static List<Node> pool = new List<Node>();

        public Node parent;
        public int treeSize;

        public Node() { }
    }
}