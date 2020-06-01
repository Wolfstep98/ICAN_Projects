using System;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using Pathfinding;

namespace NodeCanvas.Tasks.Actions
{
    [Category("A Star/Pathfinding/Set Destination")]
    [Description("Set the current destination for AI behavior.")]
    public class SetDestination : ActionTask<AIDestinationSetter>
    {
        #region Fields
        public BBParameter<Transform> target = null;
        #endregion

        #region Methods
        protected override void OnExecute()
        {
            if(target == null)
            {
                this.agent.target = null;
            }
            else
            {
                this.agent.target = this.target.value;
            }

            EndAction(true);
        }
        #endregion
    }
}