using GameEngine;
using GameEngine.Cards;
using GameEngine.Players;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ArtificialIntelligence.MonteCarloSearchTree
{
    public class MCTS
    {
        private const double epsilon = 1e-6;
        private int numberOfIterations;
        private int numberOfexpandNodes;
        private static Random random = new Random();

        private Node root;

        public MCTS(GamePosition startPosition, int numberOfIterations, int expandNodes)
        {
            this.root = new Node(null, startPosition, null);
            this.numberOfIterations = numberOfIterations;
            this.numberOfexpandNodes = expandNodes;
        }

        public PlayerMove MTCSAlgorithm()
        {
            List<Node> selectedNodes = new List<Node>();
            double reward;
            int i = 0;

            while (true)
            {
                selectedNodes = this.Selection(root);

                this.Expansion(selectedNodes.Last());

                Node selectionNode;

                if (selectedNodes.Last().Children.Count > 0)
                {
                    selectionNode = selectedNodes.Last().Children[random.Next(selectedNodes.Last().Children.Count)];
                    selectedNodes.Add(selectionNode);
                }
                else
                    selectionNode = selectedNodes.Last();

                reward = this.Simulatation(selectionNode);

                foreach (Node node in selectedNodes)
                    this.BackPropagation(node, reward);

                selectedNodes.Clear();

                if (++i >= numberOfIterations)
                    break;
            }

            return this.GetBestMove();
        }

        private List<Node> Selection(Node root)
        {
            List<Node> selectedNodes = new List<Node>();
            Node current = root;
            selectedNodes.Add(current);
            while (!current.IsLeaf)
            {
                current = BestChild(current);
                selectedNodes.Add(current);
            }
            return selectedNodes;
        }

        private void Expansion(Node node)
        {
            if (!(node.Position.MyPoints >= 66 || node.Position.OpponentPoints >= 66))
            {
                List<PlayerMove> moves = node.Position.GetPossibleMoves();
                foreach (PlayerMove move in moves)
                {
                    foreach (var item in node.Position.CreateLeafs(move).OrderBy(x => random.Next()).Take(numberOfexpandNodes))
                        node.AddChild(move, item);
                }
            }
        }

        private double Simulatation(Node node)
        {
            List<Card> randomCardsForOpponent = new List<Card>();
            List<Card> randomCardsForDeck = new List<Card>();

            PlayerPosition firstToPlay;

            if (node.Position.ImFirst)
            {
                firstToPlay = PlayerPosition.FirstPlayer;
                randomCardsForOpponent = node.Position.PossibleOpponentCards.OrderBy(
                   x => random.Next()).Take(6).ToList();
            }
            else
            {
                firstToPlay = PlayerPosition.SecondPlayer;
                randomCardsForOpponent = node.Position.PossibleOpponentCards.OrderBy(
                    x => random.Next()).Take(5).ToList();
            }

            randomCardsForDeck = node.Position.PossibleOpponentCards.Except(randomCardsForOpponent).ToList();
            randomCardsForDeck.Add(node.Position.TrumpCard);

            RandomMovesPlayer player = new RandomMovesPlayer(node.Position.MyCards);
            RandomMovesPlayer opponent = new RandomMovesPlayer(randomCardsForOpponent, node.Position.PlayedCard, node.Position.PlayedCardAnnounce);

            CustomRound customRound = new CustomRound(
                player, opponent, firstToPlay, node.Position.MyPoints, node.Position.OpponentPoints,
                node.Position.MyCards, randomCardsForOpponent, randomCardsForDeck, node.Position.TrumpCard,
                node.Position.State, node.Position.ClosedByPlayer);

            return customRound.SimulateToTheEndOfRound();
        }

        private void BackPropagation(Node node, double reward)
        {
            node.Visits++;
            node.Reward += reward;
        }

        private Node BestChild(Node node)
        {
            Node selected = null;
            double bestValue = Double.MinValue;
            double uctValue = 0.0;

            PlayerMove selectedMove = null;

            foreach (var child in node.Children)
            {
                if (child.Visits == 0)
                    return child;
            }

            //for Tuple, item1 reward, item2 visits, item3 parentVisits
            Dictionary<PlayerMove, Tuple<double, int, int>> moves = new Dictionary<PlayerMove, Tuple<double, int, int>>();

            foreach (var item in node.Children)
            {
                if (!moves.ContainsKey(item.TransitionMove))
                    moves.Add(item.TransitionMove, new Tuple<double, int, int>(item.Reward, item.Visits, item.ParentVisits));
                else
                    moves[item.TransitionMove] =
                        new Tuple<double, int, int>(
                            item.Reward + moves[item.TransitionMove].Item1,
                            item.Visits + moves[item.TransitionMove].Item2,
                            item.ParentVisits + moves[item.TransitionMove].Item3);
            }

            foreach (var item in moves.Keys)
            {
                uctValue = (double)(moves[item].Item1 / moves[item].Item2 + epsilon) +
                    Math.Sqrt(Math.Log(moves[item].Item3) / (moves[item].Item2 + epsilon)) +
                    random.NextDouble() * epsilon;

                if (uctValue > bestValue)
                {
                    selectedMove = item;
                    bestValue = uctValue;
                }
            }

            selected = node.Children.Where(x => x.TransitionMove == selectedMove).OrderBy(y => random.Next()).First();
            return selected;
        }

        private PlayerMove GetBestMove()
        {
            //for Tuple, item1 reward, item2 visits
            Dictionary<PlayerMove, Tuple<double, int>> moves = new Dictionary<PlayerMove, Tuple<double, int>>();

            foreach (var item in this.root.Children)
            {
                if (!moves.ContainsKey(item.TransitionMove))
                    moves.Add(item.TransitionMove, new Tuple<double, int>(item.Reward, item.Visits));
                else
                    moves[item.TransitionMove] =
                        new Tuple<double, int>(
                            item.Reward + moves[item.TransitionMove].Item1, item.Visits + moves[item.TransitionMove].Item2);
            }

            return moves.OrderByDescending(x => (double)(x.Value.Item1 / x.Value.Item2)).First().Key;
        }
    }
}
