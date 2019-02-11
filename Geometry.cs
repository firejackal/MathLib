// Requires my IOLib to store/restore data from files in some of the classes,
// we don't need this here, but it made my life easier in other projects.
using System.Collections.Generic;

namespace MathLib
{
    public static class Geometry
    {
        private static CollisionInfo mBaseCollisionInfo  = new CollisionInfo();
        private static CollisionInfo mEmptyCollisionInfo = new CollisionInfo();

        private static System.Random rnd = new System.Random();

        public enum ClockDirections { None, Clockwise, CounterClockwise }

        public static double DegreesToRadians(double Degrees) { return Degrees / 180.0 * System.Math.PI; }

        public static double RadiansToDegrees(double Radians) { return Radians * 180.0 / System.Math.PI; }

        public static Angle LookAt(float startX1, float startY1, float lookAtX2, float lookAtY2)
        {
            float diffX = (startX1 - lookAtX2);

            // if(the points are on the some X plane then ...
            if(diffX == 0.0F) {
                // ... if(the point is looking downwards then ...
                if(startY1 > lookAtY2)
                    return Angle.AsRadians(System.Math.PI * 0.5);  //90 Degrees (North)
                    // ... if(the point is looking upwards then ...
                else if(lookAtY2 > startY1)
                    return Angle.AsRadians(System.Math.PI * 1.5);  //270 Degrees (South)
                else
                    return Angle.AsRadians(0.0F);
            } else {
                // ... Get the arctangent of the slope ...
                float diffY = (startY1 - lookAtY2);
                double atanAngle = System.Math.Atan(diffY / diffX);

                // ... if(we're looking to the right then ...
                if(lookAtX2 > startX1) {
                    // ... if(we're looking to the top then ...
                    if(lookAtY2 < startY1) //(Left-Top Area)
                        return Angle.AsRadians((System.Math.PI * 0) - atanAngle);  //90 to 0
                        // ... if(we're looking to the bottom then ...
                    else  //(Right-Bottom Area)
                        return Angle.AsRadians((System.Math.PI * 2) - atanAngle);  // 270 to 360
                    // ... if(we're looking to the left then ...
                } else {
                    // ... if(we're looking to the bottom then ...
                    if(lookAtY2 > startY1)
                        return Angle.AsRadians((System.Math.PI * 1) - atanAngle);
                        // ... if(we're looking to the top then ...
                    else
                        return Angle.AsRadians((System.Math.PI * 1) - atanAngle);
                }
            }
        } //LookAt

        public static Angle LookAt(double startX1, double startY1, double lookAtX2, double lookAtY2)
        {
            double diffX = (startX1 - lookAtX2);

            // if(the points are on the some X plane then ...
            if(diffX == 0.0) {
                // ... if(the point is looking downwards then ...
                if(startY1 > lookAtY2)
                    return Angle.AsRadians(System.Math.PI * 0.5);  //90 Degrees (North)
                // ... if(the point is looking upwards then ...
                else if(lookAtY2 > startY1)
                    return Angle.AsRadians(System.Math.PI * 1.5);  //270 Degrees (South)
                else
                    return Angle.AsRadians(0.0);
            } else {
                // ... Get the arctangent of the slope ...
                double diffY = (startY1 - lookAtY2);
                double atanAngle = System.Math.Atan(diffY / diffX);

                // ... if(we're looking to the right then ...
                if(lookAtX2 > startX1) {
                    // ... if(we're looking to the top then ...
                    if(lookAtY2 < startY1) //(Left-Top Area)
                        return Angle.AsRadians((System.Math.PI * 0) - atanAngle);  //90 to 0
                    // ... if(we're looking to the bottom then ...
                    else  //(Right-Bottom Area)
                        return Angle.AsRadians((System.Math.PI * 2) - atanAngle);  // 270 to 360
                    // ... if(we're looking to the left then ...
                } else {
                    // ... if(we're looking to the bottom then ...
                    if(lookAtY2 > startY1)
                        return Angle.AsRadians((System.Math.PI * 1) - atanAngle);
                    // ... if(we're looking to the top then ...
                    else
                        return Angle.AsRadians((System.Math.PI * 1) - atanAngle);
                }
            }
        } //LookAt

        public static Angle LookAt2(float startX, float startY, float lookAtX, float lookAtY)
        {
            double result = System.Math.Atan2(lookAtX - startX, startY - lookAtY);
            if(result < 0) result = (System.Math.PI * 2.0F) + result;
            return Angle.AsRadians(result);
        } //LookAt2

        public static Angle GetHeading(float x, float y) { return Geometry.LookAt(0.0F, 0.0F, x, y); }

        /// <summary>Returns the distance between two points.</summary>
        public static double Distance(float X1, float Y1, float X2, float Y2) { return System.Math.Sqrt((X2 - X1) * (X2 - X1) + (Y2 - Y1) * (Y2 - Y1)); }

        /// <summary>Returns the distance between two points.</summary>
        public static double Distance(float X1, float Y1, float Z1, float X2, float Y2, float Z2)
        {
            return System.Math.Sqrt((X2 - X1) * (X2 - X1) + (Y2 - Y1) * (Y2 - Y1) + (Z2 - Z1) * (Z2 - Z1));
        } //Distance

        public static float DotProduct(float x1, float y1, float x2, float y2) { return (x1 * x2) + (y1 * y2); }

        public static float DotProduct(float x, float y) { return (x * x) + (y * y); }

        public static float CrossProduct(float x1, float y1, float x2, float y2) { return (x1 * y2) - (y1 * x2); }

        public static double Length(float x, float y) { return System.Math.Sqrt(DotProduct(x, y, x, y)); }

        public static void Normalize(float x, float y, out float outX, out float outY)
        {
            outX = 0.0f;
            outY = 0.0f;
            float len = (float)Length(x, y);
            if(len == 0.0F) return;
            outX = x / len;
            outY = y / len;
        } //Normalize

        public static void SetMagnitude(float x, float y, float length, out float outX, out float outY)
        {
            Normalize(x, y, out outX, out outY);
            outX *= length;
            outY *= length;
        } //SetMagnitude

        public static void RotatePointCounterClockwise(out float outX, out float outY, float centerX, float centerY, float offsetX, float offsetY, Angle angle)
        {
            // cosine is X, sine is Y
            outX = (float)(centerX + ((System.Math.Cos(angle.Radians) * offsetY) + (System.Math.Sin(angle.Radians) * offsetX)));
            outY = (float)(centerY - ((System.Math.Sin(angle.Radians) * offsetY) + (System.Math.Cos(angle.Radians) * offsetX)));
        } //RotatePointCounterClockwise

        public static void RotatePoint(out float outX, out float outY, float offsetX, float offsetY, float distance, Angle angle)
        {
            outX = (float)(offsetX - distance * System.Math.Cos(angle.Radians));
            outY = (float)(offsetY + distance * System.Math.Sin(angle.Radians));
        } //RotatePoint

        /// <summary>Rotates a position around a center.</summary>
        /// <param name="angle">Angle to be rotated clockwise.</param>
        /// <remarks>Added clockwise version to support GDI+ rendering methods.</remarks>
        public static void RotatePointClockwise(out float outX, out float outY, float centerX, float centerY, float offsetX, float offsetY, Angle angle)
        {
            outX = (float)(centerX + ((System.Math.Cos(angle.Radians) * offsetX) - (System.Math.Sin(angle.Radians) * offsetY)));
            outY = (float)(centerY + ((System.Math.Sin(angle.Radians) * offsetX) + (System.Math.Cos(angle.Radians) * offsetY)));
        } //RotatePointClockwise

        /// <summary>Rotates a position around a center.</summary>
        /// <param name="angle">Angle to be rotated clockwise.</param>
        /// <remarks>Added clockwise version to support GDI+ rendering methods.</remarks>
        public static void RotatePointClockwise(out double outX, out double outY, double centerX, double centerY, double offsetX, double offsetY, Angle angle)
        {
            outX = (centerX + ((System.Math.Cos(angle.Radians) * offsetX) - (System.Math.Sin(angle.Radians) * offsetY)));
            outY = (centerY + ((System.Math.Sin(angle.Radians) * offsetX) + (System.Math.Cos(angle.Radians) * offsetY)));
        } //RotatePointClockwise

        public static void Push(ref float x, ref float y, float force, Angle angle)
        {
            RotatePointClockwise(out x, out y, x, y, 0.0F, -force, angle);
        } //Push

        public static void Push(ref double x, ref double y, double force, Angle angle)
        {
            RotatePointClockwise(out x, out y, x, y, 0.0, -force, angle);
        } //Push

        public struct Angle
        {
            public double Radians; //default is radians

            public float RadiansF { get { return (float)(this.Radians); } }

            public double Degrees
            {
                get { return RadiansToDegrees(this.Radians); }
                set { this.Radians = DegreesToRadians(value); }
            } //Degrees

            public float DegreesF { get { return (float)(RadiansToDegrees(this.Radians)); } }

            private static double AddAngle(double angle, double toAdd)
            {
                double wAngle = angle;
                double fAngleMax = (System.Math.PI * 2);

                // if(we're increasing the value then ...
                if(System.Math.Abs(toAdd) == toAdd) {
                    wAngle += toAdd;
                    if(wAngle >= fAngleMax) wAngle = (wAngle - fAngleMax);
                    // if(we're decreasing the value then ...
                } else {
                    wAngle = (wAngle - System.Math.Abs(toAdd));
                    if(wAngle <= 0) wAngle = (fAngleMax - System.Math.Abs(wAngle));
                }

                return wAngle;
            } //AddAngle
            
