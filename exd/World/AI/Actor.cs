using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exd.World.Buildings;
using exd.World.Helpers;
using exd.World.ResourceCostHelper;
using exd.World.Resources;

namespace exd.World.AI
{
    public class Actor : Placeable, IUpdateable
    {
        protected ActorTask CurrentTask = null;
        protected ActorTaskStep[] CurrentTaskSteps = null;
        protected PromiseToken[] CurrentPromiseTokens = null;
        protected double CurrentTaskStepDuration = 0;
        protected int CurrentTaskStepIndex = 0;

        /// <summary>
        /// The actor's movement speed in squares/sec
        /// </summary>
        public double MovementSpeed = 0.5;

        /// <summary>
        /// The actor's gather speed in units/sec
        /// </summary>
        public double GatherSpeed = 0.3;

        /// <summary>
        /// The actor's build speed in units/sec
        /// </summary>
        public double BuildSpeed = 0.3;

        /// <summary>
        /// Meter buildup until the current task is done
        /// </summary>
        protected double TaskBuildup = 0;

        /// <summary>
        /// The maximum weight our actor can carry
        /// </summary>
        public double MaxCarryWeight = 1000;

        public ResourceCosts ResourcesCarried = new ResourceCosts();

        public Actor(WorldLocation location, PlaceableRotation rotation = PlaceableRotation.Rotate0Degrees, double? dob = null)
            : base(location, rotation, dob)
        {
        }

        private double GetTaskStepDuration(ActorTaskStep newstep)
        {
            const double DiagonalMovementCost = 1.41421356237;

            switch (newstep.StepType)
            {
                case ActorTaskType.Move:
                    return newstep.Location.X != Location.X && newstep.Location.Y != Location.Y ? DiagonalMovementCost : 1;
                case ActorTaskType.Gather:
                    return ((AbstractGroundResource)newstep.Placeable).GetGatherDuration(this);
                case ActorTaskType.Build:
                    return ((Building)newstep.Placeable).BuiltBuildupRequired;
                case ActorTaskType.DropResources:
                    return ((Building)newstep.Placeable).GetDropDuration(this);
                default:
                    throw new InvalidOperationException("Invalid task step.");
            }
        }

        private void StepDone()
        {
            if (++CurrentTaskStepIndex < CurrentTaskSteps.Length)
                CurrentTaskStepDuration = GetTaskStepDuration(CurrentTaskSteps[CurrentTaskStepIndex]);
            else
                TaskDone();
        }

        private void TaskDone()
        {
            GameWorld.ActorCentralIntelligence.TaskDone(CurrentTask);

            if (CurrentPromiseTokens != null)
            {
                foreach (var token in CurrentPromiseTokens)
                    token.Done();
                CurrentPromiseTokens = null;
            }

            CurrentTask = null;
            CurrentTaskSteps = null;
        }

        public void Update(double delta)
        {
            if (CurrentTask == null)
                CurrentTask = GameWorld.ActorCentralIntelligence.RequestNewTaskFor(this);

            if (CurrentTask != null && CurrentTaskSteps == null)
            {
                TaskBuildup = 0;
                CurrentTaskStepIndex = 0;
                CurrentTaskSteps = GameWorld.ActorCentralIntelligence.ResolveTaskIntoSteps(CurrentTask);
                CurrentTaskStepDuration = GetTaskStepDuration(CurrentTaskSteps[0]);

                // if this is a feeder task, promise the resources to the main task
                if (CurrentTask.Type == ActorTaskType.FeedBuilding)
                {
                    var building = (Building)CurrentTask.Target;
                    CurrentPromiseTokens = CurrentTaskSteps.Where(t => t.StepType == ActorTaskType.Gather)
                        .Select(t => ((AbstractGroundResource)t.Placeable).GetRemainingResources())
                        .Select(r => building.Promise(r))
                        .ToArray();
                }
            }

            if (CurrentTaskSteps != null && CurrentTaskSteps.Any() && CurrentTaskStepIndex < CurrentTaskSteps.Length)
            {
                var step = CurrentTaskSteps[CurrentTaskStepIndex];

                // step-specific logic
                switch (step.StepType)
                {
                    case ActorTaskType.Move:
                        if ((TaskBuildup += MovementSpeed * delta / 1000.0) >= CurrentTaskStepDuration)
                        {
                            TaskBuildup -= CurrentTaskStepDuration;
                            Location = step.Location;
                            StepDone();
                        }
                        break;
                    case ActorTaskType.Gather:
                        if ((TaskBuildup += GatherSpeed * delta / 1000.0) >= CurrentTaskStepDuration)
                        {
                            TaskBuildup -= CurrentTaskStepDuration;
                            ((AbstractGroundResource)CurrentTaskSteps[CurrentTaskStepIndex].Placeable)
                                .GatherFinished(this);
                            StepDone();
                        }
                        break;
                    case ActorTaskType.DropResources:
                        if ((TaskBuildup += GatherSpeed * delta / 1000.0) >= CurrentTaskStepDuration)
                        {
                            TaskBuildup -= CurrentTaskStepDuration;
                            ((Building)CurrentTaskSteps[CurrentTaskStepIndex].Placeable)
                                .DropResourcesFinished(this);
                            StepDone();
                        }
                        break;
                    case ActorTaskType.Build:
                        // the buildup is stored on the building
                        var building = (Building)CurrentTaskSteps[CurrentTaskStepIndex].Placeable;
                        building.BuiltBuildup += BuildSpeed * delta / 1000.0;
                        if (building.Built)
                        {
                            StepDone();
                        }
                        break;
                    default:
                        throw new InvalidOperationException("Invalid task step.");
                }
            }
        }

        internal void AddCarry(ResourceCosts resources)
        {
            var carriedweight = ResourcesCarried.TotalWeight;
            var newweight = resources.TotalWeight;

            if (carriedweight + newweight <= MaxCarryWeight)
                ResourcesCarried.Add(resources);
            else
            {
                // drop on the ground left overs
                var newstack = resources.SplitToWeight(MaxCarryWeight - carriedweight);
                ResourcesCarried.Add(resources);
                GameWorld.Placeables.Add(new ResourceGroundStack(Location, newstack), true);
            }
        }
    }
}
