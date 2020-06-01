
[UnityEngine.ExecuteInEditMode]
class DrawProceduralRenderer : LoadableBehaviour
{
    public int VertexCount = 6;
    public int MaxVertexCount = 0;
    public UnityEngine.Material material = null;
    public UnityEngine.Material[] materials = null;
    public UnityEngine.MeshTopology meshTopology = UnityEngine.MeshTopology.Triangles;
    public UnityEngine.Rendering.CameraEvent CameraEvent = UnityEngine.Rendering.CameraEvent.AfterForwardAlpha;

    [UnityEngine.SerializeField]
    private int rendererContextIndex = 0;

    [UnityEngine.SerializeField]
    private bool drawInScene = true;

    private UnityEngine.Rendering.CommandBuffer commandBuffer = null;
    private UnityEngine.Rendering.CameraEvent usedCameraEvent = UnityEngine.Rendering.CameraEvent.AfterEverything;
    private UnityEngine.MaterialPropertyBlock materialPropertyBlock;

    [System.NonSerialized]
    CameraEventHandler cameraEventHandler;

    public UnityEngine.MaterialPropertyBlock MaterialPropertyBlock
    {
        get
        {
            if (this.materialPropertyBlock == null)
            {
                this.materialPropertyBlock = new UnityEngine.MaterialPropertyBlock();
            }

            return this.materialPropertyBlock;
        }
    }

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
        this.cameraEventHandler = CameraEventHandler.GetCameraEventHandler(this.rendererContextIndex);
        return this.cameraEventHandler != null;
    }

    protected override void Load()
    {
        this.cameraEventHandler.OnPreCullEvents += this.OnPreCullEvent;

        if (this.commandBuffer == null)
        {
            this.commandBuffer = new UnityEngine.Rendering.CommandBuffer();
            this.commandBuffer.name = this.name;
            this.AddCommandBuffer(this.CameraEvent);
        }

        base.Load();
    }

    protected override void Unload()
    {
        base.Unload();
        if (this.commandBuffer != null)
        {
            if (this.cameraEventHandler != null && this.cameraEventHandler.CameraComponent)
            {
                this.RemoveCommandBuffer();
            }

            this.commandBuffer.Release();
            this.commandBuffer = null;
        }
            // Here a tricks : a reference to a component that has been destroyed act as null on a comparaison.
        if (this.cameraEventHandler != null)
        {
            this.cameraEventHandler.OnPreCullEvents -= this.OnPreCullEvent;
        }

        this.cameraEventHandler = null;
    }

    private void OnPreCullEvent(UnityEngine.Camera camera)
    {
        // handling public change of this.CameraEvent
        if (this.usedCameraEvent != this.CameraEvent)
        {
            this.RemoveCommandBuffer();
            this.AddCommandBuffer(this.CameraEvent);
        }

        // we could have a component that do refresh only when needed but it add a bunch of code so....
        this.commandBuffer.Clear();

        if (this.VertexCount > 0)
        {
            if (this.material != null)
            {
                this.commandBuffer.DrawProcedural(this.transform.localToWorldMatrix, this.material, 0, this.meshTopology, this.VertexCount, 1, this.MaterialPropertyBlock);
            }

            int materialsCount = this.materials != null ? this.materials.Length : 0;
            int vertexCount = (this.MaxVertexCount > 0) ? System.Math.Min(this.VertexCount, this.MaxVertexCount) : this.VertexCount;
            this.materialPropertyBlock.SetFloat("_VertexCount", vertexCount);
            for (int i = 0; i < materialsCount; ++i)
            {
                this.commandBuffer.DrawProcedural(this.transform.localToWorldMatrix, this.materials[i], 0, this.meshTopology, vertexCount, 1, this.MaterialPropertyBlock);
            }
        }
    }

    private void RemoveCommandBuffer()
    {
        this.cameraEventHandler.CameraComponent.RemoveCommandBuffer(this.usedCameraEvent, this.commandBuffer);
#if UNITY_EDITOR
        if (this.drawInScene)
        {
            int sceneViewCount = UnityEditor.SceneView.sceneViews.Count;
            for (int i = 0; i < sceneViewCount; ++i)
            {
                UnityEditor.SceneView sceneView = UnityEditor.SceneView.sceneViews[i] as UnityEditor.SceneView;
                sceneView.camera.RemoveCommandBuffer(this.usedCameraEvent, this.commandBuffer);
            }
        }
#endif
    }

    private void AddCommandBuffer(UnityEngine.Rendering.CameraEvent cameraEvent)
    {
        this.usedCameraEvent = cameraEvent;
        this.cameraEventHandler.CameraComponent.AddCommandBuffer(this.usedCameraEvent, this.commandBuffer);

#if UNITY_EDITOR
        if (this.drawInScene)
        {
            int sceneViewCount = UnityEditor.SceneView.sceneViews.Count;
            for (int i = 0; i < sceneViewCount; ++i)
            {
                UnityEditor.SceneView sceneView =  UnityEditor.SceneView.sceneViews[i] as UnityEditor.SceneView;
                sceneView.camera.AddCommandBuffer(this.usedCameraEvent, this.commandBuffer);
            }
        }
#endif
    }
}
