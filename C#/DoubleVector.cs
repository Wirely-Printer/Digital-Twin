using System;
using UnityEngine;

[Serializable]
public struct DoubleVector3
{
    public double x;
    public double y;
    public double z;

    public DoubleVector3(double x, double y, double z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    // Conversion from Vector3 to DoubleVector3
    public static implicit operator DoubleVector3(Vector3 v)
    {
        return new DoubleVector3(v.x, v.y, v.z);
    }

    // Conversion from DoubleVector3 to Vector3
    public static implicit operator Vector3(DoubleVector3 v)
    {
        return new Vector3((float)v.x, (float)v.y, (float)v.z);
    }

    // Define additional operations, e.g., addition, subtraction, magnitude, etc.
    public static DoubleVector3 operator +(DoubleVector3 a, DoubleVector3 b)
    {
        return new DoubleVector3(a.x + b.x, a.y + b.y, a.z + b.z);
    }

    public static DoubleVector3 operator -(DoubleVector3 a, DoubleVector3 b)
    {
        return new DoubleVector3(a.x - b.x, a.y - b.y, a.z - b.z);
    }

    public static DoubleVector3 operator *(DoubleVector3 a, double d)
    {
        return new DoubleVector3(a.x * d, a.y * d, a.z * d);
    }

    public double Magnitude()
    {
        return Math.Sqrt(x * x + y * y + z * z);
    }

    public DoubleVector3 Normalize()
    {
        double magnitude = Magnitude();
        return new DoubleVector3(x / magnitude, y / magnitude, z / magnitude);
    }

    public static DoubleVector3 MoveTowards(DoubleVector3 current, DoubleVector3 target, double maxDistanceDelta)
    {
    DoubleVector3 delta = target - current;
    double distance = delta.Magnitude();
    if (distance <= maxDistanceDelta || distance == 0.0)
        return target;
    return current + delta.Normalize() * maxDistanceDelta;
    }

}
