using System;
using System.Collections.Generic;
using UnityEngine;

public class CustomCameraBehaviourController : MonoBehaviour
{
    #region Fields & Properties
    [Header("Parameters")]
    [SerializeField]
    private bool cameraLocked = false;
    [SerializeField]
    private int cameraLayerMask = 0;
    [SerializeField]
    private float distance = 20.0f;
    [SerializeField]
    private Vector2 maxYRotation = Vector2.zero;

    [SerializeField]
    private Vector3 currentCamPosition = Vector3.zero;
    [SerializeField]
    private Vector3 previousPosition = Vector3.zero;
    [SerializeField]
    private Vector3 targetPosition = Vector3.zero;

    [SerializeField]
    private float currentCameraTransitionTime = 0.0f;
    [SerializeField]
    private float cameraTransitionTime = 0.5f;
    [SerializeField]
    private float cameraDistanceThreshold = 0.5f;

    [SerializeField]
    private int indexCurrentCamera = 0;
    [SerializeField]
    private int previousIndexCamera = 0;
    [SerializeField]
    private CameraBehaviours currentCameraBehaviour = CameraBehaviours.FreeCam;
    public CameraBehaviours CurrentCameraBehaviour { get { return this.currentCameraBehaviour; } }

    [SerializeField]
    private CameraProperties[] cameras = null;
    [SerializeField]
    private Dictionary<CameraBehaviours, int> cameraBehaviourIndex = null;

    [Header("References")]
    [SerializeField]
    private Transform pivot = null;
    [SerializeField]
    private Transform playerTransform = null;
    [SerializeField]
    private CustomCharacterControllerController customCharacterControllerController = null;
    [SerializeField]
    private Transform weapon = null;
    [SerializeField]
    private CameraShake cameraShake = null;
    [SerializeField]
    private GameCursor gameCursor = null;

    [Header("Debug")]
    [SerializeField]
    private bool showCameraRay = true;
    #endregion

    #region Methods
    #region Intialization
    private void Awake()
	{
        this.Initialize();
	}
	
	private void Initialize()
	{
#if UNITY_EDITOR
        if (this.pivot == null)
            Debug.LogError("[Missing Reference] - pivot is missing !");
        if (this.playerTransform == null)
            Debug.LogError("[Missing Reference] - playerTransform is missing !");
        if (this.customCharacterControllerController == null)
            Debug.LogError("[Missing Reference] - customCharacterControllerController is missing !");
        if (this.weapon == null)
            Debug.LogError("[Missing Reference] - weapon is missing !");
        if (this.gameCursor == null)
            Debug.LogError("[Missing Reference] - gameCursor is missing !");

        if (this.cameras.Length == 0)
            Debug.LogError("[Missing Reference] - cameras are missing !");
#endif

        this.cameraBehaviourIndex = new Dictionary<CameraBehaviours, int>(this.cameras.Length);
        for(int i = 0; i < this.cameras.Length;i++)
        {
            CameraBehaviours behaviour = this.cameras[i].Behaviour;
            if(!this.cameraBehaviourIndex.ContainsKey(behaviour))
            {
                this.cameraBehaviourIndex.Add(behaviour, i);
            }
            else
            {
                Debug.LogError("[Camera behaviour duplicate] - More than 1 camera behaviour : " + behaviour);
            }
        }
        this.gameCursor.IsVisible = false;
        this.pivot.position = this.cameras[this.indexCurrentCamera].Pivot.position;
        this.currentCamPosition = this.playerTransform.position + ((this.pivot.rotation * this.cameras[this.indexCurrentCamera].CameraOffset) * this.cameras[this.indexCurrentCamera].Distance);
        this.cameraLocked = true;
        this.cameraLayerMask = 1 << LayerMask.NameToLayer("PlayerCollision");
    }
    #endregion

    public void CustomUpdate()
    {
        this.UpdatePosition();

        //this.transform.position = this.playerTransform.position + ((this.pivot.rotation * this.cameras[this.indexCurrentCamera].CameraOffset) * this.cameras[this.indexCurrentCamera].Distance);
        this.transform.position = this.currentCamPosition;
        this.transform.rotation = this.pivot.rotation;

        //Weapon feedback
        this.weapon.localRotation = Quaternion.Euler(new Vector3(this.pivot.rotation.eulerAngles.x, 0.0f, 0.0f));

        if (this.currentCameraBehaviour == CameraBehaviours.SprintCam)
            this.cameraShake.Shake();
    }

