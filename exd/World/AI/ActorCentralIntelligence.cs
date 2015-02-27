using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exd.World.Buildings;
using exd.World.Helpers;
using exd.World.Resources;

namespace exd.World.AI
{
    public enum ActorTaskType
    {
        Move,
        Gather,
        Build,
        FeedBuilding,
        Hunt,
        DropResources,
        FailTask
    }

    [DebuggerDisplay("{Type} {Target}, assigned to {Assigned}")]
    public class ActorTask
    {
        internal Placeable Target { get; set; }
        internal ActorTaskType Type { get; set; }
        internal Actor Assigned { get; set; }
        internal List<ActorTask> DependsOn { get; set; }
        internal ActorTask DependencyFor { get; set; }
        internal bool Done { get; set; }

        internal bool AllDependsDone { get { return DependsOn == null || !DependsOn.Any(t => !t.Done); } }

        internal ActorTask(Placeable target, ActorTaskType type)
        {
            Target = target;
            Type = type;
        }
    }

    [DebuggerDisplay("{StepType} @ {Location} on {Placeable}")]
    public struct ActorTaskStep
    {
        public WorldLocation Location;
        public ActorTaskType StepType;
        public Placeable Placeable;

        public ActorTaskStep(WorldLocation location, ActorTaskType steptype, Placeable placeable)
        {
            Location = location;
            StepType = steptype;
            Placeable = placeable;
        }

        public ActorTaskStep(ActorTaskType steptype)
            : this(new WorldLocation(0, 0), steptype, null)
        {

        }
    }

    public class ActorCentralIntelligence
    {
        private List<ActorTask> Tasks = new List<ActorTask>();
        private List<Tuple<double, ActorTask>> RecentlyFailedTasks = new List<Tuple<double, ActorTask>>();

        public void AddGatherTask(AbstractGroundResource resource)
        {
            Tasks.Add(new ActorTask(resource, ActorTaskType.Gather));
        }

        public void AddGatherTask(IEnumerable<AbstractGroundResource> resources)
        {
            foreach (var resource in resources)
                AddGatherTask(resource);
        }

        public void AddBuildTask(Building building)
        {
            Tasks.Add(new ActorTask(building, ActorTaskType.Build) { DependsOn = new List<ActorTask>() });
        }

        public ActorTask RequestNewTaskFor(Actor actor)
        {
            RecentlyFailedTasks.RemoveAll(t => GameWorld.Now - t.Item1 >= 500);

            var tasks = Tasks.Where(t => t.Assigned == null && !RecentlyFailedTasks.Any(ft => ft.Item2 == t))
                .OrderBy(t => t.Target.Location.Distance(actor.Location));

            // go one by one through the list of tasks 
            foreach (var task in tasks)
            {
                // does this task need to be fed?
                if (task.Type == ActorTaskType.Build)
                {
                    var building = (Building)task.Target;
                    if (!building.PromiseCovers(building.ResourceCosts))
                    {
                        var feedtask = new ActorTask(building, ActorTaskType.FeedBuilding) { DependencyFor = task, Assigned = actor };
                        task.DependsOn.Add(feedtask);
                        return feedtask;
                    }
                }

                // if it's a normal task, just take it over
                task.Assigned = actor;
                return task;
            }

            // nothing left for this guy!
            return null;
        }

        class AStarNode : AStarSearch.IHasNeighbours<AStarNode>
        {
            public readonly WorldLocation Location;

            public AStarNode(WorldLocation location) { Location = location; }

            public IEnumerable<AStarNode> Neighbours
            {
                get
                {
                    // request a list of placeables in our way
                    var placeables = GameWorld.Placeables.GetPlaceables(new WorldRectangle(
                        (int)Location.X - 1, (int)Location.Y - 1, 2, 2)).ToArray();

                    // and figure out which neighbors are valid
                    var loc = Location.Offset(-1, -1);
                    if (GameWorld.IsLocationValid(loc) && !placeables.Any(p => !p.Passable && p.Location == loc))
                        yield return new AStarNode(loc);
                    loc = Location.Offset(+0, -1);
                    if (GameWorld.IsLocationValid(loc) && !placeables.Any(p => !p.Passable && p.Location == loc))
                        yield return new AStarNode(loc);
                    loc = Location.Offset(+1, -1);
                    if (GameWorld.IsLocationValid(loc) && !placeables.Any(p => !p.Passable && p.Location == loc))
                        yield return new AStarNode(loc);
                    loc = Location.Offset(+1, +0);
                    if (GameWorld.IsLocationValid(loc) && !placeables.Any(p => !p.Passable && p.Location == loc))
                        yield return new AStarNode(loc);
                    loc = Location.Offset(+1, +1);
                    if (GameWorld.IsLocationValid(loc) && !placeables.Any(p => !p.Passable && p.Location == loc))
                        yield return new AStarNode(loc);
                    loc = Location.Offset(+0, +1);
                    if (GameWorld.IsLocationValid(loc) && !placeables.Any(p => !p.Passable && p.Location == loc))
                        yield return new AStarNode(loc);
                    loc = Location.Offset(-1, +1);
                    if (GameWorld.IsLocationValid(loc) && !placeables.Any(p => !p.Passable && p.Location == loc))
                        yield return new AStarNode(loc);
                    loc = Location.Offset(-1, +0);
                    if (GameWorld.IsLocationValid(loc) && !placeables.Any(p => !p.Passable && p.Location == loc))
                        yield return new AStarNode(loc);
                }
            }

