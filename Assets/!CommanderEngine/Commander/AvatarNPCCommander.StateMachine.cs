using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommanderEngine
{
    public partial class AvatarNPCCommander : CharacterCommander
    {
        private bool animatorSit { get { return animator.GetBool("Sitting"); } set { animator.SetBool("Sitting", value); } }

        protected override IEnumerator Commit(Command command)
        {
            yield return StartCoroutine(base.Commit(command));
        }
    }
}