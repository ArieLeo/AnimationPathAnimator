using System;
using UnityEngine;

namespace ATP.AnimationPathAnimator.APEventsComponent {

    [Serializable]
    public sealed class NodeEventSlot {

        [SerializeField]
        private string methodArg;

        /// <summary>
        ///     How many rows should be displayed in the inspector.
        /// </summary>
        [SerializeField]
        private int rows = 1;

        /// <summary>
        ///     Selected source component.
        /// </summary>
        [SerializeField]
        private Component sourceCo;

        [SerializeField]
        private int sourceComponentIndex;

        /// <summary>
        ///     Selected source game object.
        /// </summary>
        [SerializeField]
        private GameObject sourceGO;

        [SerializeField]
        private int sourceMethodIndex;

        [SerializeField]
        private string sourceMethodName;

        public string MethodArg {
            get { return methodArg; }
            set { methodArg = value; }
        }

        /// <summary>
        ///     Selected source component.
        /// </summary>
        public Component SourceCo {
            get { return sourceCo; }
        }

        /// <summary>
        ///     Selected source game object.
        /// </summary>
        public GameObject SourceGO {
            get { return sourceGO; }
        }

        public string SourceMethodName {
            get { return sourceMethodName; }
        }

    }

}