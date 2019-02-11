using System.Collections.Generic;

namespace MathLib.AI
{
    public interface BaseLocation
    {
        double GetDistance(BaseLocation target);
        double GetHeuristicDistance(BaseLocation target);
        void CloneTo(BaseLocation item);
        object Clone();
    } //BaseLocation interface

    public abstract class AStarPathFinding
    {
        private SortedNodeList     mSortedList    = new SortedNodeList();
        private List<DistanceNode> mDistanceNodes = new List<DistanceNode>();
        private List<Node>         mVisitedNodes  = new List<Node>();

        public List<BaseLocation> GetPath(BaseLocation startLocation, BaseLocation endLocation)
        {
            // First check to see if(the starting and end locations are valid (in bounds, and not blocked), if(not return failure.
            if(!this.IsLocationValid(startLocation) || !this.IsLocationValid(endLocation)) return null;

            this.mDistanceNodes.Clear(); //just be reading Position, and DoneDistance()
            this.mVisitedNodes.Clear(); //just be finding out if(a node exists or not at BaseLocation.

            // Add the start location.
            Node startNode = new Node(startLocation, false, 0, -1);
            this.mSortedList.Clear();
            this.mSortedList.Add(startNode);
            this.mDistanceNodes.Add(new DistanceNode(startNode));

            Node tempLocation = null;
            bool endIsFound   = false;

            // Find the path.
            while (this.mSortedList.HasNext())
            {
                // Get the next node to check.
                tempLocation = this.mSortedList.GetNext();
                // if(the node is not visited then ...
                if(FindNodeAtLocation(this.mVisitedNodes, tempLocation.Location) < 0) {
                    this.mVisitedNodes.Add(tempLocation);
                    if(tempLocation.IsEnd) {
                        endIsFound = true;
                        break;
                    } else
                        AddAdjacentLocations(tempLocation, endLocation);
                }
            }

            // Resolve the path.
            return (endIsFound ? this.TraceBackPath(startLocation, endLocation) : null);
        } //GetPath

#region "Collection Helper Functions"
        private static int FindNodeAtLocation(List<Node> nodes, BaseLocation location)
        {
            if(nodes != null && nodes.Count > 0) {
                for(int index = 0; index < nodes.Count; index++) {
                    if(nodes[index].Location.Equals(location)) return index;
                } //index
            }

            return -1;
        } //FindNodeAtLocation
#endregion 'Collection Helper Functions

#region "Node Helper Functions"
        private bool IsUnVisited(BaseLocation location)
        {
            return (FindNodeAtLocation(this.mVisitedNodes, location) < 0);
        } //IsUnVisited

        private void AddAdjacentLocations(Node previousNode, BaseLocation endLocation)
        {
            List<BaseLocation> locations = GetAdjacentLocations(previousNode.Location);
            if(locations == null || locations.Count == 0) return;

            for(int index = 0; index < locations.Count; index++) {
                if(this.IsLocationValid(locations[index]) && this.IsUnVisited(locations[index]))
                    AddLocation(locations[index], previousNode, endLocation);
            } //index
        } //AddAdjacentLocations

        private void AddLocation(BaseLocation location, Node previousNode, BaseLocation endLocation)
        {
            Node item = CreateNode(location, previousNode, endLocation);
            this.mSortedList.Add(item);
            double dist = System.Math.Min(item.DoneDistance, GetDistanceNodeValue(location, System.Convert.ToDouble(int.MaxValue)));
            SetDistanceNodeValue(location, dist);
        } //AddLocation

        private Node CreateNode(BaseLocation location, Node previousNode, BaseLocation endLocation)
        {
            bool isEnd = location.Equals(endLocation);
            double doneDist = previousNode.DoneDistance + GetDistance(location, previousNode.Location);
            double todoDist = location.GetHeuristicDistance(endLocation);
            return new Node(location, isEnd, doneDist, todoDist);
        } //CreateLocation

