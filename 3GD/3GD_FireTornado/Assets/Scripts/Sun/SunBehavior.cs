using UnityEngine;

public class SunBehavior : MonoBehaviour
{
    [SerializeField] private float angle = 18.0f;

    private void Update()
    {
        //Do a barrelroll !!!
        this.transform.Rotate(Vector3.right, this.angle * Time.deltaTime);
    }
}
