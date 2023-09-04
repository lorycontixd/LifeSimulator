using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace GOAP
{
    public class Node
    {

        public Node parent;
        public float cost;
        public Dictionary<string, object> state; // State stored in the node
        public GAction action; // Action that the node is representing

        public Node(Node parent, float cost, Dictionary<string, object> allStates, GAction action)
        {

            this.parent = parent;
            this.cost = cost;
            this.state = new Dictionary<string, object>(allStates);
            this.action = action;
        }

        public Node(Node parent, float cost, Dictionary<string, object> allStates, Dictionary<string, object> beliefStates, GAction action)
        {

            this.parent = parent;
            this.cost = cost;
            this.state = new Dictionary<string, object>(allStates);
            foreach (KeyValuePair<string, object> b in beliefStates)
            {
                if (!this.state.ContainsKey(b.Key))
                {
                    this.state.Add(b.Key, b.Value);
                }
            }

            this.action = action;
        }
    }


    public class GPlanner
    {
        private bool debugMode;
        public GPlanner(bool debugMode = false)
        {
            this.debugMode = debugMode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actions">List of all possible actions</param>
        /// <param name="goal">Desired goal to be reached with a plan.</param>
        /// <param name="beliefStates">Player belief states</param>
        /// <returns></returns>
        public Queue<GAction> Plan(List<GAction> actions, Dictionary<string, int> goal, WorldStates beliefStates)
        {
            // First create a list of available actions (achievable)
            List<GAction> usableActions = new List<GAction>();
            foreach (GAction a in actions)
            {
                if (a.IsAchievable())
                {
                    usableActions.Add(a);
                }
            }

            List<Node> leaves = new List<Node>();
            Node start = new Node(null, 0.0f, GWorld.Instance.GetWorld().GetStates(), beliefStates.GetStates(), null);

            bool success = BuildGraph(start, leaves, usableActions, goal);

            if (!success)
            {
                if (debugMode)
                    Debug.Log("NO PLAN");
                return null;
            }


            // Build leaves string
            if (debugMode)
            {
                string s = $"[GPlanner->Plan] Build graph completed. Leaves: ";
                foreach(Node leafNode in leaves)
                {
                    s += $"({leafNode.action.actionName}, {leafNode.cost}, {leafNode.state}), ";
                }
                s.Remove(s.Length - 1);
                s.Remove(s.Length - 1);
                Debug.Log(s);
            }

            // At this point there is a plan found, and we must find the cheapest node from all leaves.
            // (Cheapest node = cheapest path that took to that node)
            Node cheapest = null; // Cheapest leaf
            foreach (Node leaf in leaves)
            {
                if (cheapest == null)
                {
                    cheapest = leaf;
                }
                else
                {
                    if (leaf.cost < cheapest.cost)
                    {
                        cheapest = leaf;
                    }
                }
            }

            // Found the cheapest path. Must work backwards to find the path
            List<GAction> result = new List<GAction>();
            Node n = cheapest;

            while (n != null)
            {
                if (n.action != null)
                {
                    result.Insert(0, n.action);
                }
                n = n.parent;
            }

            // Transform the list of actions into a performable queue.
            Queue<GAction> queue = new Queue<GAction>();
            foreach (GAction a in result)
            {
                queue.Enqueue(a);
            }

            if (debugMode)
            {
                Debug.Log("The Plan is: ");
                foreach (GAction a in queue)
                {

                    Debug.Log("Q: " + a.actionName);
                }
            }
            
            return queue;
        }


        /// <summary>
        /// Build a graph of all possible paths from a given parent node towards a given goal (string, int).
        /// The graph is built through recursion, and the stop condnition is when no more actions are available or a path has been found.
        /// </summary>
        /// <param name="parent">Parent node</param>
        /// <param name="leaves">List of leaves</param>
        /// <param name="usableActions">Usable actions</param>
        /// <param name="goal">Desired goal to be reached</param>
        /// <returns></returns>
        bool BuildGraph(Node parent, List<Node> leaves, List<GAction> usableActions, Dictionary<string, int> goal, int level = 0)
        {
            // At first call, parent node is node with world states + belief states and no associated action.
            bool foundPath = false;

            
            foreach (GAction action in usableActions)
            {
                if (debugMode)
                {
                    Debug.Log($"[GPlanner->BG->1] goal: {goal.First().Key}, action: {action.actionName}, isachievable: {action.IsAchievableGiven(parent.state)}, level: {level}");
                }
                if (action.IsAchievableGiven(parent.state))
                {
                    Dictionary<string, object> currentState = new Dictionary<string, object>(parent.state ); // Set of parent states, first time it has all world states
                    // We are adding to the current state (which has all states) also the effects of the current action.
                    foreach (KeyValuePair<string, object> eff in action.effects)
                    {
                        if (!currentState.ContainsKey(eff.Key))
                        {
                            currentState.Add(eff.Key, eff.Value);
                        }
                    }
                    // So at this point, currentState has --> world states + belief states + action effects (for all usable actions)
                    Node node = new Node(parent, parent.cost + action.cost, currentState, action);
                    bool isachieved = GoalAchieved(goal, currentState);
                    if (debugMode)
                    {
                        Debug.Log($"[GPlanner->BG->2] isgoalachieved => goal: {goal.First().Key}, currentstatecount: {currentState.Count}, isachieved: {isachieved}");
                    }
                    if (isachieved)
                    {
                        leaves.Add(node); // Leaf nodes contain goal actions
                        foundPath = true;
                    }
                    else
                    {
                        List<GAction> subset = ActionSubset(usableActions, action);
                        //List<GAction> subset = ActionSubsetConditionsEffects(usableActions, action);
                        if (debugMode)
                        {
                            string s = $"[GPlanner->BG->3] !isachieved ==> Subset actions: ";
                            foreach(var a in subset)
                            {
                                s += $"{a.actionName}, ";
                            }
                            s.Remove(s.Length - 1);
                            s.Remove(s.Length - 1);
                            Debug.Log(s);
                        }
                        bool found = BuildGraph(node, leaves, subset, goal, level + 1);
                        if (found) foundPath = true;
                    }
                }
            }

            return foundPath;
        }

        /// <summary>
        /// Pick a subset of actions equal to all usable actions except a specific action.
        /// This is to narrow down available actions during BuildGraph recursion.
        /// </summary>
        /// <param name="usableActions"></param>
        /// <param name="removeMe"></param>
        /// <returns></returns>
        private List<GAction> ActionSubset(List<GAction> usableActions, GAction removeMe)
        {
            List<GAction> subset = new List<GAction>();
            foreach (GAction a in usableActions)
            {
                if (!a.Equals(removeMe))
                {
                    subset.Add(a);
                }
            }
            return subset;
        }


        private List<GAction> ActionSubsetConditionsEffects(List<GAction> usableActions, GAction currentAction)
        {
            List<GAction> subset = new List<GAction>();
            foreach (var a in usableActions)
            {
                if (!a.Equals(currentAction))
                {
                    Debug.Log($"[GPlanner->Match] Acond.: {a.actionName}, Aeff.: {currentAction.actionName} // {a.preconditions.Count}, {currentAction.effects.Count}");
                    if (MatchConditionsEffects(a.preconditions, currentAction.effects))
                    {
                        subset.Add(a);
                    }
                }
            }
            return subset;
        }

        private bool MatchConditionsEffects(Dictionary<string, object> conditions, Dictionary<string, object> effects)
        {
            foreach(var condition in conditions)
            {
                if (!effects.ContainsKey(condition.Key))
                {
                    return false;
                }
            }
            return true;
        }



        /// <summary>
        /// Check if a goal is found in the states dictionary.
        /// This means that we are looking at a leaf node because we added the goal's effect to the states (during buildup).
        /// This is an exit condition to the BuildGraph recursion.
        /// </summary>
        /// <param name="goal">Goal to be checked. </param>
        /// <param name="state"></param>
        /// <returns></returns>
        private bool GoalAchieved(Dictionary<string, int> goal, Dictionary<string, object> state)
        {
            if (debugMode)
            {
                Debug.Log($"[GPlanner->GoalAchieved] goal: {goal.First().Key}, currentstate: {StatesToString(state)}");
            }
            foreach (KeyValuePair<string, int> g in goal)
            {
                if (!state.ContainsKey(g.Key)) return false;
            }
            // states must contain the key of the goal (because it's in the effects of the goal action)
            return true;
        }


        private string StatesToString(Dictionary<string, object> states)
        {
            string s = "States: ";
            foreach(KeyValuePair<string, object> g in states)
            {
                s += $"({g.Key},{g.Value})/";
            }
            s.Remove(s.Length - 1);
            return s;
        }
    }
}
