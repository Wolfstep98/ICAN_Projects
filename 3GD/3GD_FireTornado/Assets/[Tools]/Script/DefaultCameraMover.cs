
public class DefaultCameraMover : LoadableBehaviour
{
    public float MainSpeed = 40.0f; // regular speed
    public float RotationCamSmoothTime = 0.05f;
    public float MaxZoom = 50.0f;
    public float ZoomCamSensitivity = 0.05f;
    public float MouseWheelZoomCamSensitivity = 1f;
    public float MouseWheelZoomCamSensitivityWithCollider = 0.5f;

    private UnityEngine.Camera usedCamera;
    private UnityEngine.Vector3 lastAnchorPosition;
    private UnityEngine.Transform anchor;
    private bool lastFrameMouse2 = false;
    private UnityEngine.Vector3 mouseGrabDirection;
    private UnityEngine.Vector3 mouseGrabAngleAzimuthVelocity;

    protected void OnEnable()
    {
        this.LoadIFN();
    }

    protected void OnDisable()
    {
        this.UnloadIFN();
    }

    protected override bool ResolveDependencies()
    {
        this.usedCamera = this.GetComponent<UnityEngine.Camera>();
        return this.usedCamera != null;
    }

    protected override void Load()
    {
        this.lastFrameMouse2 = false;
        base.Load();
    }

    protected override void Unload()
    {
        base.Unload();
        this.usedCamera = null;
    }

