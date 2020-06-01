
public class DebugGOMouseMover : UnityEngine.MonoBehaviour
{
    [UnityEngine.SerializeField]
    private UnityEngine.Camera usedCamera = null;

    private System.WeakReference currentGameObjectUnderMouse = new System.WeakReference(null);
    private System.WeakReference currentPickedGameObject = new System.WeakReference(null);
    private System.WeakReference previousPickedGameObject = new System.WeakReference(null);
    private UnityEngine.Vector3 lastMousePosition;

    public void Update()
    {
        if (this.usedCamera == null)
        {
            this.usedCamera = this.GetComponent<UnityEngine.Camera>();
            return;
        }

        {
            this.currentGameObjectUnderMouse.Target = this.GetCurrentGOUnderTheMouse();
        }

        if (UnityEngine.Input.GetMouseButtonDown(0))
        {
            UnityEngine.GameObject raycastGO = this.currentGameObjectUnderMouse.Target as UnityEngine.GameObject;
            if (raycastGO != null)
            {
                if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.LeftControl))
                {
                    UnityEngine.GameObject newGO = UnityEngine.GameObject.Instantiate<UnityEngine.GameObject>(raycastGO);
                    newGO.transform.parent = raycastGO.transform.parent;
                    this.currentPickedGameObject.Target = raycastGO;
                }
                else
                {
                    this.currentPickedGameObject.Target = raycastGO;
                }

                this.previousPickedGameObject.Target = this.currentPickedGameObject.Target;
            }
        }

        if (UnityEngine.Input.GetMouseButton(0))
        {
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.LeftShift))
            {
                if (this.previousPickedGameObject.Target != null && this.previousPickedGameObject.IsAlive)
                {
                    UnityEngine.GameObject go = this.previousPickedGameObject.Target as UnityEngine.GameObject;
                    UnityEngine.Vector3 mouseProjectedPosOnPlaneY0 = this.MouseProjectedPosOnPlaneY(go.transform.position.y);
                    go.transform.LookAt(mouseProjectedPosOnPlaneY0);
                }
            }
            else if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.LeftAlt))
            {
                if (this.currentPickedGameObject.Target != null && this.currentPickedGameObject.IsAlive)
                {
                    UnityEngine.GameObject go = this.currentPickedGameObject.Target as UnityEngine.GameObject;
                    float deltaMouseY = UnityEngine.Input.mousePosition.y - this.lastMousePosition.y;
                    UnityEngine.Vector3 goPosition = go.transform.position;
                    goPosition.y += deltaMouseY * 0.01f;
                    go.transform.position = goPosition;
                }
            }
            else
            {
                if (this.currentPickedGameObject.Target != null && this.currentPickedGameObject.IsAlive)
                {
                    UnityEngine.GameObject go = this.currentPickedGameObject.Target as UnityEngine.GameObject;
                    go.transform.position = this.MouseProjectedPosOnPlaneY(go.transform.position.y);
                }
            }
        }
        else
        {
            this.currentPickedGameObject.Target = null;
        }

        if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Delete))
        {
            if (this.currentPickedGameObject.Target != null && this.currentPickedGameObject.IsAlive)
            {
                if (this.previousPickedGameObject.Target == this.currentPickedGameObject.Target)
                {
                    this.previousPickedGameObject.Target = null;
                }

                if (this.currentGameObjectUnderMouse.Target == this.currentPickedGameObject.Target)
                {
                    this.currentGameObjectUnderMouse.Target = null;
                }

                UnityEngine.GameObject.DestroyImmediate(this.currentPickedGameObject.Target as UnityEngine.GameObject);
                this.currentPickedGameObject.Target = null;
            }
            else if (this.currentGameObjectUnderMouse.Target != null && this.currentGameObjectUnderMouse.IsAlive)
            {
                if (this.previousPickedGameObject.Target == this.currentGameObjectUnderMouse.Target)
                {
                    this.previousPickedGameObject.Target = null;
                }

                UnityEngine.GameObject.DestroyImmediate(this.currentGameObjectUnderMouse.Target as UnityEngine.GameObject);
                this.currentGameObjectUnderMouse.Target = null;
            }
        }


        if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Space) && this.currentGameObjectUnderMouse.Target != null && this.currentGameObjectUnderMouse.IsAlive)
        {    
            UnityEngine.GameObject go = this.currentGameObjectUnderMouse.Target as UnityEngine.GameObject;
            IDebugGOMouseMoverEventReceiver eventReceiver = go.GetComponent<IDebugGOMouseMoverEventReceiver>();
            if (eventReceiver != null)
            {
                eventReceiver.OnSpace();
            }
        }

        this.lastMousePosition = UnityEngine.Input.mousePosition;
    }

    private UnityEngine.GameObject GetCurrentGOUnderTheMouse()
    {
        UnityEngine.Ray mouseRay = this.usedCamera.ScreenPointToRay(UnityEngine.Input.mousePosition);
        UnityEngine.RaycastHit[] rayCastHit = UnityEngine.Physics.RaycastAll(mouseRay);
        if (rayCastHit != null && rayCastHit.Length > 0)
        {
            UnityEngine.GameObject raycastGO = null; // ;
            UnityEngine.Vector3 cameraPos = this.usedCamera.transform.position;
            float bestDistanceToCamera = 0;
            for (int i = 0; i < rayCastHit.Length; ++i)
            {
                UnityEngine.GameObject go = rayCastHit[i].collider.gameObject;
                UnityEngine.Vector3 objectPos = go.transform.position;
                float distanceToCamera = (objectPos - cameraPos).magnitude;
                if (distanceToCamera < bestDistanceToCamera || raycastGO == null)
                {
                    bestDistanceToCamera = distanceToCamera;
                    raycastGO = go;
                }
            }

            return raycastGO;
        }

        return null;
    }

    private UnityEngine.Vector3 MouseProjectedPosOnPlaneY(float y)
    {
        if (this.usedCamera != null)
        {
            UnityEngine.Vector3 mousePosition = UnityEngine.Input.mousePosition;
            UnityEngine.Camera camera = this.usedCamera;
            UnityEngine.Ray ray = camera.ScreenPointToRay(mousePosition);

            return ray.origin - ray.direction * ((ray.origin.y - y) / ray.direction.y);
        }
        else
        {
            return UnityEngine.Vector3.zero;
        }
    }

    private UnityEngine.Vector3 MouseProjectedPosOnPlaneY(UnityEngine.Camera camera, float y, out bool collide)
    {
        if (camera == null)
        {
            collide = false;
            return UnityEngine.Vector3.zero;
        }

        UnityEngine.Vector3 mousePosition = UnityEngine.Input.mousePosition;
        UnityEngine.Ray ray = camera.ScreenPointToRay(mousePosition);

        UnityEngine.Vector3 result = UnityEngine.Vector3.zero;

        collide = ray.direction.y != 0;
        if (collide)
        {
            float factor = (y - ray.origin.y) / ray.direction.y;
            collide = factor > 0;
            if (collide)
            {
                result = ray.origin + ray.direction * factor;
            }
        }

        return result;
    }
}