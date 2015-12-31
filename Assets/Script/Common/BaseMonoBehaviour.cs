using UnityEngine;
using System.Collections;

namespace Common {

    public class BaseMonoBehaviour : MonoBehaviour {

        public bool IsStarted { get; private set; }

        public virtual void Start() {
            IsStarted = true;
            OnEnableAfterStart();
        }

        public virtual void OnEnable() {
            if (IsStarted) {
                OnEnableAfterStart();
            }
        }
        public virtual void OnEnableAfterStart() {}
    }

}