        private static double GetDistance(BaseLocation a, BaseLocation b)
        {
            return a.GetDistance(b);
            //double dist = a.GetDistance(b)
            //bool aValid = IsLocationValid(a)
            //bool bValid = IsLocationValid(b)
            //Dim aMult As Single = 1.0F
            //Dim bMult As Single = 1.0F
            //If aValid = False) aMult = -1.0F
            //If bValid = False) bMult = -1.0F
            //Return dist * 0.5 * aMult + dist * 0.5 * bMult
        } //GetDistance
#endregion 'Node Helper Functions

#region "Tracing Helper Functions"
        private List<BaseLocation> TraceBackPath(BaseLocation startLocation, BaseLocation endLocation)
        {
            List<BaseLocation> locationList = new List<BaseLocation>();
            locationList.Add(endLocation);

            BaseLocation currentLocation = endLocation;
            while(!currentLocation.Equals(startLocation))
            {
                currentLocation = FindNextLocation(currentLocation);
                locationList.Add(currentLocation);
            }

            return locationList;
        } //TraceBackPath

        private BaseLocation FindNextLocation(BaseLocation location)
        {
            double bestValue = double.MaxValue;
            BaseLocation bestLocation = location;

            List<BaseLocation> locations = GetAdjacentLocations(location);
            double foundValue;
            if(locations != null && locations.Count > 0) {
                for(int index = 0; index < locations.Count; index++) {
                    if(IsLocationValid(locations[index])) {
                        foundValue = GetDistanceNodeValue(locations[index], double.MaxValue);
                        if(foundValue < bestValue) {
                            bestLocation = locations[index];
                            bestValue = foundValue;
                        }
                    }
                } //index
            }

            return bestLocation;
        }
#endregion 'Tracing Helper Functions

        internal abstract bool IsLocationValid(BaseLocation location);
        internal abstract List<BaseLocation> GetAdjacentLocations(BaseLocation location);

#region "Distance Map Helpers"
        private double GetDistanceNodeValue(BaseLocation location, double def)
        {
            int index = FindNodeAtLocation(this.mDistanceNodes, location);
            return (index < 0 ? def : this.mDistanceNodes[index].Value);
        } //GetDistanceNodeValue

        private void SetDistanceNodeValue(BaseLocation location, double value)
        {
            int index = FindNodeAtLocation(this.mDistanceNodes, location);
            if(index < 0)
                this.mDistanceNodes.Add(new DistanceNode(location, value));
            else
                this.mDistanceNodes[index].Value = value;
        } //SetDistanceNodeValue

        private static int FindNodeAtLocation(List<DistanceNode> nodes, BaseLocation location)
        {
            if(nodes != null && nodes.Count > 0) {
                for(int index = 0; index < nodes.Count; index++) {
                    if(nodes[index].Location.Equals(location)) return index;
                } //index
            }

            return -1;
        } //FindNodeAtLocation

        internal class DistanceNode
        {
            public BaseLocation Location;
            public double Value;

            public DistanceNode(BaseLocation location, double value)
            {
                this.Location = location;
                this.Value = value;
            } //New

            public DistanceNode(Node node)
            {
                this.Location = node.Location;
                this.Value = node.DoneDistance;
            } //New
        } //DistanceNode
#endregion 'Distance Map Helpers

        internal class Node
        {
            private BaseLocation mLocation;
            private bool mEnd;
            private double mDistDone, mDistTodo;

            public Node(BaseLocation location, bool end, double doneDistance, double todoDistance)
            {
                this.mLocation = location;
                this.mEnd      = end;
                this.mDistDone = doneDistance;
                this.mDistTodo = todoDistance;
            } //New

            public BaseLocation Location { get { return this.mLocation; } }

            public bool IsEnd { get { return this.mEnd; } }

            public double DoneDistance { get { return this.mDistDone; } }

            public double TotalDistance { get { return (this.mDistDone + this.mDistTodo); } }
        } //Node

        internal class SortedNodeList
        {
            private List<Node> mList;

            public SortedNodeList() { this.mList = new List<Node>(); }

            public void Add(Node item)
            {
                if(this.mList.Count > 0) {
                    Node tempLocation;
                    for(int x = 0; x < this.mList.Count; x++) {
                        tempLocation = this.mList[x];
                        if(tempLocation.TotalDistance < tempLocation.TotalDistance) {
                            this.mList.Insert(x, tempLocation);
                            return;
                        }
                    } //x
                }

                // all else fails, just add the item.
                this.mList.Add(item);
            } //Add