    protected void LateUpdate() 
    {
        if (!this.Loaded || this.usedCamera == null)
        {
            return;
        }

        // bouton du milieu. We use a "lastFrameMouse2" because in editor mode we have large mouse move for the first frame on game view focus.
        if (UnityEngine.Input.GetMouseButton(2))
        {
            UnityEngine.Vector2 mousePosition = UnityEngine.Input.mousePosition;
            UnityEngine.Ray ray = this.usedCamera.ScreenPointToRay(new UnityEngine.Vector3(mousePosition.x, mousePosition.y, 0));
            if (!this.lastFrameMouse2)
            {
                this.mouseGrabDirection = ray.direction;
                this.mouseGrabAngleAzimuthVelocity = UnityEngine.Vector3.zero;
            }
            else
            {

                UnityEngine.Vector2 ray2DForAzimuth = new UnityEngine.Vector2(ray.direction.z, ray.direction.x);
                float ray2DForAzimuthLength = ray2DForAzimuth.magnitude;
                ray2DForAzimuth = ray2DForAzimuth / ray2DForAzimuthLength;
                UnityEngine.Vector2 mouseGrabDirection2DForAzimuth = new UnityEngine.Vector2(this.mouseGrabDirection.z, this.mouseGrabDirection.x);
                float mouseGrabDirection2DForAzimuthLength = mouseGrabDirection2DForAzimuth.magnitude;
                mouseGrabDirection2DForAzimuth = mouseGrabDirection2DForAzimuth / mouseGrabDirection2DForAzimuthLength;
                float crossAzimuth = ray2DForAzimuth.x * mouseGrabDirection2DForAzimuth.y - ray2DForAzimuth.y * mouseGrabDirection2DForAzimuth.x;
                UnityEngine.Vector3 currentEuler = transform.eulerAngles;
                UnityEngine.Vector3 newEuler = currentEuler;
                if (crossAzimuth != 0)
                {
                    float angleInDeg = 180.0f * UnityEngine.Mathf.Asin(crossAzimuth) / UnityEngine.Mathf.PI;
                    currentEuler.y += angleInDeg;
                }

                float crossSite = ray.direction.y * mouseGrabDirection2DForAzimuthLength - ray2DForAzimuthLength * this.mouseGrabDirection.y;
                if (crossSite != 0)
                {
                    float angleInDeg = 180.0f * UnityEngine.Mathf.Asin(crossSite) / UnityEngine.Mathf.PI;
                    currentEuler.x += angleInDeg;
                }

                newEuler = UnityEngine.Vector3.SmoothDamp(currentEuler, newEuler, ref this.mouseGrabAngleAzimuthVelocity, this.RotationCamSmoothTime, 100, UnityEngine.Time.smoothDeltaTime);
                newEuler.z = 0;
                if (this.anchor != null && !UnityEngine.Input.GetKey(UnityEngine.KeyCode.LeftShift))
                {
                    UnityEngine.Vector3 anchorPositionInLocal = this.transform.worldToLocalMatrix.MultiplyPoint(this.anchor.position);
                    transform.eulerAngles = newEuler;
                    UnityEngine.Vector3 transformedAnchorPositionInLocal = this.transform.localToWorldMatrix.MultiplyVector(anchorPositionInLocal);
                    this.transform.position = this.anchor.position - transformedAnchorPositionInLocal;
                }
                else
                {
                    transform.eulerAngles = newEuler;
                }
            }
        }

        float mouseScrollWheel = UnityEngine.Input.GetAxis("Mouse ScrollWheel");
        if (mouseScrollWheel != 0)
        {  
            UnityEngine.Vector3 zoomDirection = new UnityEngine.Vector3(0, 0, 1);
            UnityEngine.Vector3 mousePosition = UnityEngine.Input.mousePosition;
            UnityEngine.Ray ray = this.usedCamera.ScreenPointToRay(mousePosition);
            float zoomDistance = 0;

            UnityEngine.RaycastHit[] rayCastHit = UnityEngine.Physics.RaycastAll(ray);

            if (rayCastHit != null && rayCastHit.Length > 0)
            {
                UnityEngine.GameObject raycastGO = rayCastHit[0].collider.gameObject;
                UnityEngine.Vector3 posToTarget = rayCastHit[0].point - this.transform.position;
                float posToTargetLength = posToTarget.magnitude;
                UnityEngine.Vector3 zoomDirectionInWorld = posToTarget / posToTargetLength;
                zoomDirection = this.transform.worldToLocalMatrix.MultiplyVector(zoomDirectionInWorld);

                zoomDistance = posToTargetLength * this.MouseWheelZoomCamSensitivityWithCollider * mouseScrollWheel;

                if (raycastGO.transform != this.anchor)
                {
                    this.anchor = raycastGO.transform;
                    this.lastAnchorPosition = this.anchor.transform.position;
                }
            }
            else
            {
                zoomDirection = this.transform.worldToLocalMatrix.MultiplyVector(ray.direction).normalized;
                zoomDistance = this.MouseWheelZoomCamSensitivity * mouseScrollWheel;
            }

            UnityEngine.Vector3 zoom = zoomDirection * zoomDistance;
            transform.Translate(zoom);
        }

        UnityEngine.Vector3 propulsion = this.GetBaseKeyboardMove();

        if (propulsion != UnityEngine.Vector3.zero)
        {
            this.anchor = null;
            this.lastAnchorPosition = UnityEngine.Vector3.zero;
        }

        UnityEngine.Vector3 keyboardMoveThisFrame = propulsion * this.MainSpeed * UnityEngine.Time.deltaTime;

        UnityEngine.Vector3 worldTranslation = transform.localToWorldMatrix.MultiplyVector(keyboardMoveThisFrame);
        worldTranslation.y = 0;
        if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.A))
        {
            worldTranslation += new UnityEngine.Vector3(0, -1, 0);
        }

        if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.E))
        {
            worldTranslation += new UnityEngine.Vector3(0, 1, 0);
        }
        
        transform.position = transform.position + worldTranslation;

        if (this.anchor != null)
        {
            this.transform.position += this.anchor.position - this.lastAnchorPosition;
            this.lastAnchorPosition = this.anchor.position;
        }

        this.lastFrameMouse2 = UnityEngine.Input.GetMouseButton(2);
    }

    private UnityEngine.Vector3 GetBaseKeyboardMove() 
    {
        UnityEngine.Vector3 velocity = new UnityEngine.Vector3();
        if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.Z) || UnityEngine.Input.GetKey(UnityEngine.KeyCode.UpArrow))
        {
            velocity += new UnityEngine.Vector3(0, 0, 1);
        }

        if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.S) || UnityEngine.Input.GetKey(UnityEngine.KeyCode.DownArrow))
        {
            velocity += new UnityEngine.Vector3(0, 0, -1);
        }

        if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.Q) || UnityEngine.Input.GetKey(UnityEngine.KeyCode.LeftArrow))
        {
            velocity += new UnityEngine.Vector3(-1, 0, 0);
        }

        if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.D) || UnityEngine.Input.GetKey(UnityEngine.KeyCode.RightArrow))
        {
            velocity += new UnityEngine.Vector3(1, 0, 0);
        }

        return velocity;
    }
}
