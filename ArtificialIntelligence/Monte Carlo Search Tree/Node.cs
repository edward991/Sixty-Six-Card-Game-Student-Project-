using GameEngine.Players;
using System.Collections.Generic;

namespace ArtificialIntelligence.MonteCarloSearchTree
{
    public class Node
    {
        private Node parent;

        public int Visits { get; set; }
        public double Reward { get; set; }

        public GamePosition Position { get; private set; }

        public PlayerMove TransitionMove { get; private set; }
        public List<Node> Children { get; private set; }

        public Node(PlayerMove transitionMove, GamePosition position, Node parent)
        {
            this.Children = new List<Node>();
            this.Visits = 0;
            this.Reward = 0.0;
            this.Position = position;
            this.TransitionMove = transitionMove;
            this.parent = parent;
        }

        public bool IsLeaf
        {
            get
            {
                if (this.Children.Count == 0)
                    return true;
                else
                    return false;
            }
        }

        public int ParentVisits
        {
            get
            {
                if (this.parent == null)
                    return 0;
                else
                    return this.parent.Visits;
            }
        }

        public void AddChild(PlayerMove transitionMove, GamePosition newPosition)
        {
            this.Children.Add(new Node(transitionMove, newPosition, this));
        }

    }
}
