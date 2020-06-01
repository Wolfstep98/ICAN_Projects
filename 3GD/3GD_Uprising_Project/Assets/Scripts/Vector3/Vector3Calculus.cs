using System;
using UnityEngine;

public class Vector3Calculus 
{
    #region Fields & Properties
    #region Enums
    public enum AngleDirection
    {
        Clockwise,
        CounterClockwise
    }
    #endregion
    #endregion

    #region Methods

    public static Vector3 RotateTowardsAxisAtAngle(Vector3 axis, float angle)
    {
        double cosinus = Math.Cos(angle);
        double minusOneCos = 1 - cosinus;
        double sinus = Math.Sin(angle);

        double M00 = cosinus + ((axis.x * axis.x) * minusOneCos);
        double M01 = (axis.y * axis.x * minusOneCos) + (axis.z * sinus);
        double M02 = (axis.z * axis.x * minusOneCos) - (axis.y * sinus);

        double M10 = (axis.x * axis.y * minusOneCos) - (axis.x * sinus);
        double M11 = cosinus + ((axis.y * axis.y) * minusOneCos);
        double M12 = (axis.x * axis.y * minusOneCos) + (axis.x * sinus);

        double M20 = (axis.x * axis.z * minusOneCos) + (axis.y * sinus);
        double M21 = (axis.y * axis.z * minusOneCos) - (axis.x * sinus);
        double M22 = cosinus + ((axis.z * axis.z) * minusOneCos);

        Vector3 result = Vector3.zero;
        result.x = (float)((axis.x * M00) + (axis.y * M10) + (axis.z * M20));
        result.y = (float)((axis.x * M01) + (axis.y * M11) + (axis.z * M21));
        result.z = (float)((axis.x * M02) + (axis.y * M12) + (axis.z * M22));

        return result;
    }

    public static Vector3 RotateVector3ByAngle(Vector3 vec, float angle, Axis axis)
    {
        double cosinusVecX = 0.0;
        double cosinusVecY = 0.0;
        double sinusVecX = 0.0f;
        double sinusVecY = 0.0f;
        Vector3 result = Vector3.zero;

        switch (axis)
        {
            case Axis.X:
                cosinusVecX = Math.Cos(angle * vec.z);
                cosinusVecY = Math.Cos(angle * vec.x);
                sinusVecX = Math.Sin(angle * vec.z);
                sinusVecY = Math.Sin(angle * vec.x);
                result.z = (float)(cosinusVecX - sinusVecY);
                result.x = (float)(cosinusVecY - sinusVecX);
                break;
            case Axis.Y:
                cosinusVecX = Math.Cos(angle * vec.z);
                cosinusVecY = Math.Cos(angle * vec.y);
                sinusVecX = Math.Sin(angle * vec.z);
                sinusVecY = Math.Sin(angle * vec.y);
                result.z = (float)(cosinusVecX - sinusVecY);
                result.y = (float)(cosinusVecY - sinusVecX);
                break;
            case Axis.Z:
                cosinusVecX = Math.Cos(angle * vec.x);
                cosinusVecY = Math.Cos(angle * vec.y);
                sinusVecX = Math.Sin(angle * vec.x);
                sinusVecY = Math.Sin(angle * vec.y);
                result.x = (float)(cosinusVecX - sinusVecY);
                result.y = (float)(cosinusVecY - sinusVecX);
                break;
            default:
                break;
        }
        return result;
    }

    public static float Angle(Vector3 a, Vector3 b, AngleDirection angleDirection = AngleDirection.Clockwise)
    {
        float dotProduct = Vector3.Dot(a, b);
        float magnitudes = a.magnitude * b.magnitude;
        float arcosin = dotProduct / magnitudes;


        return (float)Math.Acos(arcosin);
    }

	#endregion
}
