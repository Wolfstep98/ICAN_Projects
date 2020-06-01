
public class LoadableBehaviour : UnityEngine.MonoBehaviour
{
    [System.NonSerialized]
    private loadState state = loadState.notLoaded;

    enum loadState
    {
        notLoaded,
        waitingForDependencies,
        loaded
    }

    public bool Loaded
    {
        get { return this.state == loadState.loaded; }
    }

    public bool LoadIFN()
    {
        if (this.state != loadState.loaded)
        {
            if (this.ResolveDependencies())
            {
                this.Load();
                UnityEngine.Debug.Assert(this.state == loadState.loaded);
            }
            else
            {
                this.state = loadState.waitingForDependencies;
                this.StartCoroutine(this.WaitForDependencies());
            }
        }

        return this.state == loadState.loaded;
    }

    public void UnloadIFN()
    {
        if (this.state == loadState.loaded)
        {
            this.Unload();
            UnityEngine.Debug.Assert(this.state == loadState.notLoaded);
        }
    }

    protected virtual void OnDestroy()
    {
        this.UnloadIFN();
    }

    protected virtual bool ResolveDependencies()
    {
        return true;
    }

    protected virtual void Load()
    {
        this.state = loadState.loaded;
    }

    protected virtual void Unload()
    {
        this.state = loadState.notLoaded;
    }

    System.Collections.IEnumerator WaitForDependencies()
    {
        do
        {
            yield return null;
        }
        while (this.state == loadState.waitingForDependencies && !this.ResolveDependencies());

        if (this.state == loadState.waitingForDependencies)
        {
            this.Load();
            UnityEngine.Debug.Assert(this.state == loadState.loaded);
        }

        yield break;
    }
}