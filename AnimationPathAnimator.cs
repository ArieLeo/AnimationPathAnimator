﻿using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using ATP.ReorderableList;
using UnityEngine;

// ReSharper disable once CheckNamespace

namespace ATP.AnimationPathTools {

    /// <summary>
    ///     Component that allows animating transforms position along predefined
    ///     Animation Paths and also animate their rotation on x and y axis in
    ///     time.
    /// </summary>
    [ExecuteInEditMode]
    public class AnimationPathAnimator : GameComponent {
        #region FIELDS

        [SerializeField]
        private bool advancedSettingsFoldout;

        /// <summary>
        ///     Transform to be animated.
        /// </summary>
        [SerializeField]
        private Transform animatedGO;

        /// Current play time represented as a number between 0 and 1.
        [SerializeField]
        private float animationTimeRatio;

        [SerializeField]
        private AnimatorGizmos animatorGizmos;

        [SerializeField]
        private int exportSamplingFrequency = 5;

        [SerializeField]
        private PathData pathData;

        [SerializeField]
        private GUISkin skin;

        /// <summary>
        ///     Transform that the <c>animatedGO</c> will be looking at.
        /// </summary>
        [SerializeField]
        private Transform targetGO;

        #endregion FIELDS

        #region OPTIONS

        [SerializeField]
        protected bool EnableControlsInPlayMode = true;

        [SerializeField]
        protected float MaxAnimationSpeed = 0.3f;

        /// <summary>
        ///     Value of the jump when modifier key is pressed.
        /// </summary>
        [SerializeField]
        private readonly float shortJumpValue = 0.002f;

        [SerializeField]
        private bool autoPlay = true;

        /// <summary>
        ///     How much look forward point should be positioned away from the
        ///     animated object.
        /// </summary>
        /// <remarks>Value is a time in range from 0 to 1.</remarks>
        [SerializeField]
        private float forwardPointOffset = 0.05f;

        [SerializeField]
        private AnimatorHandleMode handleMode =
            AnimatorHandleMode.None;

        [SerializeField]
        private AnimationPathBuilderHandleMode movementMode =
            AnimationPathBuilderHandleMode.MoveAll;

        [SerializeField]
        private float positionLerpSpeed = 0.1f;

        [SerializeField]
        private AnimatorRotationMode rotationMode =
            AnimatorRotationMode.Forward;

        [SerializeField]
        private float rotationSpeed = 3.0f;

        [SerializeField]
        private AnimationPathBuilderTangentMode tangentMode =
            AnimationPathBuilderTangentMode.Smooth;

        [SerializeField]
        private bool updateAllMode;

        [SerializeField]
        private WrapMode wrapMode = WrapMode.Clamp;

        #endregion OPTIONS

        #region PROPERTIES

        public float AnimationTimeRatio {
            get { return animationTimeRatio; }
            set {
                animationTimeRatio = value;

                // Limit animationTimeRatio in edit mode.
                if (!Application.isPlaying && animationTimeRatio > 1) {
                    animationTimeRatio = 1;
                }

                // Animate while animation is running.
                if (Application.isPlaying && IsPlaying && !Pause) {
                    Animate();
                }
                // Update animation with keys while animation is stopped or paused.
                else {
                    UpdateAnimation();
                }
            }
        }

        public AnimatorGizmos AnimatorGizmos {
            get { return animatorGizmos; }
        }

        public bool AutoPlay {
            get { return autoPlay; }
            set { autoPlay = value; }
        }

        public virtual float FloatPrecision {
            get { return 0.001f; }
        }

        public AnimatorHandleMode HandleMode {
            get { return handleMode; }
            set { handleMode = value; }
        }

        /// <summary>
        ///     If animation is currently enabled.
        /// </summary>
        /// <remarks>When it's true, it means that the EaseTime coroutine is running.</remarks>
        public bool IsPlaying { get; set; }