            public static bool operator ==(Angle a, Angle b) { return (a.Radians == b.Radians); }

            public static bool operator !=(Angle a, Angle b) { return (a.Radians != b.Radians); }

            public static Angle operator +(Angle a, Angle b)
            {
                Angle result = new Angle();
                result.Radians = AddAngle(a.Radians, b.Radians);
                return result;
            } //+

            public static Angle operator -(Angle a, Angle b)
            {
                Angle result = new Angle();
                result.Radians = AddAngle(a.Radians, 0 - b.Radians);
                return result;
            } //-

            public static bool operator <(Angle a, Angle b) { return (a.Radians < b.Radians); }

            public static bool operator <=(Angle a, Angle b) { return (a.Radians <= b.Radians); }

            public static bool operator >(Angle a, Angle b) { return (a.Radians > b.Radians); }

            public static bool operator >=(Angle a, Angle b) { return (a.Radians >= b.Radians); }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            } //GetHashCode

            public override bool Equals(object obj)
            {
                Angle target = (Angle)obj;
                if(target == null) return false;
                return (this == target);
            } //Equals

            public static Angle operator *(Angle a, double b)
            {
                Angle result = new Angle();
                result.Radians = (a.Radians * b);
                return result;
            } //*

            public static Angle AsDegrees(double value)
            {
                Angle result = new Angle();
                result.Degrees = value;
                return result;
            } //AsDegrees

            public static Angle AsRadians(double value)
            {
                Angle result = new Angle();
                result.Radians = value;
                return result;
            } //AsDegrees

            public static Angle Empty()
            {
                Angle result = new Angle();
                result.Radians = 0.0F;
                return result;
            } //AsDegrees

            public static Angle PI() { return AsRadians(System.Math.PI); }

            public static Angle HalfPI() { return AsRadians(System.Math.PI * 0.5F); }

            public static Angle PI2() { return AsRadians(System.Math.PI * 2.0F); }

            public static ClockDirections ComputeShorterChangeDirection(Angle source, Angle that)
            {
                if(source.Radians == that.Radians)
                    return ClockDirections.None;
                else {
                    double leftDist, rightDist;
                    if(that.Radians < source.Radians) {
                        leftDist = (source.Radians - that.Radians);
                        rightDist = (that.Radians + (System.Math.PI * 2.0F - source.Radians));
                    } else {
                        leftDist = (source.Radians + (System.Math.PI * 2.0F - that.Radians));
                        rightDist = (that.Radians - source.Radians);
                    }

                    if(leftDist < rightDist)
                        return ClockDirections.CounterClockwise;
                    else
                        return ClockDirections.Clockwise;
                }
            } //ComputeShorterChangeDirection

            public static void GetDifference(Angle sourceAngle, Angle checkAngle, out Angle outLeftDiff, out Angle outRightDiff)
            {
                outLeftDiff = Angle.Empty();
                outRightDiff = Angle.Empty();

                // Return the difference from the right ...
                // ... if(the checking angle is to the right of the source angle then ...
                if (checkAngle < sourceAngle)
                {
                    // ... Return the difference from to the right.
                    outRightDiff = (sourceAngle - checkAngle);
                    // ... Return the difference to 0 degrees and then add it to the check angle.
                    outLeftDiff = (checkAngle + (Angle.PI2() - sourceAngle));
                }
                else if (sourceAngle < checkAngle)
                {
                    // ... Return the difference to 360 degrees, and then add it to the check angle.
                    outLeftDiff = (checkAngle - sourceAngle);
                    // ... Return the difference from to the left.
                    outRightDiff = sourceAngle + (Angle.PI2() - checkAngle);
                }
            } //GetDifference Function

            public static Angle GraduallyChange(Angle currentAngle, Angle destAngle, Angle changeValue)
            {
                Angle leftDiff, rightDiff;
                GetDifference(currentAngle, destAngle, out leftDiff, out rightDiff);

                // Changing the angle left will take shorter ...
                if (leftDiff <= rightDiff) {
                    if (destAngle + (PI2() - currentAngle) == leftDiff) {
                        // ... Add to the degrees ...
                        currentAngle += changeValue;
                        if (currentAngle <= (destAngle + changeValue)) {
                            if (currentAngle > destAngle) currentAngle = destAngle;
                        }
                    } else {
                        // ... Add to the degrees.
                        currentAngle += changeValue;
                        if (currentAngle > destAngle) currentAngle = destAngle;
                    }
                    // Changing the angle right will take shorter ...
                } else if(rightDiff < leftDiff) {
                    if(currentAngle + (PI2() - destAngle) == rightDiff) {
                        // ... Subtract from the degrees ...
                        currentAngle -= changeValue;

                        if(currentAngle >= (destAngle - changeValue)) {
                            if (currentAngle < destAngle) currentAngle = destAngle;
                        }
                    } else {
                        // ... Subtract from the degrees.
                        currentAngle -= changeValue;
                        if (currentAngle < destAngle) currentAngle = destAngle;
                    }
                }

                return currentAngle;
            } //GraduallyChange Function
        } //Angle

        public struct Vector
        {
            public float X, Y;

            public Vector(Vector a)
            {
                this.X = a.X;
                this.Y = a.Y;
            } //New

            public Vector(float x, float y)
            {
                this.X = x;
                this.Y = y;
            } //New

            public Vector(double x, double y)
            {
                this.X = (float)x;
                this.Y = (float)y;
            } //New

            public Vector(Angle angle, float distance)
            {
                this.X = -(float)(System.Math.Cos(angle.Radians)) * distance;
                this.Y = (float)(System.Math.Sin(angle.Radians)) * distance;
            } //New

            public void Clear()
            {
                this.X = 0.0F;
                this.Y = 0.0F;
            } //Clear

            public float Dot(Vector b) { return DotProduct(this.X, this.Y, b.X, b.Y); }

            /// <summary>Cross Product</summary>
            public float Cross(Vector b) { return CrossProduct(this.X, this.Y, b.X, b.Y); }

            /// <summary>Uses Pythagorean theory to get the length of a vector.</summary>
            public double Length()  { return Geometry.Length(this.X, this.Y); }

            /// <summary>Normalizes the vector to values between -1 and 1.</summary>
            public void Normalize() { Geometry.Normalize(this.X, this.Y, out this.X, out this.Y); }

            public void SetMagnitude(float length) { Geometry.SetMagnitude(this.X, this.Y, length, out this.X, out this.Y); }

            /// <summary>Returns the distance between two points.</summary>
            public double Distance(Vector b) { return Geometry.Distance(this.X, this.Y, b.X, b.Y); }

            public double Distance(float x, float y) { return Geometry.Distance(this.X, this.Y, x, y); }

            /// <summary>Returns the distance between two points.</summary>
            public Vector DistanceVector(Vector b) { return new Vector(this.X - b.X, this.Y - b.Y); }

            public bool IsEmpty() { return (this.X == 0.0F && this.Y == 0.0F); }

            public void Rotate(Vector distance, Angle angle) { RotatePointClockwise(out this.X, out this.Y, this.X, this.Y, distance.X, distance.Y, angle); }

            public void Rotate(float distance, Angle angle) { RotatePoint(out this.X, out this.Y, this.X, this.Y, distance, angle); }
            
            public void Rotate(Angle angle) { RotatePointClockwise(out this.X, out this.Y, 0.0F, 0.0F, this.X, this.Y, angle); }

            public Angle LookAt(Vector target) { return Geometry.LookAt(this.X, this.Y, target.X, target.Y); }

            public Angle LookAt(float X, float Y) { return Geometry.LookAt(this.X, this.Y, X, Y); }

            public Angle Angle() { return Geometry.Angle.AsRadians(System.Math.Atan2(this.Y, this.X)); }

            public Angle Angle(Vector target)
            {
                float dot = this.Dot(target);
                float cross = this.Cross(target);
                float anglef = (float)(System.Math.Atan2(cross, dot));
                return Geometry.Angle.AsRadians(anglef);
            } //Angle

            public Angle Heading() { return Geometry.LookAt2(0.0F, 0.0F, this.X, this.Y); }

            public void Negate()
            {
                this.X = (0 - this.X);
                this.Y = (0 - this.Y);
            } //Negate

            public void Push(float force, Angle angle) { Geometry.RotatePointClockwise(out this.X, out this.Y, this.X, this.Y, 0.0F, -force, angle); }

            public void ApplyDamping(float dampingValue)
            {
                this.X *= (float)(System.Math.Exp((double)(-dampingValue)));
                this.Y *= (float)(System.Math.Exp((double)(-dampingValue)));
            } //ApplyDamping

            /// <summary>Returns the perpendicular vector.</summary>
            public Vector Perp() { return new Vector(-this.Y, this.X); }

            public Vector Unit() { return this * (float)(1.0 / this.Length()); }

            public void Transform(Vector trans, Angle rotation)
            {
                Vector D = this;
                D.Rotate(rotation);
                this.X = trans.X + D.X;
                this.Y = trans.Y + D.Y;
            } //Transform

            public void Randomize(Vector min, Vector max)
            {
                this.X = (float)(rnd.NextDouble() * (max.X - min.X)) + min.X;
                this.Y = (float)(rnd.NextDouble() * (max.Y - min.Y)) + min.Y;
            } //Randomize

