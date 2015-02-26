﻿using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ATP.ReorderableList;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace ATP.AnimationPathTools {
    /// <summary>
    ///     Component that allows animating transforms position along predefined
    ///     Animation Paths and also animate their rotation on x and y axis in
    ///     time.
    /// </summary>
    [RequireComponent(typeof (AnimationPathBuilder))]
    [ExecuteInEditMode]
    public class AnimationPathAnimator : GameComponent {

        #region FIELDS
        /// <summary>
        ///     If animation is currently enabled (may be paused).
        /// </summary>
        /// <remarks>Used in play mode.</remarks>
        private bool isPlaying;
        private int rotationCurveSampling = 20;
        #endregion FIELDS

        #region SERIALIZED FIELDS
        [SerializeField]
        private GizmoDrawer gizmoDrawer;

        /// <summary>
        ///     Path used to animate the <c>animatedGO</c> transform.
        /// </summary>
        [SerializeField]
        private AnimationPathBuilder animationPathBuilder;
        [SerializeField]
#pragma warning disable 169
            private bool advancedSettingsFoldout;
#pragma warning restore 169

        /// <summary>
        ///     Transform to be animated.
        /// </summary>
        [SerializeField] private Transform animatedGO;

        /// Current play time represented as a number between 0 and 1.
        [SerializeField] private float animTimeRatio;
#pragma warning restore 169
#pragma warning restore 169

        [SerializeField] private PathData pathData;
        [SerializeField] private GUISkin skin;

        /// <summary>
        ///     Transform that the <c>animatedGO</c> will be looking at.
        /// </summary>
        [SerializeField]
#pragma warning disable 649
            private Transform targetGO;
#pragma warning restore 649
        #region OPTIONS

        /// <summary>
        ///     Value of the jump when modifier key is pressed.
        /// </summary>
        [SerializeField]
        private float shortJumpValue = 0.002f;

        [SerializeField]
        private WrapMode wrapMode = WrapMode.Clamp;

        [SerializeField]
#pragma warning disable 169
        private bool enableControlsInPlayMode = true;
#pragma warning restore 169

        [SerializeField]
#pragma warning disable 169
        private float maxAnimationSpeed = 0.3f;
#pragma warning restore 169

        /// <summary>
        ///     How much look forward point should be positioned away from the
        ///     animated object.
        /// </summary>
        /// <remarks>Value is a time in range from 0 to 1.</remarks>
        [SerializeField]
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        // ReSharper disable once ConvertToConstant.Local
        private float forwardPointOffset = 0.05f;

        [SerializeField]
        private AnimatorRotationMode rotationMode =
            AnimatorRotationMode.Forward;

        [SerializeField]
        private AnimatorHandleMode handleMode =
            AnimatorHandleMode.None;

        [SerializeField]
        private bool updateAllMode;

        [SerializeField]
        private bool autoPlay = true;

        [SerializeField]
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        // ReSharper disable once ConvertToConstant.Local
        private float positionLerpSpeed = 0.1f;

        [SerializeField]
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        // ReSharper disable once ConvertToConstant.Local
        private float rotationSpeed = 3.0f;
        #endregion

        #endregion SERIALIZED FIELDS
        #region PUBLIC PROPERTIES
        public WrapMode WrapMode {
            get { return wrapMode; }
            set { wrapMode = value; }
        }


        /// <summary>
        ///     Path used to animate the <c>animatedGO</c> transform.
        /// </summary>
        public AnimationPathBuilder AnimationPathBuilder {
            get { return animationPathBuilder; }
        }

        public float AnimationTimeRatio {
            get { return animTimeRatio; }
        }

        public bool AutoPlay {
            get { return autoPlay; }
            set { autoPlay = value; }
        }

        public AnimatorHandleMode HandleMode {
            get { return handleMode; }
            set { handleMode = value; }
        }

        /// <summary>
        ///     If animation is currently enabled.
        /// </summary>
        /// <remarks>
        ///     Used in play mode. You can use it to stop animation.
        /// </remarks>
        public bool IsPlaying {
            get { return isPlaying; }
            set { isPlaying = value; }
        }

        public PathData PathData {
            get { return pathData; }
            set { pathData = value; }
        }

        public bool Pause { get; set; }

        public AnimatorRotationMode RotationMode {
            get { return rotationMode; }
            set { rotationMode = value; }
        }

        public GUISkin Skin {
            get { return skin; }
            set { skin = value; }
        }

        public bool UpdateAllMode {
            get { return updateAllMode; }
            set { updateAllMode = value; }
        }

        /// <summary>
        ///     Value of the jump when modifier key is pressed.
        /// </summary>
        public virtual float ShortJumpValue {
            get { return shortJumpValue; }
        }

        #endregion PUBLIC PROPERTIES

        #region PRIVATE/PROTECTED PROPERTIES

        public virtual float FloatPrecision {
            get { return 0.001f; }
        }

        public GizmoDrawer GizmoDrawer {
            get { return gizmoDrawer; }
        }

        protected virtual int RotationCurveSampling {
            get { return rotationCurveSampling; }
        }

        #endregion

        #region UNITY MESSAGES

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private void Awake() {
            //InitializeEaseCurve();
            //InitializeRotationCurve();

            // Initialize animatedGO field.
            if (animatedGO == null && Camera.main.transform != null) {
                animatedGO = Camera.main.transform;
            }

            // Initialize AnimationPathBuilder field.
            animationPathBuilder = GetComponent<AnimationPathBuilder>();
        }

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private void OnDisable() {
            animationPathBuilder.PathReset -= animationPathBuilder_PathReset;
        }

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        // TODO Refactor.
        private void OnDrawGizmosSelected() {
            // Return if path asset file is not assigned.
            if (PathData == null) return;

            if (rotationMode == AnimatorRotationMode.Target
                && targetGO != null) {

                GizmoDrawer.DrawTargetIcon(targetGO.position);
            }

            if (rotationMode == AnimatorRotationMode.Forward) {
                var globalForwardPointPosition = GetForwardPoint(true);
                GizmoDrawer.DrawForwardPointIcon(globalForwardPointPosition);
            }

            // Return if handle mode is not rotation mode.
            if (handleMode == AnimatorHandleMode.Rotation) {
                var localPointPositions =
                    PathData.RotationPath.SamplePathForPoints(
                        RotationCurveSampling);

                var globalPointPositions =
                    new Vector3[localPointPositions.Count];

                for (var i = 0; i < localPointPositions.Count; i++) {
                    globalPointPositions[i] = transform.TransformPoint(localPointPositions[i]);
                }
                GizmoDrawer.DrawRotationGizmoCurve(globalPointPositions);

                //GizmoDrawer.DrawCurrentRotationPointGizmo();
                HandleDrawingCurrentRotationPointGizmo();

                DrawRotationPointGizmos();
            }
        }

        private void DrawRotationPointGizmos() {
            var rotationPointPositions = GetGlobalRotationPointPositions();

            // Path node timestamps.
            var nodeTimestamps = PathData.GetPathTimestamps();

            for (var i = 0; i < rotationPointPositions.Length; i++) {
                // Return if current animation time is the same as any node
                // time.
                if (Math.Abs(nodeTimestamps[i] - AnimationTimeRatio) <
                    FloatPrecision) {
                    continue;
                }

                GizmoDrawer.DrawRotationPointGizmo(rotationPointPositions[i]);
            }
        }

        private void HandleDrawingCurrentRotationPointGizmo() {
            // Get current animation time.
            var currentAnimationTime = AnimationTimeRatio;

            // Node path node timestamps.
            var nodeTimestamps = PathData.GetPathTimestamps();

            // Return if current animation time is the same as any node time.
            if (nodeTimestamps.Any(nodeTimestamp =>
                Math.Abs(nodeTimestamp - currentAnimationTime)
                < FloatPrecision)) {

                return;
            }

            // Get rotation point position.
            var localRotationPointPosition =
                PathData.GetRotationAtTime(currentAnimationTime);
            var globalRotationPointPosition =
                transform.TransformPoint(localRotationPointPosition);
            GizmoDrawer.DrawCurrentRotationPointGizmo(
                globalRotationPointPosition);
        }

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        public virtual void OnEnable() {
            // Subscribe to events.
            animationPathBuilder.PathReset += animationPathBuilder_PathReset;
            animationPathBuilder.NodeAdded += animationPathBuilder_NodeAdded;
            animationPathBuilder.NodeRemoved += animationPathBuilder_NodeRemoved;
            animationPathBuilder.NodeTimeChanged +=
                animationPathBuilder_NodeTimeChanged;
            animationPathBuilder.NodePositionChanged +=
                animationPathBuilder_NodePositionChanged;
            //RotationPointPositionChanged += this_RotationPointPositionChanged;
            pathData.NodeTiltChanged += this_NodeTiltChanged;

            if (gizmoDrawer == null) {
                gizmoDrawer = ScriptableObject.CreateInstance<GizmoDrawer>();
            }
        }

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private void OnValidate() {
        }

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private void Start() {
            if (Application.isPlaying && autoPlay) {
                isPlaying = true;

                StartEaseTimeCoroutine();
            }
        }

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private void Update() {
            // In play mode, update animation time with delta time.
            if (Application.isPlaying && isPlaying && !Pause) {
                Animate();
            }
        }

        #endregion UNITY MESSAGES

        #region EVENT INVOCATORS

        //protected virtual void OnRotationPointPositionChanged() {
        //    var handler = RotationPointPositionChanged;
        //    if (handler != null) handler(this, EventArgs.Empty);
        //}

        #endregion EVENT INVOCATORS

        #region EVENT HANDLERS

        private void animationPathBuilder_NodeAdded(object sender,
            EventArgs eventArgs) {
            PathData.UpdateCurveWithAddedKeys(PathData.EaseCurve);
            PathData.UpdateCurveWithAddedKeys(PathData.TiltingCurve);
            PathData.UpdateRotationCurvesWithAddedKeys();
        }

        private void animationPathBuilder_NodePositionChanged(
            object sender,
            EventArgs e) {
            if (!Application.isPlaying) Animate();
            if (Application.isPlaying) UpdateAnimatedGO();
        }

        private void animationPathBuilder_NodeRemoved(
            object sender,
            EventArgs e) {
            PathData.UpdateCurveWithRemovedKeys(PathData.EaseCurve);
            PathData.UpdateCurveWithRemovedKeys(PathData.TiltingCurve);
            pathData.UpdateRotationPathWithRemovedKeys();
        }

        private void animationPathBuilder_NodeTimeChanged(
            object sender,
            EventArgs e) {
            PathData.UpdateRotationCurvesTimestamps();
            PathData.UpdateCurveTimestamps(PathData.EaseCurve);
            PathData.UpdateCurveTimestamps(PathData.TiltingCurve);
        }

        private void animationPathBuilder_PathReset(
            object sender,
            EventArgs eventArgs) {
            PathData.Reset();

            // Change handle mode to None.
            handleMode = AnimatorHandleMode.None;
            // Change rotation mode to None.
            rotationMode = AnimatorRotationMode.Forward;
        }

        private void this_NodeTiltChanged(object sender, EventArgs e) {
            if (!Application.isPlaying) Animate();
            if (Application.isPlaying) UpdateAnimatedGO();
        }

        //private void this_RotationPointPositionChanged(object sender,
        //    EventArgs e) {
        //    if (!Application.isPlaying) Animate();
        //}

        #endregion EVENT HANDLERS

        #region PUBLIC METHODS

        // NOTE Animator.
        // TODO Remove the globalPosition arg. and create separate method.
        public Vector3 GetForwardPoint(bool globalPosition) {
            // Timestamp offset of the forward point.
            var forwardPointDelta = forwardPointOffset;
            // Forward point timestamp.
            var forwardPointTimestamp = animTimeRatio + forwardPointDelta;
            var localPosition =
                animationPathBuilder.GetVectorAtTime(forwardPointTimestamp);

            // Return global position.
            if (globalPosition) {
                return transform.TransformPoint(localPosition);
            }

            return localPosition;
        }

        // NOTE Animator.
        public Vector3 GetGlobalNodePosition(int nodeIndex) {
            var localNodePosition =
                animationPathBuilder.GetNodePosition(nodeIndex);
            var globalNodePosition = transform.TransformPoint(localNodePosition);

            return globalNodePosition;
        }

        // NOTE Animator.
        public void StartEaseTimeCoroutine() {
            // Check for play mode.
            StartCoroutine("EaseTime");
        }

        // NOTE Animator.
        public void StopEaseTimeCoroutine() {
            StopCoroutine("EaseTime");

            // Reset animation.
            isPlaying = false;
            Pause = false;
            animTimeRatio = 0;
        }

        //public void UpdateNodeTilting(int keyIndex, float newValue) {
        //    // Copy keyframe.
        //    var keyframeCopy = PathData.TiltingCurve.keys[keyIndex];
        //    // Update keyframe value.
        //    keyframeCopy.value = newValue;

        //    // Replace old key with updated one.
        //    PathData.TiltingCurve.RemoveKey(keyIndex);
        //    PathData.TiltingCurve.AddKey(keyframeCopy);
        //    PathData.SmoothCurve(PathData.TiltingCurve);
        //    EaseCurveExtremeNodes(PathData.TiltingCurve);

        //    OnNodeTiltChanged();
        //}

        public void UpdateWrapMode() {
            animationPathBuilder.SetWrapMode(wrapMode);
        }

        #endregion PUBLIC METHODS

        #region PRIVATE METHODS

        public void Animate() {
            AnimateObject();
            HandleAnimatedGORotation();
            TiltObject();
        }

        /// <summary>
        ///     Update animatedGO position, rotation and tilting based on current
        ///     animTimeRatio.
        /// </summary>
        /// <remarks>
        ///     Used to update animatedGO with keys, in
        ///     play mode.
        /// </remarks>
        public void UpdateAnimatedGO() {
            UpdateAnimatedGOPosition();
            UpdateAnimatedGORotation();
            // Update animatedGO tilting.
            TiltObject();
        }


        private void AnimateObject() {
            if (animatedGO == null
                || animationPathBuilder == null) {
                return;
            }

            var positionAtTimestamp =
                animationPathBuilder.GetVectorAtTime(animTimeRatio);

            var globalPositionAtTimestamp =
                transform.TransformPoint(positionAtTimestamp);

            if (Application.isPlaying) {
                // Update position.
                animatedGO.position = Vector3.Lerp(
                    animatedGO.position,
                    globalPositionAtTimestamp,
                    positionLerpSpeed);
            }
            else {
                animatedGO.position = globalPositionAtTimestamp;
            }
        }

        // ReSharper disable once UnusedMember.Local
        private IEnumerator EaseTime() {
            while (true) {
                // If animation is not paused..
                if (!Pause) {
                    // Ease time.
                    var timeStep = PathData.EaseCurve.Evaluate(animTimeRatio);
                    animTimeRatio += timeStep*Time.deltaTime;
                }

                yield return null;
            }
            // ReSharper disable once FunctionNeverReturns
        }

        // NOTE Animator.
        private Vector3[] GetGlobalRotationPointPositions() {
            var localPositions = PathData.GetRotationPointPositions();
            Vector3[] globalPositions = new Vector3[localPositions.Length];

            for (int i = 0; i < localPositions.Length; i++) {
                globalPositions[i] = transform.TransformPoint(localPositions[i]);
            }

            return globalPositions;
        }


        private void HandleAnimatedGORotation() {
            if (animatedGO == null) return;

            // Look at target.
            if (targetGO != null
                && rotationMode == AnimatorRotationMode.Target) {
                // In play mode use Quaternion.Slerp();
                if (Application.isPlaying) {
                    RotateObjectWithSlerp(targetGO.position);
                }
                // In editor mode use Transform.LookAt().
                else {
                    RotateObjectWithLookAt(targetGO.position);
                }
            }
            // Use rotation path.
            if (rotationMode == AnimatorRotationMode.Custom) {
                RotateObjectWithAnimationCurves();
            }
            // Look forward.
            else if (rotationMode == AnimatorRotationMode.Forward) {
                var globalForwardPoint = GetForwardPoint(true);

                // In play mode..
                if (Application.isPlaying) {
                    RotateObjectWithSlerp(globalForwardPoint);
                }
                else {
                    RotateObjectWithLookAt(globalForwardPoint);
                }
            }
        }

        //private void InitializeEaseCurve() {
        //    var firstKey = new Keyframe(0, 0, 0, 0);
        //    var lastKey = new Keyframe(1, 1, 0, 0);

        //    PathData.EaseCurve.AddKey(firstKey);
        //    PathData.EaseCurve.AddKey(lastKey);
        //}

        //private void InitializeRotationCurve() {
        //    var firstKey = new Keyframe(0, 0, 0, 0);
        //    var lastKey = new Keyframe(1, 0, 0, 0);

        //    PathData.TiltingCurve.AddKey(firstKey);
        //    PathData.TiltingCurve.AddKey(lastKey);
        //}


        private void RotateObjectWithAnimationCurves() {
            var lookAtTarget =
                PathData.RotationPath.GetVectorAtTime(animTimeRatio);
            // Convert target position to global coordinates.
            var lookAtTargetGlobal = transform.TransformPoint(lookAtTarget);

            // In play mode use Quaternion.Slerp();
            if (Application.isPlaying) {
                RotateObjectWithSlerp(lookAtTargetGlobal);
            }
            // In editor mode use Transform.LookAt().
            else {
                RotateObjectWithLookAt(lookAtTargetGlobal);
            }
        }

        private void RotateObjectWithLookAt(Vector3 targetPos) {
            animatedGO.LookAt(targetPos);
        }

        private void RotateObjectWithSlerp(Vector3 targetPosition) {
            // Return when point to look at is at the same position as the
            // animated object.
            if (targetPosition == animatedGO.position) return;

            // Calculate direction to target.
            var targetDirection = targetPosition - animatedGO.position;
            // Calculate rotation to target.
            var rotation = Quaternion.LookRotation(targetDirection);
            // Calculate rotation speed.
            var speed = Time.deltaTime*rotationSpeed;

            // Lerp rotation.
            animatedGO.rotation = Quaternion.Slerp(
                animatedGO.rotation,
                rotation,
                speed);
        }

        private void TiltObject() {
            if (animatedGO == null) return;

            // Get current animatedGO rotation.
            var eulerAngles = animatedGO.rotation.eulerAngles;
            // Get rotation from tiltingCurve.
            var zRotation = PathData.TiltingCurve.Evaluate(animTimeRatio);
            // Update value on Z axis.
            eulerAngles = new Vector3(eulerAngles.x, eulerAngles.y, zRotation);
            // Update animatedGO rotation.
            animatedGO.rotation = Quaternion.Euler(eulerAngles);
        }

        private void UpdateAnimatedGOPosition() {
            // Get animatedGO position at current animation time.
            var positionAtTimestamp =
                animationPathBuilder.GetVectorAtTime(animTimeRatio);
            var globalPositionAtTimestamp =
                transform.TransformPoint(positionAtTimestamp);

            // Update animatedGO position.
            animatedGO.position = globalPositionAtTimestamp;
        }

        private void UpdateAnimatedGORotation() {
            if (animatedGO == null) return;

            switch (rotationMode) {
                case AnimatorRotationMode.Forward:
                    var globalForwardPoint = GetForwardPoint(true);

                    RotateObjectWithLookAt(globalForwardPoint);

                    break;

                case AnimatorRotationMode.Custom:
                    // Get rotation point position.
                    var rotationPointPos =
                        PathData.RotationPath.GetVectorAtTime(animTimeRatio);

                    // Convert target position to global coordinates.
                    var rotationPointGlobalPos =
                        transform.TransformPoint(rotationPointPos);

                    // Update animatedGO rotation.
                    RotateObjectWithLookAt(rotationPointGlobalPos);

                    break;

                case AnimatorRotationMode.Target:
                    if (targetGO == null) return;

                    RotateObjectWithLookAt(targetGO.position);
                    break;
            }
        }

        //private void UpdateRotationPathWithRemovedKeys() {
        //    // AnimationPathBuilder node timestamps.
        //    var pathTimestamps = PathData.GetPathTimestamps();
        //    // Get values from rotationPath.
        //    var rotationCurvesTimestamps = PathData.RotationPath.GetTimestamps();

        //    // For each timestamp in rotationPath..
        //    for (var i = 0; i < rotationCurvesTimestamps.Length; i++) {
        //        // Check if same timestamp exist in rotationPath.
        //        var keyExists = pathTimestamps.Any(nodeTimestamp =>
        //            Math.Abs(rotationCurvesTimestamps[i] - nodeTimestamp)
        //            < FloatPrecision);

        //        // If key exists check next timestamp.
        //        if (keyExists) continue;

        //        // Remove node from rotationPath.
        //        PathData.RotationPath.RemoveNode(i);

        //        break;
        //    }
        //}

        #endregion PRIVATE METHODS
    }
}