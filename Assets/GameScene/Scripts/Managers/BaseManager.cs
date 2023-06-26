using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lore.Game.Managers
{
    public abstract class BaseManager : MonoBehaviour
    {
        public bool IsSetup { get; protected set; }


        public virtual void Start()
        {
            IsSetup = true;
        }
    }

}
