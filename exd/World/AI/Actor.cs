using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exd.World.Helpers;
using exd.World.ResourceCostHelper;
using exd.World.Resources;

namespace exd.World.AI
{
    public class Actor : Placeable, IUpdateable
    {
        protected ActorTask CurrentTask = null;
        protected ActorTaskStep[] CurrentTaskSteps = null;
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
        /// Meter buildup until the current task is done
        /// </summary>
        protected double TaskBuildup = 0;

        protected ResourceCosts ResourcesCarried = new ResourceCosts();

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
                    default:
                        throw new InvalidOperationException("Invalid task step.");
                }
            }
        }

        internal void AddCarry(ResourceCosts resources)
        {
            ResourcesCarried.Add(resources);
        }
    }
}