            public void AppendFromData(IOLib.XINI.EntryItem parentEntry, IOLib.XINI.AppendModes appendMode)
            {
                parentEntry.AppendChildEntryValue("X", ref this.X, appendMode);
                parentEntry.AppendChildEntryValue("Y", ref this.Y, appendMode);
            } //AppendPointFromData

            public static Vector NormalizedVector(Vector a)
            {
                Vector result = new Vector(a.X, a.Y);
                result.Normalize();
                return result;
            } //NormalizedVector

            public static Vector RotateVector(Vector offset, Vector distance, Angle Angle)
            {
                Vector result = new Vector(0.0F, 0.0F);
                RotatePointCounterClockwise(out result.X, out result.Y, offset.X, offset.Y, distance.X, distance.Y, Angle);
                return result;
            } //RotateVector

            public static Vector RotateVector(float offsetX, float offsetY, Vector distance, Angle Angle)
            {
                Vector result = new Vector(0.0F, 0.0F);
                RotatePointCounterClockwise(out result.X, out result.Y, offsetX, offsetY, distance.X, distance.Y, Angle);
                return result;
            } //RotateVector

            public static Vector RotateVector(Vector offset, float distance, Angle angle)
            {
                Vector result = new Vector(0.0F, 0.0F);
                RotatePoint(out result.X, out result.Y, offset.X, offset.Y, distance, angle);
                return result;
            } //RotateVector

            public static Vector RotateVector(float offsetX, float offsetY, float distance, Angle Angle)
            {
                Vector result = new Vector(0.0F, 0.0F);
                RotatePoint(out result.X, out result.Y, offsetX, offsetY, distance, Angle);
                return result;
            } //RotateVector

            public static Vector Empty() { return new Vector(0.0F, 0.0F); }

            public override string ToString() { return "X: " + this.X + ", Y: " + this.Y; }

    #region "Operators"
            public static Vector operator +(Vector a, Vector b) { return new Vector(a.X + b.X, a.Y + b.Y); }

            public static Vector operator -(Vector a, Vector b) { return new Vector(a.X - b.X, a.Y - b.Y); }

            public static Vector operator -(Vector a) { return new Vector(0.0F - a.X, 0.0F - a.Y); }

            public static Vector operator *(Vector a, Vector b) { return new Vector(a.X * b.X, a.Y * b.Y); }

            public static Vector operator *(Vector a, float b) { return new Vector(a.X * b, a.Y * b); }

            public static Vector operator /(Vector a, Vector b) { return new Vector(a.X / b.X, a.Y / b.Y); }

            public static Vector operator /(Vector a, float b) { return new Vector(a.X / b, a.Y / b); }

            public static bool operator ==(Vector a, Vector b) { return a.X == b.X && a.Y == b.X; }