        public AnimationPathBuilderHandleMode MovementMode {
            get { return movementMode; }
            set { movementMode = value; }
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

        /// <summary>
        ///     Value of the jump when modifier key is pressed.
        /// </summary>
        public virtual float ShortJumpValue {
            get { return shortJumpValue; }
        }

        public GUISkin Skin {
            get { return skin; }
            set { skin = value; }
        }

        public AnimationPathBuilderTangentMode TangentMode {
            get { return tangentMode; }
            set { tangentMode = value; }
        }

        public Transform Transform { get; set; }

        public bool UpdateAllMode {
            get { return updateAllMode; }
            set { updateAllMode = value; }
        }

        public WrapMode WrapMode {
            get { return wrapMode; }
            set { wrapMode = value; }
        }

        #endregion PROPERTIES

        #region UNITY MESSAGES

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        public virtual void OnEnable() {
            Transform = GetComponent<Transform>();

            if (pathData != null) {
                PathData.RotationPointPositionChanged +=
                    PathData_RotationPointPositionChanged;

                PathData.NodePositionChanged += PathData_NodePositionChanged;

                PathData.NodeTiltChanged += PathData_NodeTiltChanged;

                PathData.PathReset += PathData_PathReset;
            }

            if (animatorGizmos == null) {
                animatorGizmos =
                    ScriptableObject.CreateInstance<AnimatorGizmos>();
            }
        }

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private void Awake() {
            skin = Resources.Load("GUISkin/default") as GUISkin;

            // Initialize animatedGO field.
            if (animatedGO == null && Camera.main.transform != null) {
                animatedGO = Camera.main.transform;
            }

            animatorGizmos = ScriptableObject.CreateInstance<AnimatorGizmos>();
        }

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        // TODO Refactor.
        private void OnDrawGizmosSelected() {
            // Return if path asset file is not assigned.
            if (PathData == null) return;

            if (rotationMode == AnimatorRotationMode.Target
                && targetGO != null) {
                AnimatorGizmos.DrawTargetIcon(targetGO.position);
            }

            if (rotationMode == AnimatorRotationMode.Forward) {
                var globalForwardPointPosition = GetGlobalForwardPoint();
                AnimatorGizmos.DrawForwardPointIcon(globalForwardPointPosition);
            }

            if (handleMode == AnimatorHandleMode.Rotation) {
                AnimatorGizmos.DrawRotationPathCurve(PathData, transform);

                AnimatorGizmos.DrawCurrentRotationPointGizmo(
                    PathData,
                    transform, AnimationTimeRatio);

                AnimatorGizmos.DrawRotationPointGizmos(
                    PathData, transform, AnimationTimeRatio);
            }

            AnimatorGizmos.DrawAnimationCurve(PathData, transform);
        }

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private void Start() {
            if (Application.isPlaying && autoPlay) {
                IsPlaying = true;

                StartEaseTimeCoroutine();
            }
        }

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private void Update() {
        }

        #endregion UNITY MESSAGES

        #region EVENT HANDLERS

        private void PathData_NodePositionChanged(object sender, EventArgs e) {
            if (Application.isPlaying) UpdateAnimation();
            if (!Application.isPlaying) Animate();
        }

        private void PathData_NodeTiltChanged(object sender, EventArgs e) {
            if (Application.isPlaying) UpdateAnimation();
            if (!Application.isPlaying) Animate();
        }

        private void PathData_PathReset(object sender, EventArgs e) {
            if (Application.isPlaying) UpdateAnimation();
            if (!Application.isPlaying) Animate();
        }

        private void PathData_RotationPointPositionChanged(
            object sender,
            EventArgs e) {
            UpdateAnimation();
        }

        #endregion

        #region ANIMATION

        public void Animate() {
            AnimateAnimatedGOPosition();
            AnimateAnimatedGORotation();
            AnimateAnimatedGOTilting();
        }

        public void StartEaseTimeCoroutine() {
            // Check for play mode.
            StartCoroutine("EaseTime");
            Debug.Log("start coroutine");
        }

        public void StopEaseTimeCoroutine() {
            StopCoroutine("EaseTime");
            Debug.Log("stop");

            // Reset animation.
            IsPlaying = false;
            Pause = false;
            AnimationTimeRatio = 0;
        }

        /// <summary>
        ///     Update animatedGO position, rotation and tilting based on current
        ///     AnimationTimeRatio.
        /// </summary>
        /// <remarks>
        ///     Used to update animatedGO with keys, in play mode.
        /// </remarks>
        public void UpdateAnimation() {
            UpdateAnimatedGOPosition();
            UpdateAnimatedGORotation();
            AnimateAnimatedGOTilting();
        }

        private void AnimateAnimatedGOPosition() {
            if (animatedGO == null) return;

            var positionAtTimestamp =
                PathData.GetVectorAtTime(AnimationTimeRatio);

            var globalPositionAtTimestamp =
                Transform.TransformPoint(positionAtTimestamp);

            // Update position.
            animatedGO.position = Vector3.Lerp(
                animatedGO.position,
                globalPositionAtTimestamp,
                positionLerpSpeed);

        }

        private void AnimateAnimatedGORotation() {
            if (animatedGO == null) return;

            // Look at target.
            if (targetGO != null
                && rotationMode == AnimatorRotationMode.Target) {

                RotateObjectWithSlerp(targetGO.position);
            }
            // Use rotation path.
            if (rotationMode == AnimatorRotationMode.Custom) {
                RotateObjectWithAnimationCurves();
            }
            // Look forward.
            else if (rotationMode == AnimatorRotationMode.Forward) {
                var globalForwardPoint = GetGlobalForwardPoint();

                RotateObjectWithSlerp(globalForwardPoint);
            }
        }

        private void AnimateAnimatedGOTilting() {
            if (animatedGO == null) return;

            // Get current animatedGO rotation.
            var eulerAngles = animatedGO.rotation.eulerAngles;
            // Get rotation from tiltingCurve.
            var zRotation = PathData.GetTiltingValueAtTime(AnimationTimeRatio);
            // Update value on Z axis.
            eulerAngles = new Vector3(eulerAngles.x, eulerAngles.y, zRotation);
            // Update animatedGO rotation.
            animatedGO.rotation = Quaternion.Euler(eulerAngles);
        }

        private IEnumerator EaseTime() {
            while (true) {
                // If animation is enabled and not paused..
                if (!Pause) {
                    // Ease time.
                    var timeStep =
                        PathData.GetEaseValueAtTime(AnimationTimeRatio);
                    AnimationTimeRatio += timeStep * Time.deltaTime;

                    if (AnimationTimeRatio > 1
                        && WrapMode == WrapMode.Once) {

                        AnimationTimeRatio = 1;
                        Pause = true;
                    }
                }

                yield return null;
            }
        }

        private void RotateObjectWithAnimationCurves() {
            var lookAtTarget =
                PathData.GetRotationAtTime(AnimationTimeRatio);
            // Convert target position to global coordinates.
            var lookAtTargetGlobal = Transform.TransformPoint(lookAtTarget);

            if (Application.isPlaying) {
                RotateObjectWithSlerp(lookAtTargetGlobal);
            }
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
            var speed = Time.deltaTime * rotationSpeed;

            // Lerp rotation.
            animatedGO.rotation = Quaternion.Slerp(
                animatedGO.rotation,
                rotation,
                speed);
        }

        private void UpdateAnimatedGOPosition() {
            // Get animatedGO position at current animation time.
            var positionAtTimestamp =
                PathData.GetVectorAtTime(AnimationTimeRatio);
            var globalPositionAtTimestamp =
                Transform.TransformPoint(positionAtTimestamp);

            // Update animatedGO position.
            animatedGO.position = globalPositionAtTimestamp;
        }

        private void UpdateAnimatedGORotation() {
            if (animatedGO == null) return;

            switch (rotationMode) {
                case AnimatorRotationMode.Forward:
                    var globalForwardPoint = GetGlobalForwardPoint();

                    RotateObjectWithLookAt(globalForwardPoint);

                    break;

                case AnimatorRotationMode.Custom:
                    // Get rotation point position.
                    var rotationPointPos =
                        PathData.GetRotationValueAtTime(AnimationTimeRatio);

                    // Convert target position to global coordinates.
                    var rotationPointGlobalPos =
                        Transform.TransformPoint(rotationPointPos);

                    // Update animatedGO rotation.
                    RotateObjectWithLookAt(rotationPointGlobalPos);

                    break;

                case AnimatorRotationMode.Target:
                    if (targetGO == null) return;

                    RotateObjectWithLookAt(targetGO.position);
                    break;
            }
        }

        #endregion

        #region HELPER METHODS

        public void UpdateWrapMode() {
            PathData.SetWrapMode(wrapMode);
        }

        // TODO Remove the globalPosition arg. and create separate method.
        private Vector3 GetForwardPoint() {
            // Timestamp offset of the forward point.
            var forwardPointDelta = forwardPointOffset;
            // Forward point timestamp.
            var forwardPointTimestamp = AnimationTimeRatio + forwardPointDelta;
            var localPosition = PathData.GetVectorAtTime(forwardPointTimestamp);

            return localPosition;
        }

        private Vector3 GetGlobalForwardPoint() {
            var localForwardPoint = GetForwardPoint();
            var globalForwardPoint = Transform.TransformPoint(localForwardPoint);

            return globalForwardPoint;
        }

        private Vector3[] GetGlobalRotationPointPositions() {
            var localPositions = PathData.GetRotationPointPositions();
            var globalPositions = new Vector3[localPositions.Length];

            for (var i = 0; i < localPositions.Length; i++) {
                globalPositions[i] = Transform.TransformPoint(localPositions[i]);
            }

            return globalPositions;
        }

        #endregion METHODS
    }

}