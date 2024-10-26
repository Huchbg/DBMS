using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBMS.Structures
{
    public class Node
    {
        private string Value = "";
        private Node? Left;
        private Node? Right;

        public Node(string value,Node left,Node right) {
            Value = value;
            Left = left;
            Right = right;
        }
        public Node(string value)
        {
            Value = value;
            Left = null;
            Right = null;
        }


        public Node(string value, Node left)
        {
            Value = value;
            Left= left;
            Right=null;
        }

        public Node? GetLeft() { return Left; }
        public Node? GetRight() { return Right;}
        public void SetRight(Node? right) { Right = right; }
        public void SetLeft(Node? left) {  Left = left; }
        public void SetValue(string value) {  Value = value; }
        public string GetValue() { return Value; }

        public override string ToString()
        {
            return $"{Value} {Left.Value} {Right.Value}";
        }
    }
}
