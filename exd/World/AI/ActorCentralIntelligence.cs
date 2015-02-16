using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exd.World.Helpers;
using exd.World.Resources;

namespace exd.World.AI
{
    public enum ActorTaskType
    {
        Move,
        Gather,
        Build,
        Hunt
    }

    [DebuggerDisplay("{Type} {Target}, assigned to {Assigned}")]
    public class ActorTask
    {
        internal Placeable Target { get; set; }
        internal ActorTaskType Type { get; set; }
        internal Actor Assigned { get; set; }

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
    }

    public class ActorCentralIntelligence
    {
        private List<ActorTask> Tasks = new List<ActorTask>();

        public void AddGatherTask(AbstractGroundResource resource)
        {
            Tasks.Add(new ActorTask(resource, ActorTaskType.Gather));
        }

        public void AddGatherTask(IEnumerable<AbstractGroundResource> resources)
        {
            foreach (var resource in resources)
                AddGatherTask(resource);
        }

        public ActorTask RequestNewTaskFor(Actor actor)
        {
            var task = Tasks.Where(t => t.Assigned == null)
                .OrderBy(t => t.Target.Location.Distance(actor.Location))
                .FirstOrDefault();

            if (task != null)
                task.Assigned = actor;

            return task;
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
            var path = AStarSearch.FindPath(
                new AStarNode(task.Assigned.Location),
                new AStarNode(task.Target.Location),
                (n1, n2) => WorldLocation.Distance(n1.Location, n2.Location),
                n => WorldLocation.Distance(n.Location, task.Target.Location));

            return path.Select((n, i) => new ActorTaskStep(n.Location, ActorTaskType.Move, null))
                .Reverse().Skip(1)
                .Union(new[] { new ActorTaskStep(task.Target.Location, task.Type, task.Target) })
                .ToArray();
        }

        public void TaskDone(ActorTask CurrentTask)
        {
            Tasks.Remove(CurrentTask);
            Debug.WriteLine(
                "[{0:0.00s} task done] {1} {2} @{3} by {4}",
                GameWorld.Now / 1000, CurrentTask.Type, CurrentTask.Target, CurrentTask.Target.Location, CurrentTask.Assigned);
        }
    }
}
