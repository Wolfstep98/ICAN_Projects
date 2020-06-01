using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour, IEntity
{
    #region Enums
    public enum InputType
    {
        MultiDirections,
        Sismology,
        AngleDir,
        Color
    }
    #endregion

    #region Fields
    [Header("References")]
    [SerializeField] private new SpriteRenderer renderer = null;


    [SerializeField] private InputType inputType = InputType.MultiDirections;
    [SerializeField] private float[] data;
    [SerializeField] private Vector2 direction = Vector2.zero;
    [SerializeField] private float speed = 1.0f;

    [Header("MultiDirection")]
    [SerializeField] private Vector3 mainDirection = Vector3.zero;
    [SerializeField] private Vector4 directions;
    [SerializeField] private float rotationMultiplier = 100.0f;
    [SerializeField] private bool[] upDir = null;
    [SerializeField] private bool[] downDir = null;
    [SerializeField] private bool[] leftDir = null;
    [SerializeField] private bool[] rightDir = null;
    [SerializeField] private bool inverseRot;
    [SerializeField] private bool[] rotationData = null;
    [SerializeField] private float colorChangeOverTime = 1.0f;
    private static float colorEvolution = 0.5f;
    [SerializeField] private Color colorMinusOne = Color.blue;
    [SerializeField] private Color colorOne = Color.red;
    private static Color multiDirMinusOne = Color.blue;
    private static Color multiDirOne = Color.red;
    private static Color highestColor = Color.blue;

    [SerializeField] private float startScale = 1.0f;
    [SerializeField] private float endScale = 10.0f;


    private static int colorIndex = 0;
    private static float highestValue = 0.0f;
    private static float colorValue = 0.0f;
    private static Color currentColor = Color.blue;
    private static readonly Color blue = Color.blue;
    private static readonly Color red = Color.red;
    private static readonly Color green = Color.green;

    [Header("Sismology")]
    [SerializeField] private TrailRenderer trailRenderer = null;
    [SerializeField] private AnimationCurve curve = null;
    [SerializeField] private float maxValue = 0.0f;
    [SerializeField] private float maxValueMultiplicator = 1.0f;
    private float decreaseRate = 1.0f;
    private float initialXPos = 0.0f;
    private float timer = 0.0f;
    [SerializeField] private float timerMultiplicator = 1.0f;
    [SerializeField] private float entityYSpeed = 5.0f;
    [SerializeField] private float entityXMultiplicator = 2.0f;
    [SerializeField] private Color slowColor = Color.blue;
    [SerializeField] private Color fastColor = Color.red;
    [SerializeField] private float colorBuffer = 0.0f;
    [SerializeField] private float colorBufferDecreaseTime = 0.2f;


    [Header("Color")]
    private float defaultXPos = 0.0f;
    [SerializeField] private float animColorTime = 256.0f;
    [SerializeField] private float animColorTimer = 0.0f;
    [SerializeField] private Color startingColor = Color.red;
    [SerializeField] private Color endColor = Color.blue;

    public bool IsEnable { get { return this.gameObject.activeInHierarchy; } }

    public float Speed { get { return this.speed; } set { this.speed = value; } }
    public float RotationMultiplictor { get { return this.rotationMultiplier; } set { this.rotationMultiplier = value; } }
    public Vector3 MainDirection { get { return this.mainDirection; } set { this.mainDirection = value; } }
    #endregion

    #region Methods
    public void Disable()
    {
        this.gameObject.SetActive(false);
    }

    public void Enable()
    {
        this.gameObject.SetActive(true);
    }

    public void Initialize()
    {
        this.Disable();
    }

    public void Initialize(InputType inputType)
    {
        this.inputType = inputType;

        switch(this.inputType)
        {
            case InputType.Sismology:
                this.InitializeSismology();
                break;
            case InputType.Color:
                this.InitializeColor();
                break;
            case InputType.MultiDirections:
                this.InitializeMultiDirection();
                break;
        }
    }

    public void SetupMultiDir(int[] numbers)
    {
        for(int i = 0; i < 4; i++)
        {
            upDir[i] = numbers[0] == i;
            downDir[i] = numbers[1] == i;
            leftDir[i] = numbers[2] == i;
            rightDir[i] = numbers[3] == i;
        }
    }

    private void InitializeMultiDirection()
    {
        this.trailRenderer.enabled = false;
        multiDirMinusOne = this.colorMinusOne;
        multiDirOne = this.colorOne;
        colorEvolution = this.colorChangeOverTime;
    }

    private void InitializeSismology()
    {
        this.timer = 0.0f;
        this.initialXPos = this.transform.position.x;
        this.trailRenderer.enabled = true;
    }

    private void InitializeColor()
    {
        this.animColorTimer = 0.0f;
        this.defaultXPos = this.transform.position.x;

        Keyframe[] keyframes = new Keyframe[this.data.Length];
        for(int i = 0; i < keyframes.Length; i++)
        {
            Keyframe keyframe = new Keyframe((float)i / (float)this.data.Length, data[i]);
            keyframes.SetValue(keyframe, i);
        }
        this.curve = new AnimationCurve(keyframes);
    }

    public void ParseData(float[] data)
    {
        this.data = data;
        this.maxValue = (data[0] > maxValue) ? data[0] : maxValue;
    }

    public void CustomUpdate()
    {
        switch(this.inputType)
        {
            case InputType.Sismology:
                this.SismologyMove();
                break;
            case InputType.Color:
                this.ColorMove();
                break;
            case InputType.MultiDirections:
                this.MultiDirectionMove();
                break;
        }
    }

    private void MultiDirectionMove()
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

        Vector3 direction = /*(this.transform.up + -this.transform.forward).normalized */ this.transform.up + this.transform.TransformDirection(new Vector3(up - down, right - left));

        float upOne = (up + 1.0f) / 2.0f;
        //float downOne = (down + 1.0f) / 2.0f;
        //float rightOne = (right + 1.0f) / 2.0f;
        //float leftOne = (left + 1.0f) / 2.0f;
        //float medium = (upOne + downOne + rightOne + leftOne) / 4.0f;
        this.renderer.color = currentColor; //highestColor; //Color.Lerp(this.multiDirMinusOne, this.multiDirOne, Mathf.Clamp01(upOne));

        this.transform.position += direction * this.speed * direction.magnitude * Time.deltaTime;
        this.transform.Rotate(Vector3.forward, rot * this.rotationMultiplier * Time.deltaTime * upOne);
    }

    public static void MultiDirectionColor()
    {
        colorValue += colorEvolution * highestValue * Time.deltaTime;
        colorValue = Mathf.Clamp01(colorValue);
        switch(colorIndex)
        {
            case 0:
                multiDirOne = Color.Lerp(blue, red, colorValue);
                break;
            case 1:
                multiDirOne = Color.Lerp(red, green, colorValue);
                break;
            case 2:
                multiDirOne = Color.Lerp(green, blue, colorValue);
                break;
        }
        if(colorValue == 1.0f)
        {
            colorIndex = (colorIndex + 1) % 3;
            colorValue = 0.0f;
        }

        currentColor = Color.Lerp(multiDirMinusOne, multiDirOne, highestValue);
    }

    public void SetCurentMusicPercent(float percent)
    {
        this.transform.localScale = Vector3.one * Mathf.Lerp(this.startScale, this.endScale, percent);
    }

    private float ParseDirectionData(string dir)
    {
        switch (dir)
        {
            case "up":
                for (int i = 0; i < 4; i++)
                {
                    if (this.upDir[i])
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

    public static void SetHighestColor(float value)
    {
        float clampedValue = Mathf.Clamp01((value + 1.0f) / 2.0f);
        highestValue = clampedValue;
        highestColor = Color.Lerp(multiDirMinusOne, multiDirOne, clampedValue);
    }

    private void ColorMove()
    {
        if (this.animColorTimer <= this.animColorTime)
        {
            this.animColorTimer += Time.deltaTime / this.animColorTime;
            Color color = Color.Lerp(this.startingColor, this.endColor, this.animColorTimer);
            this.renderer.material.color = color;
            Vector3 nextPos = this.transform.position;
            nextPos.y += this.entityYSpeed * Time.deltaTime;
            nextPos.x = this.defaultXPos + (this.curve.Evaluate(this.animColorTimer) * this.entityXMultiplicator);
            this.transform.position = nextPos;
        }
    }

    private void SismologyMove()
    {
        float xValue = this.GetCurveData();
        xValue *= this.maxValue * this.maxValueMultiplicator;
        xValue *= this.entityXMultiplicator;
        this.transform.Translate(new Vector3(0.0f, this.entityYSpeed * Time.deltaTime, 0.0f));
        this.transform.position = new Vector3(this.initialXPos + xValue, this.transform.position.y, this.transform.position.z);;

        if(this.maxValue > 0.0f)
            this.maxValue -= this.decreaseRate * Time.deltaTime;
        if (this.maxValue < 0.0f)
            this.maxValue = 0.0f;

        Color color;
        if(this.colorBuffer < this.data[0])
        {
            this.colorBuffer = this.data[0];
            color = Color.Lerp(this.slowColor, this.fastColor, Mathf.Clamp01(this.data[0]));
        }
        else
        {
            color = Color.Lerp(this.slowColor, this.fastColor, colorBuffer);
        }
        this.renderer.color = color;
        this.trailRenderer.material.color = color;

        this.colorBuffer -= colorBufferDecreaseTime * Time.deltaTime;
        if(this.colorBuffer < 0.0f)
        {
            this.colorBuffer = 0.0f;
        }

        this.timer += Time.deltaTime * this.timerMultiplicator * (1 + this.data[0]);
    }

    private float GetCurveData()
    {
        return this.curve.Evaluate(this.timer % 1.0f);
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawRay(this.transform.position, this.mainDirection);
    }
}
