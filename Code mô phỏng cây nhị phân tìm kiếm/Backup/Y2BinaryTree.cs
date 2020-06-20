/* ====================================
 * http://yinyangit.wordpress.com
 * Lasted Update: 15 Jan 2011
 * ====================================
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Y2Collections
{
    class Y2BinaryTreeNode<T> : IComparable
       where T : IComparable
    {

        #region Properties

        public T Value { get; set; }

        public Y2BinaryTreeNode<T> LeftChild { get; set; }
        public Y2BinaryTreeNode<T> RightChild { get; set; }
        public Y2BinaryTreeNode<T> Parent { get; set; }

        public bool IsLeaf
        {
            get { return LeftChild == null && RightChild == null; }
        }
        public bool HasLeftChild
        {
            get { return LeftChild != null; }
        }
        public bool HasRightChild
        {
            get { return RightChild != null; }
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }
        #endregion

        public Y2BinaryTreeNode(T value)
        {
            this.Value = value;
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            Y2BinaryTreeNode<T> node = obj as Y2BinaryTreeNode<T>;

            return this.Value.CompareTo(node.Value);
        }

        #endregion

        public static bool operator <(Y2BinaryTreeNode<T> node1, Y2BinaryTreeNode<T> node2)
        {
            return node1.Value.CompareTo(node2.Value) < 0;
        }
        public static bool operator >(Y2BinaryTreeNode<T> node1, Y2BinaryTreeNode<T> node2)
        {
            return node1.Value.CompareTo(node2.Value) > 0;
        }
    }

    class Y2BinaryTree<T>
        where T : IComparable
    {
        #region Properties

        public Y2BinaryTreeNode<T> Root { get; set; }
        private List<T> _list;
        public int Count { get; private set; }

        #endregion

        public Y2BinaryTree()
        {
            Count = 0;
            _list = new List<T>();
        }

        public Y2BinaryTree(T value)
            : this()
        {
            Add(value);
        }

        #region InOrder Traversal
        public virtual List<T> InOrderTraverse()
        {
            _list.Clear();
            InOrderTraverse(Root);
            return _list;
        }
        private void InOrderTraverse(Y2BinaryTreeNode<T> node)
        {
            if (node == null)
                return;
            InOrderTraverse(node.LeftChild);
            _list.Add(node.Value);
            InOrderTraverse(node.RightChild);
        }
        #endregion

        #region PreOrder Traversal
        public virtual List<T> PreOrderTraverse()
        {
            _list.Clear();
            PreOrderTraverse(Root);
            return _list;
        }
        private void PreOrderTraverse(Y2BinaryTreeNode<T> node)
        {
            if (node == null)
                return;
            _list.Add(node.Value);
            PreOrderTraverse(node.LeftChild);
            PreOrderTraverse(node.RightChild);
        }
        #endregion

        #region PostOrder Traversal
        public virtual List<T> PostOrderTraverse()
        {
            _list.Clear();
            PostOrderTraverse(Root);
            return _list;
        }
        private void PostOrderTraverse(Y2BinaryTreeNode<T> node)
        {
            if (node == null)
                return;
            PostOrderTraverse(node.LeftChild);
            PostOrderTraverse(node.RightChild);
            _list.Add(node.Value);
        }
        #endregion

        #region Add Node
        public virtual void Add(params T[] values)
        {
            Array.ForEach(values, value => Add(value));
        }
        public virtual bool Add(T value)
        {
            Y2BinaryTreeNode<T> node = new Y2BinaryTreeNode<T>(value);
            if (Root == null)
            {
                Count++;
                Root = node;
                return true;
            }

            return Add(Root, node);
        }
        private bool Add(Y2BinaryTreeNode<T> parentNode, Y2BinaryTreeNode<T> node)
        {
            if (parentNode.Value.Equals(node.Value))
                return false;

            if (parentNode > node)
            {
                if (!parentNode.HasLeftChild)
                {
                    parentNode.LeftChild = node;
                    node.Parent = parentNode;
                    Count++;
                    return true;
                }
                else
                    return Add(parentNode.LeftChild, node);
            }
            else
            {
                if (!parentNode.HasRightChild)
                {
                    parentNode.RightChild = node;
                    node.Parent = parentNode;
                    Count++;
                    return true;
                }
                else
                    return Add(parentNode.RightChild, node);
            }
        }
        #endregion

        public virtual void ClearChildren(Y2BinaryTreeNode<T> node)
        {

            if (node.HasLeftChild)
            {
                ClearChildren(node.LeftChild);
                node.LeftChild.Parent = null;
                node.LeftChild = null;
            }
            if (node.HasRightChild)
            {
                ClearChildren(node.RightChild);
                node.RightChild.Parent = null;
                node.RightChild = null;
            }
        }

        public virtual void Clear()
        {
            if (Root == null)
                return;
            ClearChildren(Root);
            Root = null;
            Count = 0;
        }

        public virtual int GetHeight()
        {
            return this.GetHeight(this.Root);
        }
        private int GetHeight(Y2BinaryTreeNode<T> startNode)
        {
            if (startNode == null)
                return 0;
            else
                return 1 + Math.Max(GetHeight(startNode.LeftChild), GetHeight(startNode.RightChild));
        }
        public virtual Y2BinaryTreeNode<T> Search(T value)
        {
            return Search(Root, value);
        }
        public virtual Y2BinaryTreeNode<T> Search(Y2BinaryTreeNode<T> node, T value)
        {
            if (node == null)
                return null;

            if (node.Value.Equals(value))
                return node;
            else
            {
                if (node.Value.CompareTo(value) > 0)
                    return Search(node.LeftChild, value);
                else
                    return Search(node.RightChild, value);
            }
        }
        public virtual Queue<T> FindPath(T value)
        {
            Queue<T> q = new Queue<T>();

            Y2BinaryTreeNode<T> node = this.Root;
            bool isFounded = false;

            while (node != null)
            {
                if (node.Value.Equals(value))
                {
                    isFounded = true;
                    break;
                }
                else
                {
                    if (node.Value.CompareTo(value) > 0)
                        node = node.LeftChild;
                    else
                        node = node.RightChild;

                    if (node != null) q.Enqueue(node.Value);
                }
            }
            if (!isFounded)
            {
                q.Clear();
                q = null;
            }

            return q;
        }
        public virtual bool Remove(T value)
        {
            return Remove(Root, value);
        }

        private bool Remove(Y2BinaryTreeNode<T> node, T value)
        {
            if (node == null)
                return false;

            if (node.Value.Equals(value))
            {
                if (node.IsLeaf) // no children
                {
                    if (node.Parent.LeftChild == node)
                        node.Parent.LeftChild = null;
                    else
                        node.Parent.RightChild = null;

                    node.Parent = null;
                }
                else if (node.HasLeftChild && node.HasRightChild)   // 2 children
                {
                    // Tìm successor node
                    Y2BinaryTreeNode<T> replacementNode = node.RightChild;

                    while (replacementNode.HasLeftChild)
                    {
                        replacementNode = replacementNode.LeftChild;
                    }
                    node.Value = replacementNode.Value;

                    Remove(replacementNode, replacementNode.Value);
                }
                else    // one child
                {
                    Y2BinaryTreeNode<T> subNode;

                    if (node.HasLeftChild)
                        subNode = node.LeftChild;
                    else
                        subNode = node.RightChild;

                    if (Root == (subNode))
                        Root = subNode;

                    subNode.Parent = node.Parent;

                    if (node.Parent.LeftChild == node)
                        node.Parent.LeftChild = subNode;
                    else
                        node.Parent.RightChild = subNode;
                }
                Count--;
                return true;
            }
            else
            {
                if (node.Value.CompareTo(value) > 0)
                    return Remove(node.LeftChild, value);
                else
                    return Remove(node.RightChild, value);
            }
        }


    }


}
