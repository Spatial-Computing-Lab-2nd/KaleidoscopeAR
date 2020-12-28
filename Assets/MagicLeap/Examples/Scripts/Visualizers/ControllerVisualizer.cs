// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
//
// Copyright (c) 2019-present, Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Developer Agreement, located
// here: https://auth.magicleap.com/terms/developer
//
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace MagicLeap
{
    /// <summary>
    /// Class to visualize controller inputs
    /// </summary>
    [RequireComponent(typeof(MLControllerConnectionHandlerBehavior))]
    public class ControllerVisualizer : MonoBehaviour
    {
        [System.Flags]
        public enum DeviceTypesAllowed : byte
        {
            Controller = 1,
            MobileApp = 2,
            All = Controller | MobileApp,
        }

        [SerializeField, Tooltip("The controller model.")]
        private GameObject _controllerModel = null;

        [Header("Controller Parts"), Space]
        [SerializeField, Tooltip("The controller's trigger model.")]
        private GameObject _trigger = null;

        [SerializeField, Tooltip("The controller's touchpad model.")]
        private GameObject _touchpad = null;

        [SerializeField, Tooltip("The controller's home button model.")]
        private GameObject _homeButton = null;

        [SerializeField, Tooltip("The controller's bumper button model.")]
        private GameObject _bumperButton = null;

        [SerializeField, Tooltip("The Game Object showing the touch model on the touchpad")]
        private Transform _touchIndicatorTransform = null;

        // Color when the button state is idle.
        private Color _defaultColor = Color.white;
        // Color when the button state is active.
        private Color _activeColor = Color.grey;

        private Material _touchpadMaterial = null;
        private Material _triggerMaterial = null;
        private Material _homeButtonMaterial = null;
        private Material _bumperButtonMaterial = null;

        private float _touchpadRadius;

        private MLControllerConnectionHandlerBehavior _controllerConnectionHandler = null;
        private bool _wasControllerValid = true;

        private const float MAX_TRIGGER_ROTATION = 35.0f;

        private float controllerTriggerValue;
        public float ControllerTriggerValue {
            get{ return controllerTriggerValue; }
            private set{ controllerTriggerValue = value; }
        }
        
        private float controllerTouchpadAngle;
        public float ControllerTriggerAngle {
            get{ return controllerTouchpadAngle; }
            private set{ controllerTouchpadAngle = value; }
        }

        //ControllerManager.csでtrueを取得後にfalseに変更しています。
        public bool IsControllerTouchpadOperating;
        /// <summary>
        /// Initialize variables, callbacks and check null references.
        /// </summary>
        void Start()
        {
            _controllerConnectionHandler = GetComponent<MLControllerConnectionHandlerBehavior>();

            if (!_controllerModel)
            {
                Debug.LogError("Error: ControllerVisualizer._controllerModel is not set, disabling script.");
                enabled = false;
                return;
            }
            if (!_trigger)
            {
                Debug.LogError("Error: ControllerVisualizer._trigger is not set, disabling script.");
                enabled = false;
                return;
            }
            if (!_touchpad)
            {
                Debug.LogError("Error: ControllerVisualizer._touchpad is not set, disabling script.");
                enabled = false;
                return;
            }
            if (!_homeButton)
            {
                Debug.LogError("Error: ControllerVisualizer._homeButton is not set, disabling script.");
                enabled = false;
                return;
            }
            if (!_bumperButton)
            {
                Debug.LogError("Error: ControllerVisualizer._bumperButton is not set, disabling script.");
                enabled = false;
                return;
            }
            if (!_touchIndicatorTransform)
            {
                Debug.LogError("Error: ControllerVisualizer._touchIndicatorTransform is not set, disabling script.");
                enabled = false;
                return;
            }

            SetVisibility(_controllerConnectionHandler.IsControllerValid());

            #if PLATFORM_LUMIN
            MLInput.OnControllerButtonUp += HandleOnButtonUp;
            MLInput.OnControllerButtonDown += HandleOnButtonDown;
            #endif

            _triggerMaterial = FindMaterial(_trigger);
            _touchpadMaterial = FindMaterial(_touchpad);
            _homeButtonMaterial = FindMaterial(_homeButton);
            _bumperButtonMaterial = FindMaterial(_bumperButton);

            // Calculate the radius of the touchpad's mesh
            Mesh mesh = _touchpad.GetComponent<MeshFilter>().mesh;
            _touchpadRadius = Vector3.Scale(mesh.bounds.extents, _touchpad.transform.lossyScale).x;
        }

        /// <summary>
        /// Update controller input based feedback.
        /// </summary>
        void Update()
        {
            UpdateTriggerVisuals();
            UpdateTouchpadIndicator();
            SetVisibility(_controllerConnectionHandler.IsControllerValid());
        }

        /// <summary>
        /// Stop input api and unregister callbacks.
        /// </summary>
        void OnDestroy()
        {
            #if PLATFORM_LUMIN
            MLInput.OnControllerButtonDown -= HandleOnButtonDown;
            MLInput.OnControllerButtonUp -= HandleOnButtonUp;
            #endif
        }

        /// <summary>
        /// Sets the visual pressure indicator for the appropriate button MeshRenderers.
        /// <param name="renderer">The meshrenderer to modify.</param>
        /// <param name="pressure">The pressure sensitivy interpolant for the meshrendere.r</param>
        /// </summary>
        private void SetPressure(MeshRenderer renderer, float pressure)
        {
            if (renderer.material.HasProperty("_Cutoff"))
            {
                renderer.material.SetFloat("_Cutoff", pressure);
            }
        }

        /// <summary>
        /// Update the touchpad's indicator: (location, directions, color).
        /// Also updates the color of the touchpad, based on pressure.
        /// </summary>
        private void UpdateTouchpadIndicator()
        {
            if (!_controllerConnectionHandler.IsControllerValid())
            {
                return;
            }

            #if PLATFORM_LUMIN
            MLInput.Controller controller = _controllerConnectionHandler.ConnectedController;
            Vector3 updatePosition = new Vector3(controller.Touch1PosAndForce.x, 0.0f, controller.Touch1PosAndForce.y);

            float touchY = _touchIndicatorTransform.localPosition.y;
            _touchIndicatorTransform.localPosition = new Vector3(updatePosition.x * _touchpadRadius / MLDevice.WorldScale, touchY, updatePosition.z * _touchpadRadius / MLDevice.WorldScale);

            if (controller.Touch1Active)
            {
                _touchIndicatorTransform.gameObject.SetActive(true);
                float angle = Mathf.Atan2(controller.Touch1PosAndForce.x, controller.Touch1PosAndForce.y);
                ControllerTriggerAngle = angle * Mathf.Rad2Deg;
                if (ControllerTriggerAngle<0)
                {
                    ControllerTriggerAngle += 360;
                }

                IsControllerTouchpadOperating = true;
                _touchIndicatorTransform.localRotation = Quaternion.Euler(0, angle * Mathf.Rad2Deg, 0);
            }
            else
            {
                _touchIndicatorTransform.gameObject.SetActive(false);
            }

            float force = controller.Touch1PosAndForce.z;
            _touchpadMaterial.color = Color.Lerp(_defaultColor, _activeColor, force);
            #endif
        }

        /// <summary>
        /// Update the rotation and visual color of the trigger.
        /// </summary>
        private void UpdateTriggerVisuals()
        {
            if (!_controllerConnectionHandler.IsControllerValid())
            {
                return;
            }

            MLInput.Controller controller = _controllerConnectionHandler.ConnectedController;

            #if PLATFORM_LUMIN
            // Change the color of the trigger
            _triggerMaterial.color = Color.Lerp(_defaultColor, _activeColor, controller.TriggerValue);
            ControllerTriggerValue = controller.TriggerValue;
            #endif

            // Set the rotation of the trigger
            Vector3 eulerRot = _trigger.transform.localRotation.eulerAngles;

            #if PLATFORM_LUMIN
            eulerRot.x = Mathf.Lerp(0, MAX_TRIGGER_ROTATION, controller.TriggerValue);
            #endif

            _trigger.transform.localRotation = Quaternion.Euler(eulerRot);
        }

        /// <summary>
        /// Attempt to get the Material of a GameObject.
        /// </summary>
        /// <param name="gameObject">The GameObject to search for a material.</param>
        /// <returns>Material of the GameObject, if it exists. Otherwise, null.</returns>
        private Material FindMaterial(GameObject gameObject)
        {
            MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
            return (renderer != null) ? renderer.material : null;
        }

        /// <summary>
        /// Sets the color of all Materials.
        /// </summary>
        /// <param name="color">The color to be applied to the materials.</param>
        private void SetAllMaterialColors(Color color)
        {
            _triggerMaterial.color = color;
            _touchpadMaterial.color = color;
            _homeButtonMaterial.color = color;
            _bumperButtonMaterial.color = color;
        }

        /// <summary>
        /// Coroutine to reset the home color back to the original color.
        /// </summary>
        private IEnumerator RestoreHomeColor()
        {
            yield return new WaitForSeconds(0.5f);
            _homeButtonMaterial.color = _defaultColor;
        }

        /// <summary>
        /// Set object visibility to value.
        /// </summary>
        /// <param name="value"> true or false to set visibility. </param>
        private void SetVisibility(bool value)
        {
            if (_wasControllerValid == value)
            {
                return;
            }

            Renderer[] rendererArray = _controllerModel.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer r in rendererArray)
            {
                r.enabled = value;
            }

            _wasControllerValid = value;
        }

        /// <summary>
        /// Handles the event for button down.
        /// </summary>
        /// <param name="controllerId">The id of the controller.</param>
        /// <param name="button">The button that is being pressed.</param>
        private void HandleOnButtonDown(byte controllerId, MLInput.Controller.Button button)
        {
            #if PLATFORM_LUMIN
            if (_controllerConnectionHandler.IsControllerValid() && _controllerConnectionHandler.ConnectedController.Id == controllerId &&
                button == MLInput.Controller.Button.Bumper)
            {
                // Sets the color of the Bumper to the active color.
                _bumperButtonMaterial.color = _activeColor;
            }
            #endif
        }

        /// <summary>
        /// Handles the event for button up.
        /// </summary>
        /// <param name="controllerId">The id of the controller.</param>
        /// <param name="button">The button that is being released.</param>
        private void HandleOnButtonUp(byte controllerId, MLInput.Controller.Button button)
        {
            #if PLATFORM_LUMIN
            if (_controllerConnectionHandler.IsControllerValid() && _controllerConnectionHandler.ConnectedController.Id == controllerId)
            {
                if (button == MLInput.Controller.Button.Bumper)
                {
                    // Sets the color of the Bumper to the default color.
                    _bumperButtonMaterial.color = _defaultColor;
                }

                else if (button == MLInput.Controller.Button.HomeTap)
                {
                    // Note: HomeTap is NOT a button. It's a physical button on the controller.
                    // But in the application side, the tap registers as a ButtonUp event and there is NO
                    // ButtonDown equivalent. We cannot detect holding down the Home (button). The OS will
                    // handle it as either a return to the icon grid or turning off the controller.
                    _homeButtonMaterial.color = _activeColor;
                    StartCoroutine(RestoreHomeColor());
                }
            }
            #endif
        }
    }
}
