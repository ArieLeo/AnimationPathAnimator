// Copyright (c) 2015 Bart�omiej Wo�k (bartlomiejwolk@gmail.com)
//  
// This file is part of the AnimationPath Animator extension for Unity.
// Licensed under the MIT license. See LICENSE file in the project root folder.

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AnimationPathAnimator.AnimatorComponent {

    /// <summary>
    ///     Class responsible for drawing all on scene handles.
    /// </summary>
    public static class SceneHandles {
        #region FIELDS

        /// <summary>
        ///     Minimum value below which arc handle drawer method will set the
        ///     value back to default.
        /// </summary>
        private const float MinValueThreshold = 0.1f;

        #endregion FIELDS

        #region METHDOS

        public static void DrawArcHandleLabels(
            Vector3[] nodeGlobalPositions,
            int offsetX,
            int offsetY,
            int labelWidth,
            int labelHeight,
            Func<int, float> calculateValueCallback,
            GUIStyle style) {

            var nodesNo = nodeGlobalPositions.Length;

            // For each path node..
            for (var i = 0; i < nodesNo; i++) {
                // Original value.
                var value = calculateValueCallback(i);
                // Value to display.
                var displayedValue = (Mathf.Abs(value) % 360)
                                     * Mathf.Sign(value);
                // Calculate number of full 360 deg. cycles.
                var cycles = Mathf.Floor(Mathf.Abs(value) / 360);

                // Get value to display.
                var labelText = String.Format(
                    "{1} : {0:0.0}",
                    displayedValue,
                    cycles);

                DrawNodeLabel(
                    nodeGlobalPositions[i],
                    labelText,
                    offsetX,
                    offsetY,
                    labelWidth,
                    labelHeight,
                    style);
            }
        }

        /// <summary>
        ///     Draws arc tool for each node.
        /// </summary>
        /// <param name="nodePositions">Positions to draw the tools.</param>
        /// <param name="curveValues">Values represented by the tools.</param>
        /// <param name="initialValue">
        ///     If tool value is 0 and user moves handle, this will be the initial
        ///     tool value.
        /// </param>
        /// <param name="allowNegative">
        ///     If tool should allow setting/displaying negative values.
        /// </param>
        /// <param name="arcValueMultiplier">
        ///     If set to 1, value of 1 will be represented as 1 degree.
        /// </param>
        /// <param name="arcHandleRadius">Radius of the arc.</param>
        /// <param name="scaleHandleSize">Size of the scale handle.</param>
        /// <param name="color">Color for the arc and scale handle.</param>
        /// <param name="callback">
        ///     Method used to update animation curve.
        /// </param>
        public static void DrawArcTools(
            Vector3[] nodePositions,
            float[] curveValues,
            float initialValue,
            bool allowNegative,
            float arcValueMultiplier,
            float arcHandleRadius,
            float scaleHandleSize,
            Color color,
            Action<int, float> callback) {

            // For each path node..
            for (var i = 0; i < nodePositions.Length; i++) {
                var iTemp = i;
                DrawArcTool(
                    curveValues[i],
                    allowNegative,
                    initialValue,
                    nodePositions[i],
                    arcValueMultiplier,
                    arcHandleRadius,
                    scaleHandleSize,
                    color,
                    value => callback(iTemp, value));
            }
        }

        /// <summary>
        ///     Draws position handles with predefined cap function.
        /// </summary>
        /// <param name="nodeGlobalPositions"></param>
        /// <param name="handleSize"></param>
        /// <param name="curveColor"></param>
        /// <param name="callback"></param>
        public static void DrawCustomPositionHandles(
            List<Vector3> nodeGlobalPositions,
            float handleSize,
            Color curveColor,
            Action<int, Vector3> callback) {

            // Cap function used to draw handle.
            Handles.DrawCapFunction capFunction = Handles.CircleCap;

            // For each node..
            for (var i = 0; i < nodeGlobalPositions.Count; i++) {
                var handleColor = curveColor;

                // Draw position handle.
                var newGlobalPos = DrawCustomPositionHandle(
                    nodeGlobalPositions[i],
                    handleSize,
                    handleColor,
                    capFunction);

                // If node was moved..
                if (newGlobalPos != nodeGlobalPositions[i]) {
                    callback(i, newGlobalPos);
                }
            }
        }

        public static void DrawCustomRotationHandle(
            Vector3 rotationPointGlobalPosition,
            float rotationHandleSize,
            Color rotationHandleColor,
            Action<Vector3> callback) {

            var handleSize =
                HandleUtility.GetHandleSize(rotationPointGlobalPosition);
            var sphereSize = handleSize * rotationHandleSize;

            Handles.color = rotationHandleColor;

            // Draw node's handle.
            var newGlobalPosition = Handles.FreeMoveHandle(
                rotationPointGlobalPosition,
                Quaternion.identity,
                sphereSize,
                Vector3.zero,
                Handles.SphereCap);

            if (newGlobalPosition != rotationPointGlobalPosition) {
                callback(newGlobalPosition);
            }
        }

        public static void DrawDefaultRotationHandle(
            Vector3 rotationPointGlobalPosition,
            Action<Vector3> callback) {

            // Draw node's handle.
            var newGlobalPosition = Handles.PositionHandle(
                rotationPointGlobalPosition,
                Quaternion.identity);

            if (newGlobalPosition != rotationPointGlobalPosition) {
                callback(newGlobalPosition);
            }
        }

        public static void DrawNodeButtons(
            List<Vector3> nodePositions,
            int buttonHoffset,
            int buttonVoffset,
            Action<int> callback,
            GUIStyle buttonStyle) {

            Handles.BeginGUI();

            // Draw add buttons for each node. Execute callback on button
            // press.
            for (var i = 0; i < nodePositions.Count; i++) {
                // Translate node's 3d position into screen coordinates.
                var guiPoint = HandleUtility.WorldToGUIPoint(
                    nodePositions[i]);

                // Draw button.
                var buttonPressed = DrawButton(
                    guiPoint,
                    buttonHoffset,
                    buttonVoffset,
                    15,
                    15,
                    buttonStyle);

                if (buttonPressed) {
                    callback(i);
                }
            }

            Handles.EndGUI();
        }

        public static void DrawNodeLabels(
            List<Vector3> nodeGlobalPositions,
            string[] text,
            int offsetX,
            int offsetY,
            int labelWidth,
            int labelHeight,
            GUIStyle style) {

            for (var i = 0; i < nodeGlobalPositions.Count; i++) {
                DrawNodeLabel(
                    nodeGlobalPositions[i],
                    text[i],
                    offsetX,
                    offsetY,
                    labelWidth,
                    labelHeight,
                    style);
            }
        }

        /// <summary>
        ///     Draw position handles using Unity's default movement handle.
        /// </summary>
        /// <param name="nodeGlobalPositions"></param>
        /// <param name="callback"></param>
        public static void DrawPositionHandles(
            List<Vector3> nodeGlobalPositions,
            Action<int, Vector3> callback) {

            // For each node..
            for (var i = 0; i < nodeGlobalPositions.Count; i++) {
                // Draw position handle.
                var newGlobalPos = Handles.PositionHandle(
                    nodeGlobalPositions[i],
                    Quaternion.identity);

                // If node was moved..
                if (newGlobalPos != nodeGlobalPositions[i]) {
                    callback(i, newGlobalPos);
                }
            }
        }

        /// <summary>
        ///     For each node in the scene draw handle that allow manipulating
        ///     tangents for each of the animation curves separately.
        /// </summary>
        /// <returns>True if any handle was moved.</returns>
        public static void DrawTangentHandles(
            List<Vector3> nodes,
            Color handleColor,
            float handleSizeConst,
            Action<int, Vector3> callback) {

            Handles.color = handleColor;

            // For each node..
            for (var i = 0; i < nodes.Count; i++) {
                var handleSize = HandleUtility.GetHandleSize(nodes[i]);
                var sphereSize = handleSize * handleSizeConst;

                // draw node's handle.
                var newHandleValue = Handles.FreeMoveHandle(
                    nodes[i],
                    Quaternion.identity,
                    sphereSize,
                    Vector3.zero,
                    Handles.CircleCap);

                // How much tangent's value changed in this frame.
                var tangentDelta = newHandleValue - nodes[i];

                // Remember if handle was moved.
                if (tangentDelta != Vector3.zero) {
                    callback(i, tangentDelta);
                }
            }
        }

        public static void DrawUpdateAllLabels(
            List<Vector3> nodeGlobalPositions,
            string[] labelText,
            int offsetX,
            int offsetY,
            int labelWidth,
            int labelHeight,
            GUIStyle style) {

            DrawNodeLabels(
                nodeGlobalPositions,
                labelText,
                offsetX,
                offsetY,
                labelWidth,
                labelHeight,
                style);
        }

        private static void DrawArcHandle(
            Vector3 position,
            Color handleColor,
            float displayedValue,
            float arcRadius) {

            Handles.color = handleColor;

            Handles.DrawWireArc(
                position,
                Vector3.up,
                Quaternion.AngleAxis(
                    0,
                    Vector3.up) * Vector3.forward,
                displayedValue,
                arcRadius);
        }

        /// <summary>
        ///     Draw handle for changing tilting value.
        /// </summary>
        /// <param name="value">Handle value.</param>
        /// <param name="position">Position to display handle.</param>
        /// <param name="initialValue"></param>
        /// <param name="scaleHandleSize">Handle size.</param>
        /// <param name="arcRadius">Position offset.</param>
        /// <param name="handleColor">Handle color.</param>
        /// <returns></returns>
        private static float DrawArcScaleHandle(
            float value,
            Vector3 position,
            float initialValue,
            float scaleHandleSize,
            float arcRadius,
            Color handleColor) {

            Handles.color = handleColor;

            // Calculate size of the scale handle.
            var handleSize = HandleUtility.GetHandleSize(position);
            var scaleSize = handleSize * scaleHandleSize;

            // Calculate displayed value.
            var handleValue = value % 360;

            // Set initial handle value. Without it, after reseting handle
            // value, the value would change really slow.
            handleValue = Math.Abs(handleValue) < MinValueThreshold
                ? initialValue
                : handleValue;

            // Draw handle.
            var newArcValue = Handles.ScaleValueHandle(
                handleValue,
                position + Vector3.forward * arcRadius * 1.3f,
                Quaternion.identity,
                scaleSize,
                Handles.ConeCap,
                1);

            // If value was changed with handle..
            if (!Utilities.FloatsEqual(
                handleValue,
                newArcValue,
                GlobalConstants.FloatPrecision)) {

                // Calculate modulo from value.
                var modValue = newArcValue % 360;

                return modValue;
            }

            // Return same value.
            return value;
        }

        /// <summary>
        ///     Draw arc handle.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="allowNegative"></param>
        /// <param name="initialValue"></param>
        /// <param name="position">Arc position.</param>
        /// <param name="arcValueMultiplier"></param>
        /// <param name="arcHandleRadius">Radius of the arc.</param>
        /// <param name="scaleHandleSize"></param>
        /// <param name="handleColor">Handle color.</param>
        /// <param name="callback">
        ///     Callback that will be executed when arc value changes. It takes
        ///     changed value as an argument.
        /// </param>
        private static void DrawArcTool(
            float value,
            bool allowNegative,
            float initialValue,
            Vector3 position,
            float arcValueMultiplier,
            float arcHandleRadius,
            float scaleHandleSize,
            Color handleColor,
            Action<float> callback) {

            // Calculate value to display.
            var arcValue = value * arcValueMultiplier;
            // Limit value to 360.
            var limitedValue = arcValue % 360;

            var handleSize = HandleUtility.GetHandleSize(position);
            var arcRadius = handleSize * arcHandleRadius;

            DrawArcHandle(
                position,
                handleColor,
                limitedValue,
                arcRadius);

            var newArcValue = DrawArcScaleHandle(
                arcValue,
                position,
                initialValue,
                scaleHandleSize,
                arcRadius,
                handleColor);

            SaveArcValue(
                arcValue,
                newArcValue,
                allowNegative,
                arcValueMultiplier,
                callback);
        }

        private static bool DrawButton(
            Vector2 position,
            int relativeXPos,
            int relativeYPos,
            int width,
            int height,
            GUIStyle style,
            string buttonText = "") {

            // Create rectangle for the "+" button.
            var rectAdd = new Rect(
                position.x + relativeXPos,
                position.y + relativeYPos,
                width,
                height);

            // Draw the "+" button.
            var addButtonPressed = GUI.Button(rectAdd, buttonText, style);

            return addButtonPressed;
        }

        private static Vector3 DrawCustomPositionHandle(
            Vector3 nodePosition,
            float handleSize,
            Color handleColor,
            Handles.DrawCapFunction capFunction) {

            // Set handle color.
            Handles.color = handleColor;

            // Get handle size.
            var movementHandleSize = HandleUtility.GetHandleSize(nodePosition);
            var sphereSize = movementHandleSize * handleSize;

            // Draw handle.
            var newPos = Handles.FreeMoveHandle(
                nodePosition,
                Quaternion.identity,
                sphereSize,
                Vector3.zero,
                capFunction);

            return newPos;
        }

        private static void DrawNodeLabel(
            Vector3 nodeGlobalPosition,
            string value,
            int offsetX,
            int offsetY,
            int labelWidth,
            int labelHeight,
            GUIStyle style) {

            // Translate node's 3d position into screen coordinates.
            var guiPoint = HandleUtility.WorldToGUIPoint(nodeGlobalPosition);

            // Create rectangle for the label.
            var labelPosition = new Rect(
                guiPoint.x + offsetX,
                guiPoint.y + offsetY,
                labelWidth,
                labelHeight);

            Handles.BeginGUI();

            // Draw label.
            GUI.Label(
                labelPosition,
                value,
                style);

            Handles.EndGUI();
        }

        /// <summary>
        ///     Convert arc value to be saved in animation curve.
        /// </summary>
        /// <param name="arcValue"></param>
        /// <param name="newArcValue"></param>
        /// <param name="allowNegative">
        ///     If negative values can be saved to the animation curve.
        /// </param>
        /// <param name="arcValueMultiplier"></param>
        /// <param name="callback">
        ///     Method used to save value to animationpath.
        /// </param>
        private static void SaveArcValue(
            float arcValue,
            float newArcValue,
            bool allowNegative,
            float arcValueMultiplier,
            Action<float> callback) {

            // Limit old value to 360.
            var modArcValue = arcValue % 360;
            var modNewArcValue = newArcValue % 360;

            // Return if value wasn't changed. todo move this up.
            if (Utilities.FloatsEqual(
                modArcValue,
                modNewArcValue,
                GlobalConstants.FloatPrecision)) return;

            var diff = Utilities.CalculateDifferenceBetweenAngles(
                modArcValue,
                modNewArcValue);

            var resultValue = arcValue + diff;

            // Convert value in degrees to back curve value.
            var curveValue = resultValue / arcValueMultiplier;

            // Handle allowNegative parameter.
            if (!allowNegative && curveValue < 0) curveValue = 0;

            // Save value to animation curve.
            callback(curveValue);
        }

        #endregion METHDOS
    }

}