            public void Clear() { this.mList.Clear(); }

            public bool HasNext() { return this.mList.Count > 0; }

            public Node GetNext()
            {
                if(this.mList.Count > 0) {
                    Node item = this.mList[0];
                    this.mList.RemoveAt(0);
                    return item;
                }
                return null;
            } //GetNext

            public override string ToString() { return this.mList.Count.ToString(); }
        } //SortedNodeList
    } //AStarPathFinding

    namespace Examples.AStarPathFindingExample
    {
        public class Map
        {
            public int Width, Height;
            public bool[,] Item;

            public Map(int width, int height)
            {
                this.Width = width;
                this.Height = height;
                this.Item = new bool[width, height];
            } //New
        } //Map

        public class MyPathFinder : AStarPathFinding
        {
            public Map Map;

            public MyPathFinder(Map map) { this.Map = map; }

            internal override List<BaseLocation> GetAdjacentLocations(BaseLocation location)
            {
                if(location == null || !(location is MyLocation)) return null;
                MyLocation myLocation = (MyLocation)location;

                List<BaseLocation> result = new List<BaseLocation>();
                result.Add(new MyLocation(myLocation.X - 1, myLocation.Y));
                result.Add(new MyLocation(myLocation.X + 1, myLocation.Y));
                result.Add(new MyLocation(myLocation.X, myLocation.Y - 1));
                result.Add(new MyLocation(myLocation.X, myLocation.Y + 1));
                return result;
            } //GetAdjacentLocations

            internal override bool IsLocationValid(BaseLocation location)
            {
                if(location == null || !(location is MyLocation)) return false;
                MyLocation myLocation = (MyLocation)location;

                if(myLocation.X >= 1 && myLocation.Y >= 1 && myLocation.X <= this.Map.Width && myLocation.Y <= this.Map.Height)
                    return !this.Map.Item[myLocation.X - 1, myLocation.Y - 1];
                else
                    return false;
            } //IsLocationValid
        } //MyPathFinder

        public class MyLocation : BaseLocation
        {
            public int X, Y;

            public MyLocation(int x, int y)
            {
                this.X = x;
                this.Y = y;
            } //New

            public MyLocation(MyLocation clone)
            {
                clone.CloneTo(this);
            } //New

            public double GetDistance(BaseLocation target)
            {
                if(target == null || !(target is MyLocation)) return double.MaxValue;
                MyLocation targetLoc = (MyLocation)target;
                int dx = (this.X - targetLoc.X);
                int dy = (this.Y - targetLoc.Y);
                return System.Math.Sqrt(dx * dx + dy * dy);
            } //GetDistance

            public double GetHeuristicDistance(BaseLocation target)
            {
                if(target == null || !(target is MyLocation)) return double.MaxValue;
                MyLocation targetLoc = (MyLocation)target;
                double cons = 1.2;
                double result = cons * System.Math.Abs(this.X - targetLoc.X);
                result += cons * System.Math.Abs(this.Y - targetLoc.Y);
                return result;
            } //GetHeuristicDistance

            public override string ToString() { return (this.X + " x " + this.Y); }

            public override bool Equals(object obj)
            {
                if(obj == null || !(obj is MyLocation)) return false;
                MyLocation b = (MyLocation)obj;
                return (this.X == b.X && this.Y == b.Y);
            } //Equals

            public override int GetHashCode() { return X * Y; }

            public void CloneTo(BaseLocation item)
            {
                if (item == null) return;

                if (item is MyLocation)
                {
                    MyLocation locItem = (MyLocation)item;
                    this.CloneTo(locItem);
                }
            } //CloneTo

            public void CloneTo(MyLocation item)
            {
                if(item == null) return;
                item.X = this.X;
                item.Y = this.Y;
            } //CloneTo

            public object Clone()
            {
                MyLocation results = new MyLocation(0, 0);
                this.CloneTo(results);
                return results;
            } //Clone Function
        } //MyLocation
    } //Examples
} //MathLib.AI namespace
