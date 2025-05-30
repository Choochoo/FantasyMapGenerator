using System.Collections.Generic;

namespace D3Voronoi
{
    public class RedBlackTree
    {
        public Point Site;
        public RedBlackTree Circle;
        public Edge Edge;
        public RedBlackTree Arc;
        public List<RedBlackTree> Collection;
        public RedBlackTree _ { get; set; }
        public RedBlackTree U { get; set; }// parent node
        public bool C { get; set; }// color - true for red, false for black
        public RedBlackTree L { get; set; } // left node
        public RedBlackTree R { get; set; }// right node
        public RedBlackTree P { get; set; }// previous node
        public RedBlackTree N { get; set; }// next node
        public double X, Y, CY;

        /// <summary>
        /// Resets the tree by clearing the root reference.
        /// </summary>
        public void Reset()
        {
            U = L = R = P = N = null;
            C = false;
        }

        /// <summary>
        /// Inserts a new node into the red-black tree after the specified node.
        /// Maintains red-black tree properties through rotations and recoloring.
        /// </summary>
        /// <param name="after">The node after which to insert the new node.</param>
        /// <param name="node">The node to insert into the tree.</param>
        public void Insert(RedBlackTree after, RedBlackTree node)
        {
            RedBlackTree parent = null;
            RedBlackTree grandpa = null;
            RedBlackTree uncle = null;

            if (after != null)
            {
                node.P = after;
                node.N = after.N;
                if (after.N != null) after.N.P = node;
                after.N = node;
                if (after.R != null)
                {
                    after = after.R;
                    while (after.L != null) after = after.L;
                    after.L = node;
                }
                else
                {
                    after.R = node;
                }
                parent = after;
            }
            else if (this._ != null)
            {
                after = RedBlackFirst(this._);
                node.P = null;
                node.N = after;
                after.P = after.L = node;
                parent = after;
            }
            else
            {
                node.P = node.N = null;
                this._ = node;
                parent = null;
            }
            node.L = node.R = null;
            node.U = parent;
            node.C = true;

            after = node;
            while (parent != null && parent.C)
            {
                grandpa = parent.U;
                if (parent == grandpa.L)
                {
                    uncle = grandpa.R;
                    if (uncle != null && uncle.C)
                    {
                        parent.C = uncle.C = false;
                        grandpa.C = true;
                        after = grandpa;
                    }
                    else
                    {
                        if (after == parent.R)
                        {
                            RedBlackRotateLeft(this, parent);
                            after = parent;
                            parent = after.U;
                        }
                        parent.C = false;
                        grandpa.C = true;
                        RedBlackRotateRight(this, grandpa);
                    }
                }
                else
                {
                    uncle = grandpa.L;
                    if (uncle != null && uncle.C)
                    {
                        parent.C = uncle.C = false;
                        grandpa.C = true;
                        after = grandpa;
                    }
                    else
                    {
                        if (after == parent.L)
                        {
                            RedBlackRotateRight(this, parent);
                            after = parent;
                            parent = after.U;
                        }
                        parent.C = false;
                        grandpa.C = true;
                        RedBlackRotateLeft(this, grandpa);
                    }
                }
                parent = after.U;
            }
            this._.C = false;
        }

