﻿using UnityEngine;

namespace ATP.AnimationPathTools.AnimatorComponent {

    public sealed class AnimatorSettings : ScriptableObject {
        #region ADVANCED FIELDS

        [SerializeField]
        private float minNodeTimeSeparation = 0.001f;

        #endregion

        #region ADVANCED PROPERTIES 

        public float MinNodeTimeSeparation {
            get { return minNodeTimeSeparation; }
            set { minNodeTimeSeparation = value; }
        }

        #endregion

        #region SHORTCUT FIELDS

        public KeyCode playModeModKey = KeyCode.RightAlt;

        public KeyCode positionHandleKey = KeyCode.G;

        [Header("Shortcuts")]
        [SerializeField]
        private KeyCode easeModeKey = KeyCode.U;

        /// <summary>
        ///     Key shortcut to jump to the end of the animation.
        /// </summary>
        [SerializeField]
        private KeyCode jumpToEndKey = KeyCode.L;

        [SerializeField]
        private KeyCode jumpToNextNodeKey = KeyCode.L;

        [SerializeField]
        private KeyCode jumpToPreviousNodeKey = KeyCode.H;

        [SerializeField]
        private KeyCode jumpToStartKey = KeyCode.H;

        [SerializeField]
        private KeyCode longJumpBackwardKey = KeyCode.J;

        [SerializeField]
        private KeyCode longJumpForwardKey = KeyCode.K;

        [SerializeField]
        private float longJumpValue = 0.01f;

        [SerializeField]
        private EventModifiers modKey = EventModifiers.Alt;

        [SerializeField]
        private KeyCode noneModeKey = KeyCode.Y;

        [SerializeField]
        private KeyCode playPauseKey = KeyCode.Space;

        [SerializeField]
        private KeyCode rotationModeKey = KeyCode.I;

        [SerializeField]
        private KeyCode shortJumpBackwardKey = KeyCode.J;

        [SerializeField]
        private KeyCode shortJumpForwardKey = KeyCode.K;

        [SerializeField]
        private KeyCode tiltingModeKey = KeyCode.O;

        [SerializeField]
        private KeyCode updateAllKey = KeyCode.P;

        #endregion

        #region SHORTCUT PROPERTIES

        public KeyCode EaseModeKey {
            get { return easeModeKey; }
            set { easeModeKey = value; }
        }

        public KeyCode JumpToEndKey {
            get { return jumpToEndKey; }
            set { jumpToEndKey = value; }
        }

        public KeyCode JumpToNextNodeKey {
            get { return jumpToNextNodeKey; }
            set { jumpToNextNodeKey = value; }
        }

        public KeyCode JumpToPreviousNodeKey {
            get { return jumpToPreviousNodeKey; }
            set { jumpToPreviousNodeKey = value; }
        }

        public KeyCode JumpToStartKey {
            get { return jumpToStartKey; }
            set { jumpToStartKey = value; }
        }

        public KeyCode LongJumpBackwardKey {
            get { return longJumpBackwardKey; }
            set { longJumpBackwardKey = value; }
        }

        public KeyCode LongJumpForwardKey {
            get { return longJumpForwardKey; }
            set { longJumpForwardKey = value; }
        }

        public float LongJumpValue {
            get { return longJumpValue; }
            set { longJumpValue = value; }
        }

        public EventModifiers ModKey {
            get { return modKey; }
            set { modKey = value; }
        }

        public KeyCode NoneModeKey {
            get { return noneModeKey; }
            set { noneModeKey = value; }
        }

        public KeyCode PlayModeModKey {
            get { return playModeModKey; }
            set { playModeModKey = value; }
        }

        public KeyCode PlayPauseKey {
            get { return playPauseKey; }
            set { playPauseKey = value; }
        }

        public KeyCode PositionHandleKey {
            get { return positionHandleKey; }
            set { positionHandleKey = value; }
        }

        public KeyCode RotationModeKey {
            get { return rotationModeKey; }
            set { rotationModeKey = value; }
        }

        public KeyCode ShortJumpBackwardKey {
            get { return shortJumpBackwardKey; }
            set { shortJumpBackwardKey = value; }
        }

        public KeyCode ShortJumpForwardKey {
            get { return shortJumpForwardKey; }
            set { shortJumpForwardKey = value; }
        }

        public KeyCode TiltingModeKey {
            get { return tiltingModeKey; }
            set { tiltingModeKey = value; }
        }

        public KeyCode UpdateAllKey {
            get { return updateAllKey; }
            set { updateAllKey = value; }
        }

        #endregion

        #region ANIMATOR FIELDS

        [Header("Animator")]

