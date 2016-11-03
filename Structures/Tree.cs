using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fabio.SharpTools.Structures
{
    public sealed class Tree<T> : TreeNode<T>
    {
        public Tree(T RootValue) : base(RootValue) { }
    }

    public sealed class TreeNodeList<T> : List<TreeNode<T>>
    {
        public TreeNode<T> Parent;

        public TreeNodeList(TreeNode<T> Parent)
        {
            this.Parent = Parent;
        }

        public new TreeNode<T> Add(TreeNode<T> Node)
        {
            base.Add(Node);
            Node.Parent = Parent;
            return Node;
        }

        public TreeNode<T> Add(T Value)
        {
            return Add(new TreeNode<T>(Value));
        }

        public override string ToString()
        {
            return "Count=" + Count.ToString();
        }

    }

    public enum TreeTraversalType
    {
        TopDown,
        BottomUp
    }

    public class TreeNode<T> : IDisposable
    {
        private TreeNode<T> _Parent;
        private TreeNodeList<T> _Children;
        private T _Value;

        private bool _IsDisposed;
        private TreeTraversalType _DisposeTraversal = TreeTraversalType.BottomUp;

        public event EventHandler Disposing;

        public TreeNode(T Value)
        {

            this.Value = Value;
            Parent = null;
            Children = new TreeNodeList<T>(this);

        }

        public TreeNode(T Value, TreeNode<T> Parent)
        {

            this.Value = Value;
            this.Parent = Parent;
            Children = new TreeNodeList<T>(this);

        }

        public TreeNode<T> Parent
        {

            get { return _Parent; }

            set
            {
                if (value == _Parent)
                    return;

                if (_Parent != null)
                    _Parent.Children.Remove(this);

                if (value != null && !value.Children.Contains(this))
                    value.Children.Add(this);

                _Parent = value;
            }

        }

        public TreeNode<T> Root
        {

            get
            {
                TreeNode<T> node = this;

                while (node.Parent != null)
                    node = node.Parent;

                return node;

            }

        }

        public TreeNodeList<T> Children
        {
            get { return _Children; }
            private set { _Children = value; }

        }

        public T Value
        {

            get { return _Value; }

            set
            {
                _Value = value;
            }

        }

        public int Depth
        {

            get
            {
                int depth = 0;
                TreeNode<T> node = this;
                while (node.Parent != null)
                {
                    node = node.Parent;
                    depth++;
                }

                return depth;

            }

        }

        public bool IsDisposed
        {
            get { return _IsDisposed; }

        }

        public TreeTraversalType DisposeTraversal
        {

            get { return _DisposeTraversal; }

            set { _DisposeTraversal = value; }

        }

        public void Dispose()
        {

            CheckDisposed();
            OnDisposing();

            if (Value is IDisposable)
            {

                if (DisposeTraversal == TreeTraversalType.BottomUp)
                {
                    foreach (TreeNode<T> node in Children)
                        node.Dispose();

                }

                (Value as IDisposable).Dispose();

                if (DisposeTraversal == TreeTraversalType.TopDown)
                {

                    foreach (TreeNode<T> node in Children)
                        node.Dispose();

                }

            }
            _IsDisposed = true;

        }

        protected void OnDisposing()
        {

            if (Disposing != null)
                Disposing(this, EventArgs.Empty);

        }

        public void CheckDisposed()
        {

            if (IsDisposed)
                throw new ObjectDisposedException(GetType().Name);

        }

        public override string ToString()
        {

            string Description = string.Empty;

            if (Value != null)
                Description = "[" + Value.ToString() + "] ";

            return Description + "Depth=" + Depth.ToString() + ", Children="
              + Children.Count.ToString();

        }

    }

}
