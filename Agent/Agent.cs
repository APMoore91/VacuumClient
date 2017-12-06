namespace IntelligentVacuum.Agent
{
    using System;
    using System.Collections.Generic;
    using Environments;

    public class Agent
    {
        private Sensor _sensor;
        private Stack<AgentAction> _plan;

        public Agent(Sensor sensor)
        {
            this._sensor = sensor;
            this._plan = new Stack<AgentAction>();
        }

        public AgentAction DecideAction(Room room)
        {
            AgentAction action;

            if(!this._plan.TryPop(out action))
            {
                BuildPlan(room);
                if(!this._plan.TryPop(out action))
                {
                    action = AgentAction.NONE;
                }
            }

            return action;
        }

        public void BuildPlan(Room room)
        {
            var explored = new HashSet<Room>();
            var frontier = new Queue<GraphNode>();
            var node = new GraphNode(room, null, AgentAction.NONE);
            frontier.Enqueue(node);

            do
            {
                if(!frontier.TryDequeue(out node))
                {
                    return;
                }

                List<GraphNode> newNodes = Explore(node);

                explored.Add(node.room);
                newNodes.RemoveAll(n => explored.Contains(n.room));

                foreach(var newNode in newNodes)
                {
                    frontier.Enqueue(newNode);
                }
            }while(!node.room.IsDirty);

            _plan.Clear();
            _plan.Push(AgentAction.CLEAN);
            while(node.parent != null)
            {
                _plan.Push(node.action);
                node = node.parent;
            }
        }

        private List<GraphNode> Explore(GraphNode node)
        {
            List<GraphNode> discovered = new List<GraphNode>();
            AgentAction[] moveActions = new AgentAction[] {
                AgentAction.MOVE_DOWN, AgentAction.MOVE_LEFT, AgentAction.MOVE_RIGHT, AgentAction.MOVE_UP
                };
            foreach(var action in moveActions)
            {
                GraphNode newNode = Transition(node, action);
                if(newNode != null)
                {
                    discovered.Add(newNode);
                }
            }
            return discovered;
        }

        private GraphNode Transition(GraphNode node, AgentAction action)
        {
            Room newRoom = null;
            Room currentRoom = node.room;

            switch(action)
            {
                case AgentAction.MOVE_DOWN:
                    newRoom = _sensor.SenseRoom(currentRoom.XAxis, currentRoom.YAxis + 1);
                    break;
                case AgentAction.MOVE_UP:
                    newRoom = _sensor.SenseRoom(currentRoom.XAxis, currentRoom.YAxis - 1);
                    break;
                case AgentAction.MOVE_LEFT:
                    newRoom = _sensor.SenseRoom(currentRoom.XAxis - 1, currentRoom.YAxis);
                    break;
                case AgentAction.MOVE_RIGHT:
                    newRoom = _sensor.SenseRoom(currentRoom.XAxis + 1, currentRoom.YAxis);
                    break;
            }

            GraphNode newNode = null;
            if(newRoom != null)
            {
                if(!newRoom.IsLocked){
                    newNode = new GraphNode(newRoom, node, action);
                }
            }
            return newNode;
        }
    }
}