        [SerializeField]
        private int countdownToStopFramesNo = 10;

        [SerializeField]
        private float forwardPointOffsetMinValue = 0.001f;

        /// <summary>
        ///     Holds references to icons that will be copied to Assets/Gizmos folder.
        /// </summary>
        [SerializeField]
        private Texture[] gizmoIcons;

        private string gizmosSubfolder = "ATP/";

        /// <summary>
        /// Used to convert animation curve value to arc handle value.
        /// </summary>
        [SerializeField]
        private float animationSpeedDenominator = 0.05f;

        [SerializeField]
        private float maxRotationSlerpSpeed = 20f;

        [SerializeField]
        private float minRotationSlerpSpeed = 0.1f;

        [SerializeField]
        private float minPositionLerpSpeed = 0.001f;

        [SerializeField]
        private float maxPositionLerpSpeed = 1;
        #endregion

        #region ANIMATOR PROPERTIES
        public float MinPositionLerpSpeed {
            get { return minPositionLerpSpeed; }
            set { minPositionLerpSpeed = value; }
        }

        public float MaxPositionLerpSpeed {
            get { return maxPositionLerpSpeed; }
            set { maxPositionLerpSpeed = value; }
        }

        public float MinRotationSlerpSpeed {
            get { return minRotationSlerpSpeed; }
            set { minRotationSlerpSpeed = value; }
        }

        public float MaxRotationSlerpSpeed {
            get { return maxRotationSlerpSpeed; }
            set { maxRotationSlerpSpeed = value; }
        }


        public int CountdownToStopFramesNo {
            get { return countdownToStopFramesNo; }
            set { countdownToStopFramesNo = value; }
        }

        public float ForwardPointOffsetMinValue {
            get { return forwardPointOffsetMinValue; }
            set { forwardPointOffsetMinValue = value; }
        }

        public Texture[] GizmoIcons {
            get { return gizmoIcons; }
            set { gizmoIcons = value; }
        }

        public string GizmosSubfolder {
            get { return gizmosSubfolder; }
            set { gizmosSubfolder = value; }
        }

        public float AnimationSpeedDenominator {
            get { return animationSpeedDenominator; }
            set { animationSpeedDenominator = value; }
        }

        #endregion

        #region INSPECTOR FIELDS
        [SerializeField]
        private string pathDataAssetDefaultName = "AnimationPath";

        #endregion
        #region INSPECTOR PROPERTIES
        public string PathDataAssetDefaultName {
            get { return pathDataAssetDefaultName; }
            set { pathDataAssetDefaultName = value; }
        }
        #endregion

        #region GIZMO FIELDS

        [Header("Gizmos")]
        [SerializeField]
        private string currentRotationPointGizmoIcon = "rec_16x16-yellow";

        [SerializeField]
        private string forwardPointIcon = "target_22x22-pink";

        /// <summary>
        ///     Color of the gizmo curve.
        /// </summary>
        [SerializeField]
        private Color gizmoCurveColor = Color.yellow;

        [SerializeField]
        private int gizmoCurveSamplingFrequency = 40;

        [SerializeField]
        private Color rotationCurveColor = Color.gray;

        [SerializeField]
        private int rotationCurveSampling = 40;

        [SerializeField]
        private string rotationPointGizmoIcon = "rec_16x16";

        [SerializeField]
        private string targetGizmoIcon = "target_22x22-blue";

        #endregion

        #region GIZMO PROPERTIES

        public string CurrentRotationPointGizmoIcon {
            get { return currentRotationPointGizmoIcon; }
            set { currentRotationPointGizmoIcon = value; }
        }

        public string ForwardPointIcon {
            get { return forwardPointIcon; }
            set { forwardPointIcon = value; }
        }

        /// <summary>
        ///     Color of the gizmo curve.
        /// </summary>
        public Color GizmoCurveColor {
            get { return gizmoCurveColor; }
            set { gizmoCurveColor = value; }
        }

        public int GizmoCurveSamplingFrequency {
            get { return gizmoCurveSamplingFrequency; }
            set { gizmoCurveSamplingFrequency = value; }
        }

        public Color RotationCurveColor {
            get { return rotationCurveColor; }
            set { rotationCurveColor = value; }
        }

        public int RotationCurveSampling {
            get { return rotationCurveSampling; }
            set { rotationCurveSampling = value; }
        }

        public string RotationPointGizmoIcon {
            get { return rotationPointGizmoIcon; }
            set { rotationPointGizmoIcon = value; }
        }

        public string TargetGizmoIcon {
            get { return targetGizmoIcon; }
            set { targetGizmoIcon = value; }
        }

        #endregion

        #region HANDLES FIELDS