    private void UpdatePosition()
    {
        //Lerp between camera changes
        if (this.pivot.position != this.cameras[this.indexCurrentCamera].Pivot.position)
        {
            this.pivot.position += (this.cameras[this.indexCurrentCamera].Pivot.position - this.pivot.position) * (Vector3.Distance(this.pivot.position, this.cameras[this.indexCurrentCamera].Pivot.position) / this.cameraTransitionTime) * Time.deltaTime;
            if(Math.Abs(Vector3.Distance(this.pivot.position, this.cameras[this.indexCurrentCamera].Pivot.position)) < 0.1)
            {
                this.pivot.position = this.cameras[this.indexCurrentCamera].Pivot.position;
            }
        }

        //Raycast towards the camera position needed to know if there is a wall or smtg
        RaycastHit raycastHit;
        Ray ray = new Ray(this.playerTransform.position, (this.pivot.rotation * this.cameras[this.indexCurrentCamera].CameraOffset).normalized);
        Debug.DrawRay(this.playerTransform.position, (this.pivot.rotation * this.cameras[this.indexCurrentCamera].CameraOffset).normalized * this.cameras[this.indexCurrentCamera].Distance, Color.red);
        if (Physics.Raycast(ray, out raycastHit, this.cameras[this.indexCurrentCamera].Distance, this.cameraLayerMask))
        {
            Debug.Log("Hit, correcting trajectory");
            this.targetPosition = raycastHit.point;
        }
        else
        {
            this.targetPosition = (this.playerTransform.position + ((this.pivot.rotation * this.cameras[this.indexCurrentCamera].CameraOffset).normalized * this.cameras[this.indexCurrentCamera].Distance));
        }
        //While the camera is not at the position she needs to be, go towards this position
        if (!this.cameraLocked)
        {
            if (this.currentCamPosition != this.playerTransform.position + ((this.pivot.rotation * this.cameras[this.indexCurrentCamera].CameraOffset).normalized * this.cameras[this.indexCurrentCamera].Distance))
            {
                //Vector3 targetPosition = (this.playerTransform.position + ((this.pivot.rotation * this.cameras[this.indexCurrentCamera].CameraOffset) * this.cameras[this.indexCurrentCamera].Distance));

                this.currentCamPosition.x = Mathf.Lerp(previousPosition.x, this.targetPosition.x, this.currentCameraTransitionTime / this.cameraTransitionTime);
                this.currentCamPosition.y = Mathf.Lerp(previousPosition.y, this.targetPosition.y, this.currentCameraTransitionTime / this.cameraTransitionTime);
                this.currentCamPosition.z = Mathf.Lerp(previousPosition.z, this.targetPosition.z, this.currentCameraTransitionTime / this.cameraTransitionTime);
                //this.currentCamPosition = this.playerTransform.position;
                //this.currentCamPosition += (this.playerTransform.position + ((this.pivot.rotation * this.cameras[this.indexCurrentCamera].CameraOffset) * this.cameras[this.indexCurrentCamera].Distance) - this.currentCamPosition)
                //    * (Vector3.Distance(this.currentCamPosition, this.playerTransform.position + ((this.pivot.rotation * this.cameras[this.indexCurrentCamera].CameraOffset) * this.cameras[this.indexCurrentCamera].Distance))
                //    / this.cameraTransitionTime)
                //    * Time.deltaTime;//(this.pivot.rotation * this.cameras[this.indexCurrentCamera].CameraOffset) * (this.cameras[this.indexCurrentCamera].Distance / this.cameraTransitionTime) * Time.deltaTime;
                if (Math.Abs(Vector3.Distance(this.currentCamPosition, this.targetPosition)) <= this.cameraDistanceThreshold) /*this.playerTransform.position + ((this.pivot.rotation * this.cameras[this.indexCurrentCamera].CameraOffset) * this.cameras[this.indexCurrentCamera].Distance)))*/
                {
                    this.currentCamPosition = this.targetPosition; // this.playerTransform.position + ((this.pivot.rotation * this.cameras[this.indexCurrentCamera].CameraOffset) * this.cameras[this.indexCurrentCamera].Distance);
                    this.cameraLocked = true;
                }
                else
                {
                    this.currentCameraTransitionTime += Time.deltaTime;
                    if (this.currentCameraTransitionTime > this.cameraTransitionTime)
                        this.currentCameraTransitionTime = this.cameraTransitionTime;
                }
            }
        }
        else
        {
            this.currentCamPosition = this.targetPosition; // this.playerTransform.position + ((this.pivot.rotation * this.cameras[this.indexCurrentCamera].CameraOffset) * this.cameras[this.indexCurrentCamera].Distance);
        }
    }