            public override bool Equals(object obj)
            {
                var node = obj as AStarNode;
                if (node != null)
                    return Location == node.Location;

                return false;
            }

            public override int GetHashCode()
            {
                return Location.GetHashCode();
            }
        }

        public ActorTaskStep[] ResolveTaskIntoSteps(ActorTask task)
        {
            if (task.Type == ActorTaskType.Gather || task.Type == ActorTaskType.Build)
            {
                // go to it and do the action
                var path = AStarSearch.FindPath(
                    new AStarNode(task.Assigned.Location),
                    new AStarNode(task.Target.Location),
                    (n1, n2) => WorldLocation.Distance(n1.Location, n2.Location),
                    n => WorldLocation.Distance(n.Location, task.Target.Location));

                return path.Select((n, i) => new ActorTaskStep(n.Location, ActorTaskType.Move, null))
                    .Reverse().Skip(1)
                    .Concat(new[] { new ActorTaskStep(task.Target.Location, task.Type, task.Target) })
                    .ToArray();
            }
            else if (task.Type == ActorTaskType.FeedBuilding)
            {
                // this is a bit special
                // whatever i'm carrying, is this good enough?
                var building = task.Target as Building;
                bool justdrop = false;
                if (task.Assigned.ResourcesCarried.Any(r => building.ResourceCosts[r.Key] > 0 && r.Value < 0))
                    justdrop = true;

                // if not, find a resource
                ResourceGroundStack res = null;
                if (!justdrop)
                {
                    foreach (var resource in GameWorld.Placeables.GetPlaceables()
                        .OfType<ResourceGroundStack>()
                        .Where(r => PromiseToken.GetTokenByResourcePromised(r) == null)
                        .OrderBy(p => p.Location.Distance(task.Assigned.Location)))
                    {
                        // can we use the stack?
                        if (resource.ResourceCosts.Any(r => building.ResourceCosts[r.Key] > 0 && r.Value < 0))
                        {
                            res = resource;
                            break;
                        }
                    }

                    // fail the task if we can't find a resource
                    if (res == null)
                        return new[] { new ActorTaskStep(ActorTaskType.FailTask) };
                }

                // path to go to the resource
                AStarSearch.Path<AStarNode> pathtores = null;
                if (!justdrop)
                {
                    pathtores = AStarSearch.FindPath(
                        new AStarNode(task.Assigned.Location),
                        new AStarNode(res.Location),
                        (n1, n2) => n1.Location.Distance(n2.Location),
                        n => n.Location.Distance(res.Location));
                }

                // and path to bring it to the building
                var pathfromrestobuilding = AStarSearch.FindPath(
                    new AStarNode(justdrop ? task.Assigned.Location : res.Location),
                    new AStarNode(building.Location),
                    (n1, n2) => n1.Location.Distance(n2.Location),
                    n => n.Location.Distance(building.Location));

                // put it together
                if (justdrop)
                    return pathfromrestobuilding.Select(n => new ActorTaskStep(n.Location, ActorTaskType.Move, null))
                        .Reverse().Skip(1)
                        .Concat(new[] { new ActorTaskStep(building.Location, ActorTaskType.DropResources, building) })
                        .ToArray();
                else
                    return pathtores.Select(n => new ActorTaskStep(n.Location, ActorTaskType.Move, null))
                        .Reverse().Skip(1)
                        .Concat(new[] { new ActorTaskStep(res.Location, ActorTaskType.Gather, res) })
                        .Concat(pathfromrestobuilding.Select(n => new ActorTaskStep(n.Location, ActorTaskType.Move, null))
                            .Reverse().Skip(1))
                        .Concat(new[] { new ActorTaskStep(building.Location, ActorTaskType.DropResources, building) })
                        .ToArray();
            }
            else
                throw new InvalidOperationException("Invalid task.");
        }

        public void TaskFail(ActorTask CurrentTask)
        {
            Tasks.Remove(CurrentTask);
            RecentlyFailedTasks.Add(Tuple.Create(GameWorld.Now, CurrentTask.DependencyFor ?? CurrentTask));
            CurrentTask.Done = true;

            Logger.Log("task fail", "{0} {1} @{2} by {3}",
                CurrentTask.Type, CurrentTask.Target, CurrentTask.Target.Location, CurrentTask.Assigned);
        }

        public void TaskDone(ActorTask CurrentTask)
        {
            Tasks.Remove(CurrentTask);
            CurrentTask.Done = true;

            Logger.Log("task done", "{0} {1} @{2} by {3}",
                CurrentTask.Type, CurrentTask.Target, CurrentTask.Target.Location, CurrentTask.Assigned);
        }
    }
}
