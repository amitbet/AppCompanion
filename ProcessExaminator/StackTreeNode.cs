using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProcessExaminator
{
    [Serializable]
    public class StackTreeNode
    {
        string Line { get; set; }
        List<StackTreeNode> m_children = new List<StackTreeNode>();
        public int StackAppearancesCounter { get; set; }
        public List<StackTreeNode> Children
        {
            get { return m_children; }
            set { m_children = value; }
        }

        public override string ToString()
        {
            return "(" + StackAppearancesCounter + ")" + Line;
        }
        /// <summary>
        /// adds the 
        /// </summary>
        /// <param name="parentLine"></param>
        /// <param name="childLine"></param>
        /// <returns></returns>
        public StackTreeNode AddToSubTree(string parentLine, string childLine)
        {
            if (parentLine == null)
            {
                this.Line = childLine;
                return this;
            }

            if (this.Line == parentLine)
            {
                var existingChild = Children.Where(c => c.Line == childLine).FirstOrDefault();
                if (existingChild == null)
                {
                    var node = new StackTreeNode { Line = childLine };
                    Children.Add(node);
                    existingChild = node;
                }
                return existingChild;
            }

            foreach (StackTreeNode childNode in Children)
            {
                return childNode.AddToSubTree(parentLine, childLine);
            }

            return null;
        }

    }
}