            public static bool operator !=(Vector a, Vector b) { return a.X != b.X && a.Y != b.X; }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                Vector target = (Vector)obj;
                if(target == null) return false;
                return (this == target);
            }
    #endregion //Operators
        } //Vector

        public struct IntegerPoint
        {
            public int X, Y;

            public IntegerPoint(int x, int y)
            {
                this.X = x;
                this.Y = y;
            } //New

            public void Clear()
            {
                this.X = 0;
                this.Y = 0;
            } //Clear

            public void Negate()
            {
                this.X = (0 - this.X);
                this.Y = (0 - this.Y);
            } //Negate

            public bool IsEmpty() { return this.X == 0 && this.Y == 0; }

            public static IntegerPoint Empty() { return new IntegerPoint(0, 0); }

            public override string ToString() { return "X: " + this.X + ", Y: " + this.Y; }
        } //IntegerPoint

        public struct Rectangle
        {
            public float Left, Top, Right, Bottom;

            public Rectangle(float left, float top, float width, float height)
            {
                this.Left = left;
                this.Top = top;
                this.Right = left + width;
                this.Bottom = top + height;
            } //New

            public float Width
            {
                get { return this.Right - this.Left; }
                set { this.Right = this.Left + value; }
            } //Width

            public float Height
            {
                get { return this.Bottom - this.Top; }
                set { this.Bottom = this.Top + value; }
            } //Height

            public Vector LeftTop
            {
                get { return new Vector(this.Left, this.Top); }
                set {
                    this.Left = value.X;
                    this.Top = value.Y;
                }
            } //LeftTop

            public Vector RightBottom
            {
                get { return new Vector(this.Right, this.Bottom); }
                set {
                    this.Right = value.X;
                    this.Bottom = value.Y;
                }
            } //RightBottom

            public static Rectangle FromLTRB(float left, float top, float right, float bottom)
            {
                Rectangle result = new Rectangle();
                result.Left = left;
                result.Top = top;
                result.Right = right;
                result.Bottom = bottom;
                return result;
            } //FromLTRB
        } //Rectangle

        public struct Vector3D
        {
            float X, Y, Z;

            public Vector3D(Vector3D a)
            {
                this.X = a.X;
                this.Y = a.Y;
                this.Z = a.Z;
            } //New

            public Vector3D(float x, float y, float z)
            {
                this.X = x;
                this.Y = y;
                this.Z = z;
            } //New

            public Vector3D(double x, double y, double z)
            {
                this.X = (float)x;
                this.Y = (float)y;
                this.Z = (float)z;
            } //New

            public void Clear()
            {
                this.X = 0.0F;
                this.Y = 0.0F;
                this.Z = 0.0F;
            } //Clear

            public bool IsEmpty() { return this.X == 0.0F && this.Y == 0.0F && this.Z == 0.0F; }

            public float Dot(Vector3D b) { return (this.X * b.X) + (this.Y * b.Y) + (this.Z * b.Z); }

            /// <summary>Uses pythagorean theory to get the length of a vector.</summary>
            public double Length() { return System.Math.Sqrt(this.X * this.X + this.Y * this.Y + this.Z * this.Z); }

            /// <summary>Normalizes the vector to values between -1 and 1.</summary>
            public void Normalize()
            {
                double length = this.Length();
                this.X = (float)(this.X / length);
                this.Y = (float)(this.Y / length);
                this.Z = (float)(this.Z / length);
            } //Normalize

            /// <summary>Returns the distance between two points.</summary>
            public double Distance(Vector3D b)
            {
                return System.Math.Sqrt((b.X - this.X) * (b.X - this.X) + (b.Y - this.Y) * (b.Y - this.Y) + (b.Z - this.Z) * (b.Z - this.Z));
            } //Distance

            public void Move(float xdistance, float ydistance, float zdistance, float xaxis, float yaxis)
            {
                this.X += (float)(zdistance * System.Math.Sin(yaxis));
                this.Z += (float)(zdistance * System.Math.Cos(yaxis));
                this.Y -= (float)(zdistance * System.Math.Sin(xaxis));

                this.X += (float)(xdistance * System.Math.Cos(yaxis));
                this.Z -= (float)(xdistance * System.Math.Sin(yaxis));
                //Me.Y += (xdistance * Math.Sin(xaxis))
            } //Move

    #region "Operators"
            public static Vector3D operator +(Vector3D a, Vector3D b) { return new Vector3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z); } 

            public static Vector3D operator -(Vector3D a, Vector3D b) { return new Vector3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z); } 

            public static Vector3D operator *(Vector3D a, Vector3D b) { return new Vector3D(a.X * b.X, a.Y * b.Y, a.Z * b.Z); }

            public static Vector3D operator *(Vector3D a, float b) { return new Vector3D(a.X * b, a.Y * b, a.Z * b); }

            public static Vector3D operator /(Vector3D a, Vector3D b) { return new Vector3D(a.X / b.X, a.Y / b.Y, a.Z / b.Z); }

            public static Vector3D operator /(Vector3D a, float b) { return new Vector3D(a.X / b, a.Y / b, a.Z / b); }

            public static bool operator ==(Vector3D a, Vector3D b) { return a.X == b.X && a.Y == b.X && a.Z == b.Z; }

            public static bool operator !=(Vector3D a, Vector3D b) { return a.X != b.X && a.Y != b.X && a.Z != b.Z; }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                Vector3D target = (Vector3D)obj;
                if(target == null) return false;
                return (this == target);
            }
    #endregion //Operators
        } //Vector3D

        public struct Line
        {
            Vector Start, End;

            public void New(Line line)
            {
                this.Start = line.Start;
                this.End = line.End;
            } //New

            public void New(Vector start, Vector end)
            {
                this.Start = start;
                this.End = end;
            } //New

            public enum IntersectResult { Parallel, Coincident, NotIntersecting, Intersecting }

            public IntersectResult Intersect(Line other, out Vector outPoint)
            {
                outPoint = new Vector();

                float denom = ((other.End.Y - other.Start.Y) * (this.End.X - this.Start.X)) - ((other.End.X - other.Start.X) * (this.End.Y - this.Start.Y));
                float nume_a = ((other.End.X - other.Start.X) * (this.Start.Y - other.Start.Y)) - ((other.End.Y - other.Start.Y) * (this.Start.X - other.Start.X));
                float nume_b = ((this.End.X - this.Start.X) * (this.Start.Y - other.Start.Y)) - ((this.End.Y - this.Start.Y) * (this.Start.X - other.Start.X));

                if(denom == 0.0F) {
                    if(nume_a == 0.0F && nume_b == 0.0F)
                        return IntersectResult.Coincident;
                    else
                        return IntersectResult.Parallel;
                }

                float ua = nume_a / denom;
                float ub = nume_b / denom;

                if(ua >= 0.0F && ua <= 1.0F && ub >= 0.0F && ub <= 1.0F) {
                    // Get the intersection point.
                    outPoint.X = this.Start.X + ua * (this.End.X - this.Start.X);
                    outPoint.Y = this.Start.Y + ua * (this.End.Y - this.Start.Y);

                    return IntersectResult.Intersecting;
                }

                return IntersectResult.NotIntersecting;
            } //Intersect
        } //Line

        public class Polygon : System.ICloneable
        {
            public List<Vector> Vertices = new List<Vector>();

            public Polygon() { }

            public Polygon(Vector min, Vector max)
            {
                this.Vertices = new List<Vector>();
                this.Vertices.Add(new Vector(min.X, min.Y));
                this.Vertices.Add(new Vector(min.X, max.Y));
                this.Vertices.Add(new Vector(max.X, max.Y));
                this.Vertices.Add(new Vector(max.X, min.Y));
            } //New

            public Polygon(int count, float scale)
            {
                this.Vertices = new List<Vector>();
                for(int i = 0; i < count; i++) {
                    double a = 2.0F * System.Math.PI * (i / (float)(count));
                    this.Vertices.Add(new Vector(System.Math.Cos(a) * scale, System.Math.Sin(a) * scale));
                } //i
            } //New

            /// <summary>Rotates the physical vectors of this polygon by an angle.</summary>
            public void Rotate(Angle rotation)
            {
                if(this.Vertices.Count == 0) return;
                for(int i = 0; i < this.Vertices.Count; i++) {
                    Rotate(this.Vertices[i], rotation);
                } //i
            } //Transform

            private static void Rotate(Vector a, Angle angle)
            {
                float tx = a.X;
                a.X = (float)(a.X * System.Math.Cos(angle.Radians) - a.Y * System.Math.Sin(angle.Radians));
                a.Y = (float)(tx * System.Math.Sin(angle.Radians) + a.Y * System.Math.Cos(angle.Radians));
                //RotatePoint(a.X, a.Y, 0.0F, 0.0F, Length(a), Angle(a))
            } //Rotate

            /// <summary>Checks for any collisions between two polygon objects.</summary>
            /// <param name="thisPosition">The position in space the source polygon exists at.</param>
            /// <param name="targetPosition">The position in space the target polygon exists at.</param>
            /// <param name="targetPoly">The target polygon to check if(this one collides with.</param>
            /// <param name="delta">The difference in the object's velocity.</param>
            /// <returns>Returns collision data associated with the results of the test.</returns>
            public CollisionInfo Collide(Vector thisPosition, Vector thisScale, Vector targetPosition, Vector targetScale, Polygon targetPoly, Vector delta)
            {
                mBaseCollisionInfo.Clear();
                if(this.Vertices.Count == 0 || targetPoly.Vertices.Count == 0) return mBaseCollisionInfo;
                mBaseCollisionInfo.Overlapped = true;         // we'll be regressing tests from there
                mBaseCollisionInfo.Collided = true;
                mBaseCollisionInfo.MTDLengthSquared = -1.0F; // flags mtd as not being calculated yet
                mBaseCollisionInfo.TEnter = 1.0F;             // flags swept test as not being calculated yet
                mBaseCollisionInfo.TLeave = 0.0F;             // <--- ....

                // test separation axes of current polygon
                int j = (this.Vertices.Count - 1);
                Vector edge, axis;
                for(int i = 0; i < this.Vertices.Count; i++) {
                    edge = (this.Vertices[i] * thisScale) - (this.Vertices[j] * thisScale); // edge
                    //edge = this.Vertices(i) - this.Vertices(j) // edge
                    axis = edge.Perp(); // sep axis is perpendicular to the edge
                    if(this.SeparatedByAxis(thisPosition, thisScale, axis, targetPosition, targetScale, targetPoly, delta, ref mBaseCollisionInfo)) return mEmptyCollisionInfo; //New CollisionInfo
                    j = i;
                } //i

                // test separation axes of other polygon
                j = (targetPoly.Vertices.Count - 1);
                for(int i = 0; i < targetPoly.Vertices.Count; i++) {
                    edge = (targetPoly.Vertices[i] * targetScale) - (targetPoly.Vertices[j] * targetScale); // edge
                    //edge = targetPoly.Vertices(i) - targetPoly.Vertices(j) // edge
                    axis = edge.Perp(); // sep axis is perpendicular ot the edge
                    if(this.SeparatedByAxis(thisPosition, thisScale, axis, targetPosition, targetScale, targetPoly, delta, ref mBaseCollisionInfo)) return mEmptyCollisionInfo; //New CollisionInfo
                    j = i;
                } //j

                // sanity checks
                mBaseCollisionInfo.Overlapped = mBaseCollisionInfo.Overlapped && (mBaseCollisionInfo.MTDLengthSquared >= 0.0F);
                mBaseCollisionInfo.Collided = mBaseCollisionInfo.Collided && (mBaseCollisionInfo.TEnter <= mBaseCollisionInfo.TLeave);

                // normalise normals
                mBaseCollisionInfo.NEnter.Normalize();
                mBaseCollisionInfo.NLeave.Normalize();

                return mBaseCollisionInfo;
            } //Collide

            private void CalculateInterval(Vector position, Vector scale, Vector axis, out float min, out float max)
            {
                min = (position + (this.Vertices[0] * scale)).Dot(axis);
                max = min;

                if(this.Vertices.Count > 1) {
                    for(int i = 1; i < this.Vertices.Count; i++) {
                        float d = (position + (this.Vertices[i] * scale)).Dot(axis);
                        if(d < min)
                            min = d;
                        else if(d > max)
                            max = d;
                    } //i 
                }
            } //CalculateInterval

            private bool SeparatedByAxis(Vector thisPosition, Vector thisScale, Vector axis, Vector targetPosition, Vector targetScale, Polygon targetPoly, Vector delta, ref CollisionInfo info)
            {
                // calculate both polygon intervals along the axis we are testing
                float mina = 0, maxa = 0, minb = 0, maxb = 0;
                this.CalculateInterval(thisPosition, thisScale, axis, out mina, out maxa);
                targetPoly.CalculateInterval(targetPosition, targetScale, axis, out minb, out maxb);

                // calculate the two possible overlap ranges.
                // either we overlap on the left or right of the polygon.
                float d0 = (maxb - mina); // //left' side
                float d1 = (minb - maxa); // //right' side
                float v = axis.Dot(delta); // project delta on axis for swept tests

                bool sep_overlap = SeparatedByAxisOverlap(axis, d0, d1, ref info);
                bool sep_swept = SeparatedByAxisSwept(axis, d0, d1, v, ref info);

                // both tests didnt find any collision
                // return separated
                return sep_overlap && sep_swept;
            } //SeparatedByAxis

            /// <summary>Checks if(there is an overlap collision.</summary>
            private static bool SeparatedByAxisOverlap(Vector axis, float d0, float d1, ref CollisionInfo info)
            {
                if(!info.Overlapped) return true;

                // intervals do not overlap. 
                // so no overlap possible.
                if(d0 < 0.0F || d1 > 0.0F) {
                    info.Overlapped = false;
                    return true;
                }

                // find out if(we overlap on the //right' or //left' of the polygon.
                float overlap = d1; //IIf(d0 < -d1, d0, d1)
                if(d0 < -d1) overlap = d0;

                // the axis length squared
                float axis_length_squared = axis.Dot(axis);
                //assert(axis_length_squared > 0.00001f);
                if(axis_length_squared > 0.00001F) info.Overlapped = false; else return true;

                // the mtd vector for that axis
                Vector sep = axis * (overlap / axis_length_squared);

                // the mtd vector length squared.
                float sep_length_squared = sep.Dot(sep);

                // if(that vector is smaller than our computed MTD (or the mtd hasn't been computed yet)
                // use that vector as our current mtd.
                if(sep_length_squared < info.MTDLengthSquared || (info.MTDLengthSquared < 0.0F)) {
                    info.MTDLengthSquared = sep_length_squared;
                    info.MTD = sep;
                }

                return false;
            } //SeparatedByAxisOverlap

            /// <summary>Checks if(there is a collision between two objects using sweep collision.</summary>
            private static bool SeparatedByAxisSwept(Vector axis, float d0, float d1, float v, ref CollisionInfo info)
            {
                if(!info.Collided) return true;

                // projection too small. ignore test
                if(System.Math.Abs(v) < 0.0000001F) return true;

                Vector N0 = axis;
                Vector N1 = axis; N1.Negate();
                float t0 = d0 / v; // estimated time of collision to the //left' side
                float t1 = d1 / v; // estimated time of collision to the //right' side

                // sort values on axis
                // so we have a valid swept interval
                if(t0 > t1) {
                    float tempf1 = t0;
                    t0 = t1;
                    t1 = tempf1;
                    Vector tempV = N0;
                    N0 = N1;
                    N1 = tempV;
                }

                // swept interval outside [0, 1] boundaries. 
                // polygons are too far apart
                if(t0 > 1.0F || t1 < 0.0F) {
                    info.Collided = false;
                    return true;
                }

                // the swept interval of the collison result hasn't been
                // performed yet.
                if(info.TEnter > info.TLeave) {
                    info.TEnter = t0;
                    info.TLeave = t1;
                    info.NEnter = N0;
                    info.NLeave = N1;
                    // not separated
                    return false;
                    // else, make sure our current interval is in 
                    // range [info.m_tenter, info.m_tleave];
                } else {
                    // separated.
                    if(t0 > info.TLeave || t1 < info.TEnter) {
                        info.Collided = false;
                        return true;
                    }

                    // reduce the collison interval to minimal
                    if(t0 > info.TEnter) {
                        info.TEnter = t0;
                        info.NEnter = N0;
                    }
                    if(t1 < info.TLeave) {
                        info.TLeave = t1;
                        info.NLeave = N1;
                    }

                    // not separated
                    return false;
                }
            } //SeparatedByAxisSwept

            public void AppendFromData(IOLib.XINI.EntryItem parentEntry, IOLib.XINI.AppendModes appendMode)
            {
                if(appendMode == IOLib.XINI.AppendModes.Read) {
                    if(parentEntry.Children.Count > 0) {
                        List<Vector> vectors = new List<Vector>();
                        foreach(IOLib.XINI.EntryItem childEntry in parentEntry.Children) {
                            if(string.Equals(childEntry.Name, "Point", System.StringComparison.CurrentCultureIgnoreCase)) {
                                Vector newItem = new Vector();
                                newItem.AppendFromData(childEntry, appendMode);
                                vectors.Add(newItem);
                            }
                        } //childEntry
                        if(vectors.Count > 0) {
                            this.Vertices.Clear();
                            for(int index = 0; index < vectors.Count; index++) {
                                this.Vertices.Add(new Vector(vectors[index].X, vectors[index].Y));
                            }
                        }
                    }
                } else if(appendMode == IOLib.XINI.AppendModes.Save) {
                    if(this.Vertices.Count > 0) {
                        for(int index = 0; index < this.Vertices.Count; index++) {
                            IOLib.XINI.EntryItem childEntry = parentEntry.AddChild("Point");
                            this.Vertices[index].AppendFromData(childEntry, appendMode);
                        } //index
                    }
                }
            } //AppendFromData

            public object Clone()
            {
                Polygon result = new Polygon();
                //out.Vertices = this.Vertices.Clone()
                if(this.Vertices.Count > 0) {
                    for(int index = 0; index < this.Vertices.Count; index++) {
                        result.Vertices.Add(new Vector(this.Vertices[index].X, this.Vertices[index].Y));
                    } //index
                }
                return result;
            } //Clone
        } //Polygon

        public struct CollisionInfo
        {
            // overlaps
            public bool Overlapped;
            public float MTDLengthSquared;
            public Vector MTD;
            // swept
            public bool Collided;
            public Vector NEnter;
            public Vector NLeave;
            public float TEnter;
            public float TLeave;

            public void Clear()
            {
                this.Overlapped = false;
                this.MTDLengthSquared = 0.0F;
                this.MTD.Clear();
                this.Collided = false;
                this.NEnter.Clear();
                this.NLeave.Clear();
                this.TEnter = 0.0F;
                this.TLeave = 0.0F;
            } //Clear

            public static CollisionInfo Empty() { return new CollisionInfo(); }
        } //CollisionInfo

        public class BodyDataCollection : List<BodyData>
        {
            public BodyDataCollection() { }

            public BodyData Add(string fileName)
            {
                base.Add(new BodyData(fileName));
                return base[base.Count - 1];
            } //Add

            public BodyData this[string id]
            {
                get {
                    int index = this.FindIndex(id);
                    if(index < 0) return null;
                    return base[index];
                }

            } //Find
            
            public int FindIndex(string id, bool ignoreCase = true)
            {
                if(this.Count > 0) {
                    for(int index = 0; index < this.Count; index++) {
                        if(ignoreCase) {
                            if(string.Equals(base[index].ID, id, System.StringComparison.CurrentCultureIgnoreCase)) return index;
                        } else {
                            if(string.Equals(base[index].ID, id)) return index;
                        }
                    } //index
                }

                return -1;
            } //FindIndex

            public void AddFromPath(string pathName)
            {
                string[] files = System.IO.Directory.GetFiles(pathName, "*.shape", System.IO.SearchOption.AllDirectories);
                if(files == null || files.Length == 0) return;

                foreach(string fileName in files) {
                    this.Add(fileName);
                } //fileName
            } //AddFromPath
        } //BodyDataCollection

        public class BodyData
        {
            public string FileName = "";
            public string ID       = "";
            public List<Polygon> Polygons = new List<Polygon>();

            public BodyData() {}

            public BodyData(Vector min, Vector max)
            {
                Polygon newPolygon = new Polygon(min, max);
                newPolygon.Rotate(Angle.AsRadians(0.0F));
                this.Polygons.Add(newPolygon);
            } //New

            public BodyData(int count, float scale)
            {
                Polygon newPolygon = new Polygon(count, scale);
                newPolygon.Rotate(Angle.AsRadians(rnd.NextDouble() * System.Math.PI));
                this.Polygons.Add(newPolygon);
            } //New

            public BodyData(string fileName) { this.LoadFromFile(fileName); }

            public bool LoadFromFile(string fileName)
            {
                IOLib.XINI cXINI = new IOLib.XINI();
                if(!cXINI.LoadFromFile(fileName)) return false;
                this.FileName = fileName;

                IOLib.XINI.EntryItem parentEntry = cXINI.Root.AppendChildEntry("Shape", IOLib.XINI.AppendModes.Read);
                this.AppendFromData(parentEntry, IOLib.XINI.AppendModes.Read);

                return true;
            } //LoadFromFile

            public bool SaveToFile(string fileName)
            {
                IOLib.XINI cXINI = new IOLib.XINI();
                cXINI.Name = "Shape";
                cXINI.Version = "1";

                IOLib.XINI.EntryItem parentEntry = cXINI.Root.AppendChildEntry("Shape", IOLib.XINI.AppendModes.Save);
                this.AppendFromData(parentEntry, IOLib.XINI.AppendModes.Save);

                return cXINI.SaveToFile(fileName);
            } //SaveToFile

            public void AppendFromData(IOLib.XINI.EntryItem parentEntry, IOLib.XINI.AppendModes appendMode)
            {
                parentEntry.AppendChildEntryValue("ID", ref this.ID, appendMode);
                AppendBodiesFromData(parentEntry, appendMode);
            } //AppendFromData

            private void AppendBodiesFromData(IOLib.XINI.EntryItem parentEntry, IOLib.XINI.AppendModes appendMode)
            {
                if(appendMode == IOLib.XINI.AppendModes.Read) {
                    if(parentEntry.Children.Count > 0) {
                        foreach(IOLib.XINI.EntryItem childEntry in parentEntry.Children) {
                            if(string.Equals(childEntry.Name, "Body", System.StringComparison.CurrentCultureIgnoreCase)) {
                                Polygon newItem = new Polygon();
                                newItem.AppendFromData(childEntry, appendMode);
                                this.Polygons.Add(newItem);
                            }
                        } //childEntry
                    }
                } else if(appendMode == IOLib.XINI.AppendModes.Save) {
                    if(this.Polygons.Count > 0) {
                        for(int index = 0; index < this.Polygons.Count; index++) {
                            IOLib.XINI.EntryItem childEntry = parentEntry.AddChild("Body");
                            this.Polygons[index].AppendFromData(childEntry, appendMode);
                        } //index
                    }
                }
            } //AppendFromData

            public void Rotate(Angle rotation)
            {
                if(this.Polygons.Count > 0) {
                    for(int index = 0; index < this.Polygons.Count; index++) {
                        this.Polygons[index].Rotate(rotation);
                    } //index
                }
            } //Rotate
        } //BodyData

        public class Bodies<type> : List<type> where type : Body
        {
            public Body FindClosest(float testX, float testY)
            {
                if(base.Count > 0) {
                    int lastIndex = 0;
                    double lastDist = 0.0;

                    for(int index = 0; index < base.Count; index++) {
                        double dist = Distance(testX, testY, this[index].Position.X, this[index].Position.Y);
                        if(lastIndex == 0 || dist < lastDist) {
                            lastIndex = index;
                            lastDist = dist;
                        }
                    } //index

                    return this[lastIndex];
                }

                return null;
            } //FindClosest

            public virtual void Update(float deltaTime, bool ignoreMass = false)
            {
                if(this.Count > 0) {
                    for(int i = (base.Count - 1); i >= 0; i--) {
                        base[i].Update(deltaTime, ignoreMass);
                    } //i
                }

                if(this.Count > 0) {
                    for(int i = 0; i < this.Count; i++) {
                        for(int j = (i + 1); j < this.Count; j++) {
                            // both bodies static. skip
                            if(base[i].InvMass == 0.0F && base[j].InvMass == 0.0F) break;

                            Geometry.CollisionInfo info = base[i].Collide(base[j], deltaTime);

                            if(info.Collided || info.Overlapped) base[i].RespondToCollision(base[j], info, deltaTime);
                        } //j
                    } //i
                }
            } //Update
        } //Bodies

        public class Body
        {
            public Vector Position;
            public BodyData Data;
            public Vector Velocity = new Vector();
            public float InvMass;
            public bool CollisionEnabled = true;
            public Vector Scale = new Vector(1.0F, 1.0F);

            public void New() {}

            public void New(Vector position, Vector min, Vector max, float mass)
            {
                this.Position = position;
                this.Mass = mass;
                this.Data = new BodyData(min, max);
            } //New

            public void New(Vector position, int count, float scale, float mass)
            {
                this.Position = position;
                this.Mass = mass;
                this.Data = new BodyData(count, scale);
            } //New

            public CollisionInfo Collide(Body body, float deltaTime)
            {
                if(this.Data == null || this.Data.Polygons.Count == 0 || body.Data.Polygons.Count == 0) return CollisionInfo.Empty();
                if(!this.CollisionEnabled || !body.CollisionEnabled) return CollisionInfo.Empty();
                // the relative velocity used to compute swept collisions
                Vector delta = ((this.Velocity * deltaTime) - (body.Velocity * deltaTime));
                // return collision data
                
                CollisionInfo results;
                for(int thisIndex = 0; thisIndex < this.Data.Polygons.Count; thisIndex++) {
                    for(int thatIndex = 0; thatIndex < body.Data.Polygons.Count; thatIndex++) {
                        results = this.Data.Polygons[thisIndex].Collide(this.Position, this.Scale, body.Position, body.Scale, body.Data.Polygons[thatIndex], delta);
                        if(results.Collided) return results;
                    } //thatIndex
                } //thisIndex

                return CollisionInfo.Empty();
            } //Collide

            public virtual void Update(float deltaTime, bool ignoreMass = false)
            {
                if(!ignoreMass && this.InvMass == 0.0F) {
                    this.Velocity.Clear();
                    return;
                }

                this.Position += (this.Velocity * deltaTime);
            } //Update

            public virtual void RespondToCollision(Body targetBody, CollisionInfo info, float deltaTime)
            {
                // overlapped. then separate the bodies.
                if(info.Overlapped) {
                    this.Position += info.MTD * (this.InvMass / (this.InvMass + targetBody.InvMass));
                    targetBody.Position -= info.MTD * (targetBody.InvMass / (this.InvMass + targetBody.InvMass));
                }

                float tcoll = 0.0F;
                Vector ncoll = new Vector(0.0F, 0.0F);

                // move bodies to collision time
                if(info.Collided && info.TEnter > 0.0F) {
                    tcoll = info.TEnter;
                    ncoll = info.NEnter;
                } else {
                    if(info.MTDLengthSquared < 0.00001F) return;
                    tcoll = 0.0F;
                    ncoll = info.MTD / (float)(System.Math.Sqrt(info.MTDLengthSquared));
                }

                this.Position += (this.Velocity * deltaTime) * tcoll;
                targetBody.Position += (targetBody.Velocity * deltaTime) * tcoll;

                // Get the dot product of the difference in the velocities and the ncoll
                float vn = (this.Velocity - targetBody.Velocity).Dot(ncoll);
                if(vn > 0.0F) return;

                float CoR = 1.0F;
                float numer = -(1.0F + CoR) * vn;
                float denom = (this.InvMass + targetBody.InvMass);
                Vector j = ncoll * (numer / denom);

                this.Velocity += j * this.InvMass;
                targetBody.Velocity -= j * targetBody.InvMass;
            } //RespondToCollision

            public float Mass
            {
                get {
                    if(this.InvMass <= 0.0F)
                        return 0.0F;
                    else
                        return 1.0F / this.InvMass;
                }
                set {
                    if(value <= 0.0F)
                        this.InvMass = 0.0F;
                    else
                        this.InvMass = 1.0F / value;
                }
            } //Mass

            public void Push(float force, Angle angle) { this.Velocity.Push(force, angle); }
        } //Body

        public static class CollisionTests
        {
            private static LineIntersectResults sTest1A, sTest1B;
            private static LineIntersectResults sTest2A, sTest2B;

            public enum LineIntersectResults { Clockwise = 0, Line = 1, CounterClockwise = 2 }

            public static bool PointInAxisAlignedRectangleTest(Vector point, Vector corner, Vector side1, Vector side2)
            {
                Vector v = (point - corner);

                float dot_v_v1 = v.Dot(side1);
                if(0 <= dot_v_v1) {
                    if(dot_v_v1 <= side1.Dot(side1)) {
                        float dot_v_v2 = v.Dot(side2);
                        if(0 <= dot_v_v2 && dot_v_v2 <= side2.Dot(side2)) return true;
                    }
                }

                return false;
            } //PointInAxisAlignedRectangleTest

            public static bool PointInAxisAlignedRectangleTest2(Vector point, Vector corner, Vector corner2, Vector corner3)
            {
                // Convert the corners and return the results
                return PointInAxisAlignedRectangleTest(point, corner, corner2 - corner, corner3 - corner);
            } //PointInAxisAlignedRectangleTest2

            public static bool AABB(Vector point, Vector lT, Vector rB)
            {
                return AABB(point, lT.X, lT.Y, rB.X, rB.Y);
            } //AABB

            public static bool AABB(Vector point, Rectangle r)
            {
                return AABB(point, r.Left, r.Top, r.Right, r.Bottom);
            } //AABB

            public static bool AABB(Vector point, float x1, float y1, float x2, float y2)
            {
                return point.X >= x1 && point.Y >= y1 && point.X <= x2 && point.Y <= y2;
            } //AABB

            public static bool AABB(Rectangle a, Rectangle b)
            {
                return a.Right >= b.Left && a.Bottom >= b.Top && a.Left <= b.Right && a.Top <= b.Bottom;
            } //AABB

            public static bool CircleInCircleCollisionTest(float circle1X, float circle1Y, float circle1Radius, float circle2X, float circle2Y, float circle2Radius)
            {
                return System.Math.Abs(Distance(circle1X, circle1Y, circle2X, circle2Y)) < (circle1Radius + circle2Radius);
            } //CircleInCircleCollisionTest

            public static bool CircleInAABBCollisionTest(float circleX, float circleY, float circleRadius, float rectLeft, float rectTop, float rectWidth, float rectHeight)
            {
                // get distance to center of the rectangle.
                float rectHalfW = (rectWidth * 0.5F); //to reduce the multiplication used
                float rectHalfH = (rectHeight * 0.5F);
                float diffX = System.Math.Abs(circleX - (rectLeft + rectHalfW));
                float diffY = System.Math.Abs(circleY - (rectTop + rectHalfH));

                // check outside edge of circle (as a box.)
                if(diffX > rectHalfW + circleRadius || diffY > rectHalfH + circleRadius) return false;
                // check inside edge of circle (as a box.)
                if(diffX <= rectHalfW || diffY <= rectHalfH) return true;
                // check edges of the rectangle.
                return Geometry.DotProduct(diffX - rectHalfW, diffY - rectHalfH) <= (circleRadius * circleRadius);
            } //CircleInAABBCollisionTest

            public static bool CircleInNonAABBCornerIntersectTest(float circleX, float circleY, float circleRadius, float boxPoint1X, float boxPoint1Y, float boxPoint2X, float boxPoint2Y, float boxPoint3X, float boxPoint3Y)
            {
                Geometry.CollisionTests.CircleOnSlopeIntersectTest(boxPoint1X, boxPoint1Y, boxPoint2X, boxPoint2Y, circleX, circleY, circleRadius, out sTest1A, out sTest2A);
                Geometry.CollisionTests.CircleOnSlopeIntersectTest(boxPoint2X, boxPoint2Y, boxPoint3X, boxPoint3Y, circleX, circleY, circleRadius, out sTest1B, out sTest2B);

                if(sTest1A == LineIntersectResults.Line && sTest1B == LineIntersectResults.Line) {
                    if(sTest2A == LineIntersectResults.Clockwise && sTest2B == LineIntersectResults.Clockwise)
                        // cause we are testing with a circle, we need to check the middle point.
                        return PointInCircleCollisionTest(circleX, circleY, circleRadius, boxPoint2X, boxPoint2Y);
                    else
                        return true;
                } else if(sTest1A != LineIntersectResults.Clockwise && sTest1B != LineIntersectResults.Clockwise)
                    return true;
                else
                    return false;
            } //CircleInNonAABBCornerIntersectTest

            public static bool CircleInNonAABBCollisionTest(float circleX, float circleY, float circleRadius, float boxPoint1X, float boxPoint1Y, float boxPoint2X, float boxPoint2Y, float boxPoint3X, float boxPoint3Y, float boxPoint4X, float boxPoint4Y)
            {
                // Going to check opposite corners for collisions.
                // need to some how check the two other corners without doing the slope intersect test again
                return CircleInNonAABBCornerIntersectTest(circleX, circleY, circleRadius, boxPoint1X, boxPoint1Y, boxPoint2X, boxPoint2Y, boxPoint3X, boxPoint3Y) && 
                       CircleInNonAABBCornerIntersectTest(circleX, circleY, circleRadius, boxPoint3X, boxPoint3Y, boxPoint4X, boxPoint4Y, boxPoint1X, boxPoint1Y);
            } //CircleInNonAABBCollisionTest

            public static void CircleOnSlopeIntersectTest(float startX, float startY, float endX, float endY, float pointX, float pointY, float pointRadius, out LineIntersectResults outSide, out LineIntersectResults outPointSide)
            {
                outSide = LineIntersectResults.Line;
                outPointSide = LineIntersectResults.Line;

                double dr = System.Math.Sqrt(DotProduct(endX - startX, endY - startY));
                float lineTest = CrossProduct(endX - startX, endY - startY, pointX - startX, pointY - startY);
                double discTest = ((pointRadius * pointRadius) * (dr * dr)) - (lineTest * lineTest);

                if(lineTest > 0.0F) {
                    outPointSide = LineIntersectResults.CounterClockwise;
                    if(discTest < 0.0F) outSide = LineIntersectResults.CounterClockwise; else outSide = LineIntersectResults.Line;
                } else if(lineTest < 0.0F) {
                    outPointSide = LineIntersectResults.Clockwise;
                    if(discTest < 0.0F) outSide = LineIntersectResults.Clockwise; else outSide = LineIntersectResults.Line;
                } else {
                    outSide = LineIntersectResults.Line;
                    outPointSide = LineIntersectResults.Line;
                }
            } //CircleOnSlopeIntersectTest

            public static bool PointInCircleCollisionTest(float circleX, float circleY, float circleRadius, float pointX, float pointY)
            {
                return Geometry.DotProduct(System.Math.Abs(circleX - pointX), System.Math.Abs(circleY - pointY)) <= (circleRadius * circleRadius);
            } //PointInCircleCollisionTest

            /// <summary>Returns which side of a line a point is on.</summary>
            public static void PointOnSlopeIntersectTest(float startX, float startY, float endX, float endY, float pointX, float pointY, out LineIntersectResults outResults)
            {
                float test = CrossProduct(endX - startX, endY - startY, pointX - startX, pointY - startY);
                if(test > 0.0F)
                    outResults = LineIntersectResults.CounterClockwise;
                else if(test < 0.0F)
                    outResults = LineIntersectResults.Clockwise;
                else
                    outResults = LineIntersectResults.Line;
            } //PointOnSlopeIntersectTest

            public static bool LineOnLineIntersectTest(float line1StartX, float line1StartY, float line1EndX, float line1EndY, float line2StartX, float line2StartY, float line2EndX, float line2EndY)
            {
                // check line2 points against line1 slope
                // two tests for, if(line is between the slope.
                PointOnSlopeIntersectTest(line1StartX, line1StartY, line1EndX, line1EndY, line2StartX, line2StartY, out sTest1A);
                PointOnSlopeIntersectTest(line1StartX, line1StartY, line1EndX, line1EndY, line2EndX, line2EndY, out sTest1B);

                if(sTest1A != sTest1B) {
                    // check line1 points against line2 slope
                    PointOnSlopeIntersectTest(line2StartX, line2StartY, line2EndX, line2EndY, line1StartX, line1StartY, out sTest1A);
                    PointOnSlopeIntersectTest(line2StartX, line2StartY, line2EndX, line2EndY, line1EndX, line1EndY, out sTest1B);
                    if(sTest1A != sTest1B) return true;
                }

                return false;
            } //LineOnLineIntersectTest

            public static bool PointInNonAABBCornerIntersectTest(float pointX, float pointY, float point1X, float point1Y, float point2X, float point2Y, float point3X, float point3Y)
            {
                PointOnSlopeIntersectTest(point1X, point1Y, point2X, point2Y, pointX, pointY, out sTest1A);
                PointOnSlopeIntersectTest(point2X, point2Y, point3X, point3Y, pointX, pointY, out sTest1B);
                return sTest1A == LineIntersectResults.CounterClockwise && sTest1B == LineIntersectResults.CounterClockwise;
            } //PointInNonAABBCornerIntersectTest

            public static bool PointInNonAABBCollisionTest(float pointX, float pointY, float point1X, float point1Y, float point2X, float point2Y, float point3X, float point3Y, float point4X, float point4Y)
            {
                return PointInNonAABBCornerIntersectTest(pointX, pointY, point1X, point1Y, point2X, point2Y, point3X, point3Y) && 
                       PointInNonAABBCornerIntersectTest(pointX, pointY, point3X, point3Y, point4X, point4Y, point1X, point1Y);
            } //PointInNonAABBCollisionTest
            
            public static bool NonAABBInNonAABBIntersectTest(float aTopLeftX, float aTopLeftY, float aTopRightX, float aTopRightY, float aBottomLeftX, float aBottomLeftY, float aBottomRightX, float aBottomRightY, float bTopLeftX, float bTopLeftY, float bTopRightX, float bTopRightY, float bBottomLeftX, float bBottomLeftY, float bBottomRightX, float bBottomRightY)
            {
                // This is a long function for checking if(two non-axis-aligned (scaled/rotated) rectangle collides with another.
                // for(this code we will be doing line intersect tests.
                // for(a collision to be true, any of rect1's lines must collide with rect2's lines.
                // each rectangle has 4 sides, so that is 4*4=16 tests.

                if(LineOnLineIntersectTest(aTopLeftX, aTopLeftY, aTopRightX, aTopRightY, bTopLeftX, bTopLeftY, bTopRightX, bTopRightY)) //north line to target north line
                    return true;
                else if(LineOnLineIntersectTest(aTopLeftX, aTopLeftY, aTopRightX, aTopRightY, bTopRightX, bTopRightY, bBottomRightX, bBottomRightY)) //north line to target east line
                    return true;
                else if(LineOnLineIntersectTest(aTopLeftX, aTopLeftY, aTopRightX, aTopRightY, bBottomRightX, bBottomRightY, bBottomLeftX, bBottomLeftY)) //north line to target south line
                    return true;
                else if(LineOnLineIntersectTest(aTopLeftX, aTopLeftY, aTopRightX, aTopRightY, bBottomLeftX, bBottomLeftY, bTopLeftX, bTopLeftY)) //north line to target west line
                    return true;
                else if(LineOnLineIntersectTest(aTopRightX, aTopRightY, aBottomRightX, aBottomRightY, bTopLeftX, bTopLeftY, bTopRightX, bTopRightY)) //east line to target north line
                    return true;
                else if(LineOnLineIntersectTest(aTopRightX, aTopRightY, aBottomRightX, aBottomRightY, bTopRightX, bTopRightY, bBottomRightX, bBottomRightY)) //east line to target east line
                    return true;
                else if(LineOnLineIntersectTest(aTopRightX, aTopRightY, aBottomRightX, aBottomRightY, bBottomRightX, bBottomRightY, bBottomLeftX, bBottomLeftY)) //east line to target south line
                    return true;
                else if(LineOnLineIntersectTest(aTopRightX, aTopRightY, aBottomRightX, aBottomRightY, bBottomLeftX, bBottomLeftY, bTopLeftX, bTopLeftY)) //east line to target west line
                    return true;
                else if(LineOnLineIntersectTest(aBottomRightX, aBottomRightY, aBottomLeftX, aBottomLeftY, bTopLeftX, bTopLeftY, bTopRightX, bTopRightY)) //south line to target north line
                    return true;
                else if(LineOnLineIntersectTest(aBottomRightX, aBottomRightY, aBottomLeftX, aBottomLeftY, bTopRightX, bTopRightY, bBottomRightX, bBottomRightY)) //south line to target east line
                    return true;
                else if(LineOnLineIntersectTest(aBottomRightX, aBottomRightY, aBottomLeftX, aBottomLeftY, bBottomRightX, bBottomRightY, bBottomLeftX, bBottomLeftY)) //south line to target south line
                    return true;
                else if(LineOnLineIntersectTest(aBottomRightX, aBottomRightY, aBottomLeftX, aBottomLeftY, bBottomLeftX, bBottomLeftY, bTopLeftX, bTopLeftY)) //south line to target west line
                    return true;
                else if(LineOnLineIntersectTest(aBottomLeftX, aBottomLeftY, aTopLeftX, aTopLeftY, bTopLeftX, bTopLeftY, bTopRightX, bTopRightY)) //west line to target north line
                    return true;
                else if(LineOnLineIntersectTest(aBottomLeftX, aBottomLeftY, aTopLeftX, aTopLeftY, bTopRightX, bTopRightY, bBottomRightX, bBottomRightY)) //west line to target east line
                    return true;
                else if(LineOnLineIntersectTest(aBottomLeftX, aBottomLeftY, aTopLeftX, aTopLeftY, bBottomRightX, bBottomRightY, bBottomLeftX, bBottomLeftY)) //west line to target south line
                    return true;
                else if(LineOnLineIntersectTest(aBottomLeftX, aBottomLeftY, aTopLeftX, aTopLeftY, bBottomLeftX, bBottomLeftY, bTopLeftX, bTopLeftY)) //west line to target west line
                    return true;
                else
                    return false;
            } //NonAABBInNonAABBIntersectTest

            public static bool NonAxisAlignedRectangleLineTest(float center1X, float center1Y, float size1X, float size1Y, Angle angle1, float center2X, float center2Y, float size2X, float size2Y, Angle angle2)
            {
                // This version of the function will transform two rectangles for testing. :)
                float half1x = size1X * 0.5F;
                float half1y = size1Y * 0.5F;
                float half2x = size2X * 0.5F;
                float half2y = size2Y * 0.5F;

                // First check, do a circular test to make sure the object is within bounds, don't want to waste cpu power.
                if(!CircleInCircleCollisionTest(center1X, center1Y, System.Math.Max(half1x, half1y), center2X, center2Y, System.Math.Max(half2x, half2y))) return false;

                // Now let's get to the cpu intensive part, 
                Vector rect1tl, rect1tr, rect1bl, rect1br, rect2tl, rect2tr, rect2bl, rect2br;
                RotatePointClockwise(out rect1tl.X, out rect1tl.Y, center1X, center1Y, -half1x, -half1y, angle1);
                RotatePointClockwise(out rect1tr.X, out rect1tr.Y, center1X, center1Y, half1x, -half1y, angle1);
                RotatePointClockwise(out rect1bl.X, out rect1bl.Y, center1X, center1Y, -half1x, half1y, angle1);
                RotatePointClockwise(out rect1br.X, out rect1br.Y, center1X, center1Y, half1x, half1y, angle1);
                RotatePointClockwise(out rect2tl.X, out rect2tl.Y, center2X, center2Y, -half2x, -half2y, angle2);
                RotatePointClockwise(out rect2tr.X, out rect2tr.Y, center2X, center2Y, half2x, -half2y, angle2);
                RotatePointClockwise(out rect2bl.X, out rect2bl.Y, center2X, center2Y, -half2x, half2y, angle2);
                RotatePointClockwise(out rect2br.X, out rect2br.Y, center2X, center2Y, half2x, half2y, angle2);

                // Now we can use the regular aligned rectangle line test to continue.
                return NonAABBInNonAABBIntersectTest(rect1tl.X, rect1tl.Y, rect1tr.X, rect1tr.Y, rect1bl.X, rect1bl.Y, rect1br.X, rect1br.Y, rect2tl.X, rect2tl.Y, rect2tr.X, rect2tr.Y, rect2bl.X, rect2bl.Y, rect2br.X, rect2br.Y);
            } //NonAxisAlignedRectangleLineTest
        } //CollisionTests

        public static class Physics
        {
            /// <summary>The Euler method for numerical integration.</summary>
            /// <remarks>v = v + a * dt</remarks>
            public static Vector ComputeVelocity(Vector velocity, Vector acceleration, float deltaValue)
            {
                return velocity + (acceleration * deltaValue);
            } //ComputeVelocity

            /// <remarks>a = f / m</remarks>
            public static Vector ComputeAcceleration(Vector force, float mass) { return force * mass; }

            /// <remarks>p = p + v * dt</remarks>
            public static Vector ComputePosition(Vector position, Vector velocity, float deltaValue)
            {
                return position + (velocity * deltaValue);
            } //ComputePosition

            /// <param name="k">Spring coefficient (elasticity coefficient, weights the spring's final force.)</param>
            /// <remarks>F = -k * dx
            /// dx (delta) = xc - xi (dx = current_position - inertial_position)</remarks>
            public static Vector ComputeSpringSingleForce(Vector pointA, Vector pointB, float k, float inertial_distance, float SpringTolerance = 0.0000000005F)
            {
                // Calculate the distance between the two points.
                Vector posDistance = (pointB - pointA);
                double fDistance = posDistance.Length();

                // if(the distance is less then the value tolerated then ... exit this procedure.
                if(fDistance < SpringTolerance) return Vector.Empty();

                // Normalize the distance.
                Vector posNormal = (posDistance / (float)fDistance);

                // Calculate the force.
                double delta = (fDistance - inertial_distance);
                double intensity = (-k * delta);

                // return the values.
                return posNormal * (float)(intensity);
            } //ComputeSpringSingleForce

            public static Vector ComputeSpringForces(Vector pointA, Vector pointB, float springConstant, float springStiffness, float springRestLength, float springSnap, bool outSnapped = false)
            {
                // Compute the distance.
                Vector posDistance = (pointB - pointA);
                double fDistance = posDistance.Length();

                // Check the break ...
                outSnapped = false;
                // ... if(the distance is beyond the maximum length then ...
                if(fDistance > (springRestLength * springSnap))
                    outSnapped = true;
                    // ... if(the distance is less then the rest length then ...
                else if(fDistance < (springRestLength / springSnap))
                    outSnapped = true;

                // Normalize the distance.
                Vector posNormal = (posDistance / (float)fDistance);

                // Calculate delta.
                double fSpringDelta = springStiffness * (fDistance - springRestLength);
                // Calculate the spring force. (F = -k * dx)
                double fSpringForce = (-springConstant * fSpringDelta);

                // Store the force.
                return posNormal * (float)fSpringForce;
            } //ComputeSpringForces

            public static Vector ComputeSpringForces2(Vector pointA, Vector pointB, Vector velocityA, Vector velocityB, float springConstant, float springLength, float frictionConstant)
            {
                Vector springVector = pointA - pointB;  // Vector Between The Two Masses

                double r = springVector.Length(); // Distance Between The Two Masses

                Vector force = Vector.Empty(); // Force Initially Has A Zero Value

                if(r != 0) // To Avoid A Division By Zero... Check if(r is Zero
                    // The Spring Force is Added To The Force      
                    force += -(springVector / (float)(r)) * (float)(r - springLength) * springConstant; //force = -k * (x - d)

                force += -(velocityA - velocityB) * frictionConstant; // The Friction Force is Added To The force
                // With This Addition We Obtain The Net Force Of The Spring
                return force;
            } //ComputeSpringForces2

            public static Vector ComputeStringForces(Vector pointA, Vector pointB, float stringMaximumLength)
            {
                double pointDistance = (pointB - pointA).Length();
                Vector pointDistanceRev = (pointA - pointB);

                // ... Get the distance between this point and the previous ...
                double fIntensity = 0.0F;
                if(pointDistance != 0) fIntensity = (pointDistance - stringMaximumLength) / pointDistance;

                return pointDistanceRev * (float)fIntensity;
            } //ComputeStringForces

            public static void ComputeImpulseAgainstObject(ref float sourcePositionX, ref float sourcePositionY, ref float sourceVelocityX, ref float sourceVelocityY, float sourceRadius, float sourceMass, float targetPositionX, ref float  targetPositionY, ref float targetVelocityX, ref float targetVelocityY, float targetRadius, float targetMass, float elasticity = 1.0F)
            {
                Vector diff = new Vector(targetPositionX - sourcePositionX, targetPositionY - sourcePositionY);
                float move = (float)(sourceRadius + targetRadius - diff.Length()) * 0.5F;

                Vector normal = diff.Unit();
                Vector shift = (normal * move);

                sourcePositionX -= shift.X;
                sourcePositionY -= shift.Y;
                targetPositionX += shift.X;
                targetPositionY += shift.Y;

                float vdX = sourceVelocityX - targetVelocityX;
                float vdY = sourceVelocityY - targetVelocityY;
                float dp = Geometry.DotProduct(vdX, vdY, normal.X, normal.Y);
                Vector impulse = normal * (2 * ((sourceMass * targetMass) / (sourceMass + targetMass)) * dp);
                impulse *= elasticity;

                sourceVelocityX -= (impulse.X / sourceMass);
                sourceVelocityY -= (impulse.Y / sourceMass);
                targetVelocityX += (impulse.X / targetMass);
                targetVelocityY += (impulse.Y / targetMass);
            } //ComputeImpulseAgainstObject

            public static void UpdateRopePhysics(List<BasePhysicsObject> objects, float springConstant, float springLength, float frictionConstant)
            {
                if(objects.Count < 2) return;

                // Apply forces to all springs.
                Vector force;
                for(int index = 0; index < objects.Count - 2; index++) {
                    force = ComputeSpringForces2(objects[index].Position, objects[index + 1].Position, objects[index].Velocity, objects[index + 1].Velocity, springConstant, springLength, frictionConstant);
                    objects[index].AddForces(force); // Force is Applied To mass1
                    objects[index + 1].AddForces(-force); // The Opposite Of Force is Applied To mass2
                } //index
            } //UpdateRopePhysics

            public class SpringObject
            {
                public BasePhysicsObject Object1;
                public BasePhysicsObject Object2;
                public float SpringConstant;
                public float SpringLength;
                public float FrictionConstant;

                public SpringObject() {}

                public SpringObject(BasePhysicsObject object1, BasePhysicsObject object2, float springConstant, float springForce, float frictionConstant, float springLength)
                {
                    this.Object1 = object1;
                    this.Object2 = object2;
                    this.SpringConstant = springConstant;
                    this.SpringLength = springLength;
                    this.FrictionConstant = frictionConstant;
                } //New

                public void Update()
                {
                    Vector force = ComputeSpringForces2(Object1.Position, Object2.Position, Object1.Velocity, Object2.Velocity, this.SpringConstant, this.SpringLength, this.FrictionConstant);
                    Object1.AddForces(force); // Force is Applied To mass1
                    Object2.AddForces(-force); // The Opposite Of Force is Applied To mass2
                } //Update
            } //SpringObject

            public class RopeObject
            {
                List<BasePhysicsObject> Objects = new List<BasePhysicsObject>();
                float SpringConstant, SpringLength, SpringFrictionConstant;

                public RopeObject(List<BasePhysicsObject> objects, float SpringConstant, float SpringLength, float SpringFrictionConstant)
                {
                    this.Objects = objects;
                    this.SpringConstant = SpringConstant;
                    this.SpringLength = SpringLength;
                    this.SpringFrictionConstant = SpringFrictionConstant;

                    if(this.Objects.Count > 1) {
                        //for(int index = 0 To (this.Objects.Count - 1)
                        //    this.Objects(index).Position = new Vector(index * SpringLength, 0.0F) // Set X-Position Of masses[a] With springLength Distance To Its Neighbor
                        //} //obj
                    }
                } //New

                public void Update()
                {
                    UpdateRopePhysics(this.Objects, this.SpringConstant, this.SpringLength, this.SpringFrictionConstant);
                } //Update
            } //RopeObject

            public interface BasePhysicsObject
            {
                Vector Position { get; set; }

                Vector Velocity { get; set; }
                
                void AddForces(Vector forces);
                void Update(float deltaTime);
            } //BasePhysicsObject
        } //Physics
    } //Geometry
} // MathLib namespace