    public void UpdateRotation(Vector3 rotation)
    {
        Vector3 previousRotation = this.pivot.localRotation.eulerAngles;
        this.pivot.Rotate(Vector3.right, ((this.customCharacterControllerController.InverseMouseYMovement) ? 1 : -1) * rotation.y * this.customCharacterControllerController.MouseYSensitivity * Time.deltaTime, Space.Self);

        Vector3 currentRotation = this.pivot.localRotation.eulerAngles;
        if(currentRotation.x >= 0.0f && currentRotation.x <= this.maxYRotation.x + 10.0f)
            currentRotation.x = Mathf.Clamp(this.pivot.rotation.eulerAngles.x, 0.0f, this.maxYRotation.x);
        else if(currentRotation.x <= 360.0f && currentRotation.x >= this.maxYRotation.y - 10.0f)
            currentRotation.x = Mathf.Clamp(this.pivot.rotation.eulerAngles.x, this.maxYRotation.y, 360.0f);
        this.pivot.localRotation = Quaternion.Euler(currentRotation);        
        //this.ClampRotation(previousRotation);
    }

    private void ClampRotation(Vector3 previousRotation)
    {
        //Vector3 rotation = this.pivot.localRotation.eulerAngles;

        //Debug.Log("Rotation : " + rotation);
        //if (rotation.x > this.maxYRotation.x && rotation.x <= this.maxYRotation.y)
        //{
        //    Debug.Log("Clamp !" + rotation);
        //    if (previousRotation.x < this.maxYRotation.x || previousPosition.x == this.maxYRotation.x)
        //    {
        //        rotation.x = this.maxYRotation.x;
        //        Debug.Log("Rotation Clamped x");
        //    }
        //    else if (previousRotation.x >= this.maxYRotation.y)
        //    {
        //        rotation.x = this.maxYRotation.y;
        //        Debug.Log("Rotation Clamped y");
        //    }
        //    Quaternion quaternion = new Quaternion();
        //    quaternion.eulerAngles = rotation;
        //    this.pivot.localRotation = quaternion;
        //}
    }

    public void UpdateCameraBehaviour(CameraBehaviours behaviour)
    {
        if (this.currentCameraBehaviour == CameraBehaviours.ShoulderCam && behaviour == CameraBehaviours.SprintCam)
        {
            return;
        }
        if (behaviour == CameraBehaviours.ShoulderCam)
        {
            this.gameCursor.IsVisible = true;
        }
        else
        {
            this.gameCursor.IsVisible = false;
        }

        if(this.currentCameraBehaviour == CameraBehaviours.SprintCam)
        {
            this.cameraShake.Reset();
        }

        this.cameraLocked = false;
        this.currentCameraTransitionTime = 0.0f;
        this.previousIndexCamera = this.indexCurrentCamera;
        this.currentCameraBehaviour = behaviour;
        this.indexCurrentCamera = this.cameraBehaviourIndex[behaviour];
        this.previousPosition = this.currentCamPosition;
        //this.pivot = this.cameras[this.indexCurrentCamera].Pivot;
    }

    #region Debug
    private void OnDrawGizmosSelected()
    {
        if (this.showCameraRay)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawRay(this.playerTransform.position, (this.pivot.rotation * this.cameras[this.indexCurrentCamera].CameraOffset).normalized * this.cameras[this.indexCurrentCamera].Distance);
        }
    }
    #endregion
    #endregion
}
