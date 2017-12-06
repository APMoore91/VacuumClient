namespace IntelligentVacuum.Agent
{
    using System;
    using System.Collections.Generic;
    using Environments;

    public class GraphNode
    {
        public Room room {get;set;}
        public GraphNode parent {get;set;}
        public AgentAction action {get;set;}

        public GraphNode(Room room, GraphNode parent, AgentAction action)
        {
            this.room = room;
            this.parent = parent;
            this.action = action;
        }
    }
}