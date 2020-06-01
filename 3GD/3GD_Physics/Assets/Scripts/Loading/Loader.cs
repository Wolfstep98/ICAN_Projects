using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour 
{
    #region Fields & Properties

    [Header("Parameters")]
    [SerializeField]
    private float minimumLoadingTime = 2.0f;
    [SerializeField]
    private float rotationSpeed = 5.0f;

    [Header("Data")]
    [SerializeField]
    private bool continueLoader = false;
    [SerializeField]
    private Coroutine coroutine = null;

    [Header("References")]
    [SerializeField]
    private RectTransform logo = null;
	#endregion

	#region Methods
    public void StartLoader()
    {
        this.continueLoader = true;
        this.coroutine = StartCoroutine(this.Loading(this.minimumLoadingTime));
    }

    private IEnumerator Loading(float minimumLoadingTime)
    {
        float currentTime = 0.0f;
        while(this.continueLoader || currentTime < minimumLoadingTime)
        {
            this.logo.Rotate(Vector3.forward, this.rotationSpeed * Time.deltaTime);
            yield return null;
            currentTime += Time.deltaTime;
        }
        this.gameObject.SetActive(false);
        this.coroutine = null;
    }

    public void StopLoader()
    {
        this.continueLoader = false;
    }
	#endregion
}
