/*
 * A simple A* Path Finding class.
 * This class requires to be overloaded in order to work on any type of map.
 * 
 * Build 2: Saturday, April 19th, 2014
 *    - (16:25) FindPath(), EnumConnectingNodes() return arguments are now a parameter.
*/

using System;
using System.Collections.Generic;

namespace MathLib.AI
{
    public abstract class AStarPathFinding2
    {
        public void FindPath(MapPosition startPosition, MapPosition endPosition, ref List<MapPosition> results)
        {
            if(results == null) results = new List<MapPosition>();
            results.Clear();

            // if(the locations are the same then ... exit this procedure.
            if(object.Equals(startPosition, endPosition)) return;

            // Add the starting square to the open list.
            NodesCollection openedNodes = new NodesCollection();
            NodesCollection closedNodes = new NodesCollection();
            openedNodes.Add(startPosition);

            MapCellsCollection parentNodes = new MapCellsCollection();
            int lowestItem, foundItem, nodeIndex, parentIndex;
            NodeItem currentNode;
            List<MapPosition> connectingNodes = new List<MapPosition>();
            do {
                // ... Look for the lowest F cost square on the open list ...
                lowestItem = openedNodes.FindByLowestF();
                if(lowestItem < 0) return;
                currentNode = openedNodes[lowestItem];

                // ... Switch it to the close list ...
                closedNodes.Add(currentNode);
                openedNodes.RemoveAt(lowestItem);

                // ... if(we added the target square to the closed list, then quit.
                if(object.Equals(currentNode.Position, endPosition)) break;

                // ... for(each of the cell's sides to this current square ...
                this.EnumConnectingNodes(currentNode.Position, ref connectingNodes);
                if(connectingNodes != null && connectingNodes.Count > 0) {
                    for(nodeIndex = 0; nodeIndex < connectingNodes.Count; nodeIndex++) {
                        if(closedNodes.FindIndex(connectingNodes[nodeIndex]) < 0) {
                            // ... Find if the cell is in the open list ...
                            foundItem = openedNodes.FindIndex(connectingNodes[nodeIndex]);
                            // ... if(the cell isn't in the open list then add it ...
                            if(foundItem < 0) {
                                openedNodes.Add(connectingNodes[nodeIndex], (currentNode.G + 1), connectingNodes[nodeIndex].GetDistance(endPosition));

                                parentIndex = parentNodes.FindIndex(connectingNodes[nodeIndex]);
                                if(parentIndex < 0)
                                    parentNodes.Add(connectingNodes[nodeIndex], currentNode.Position);
                                else
                                    parentNodes[parentIndex].Next = currentNode.Position;
                            } else {
                                if(openedNodes[foundItem].G < currentNode.G) {
                                    openedNodes[foundItem].G = (currentNode.G + 1);

                                    parentIndex = parentNodes.FindIndex(connectingNodes[nodeIndex]);
                                    if(parentIndex < 0)
                                        parentNodes.Add(connectingNodes[nodeIndex], currentNode.Position);
                                    else
                                        parentNodes[parentIndex].Next = currentNode.Position;
                                }
                            }
                        }
                    } //nodeIndex
                }
            } while(true);

            // Add the last cell to the list.
            results.Insert(0, (MapPosition)endPosition.Clone());

            // Now that we're done, let's check the results and return it.
            int currentAddress = parentNodes.FindIndex(endPosition);
            do {
                if(currentAddress < 0 || (parentNodes[currentAddress].Next == null || parentNodes[currentAddress].Next.IsEmpty()))
                    break;
                else {
                    if(!object.Equals(parentNodes[currentAddress].Next, startPosition))
                        results.Insert(0, (MapPosition)parentNodes[currentAddress].Next.Clone());
                    currentAddress = parentNodes.FindIndex(parentNodes[currentAddress].Next);
                }
            } while(true);
        } // FindPath

        public abstract void EnumConnectingNodes(MapPosition position, ref List<MapPosition> results);

        private class NodesCollection : List<NodeItem>
        {
            public NodeItem Add(MapPosition position, int g = 0, int h = 0)
            {
                base.Add(new NodeItem(position, g, h));
                return base[base.Count - 1];
            } // Add

            public int FindByLowestF()
            {
                int result = -1;
                int lastF = 0;

                if(base.Count > 0) {
                    for(int index = 0; index < base.Count; index++) {
                        if(result < 0 || base[index].F < lastF) { result = index; lastF = base[index].F; }
                    } //index
                }

                return result;
            } // FindByLowestF

            public int FindIndex(MapPosition position)
            {
                if(base.Count > 0) {
                    for(int index = 0; index < base.Count; index++) {
                        if(object.Equals(base[index].Position, position)) return index;
                    } //index
                }

                return -1;
            } // FindIndex

            public NodeItem this[MapPosition position]
            {
                get {
                    int index = this.FindIndex(position);
                    if(index < 0) return null;
                    return base[index];
                }
            }
        } // NodesCollection

        private class NodeItem
        {
            public MapPosition Position;
            public int         G, H;

            public NodeItem() {}

            public NodeItem(MapPosition position, int g, int h)
            {
                this.Position = position;
                this.G = g;
                this.H = h;
            } // New

            public int F { get { return (this.G + this.H); } }

            public override string ToString() { return "Pos = (" + this.Position.ToString() + ") F = " + this.F + " G = " + this.G + " H = " + this.H; }
        } // NodeItem

        private class MapCellsCollection : List<MapCellItem>
        {
            public MapCellItem Add(MapPosition current, MapPosition next)
            {
                base.Add(new MapCellItem(current, next));
                return base[base.Count - 1];
            } // Add

            public int FindIndex(MapPosition position)
            {
                if(base.Count > 0) {
                    for(int index = 0; index < base.Count; index++) {
                        if(object.Equals(base[index].Current, position)) return index;
                    } //index
                }

                return -1;
            } // FindIndex

            public MapCellItem this[MapPosition position]
            {
                get {
                    int index = this.FindIndex(position);
                    if(index < 0) return null;
                    return base[index];
                }
            }
        } // MapCellsCollection

        private class MapCellItem
        {
            public MapPosition Current, Next;

            public MapCellItem(MapPosition current, MapPosition next)
            {
                this.Current = current;
                this.Next = next;
            } // New
        } // MapCellItem

        public abstract class MapPosition : ICloneable
        {
            public abstract int GetDistance(MapPosition target);
            public abstract bool IsEmpty();
            public abstract object Clone();
        } // MapPosition
    } // AStarPathFinding
} // MathLib.AI namespace
