using UnityEngine;

namespace Game
{

    public class PlayerController : MonoBehaviour
    {
        #region Fields
        [SerializeField] private Vector4 directions;
        [SerializeField] private float speed = 0.5f;
        [SerializeField] private float rotationMultiplier = 100.0f;
        [SerializeField] private bool[] upDir = null;
        [SerializeField] private bool[] downDir = null;
        [SerializeField] private bool[] leftDir = null;
        [SerializeField] private bool[] rightDir = null;
        [SerializeField] private bool inverseRot;
        [SerializeField] private bool[] rotationData = null;
        #endregion

        #region Methods
        public void CustomAwake()
        {
            //this.upDir = new bool[4];
            //this.downDir = new bool[4];
            //this.leftDir = new bool[4];
            //this.rightDir = new bool[4];
        }

        public void CustomUpdate()
        {

        }
        
        public void Move()
        {
            float up = this.ParseDirectionData("up");
            float down = this.ParseDirectionData("down");
            float right = this.ParseDirectionData("right");
            float left = this.ParseDirectionData("left");
            float rot = this.ParseDirectionData("rot") * ((inverseRot) ? -1 : 1);
            
            //Vector2 left = new Vector2(this.directions.x, 0);
            ////Vector2 right = new Vector2(this.directions.y, 0);
            //Vector2 upRight = new Vector2(this.directions.x, this.directions.y);
            //Vector2 downLeft = new Vector2(this.directions.z, this.directions.w);
            //Vector3 direction = this.transform.up + this.transform.TransformDirection((Vector3)(upRight + downLeft));

            Vector3 direction = this.transform.up + this.transform.TransformDirection(new Vector3(up - down, right - left));

            this.transform.position += direction * this.speed * direction.magnitude * Time.deltaTime;
            this.transform.Rotate(Vector3.forward, rot * this.rotationMultiplier * Time.deltaTime);
        }

        private float ParseDirectionData(string dir)
        {
            switch(dir)
            {
                case "up":
                    for(int i = 0; i < 4; i++)
                    {
                        if(this.upDir[i])
                        {
                            return this.ParseDirection(this.directions[i]);
                        }
                    }
                    break;
                case "down":
                    for (int i = 0; i < 4; i++)
                    {
                        if (this.downDir[i])
                        {
                            return this.ParseDirection(this.directions[i]);
                        }
                    }
                    break;
                case "left":
                    for (int i = 0; i < 4; i++)
                    {
                        if (this.leftDir[i])
                        {
                            return this.ParseDirection(this.directions[i]);
                        }
                    }
                    break;
                case "right":
                    for (int i = 0; i < 4; i++)
                    {
                        if (this.rightDir[i])
                        {
                            return this.ParseDirection(this.directions[i]);
                        }
                    }
                    break;
                case "rot":
                    for (int i = 0; i < 4; i++)
                    {
                        if (this.rotationData[i])
                        {
                            return this.ParseDirection(this.directions[i]);
                        }
                    }
                    break;
                default:
                    return -1.0f;
            }
            return -1.0f;
        }

        private float ParseDirection(float data)
        {
            return Mathf.Lerp(-1.0f, 1.0f, data);
        }

        public void SetDirection(Vector4 directions)
        {
            this.directions = directions;
        }
        #endregion

        #region Debug
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.black;
            Vector2 upRight = new Vector2(this.directions.x, this.directions.y);
            Vector2 downLeft = new Vector2(this.directions.z, this.directions.w);
            Vector3 direction = this.transform.up + this.transform.TransformDirection((Vector3)(upRight + downLeft));
            Gizmos.DrawRay(this.transform.position, new Vector3(direction.x, 0, 0));
            Gizmos.DrawRay(this.transform.position, new Vector3(0, direction.y, 0));
            Gizmos.DrawRay(this.transform.position, new Vector3(0, 0, direction.z));
            //Gizmos.DrawRay(this.transform.position, new Vector3(this.directions.w, 0, 0));
        }
        #endregion
    }
}