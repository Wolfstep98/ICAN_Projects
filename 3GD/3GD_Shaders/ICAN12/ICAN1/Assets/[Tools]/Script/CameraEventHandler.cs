
[UnityEngine.ExecuteInEditMode]
class CameraEventHandler : LoadableBehaviour
{
    [UnityEngine.SerializeField]
    int rendererContextIndex = 0;

    [System.NonSerialized]
    private static System.Collections.Generic.List<CameraEventHandler> cameraEventHandlerList = new System.Collections.Generic.List<CameraEventHandler>();
    private UnityEngine.Camera cameraComponent;

    public delegate void OnCameraEvent(UnityEngine.Camera camera);

    public event OnCameraEvent OnPreCullEvents;

    public UnityEngine.Camera CameraComponent
    {
        get { return this.cameraComponent; }
    }

    public static CameraEventHandler GetCameraEventHandler(int rendererContextIndex)
    {
        int cameraEventHandlerListCount = CameraEventHandler.cameraEventHandlerList != null ? CameraEventHandler.cameraEventHandlerList.Count : 0;
        for (int i = 0; i < cameraEventHandlerListCount; ++i)
        {
            if (CameraEventHandler.cameraEventHandlerList[i].RendererContextIndex == rendererContextIndex)
            {
                return CameraEventHandler.cameraEventHandlerList[i];
            }
        }

        return null;
    }

    public int RendererContextIndex
    {
        get { return this.rendererContextIndex; }
    }

    protected void OnPreCull()
    {
        if (!this.Loaded)
        {
            this.LoadIFN();
        }

        if (this.cameraComponent != null)
        {
            if (this.OnPreCullEvents != null)
            {
                this.OnPreCullEvents.Invoke(this.cameraComponent);
            }
        }
    }

    protected void OnEnable()
    {
        this.LoadIFN();
    }

    protected override bool ResolveDependencies()
    {
        this.cameraComponent = this.GetComponent<UnityEngine.Camera>();
        return this.cameraComponent != null;
    }

    protected override void Load()
    {
        if (CameraEventHandler.cameraEventHandlerList == null)
        {
            CameraEventHandler.cameraEventHandlerList = new System.Collections.Generic.List<CameraEventHandler>();
        }

        int cameraEventHandlerListCount = CameraEventHandler.cameraEventHandlerList.Count;
        for (int i = 0; i < cameraEventHandlerListCount; ++i)
        {
            if (CameraEventHandler.cameraEventHandlerList[i].RendererContextIndex == this.rendererContextIndex)
            {
                UnityEngine.Debug.LogErrorFormat(this.gameObject, "An object of type {0} as been already registered", this.GetType());
            }
        }

        CameraEventHandler.cameraEventHandlerList.Add(this);
        base.Load();
    }

    protected override void Unload()
    {
        base.Unload();
        int cameraEventHandlerListCount = CameraEventHandler.cameraEventHandlerList.Count;
        for (int i = 0; i < cameraEventHandlerListCount; ++i)
        {
            if (CameraEventHandler.cameraEventHandlerList[i] == this)
            {
                CameraEventHandler.cameraEventHandlerList[i] = CameraEventHandler.cameraEventHandlerList[CameraEventHandler.cameraEventHandlerList.Count - 1];
                CameraEventHandler.cameraEventHandlerList.RemoveAt(CameraEventHandler.cameraEventHandlerList.Count - 1);
                break;
            }
        }
    }
}
