using System;
using System.Threading;
namespace ThreadedBinarySearchTree
{
    class Program
    {
        static void Main(string[] args)
        {
            ThreadedBinarySearchTree t = new ThreadedBinarySearchTree();
           
            Console.ReadLine();

        }
    }


    class Node// class of node
    {
        public Node LeftNode { get; set; }
        public Node RightNode { get; set; }
        public int Data { get; set; }// data inside the node , in our drill it will be integer
    }

    class ThreadedBinarySearchTree

    {
        private ReaderWriterLockSlim TreeLock = new ReaderWriterLockSlim();

        public Node Root { get; set; }
        //add num to the tree, if it already exists, do nothing
        public void add(int num)
        {
            TreeLock.EnterWriteLock();//Enter the lock
            try
            {
                Node before = null, after = this.Root;
                while (after != null)
                {
                    before = after;
                    if (num < after.Data) //Is new node in left tree? 
                        after = after.LeftNode;
                    else if (num > after.Data) //Is new node in right tree?
                        after = after.RightNode;
                    else
                    {
                        //Exist same value
                        return;
                    }
                }

                Node newNode = new Node();
                newNode.Data = num;

                if (this.Root == null)//Tree ise empty
                    this.Root = newNode;
                else
                {
                    if (num < before.Data)
                        before.LeftNode = newNode;
                    else
                        before.RightNode = newNode;
                }


            }
            finally
            {
                TreeLock.ExitWriteLock();//Exit from the lock
            }


            return;

        }

        // remove num from the tree, if it doesn't exists, do nothing
        public void remove(int num)
        {
            TreeLock.EnterWriteLock();
            try
            {
                this.Root = Delete(this.Root, num);
            }
            finally
            {
                TreeLock.ExitWriteLock();//Exit from the lock

            }
        }

        private Node Delete(Node parent, int key)
        {
            if (parent == null) return parent;

            if (key < parent.Data) parent.LeftNode = Delete(parent.LeftNode, key);
            else if (key > parent.Data)
                parent.RightNode = Delete(parent.RightNode, key);

            // if value is same as parent's value, then this is the node to be deleted  
            else
            {
                // node with only one child or no child  
                if (parent.LeftNode == null)
                    return parent.RightNode;
                else if (parent.RightNode == null)
                    return parent.LeftNode;

                // node with two children: Get the inorder successor (smallest in the right subtree)  
                parent.Data = MinValue(parent.RightNode);

                // Delete the inorder successor  
                parent.RightNode = Delete(parent.RightNode, parent.Data);
            }

            return parent;
        }
        private int MinValue(Node node)
        {
            int minv = node.Data;

            while (node.LeftNode != null)
            {
                minv = node.LeftNode.Data;
                node = node.LeftNode;
            }

            return minv;
        }

        // search num in the tree and return true/false accordingly
        public bool search(int num)
        {
            TreeLock.EnterReadLock();
            try
            {
                Node node = this.Find(num, this.Root);
                if (node == null)
                {
                    Console.WriteLine("\nFalse");
                    return false;
                }
                Console.WriteLine("\nTrue");    
                return true;
            }
            finally
            {
                TreeLock.ExitReadLock();
            }

        }
        private Node Find(int value, Node parent)
        {
            if (parent != null)
            {
                if (value == parent.Data) return parent;
                if (value < parent.Data)
                    return Find(value, parent.LeftNode);
                else
                    return Find(value, parent.RightNode);
            }

            return null;
        }

        // remove all items from tree
        public void clear() 
        {

            while (this.Root != null)
            {

                this.remove(this.Root.Data);

            }


        } 

        // print the values of three from the smallest to largest in comma delimited form. For example; -15,0,1,3,5,23,40,89,201. If the tree is empty do nothing. 
        public void print()
        {
            TreeLock.EnterReadLock();
            try
            {
                TraverseInOrder(this.Root);
            }
            finally
            {
                TreeLock.ExitReadLock();
            }

        }
        public void TraverseInOrder(Node parent)//print the element from the smallest number to the greaterst
        {
            if (parent != null)
            {
                TraverseInOrder(parent.LeftNode);
                Console.Write(parent.Data + " ");
                TraverseInOrder(parent.RightNode);
            }
        }

    }
}