        [Header("Handles")]
        [SerializeField]
        private int addButtonOffsetH = 25;

        //[SerializeField]
        //private float minEaseValueThreshold = 0.01f;

        [SerializeField]
        private int addButtonOffsetV = 10;

        [SerializeField]
        private float arcHandleRadius = 0.6f;

        [SerializeField]
        private int arcValueMultiplierNumerator = 360;

        [SerializeField]
        private int defaultLabelHeight = 10;

        [SerializeField]
        private int defaultLabelWidth = 30;

        [SerializeField]
        private int easeValueLabelOffsetX = -20;
        [SerializeField]
        private int easeValueLabelOffsetY = -25;
        [SerializeField]
        private float initialEaseArcValue = 5f;

        [SerializeField]
        private float movementHandleSize = 0.12f;

        [SerializeField]
        private int removeButtonH = 44;

        [SerializeField]
        private int removeButtonV = 10;

        [SerializeField]
        private Color rotationHandleColor = Color.magenta;

        [SerializeField]
        private float rotationHandleSize = 0.26f;

        [SerializeField]
        private float scaleHandleSize = 1.5f;

        [SerializeField]
        private int updateAllLabelOffsetX = -35;
        [SerializeField]
        private int updateAllLabelOffsetY = -25;
        [SerializeField]
        private string updateAllLabelText = "A";
        [SerializeField]
        private float initialTiltingArcValue = 5f;

        [SerializeField]
        private float tiltingValueMultiplierDenominator = 1;

        #endregion

        #region HANDLES PROPERTIES

        public int AddButtonOffsetH {
            get { return addButtonOffsetH; }
            set { addButtonOffsetH = value; }
        }

        public int AddButtonOffsetV {
            get { return addButtonOffsetV; }
            set { addButtonOffsetV = value; }
        }

        public float ArcHandleRadius {
            get { return arcHandleRadius; }
            set { arcHandleRadius = value; }
        }

        public int ArcValueMultiplierNumerator {
            get { return arcValueMultiplierNumerator; }
            set { arcValueMultiplierNumerator = value; }
        }

        public int DefaultLabelHeight {
            get { return defaultLabelHeight; }
            set { defaultLabelHeight = value; }
        }

        public int DefaultLabelWidth {
            get { return defaultLabelWidth; }
            set { defaultLabelWidth = value; }
        }

        public int EaseValueLabelOffsetX {
            get { return easeValueLabelOffsetX; }
            set { easeValueLabelOffsetX = value; }
        }

        public int EaseValueLabelOffsetY {
            get { return easeValueLabelOffsetY; }
            set { easeValueLabelOffsetY = value; }
        }

        public float InitialEaseArcValue {
            get { return initialEaseArcValue; }
            set { initialEaseArcValue = value; }
        }

        public float InitialTiltingArcValue {
            get { return initialTiltingArcValue; }
            set { initialTiltingArcValue = value; }
        }

        public Color MoveAllModeColor {
            get { return Color.red; }
        }

        public float MovementHandleSize {
            get { return movementHandleSize; }
            set { movementHandleSize = value; }
        }

        public Color PositionHandleColor {
            get { return Color.yellow; }
        }

        public int RemoveButtonH {
            get { return removeButtonH; }
            set { removeButtonH = value; }
        }

        public int RemoveButtonV {
            get { return removeButtonV; }
            set { removeButtonV = value; }
        }

        public Color RotationHandleColor {
            get { return rotationHandleColor; }
            set { rotationHandleColor = value; }
        }

        public float RotationHandleSize {
            get { return rotationHandleSize; }
            set { rotationHandleSize = value; }
        }

        public float ScaleHandleSize {
            get { return scaleHandleSize; }
            set { scaleHandleSize = value; }
        }

        public int UpdateAllLabelOffsetX {
            get { return updateAllLabelOffsetX; }
            set { updateAllLabelOffsetX = value; }
        }

        public int UpdateAllLabelOffsetY {
            get { return updateAllLabelOffsetY; }
            set { updateAllLabelOffsetY = value; }
        }

        public string UpdateAllLabelText {
            get { return updateAllLabelText; }
            set { updateAllLabelText = value; }
        }

        public float TiltingValueMultiplierDenominator {
            get { return tiltingValueMultiplierDenominator; }
            set { tiltingValueMultiplierDenominator = value; }
        }
        /// <summary>
        /// Minimum ease value below which ease handle drawer will set ease
        /// value back to default.
        /// </summary>
        //public float MinEaseValueThreshold {
        //    get { return minEaseValueThreshold; }
        //    set { minEaseValueThreshold = value; }
        //}

        #endregion
    }

}