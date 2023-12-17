using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fourtyfourty.gearController
{
    public class GearControllerStateExtension : MonoBehaviour
    {
        public enum GearState
        {
            Neutral,
            OnEnable,
            OnDisable
        }

        public GearState gearState;
    }
}