        /// <summary>
        /// Removes a node from the red-black tree while maintaining tree properties.
        /// Performs necessary rotations and recoloring to preserve red-black tree invariants.
        /// </summary>
        /// <param name="node">The node to remove from the tree.</param>
        public void Remove(RedBlackTree node)
        {
            if (node.N != null) node.N.P = node.P;
            if (node.P != null) node.P.N = node.N;
            node.N = node.P = null;

            RedBlackTree parent = node.U;
            RedBlackTree sibling = null;
            RedBlackTree left = node.L;
            RedBlackTree right = node.R;
            RedBlackTree next = null;
            bool red = false;

            if (left == null) next = right;
            else if (right == null) next = left;
            else next = RedBlackFirst(right);

            if (parent != null)
            {
                if (parent.L == node) parent.L = next;
                else parent.R = next;
            }
            else
            {
                this._ = next;
            }

            if (left != null && right != null)
            {
                red = next.C;
                next.C = node.C;
                next.L = left;
                left.U = next;
                if (next != right)
                {
                    parent = next.U;
                    next.U = node.U;
                    node = next.R;
                    parent.L = node;
                    next.R = right;
                    right.U = next;
                }
                else
                {
                    next.U = parent;
                    parent = next;
                    node = next.R;
                }
            }
            else
            {
                red = node.C;
                node = next;
            }

            if (node != null) node.U = parent;
            if (red) return;
            if (node != null && node.C) { node.C = false; return; }

            do
            {
                if (node == this._) break;
                if (node == parent.L)
                {
                    sibling = parent.R;
                    if (sibling.C)
                    {
                        sibling.C = false;
                        parent.C = true;
                        RedBlackRotateLeft(this, parent);
                        sibling = parent.R;
                    }
                    if ((sibling.L != null && sibling.L.C)
                        || (sibling.R != null && sibling.R.C))
                    {
                        if (sibling.R == null || !sibling.R.C)
                        {
                            sibling.L.C = false;
                            sibling.C = true;
                            RedBlackRotateRight(this, sibling);
                            sibling = parent.R;
                        }
                        sibling.C = parent.C;
                        parent.C = sibling.R.C = false;
                        RedBlackRotateLeft(this, parent);
                        node = this._;
                        break;
                    }
                }
                else
                {
                    sibling = parent.L;
                    if (sibling.C)
                    {
                        sibling.C = false;
                        parent.C = true;
                        RedBlackRotateRight(this, parent);
                        sibling = parent.L;
                    }
                    if ((sibling.L != null && sibling.L.C)
                      || (sibling.R != null && sibling.R.C))
                    {
                        if (sibling.L == null || !sibling.L.C)
                        {
                            sibling.R.C = false;
                            sibling.C = true;
                            RedBlackRotateLeft(this, sibling);
                            sibling = parent.L;
                        }
                        sibling.C = parent.C;
                        parent.C = sibling.L.C = false;
                        RedBlackRotateRight(this, parent);
                        node = this._;
                        break;
                    }
                }
                sibling.C = true;
                node = parent;
                parent = parent.U;
            } while (!node.C);

            if (node != null) node.C = false;
        }

        /// <summary>
        /// Performs a left rotation around the specified node to maintain red-black tree properties.
        /// </summary>
        /// <param name="tree">The tree containing the node.</param>
        /// <param name="node">The node to rotate around.</param>
        public void RedBlackRotateLeft(RedBlackTree tree, RedBlackTree node)
        {
            var p = node;
            var q = node.R;
            var parent = p.U;

            if (parent != null)
            {
                if (parent.L == p) parent.L = q;
                else parent.R = q;
            }
            else
            {
                tree._ = q;
            }

            q.U = parent;
            p.U = q;
            p.R = q.L;
            if (p.R != null) p.R.U = p;
            q.L = p;
        }

        /// <summary>
        /// Performs a right rotation around the specified node to maintain red-black tree properties.
        /// </summary>
        /// <param name="tree">The tree containing the node.</param>
        /// <param name="node">The node to rotate around.</param>
        public void RedBlackRotateRight(RedBlackTree tree, RedBlackTree node)
        {
            var p = node;
            var q = node.L;
            var parent = p.U;

            if (parent != null)
            {
                if (parent.L == p) parent.L = q;
                else parent.R = q;
            }
            else
            {
                tree._ = q;
            }

            q.U = parent;
            p.U = q;
            p.L = q.R;
            if (p.L != null) p.L.U = p;
            q.R = p;
        }

        public RedBlackTree RedBlackFirst(RedBlackTree node)
        {
            while (node.L != null) node = node.L;
            return node;
        }

    }
}

