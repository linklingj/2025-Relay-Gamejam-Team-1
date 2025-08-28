using UnityEngine;

namespace SeolMJ
{
    public interface IVector
    {
        public int Length { get; }
        public IScalar this[int index] { get; set; }

        public IVector Clone();

        #region Calculations

        public static IVector operator +(IVector a)
        {
            return a.Clone();
        }

        public static IVector operator -(IVector a)
        {
            IVector value = a.Clone();
            for (int i = 0; i < value.Length; i++)
            {
                value[i].FromDouble(-value[i].AsDouble());
            }
            return value;
        }

        public static IVector operator +(IVector a, IVector b)
        {
            IVector values = a.Clone();
            for (int i = 0; i < a.Length; i++)
            {
                double value = a[i].AsDouble();
                if (i < b.Length)
                {
                    value += b[i].AsDouble();
                }
                values[i].FromDouble(value);
            }
            return values;
        }

        public static IVector operator +(IVector a, IScalar b)
        {
            IVector values = a.Clone();
            for (int i = 0; i < a.Length; i++)
            {
                values[i].FromDouble(a[i].AsDouble() + b.AsDouble());
            }
            return values;
        }

        public static IVector operator +(IScalar a, IVector b)
        {
            return b + a;
        }

        public static IVector operator -(IVector a, IVector b)
        {
            return a + (-b);
        }

        public static IVector operator -(IVector a, IScalar b)
        {
            IVector values = a.Clone();
            for (int i = 0; i < a.Length; i++)
            {
                values[i].FromDouble(a[i].AsDouble() - b.AsDouble());
            }
            return values;
        }

        public static IVector operator -(IScalar a, IVector b)
        {
            IVector values = b.Clone();
            for (int i = 0; i < b.Length; i++)
            {
                values[i].FromDouble(a.AsDouble() - b[i].AsDouble());
            }
            return values;
        }

        public static IVector operator *(IVector a, IVector b)
        {
            IVector values = a.Clone();
            for (int i = 0; i < a.Length; i++)
            {
                double value = a[i].AsDouble();
                if (i < b.Length)
                {
                    value *= b[i].AsDouble();
                }
                values[i].FromDouble(value);
            }
            return values;
        }

        public static IVector operator *(IVector a, IScalar b)
        {
            IVector values = a.Clone();
            for (int i = 0; i < a.Length; i++)
            {
                values[i].FromDouble(a[i].AsDouble() * b.AsDouble());
            }
            return values;
        }

        public static IVector operator *(IScalar a, IVector b)
        {
            return b * a;
        }

        public static IVector operator /(IVector a, IVector b)
        {
            IVector values = a.Clone();
            for (int i = 0; i < a.Length; i++)
            {
                double value = a[i].AsDouble() / b[i].AsDouble();
                if (i < b.Length)
                {
                    value /= b[i].AsDouble();
                }
                values[i].FromDouble(value);
            }
            return values;
        }

        public static IVector operator /(IVector a, IScalar b)
        {
            IVector values = a.Clone();
            for (int i = 0; i < a.Length; i++)
            {
                values[i].FromDouble(a[i].AsDouble() / b.AsDouble());
            }
            return values;
        }

        public static IVector operator /(IScalar a, IVector b)
        {
            IVector values = b.Clone();
            for (int i = 0; i < b.Length; i++)
            {
                values[i].FromDouble(a.AsDouble() / b[i].AsDouble());
            }
            return values;
        }

        #endregion
    }

    public interface IVector<T> : IVector
    {
        public new IScalar<T> this[int index] { get; set; }

        public new IVector<T> Clone();

        #region Calculations

        public static IVector<T> operator +(IVector<T> a)
        {
            return a.Clone();
        }

        public static IVector<T> operator -(IVector<T> a)
        {
            IVector<T> value = a.Clone();
            for (int i = 0; i < value.Length; i++)
            {
                value[i].FromDouble(-value[i].AsDouble());
            }
            return value;
        }

        public static IVector<T> operator +(IVector<T> a, IVector<T> b)
        {
            IVector<T> values = a.Clone();
            for (int i = 0; i < a.Length; i++)
            {
                double value = a[i].AsDouble();
                if (i < b.Length)
                {
                    value += b[i].AsDouble();
                }
                values[i].FromDouble(value);
            }
            return values;
        }

        public static IVector<T> operator +(IVector<T> a, IScalar<T> b)
        {
            IVector<T> values = a.Clone();
            for (int i = 0; i < a.Length; i++)
            {
                values[i].FromDouble(a[i].AsDouble() + b.AsDouble());
            }
            return values;
        }

        public static IVector<T> operator +(IScalar<T> a, IVector<T> b)
        {
            return b + a;
        }

        public static IVector<T> operator -(IVector<T> a, IVector<T> b)
        {
            return a + (-b);
        }

        public static IVector<T> operator -(IVector<T> a, IScalar<T> b)
        {
            IVector<T> values = a.Clone();
            for (int i = 0; i < a.Length; i++)
            {
                values[i].FromDouble(a[i].AsDouble() - b.AsDouble());
            }
            return values;
        }

        public static IVector<T> operator -(IScalar<T> a, IVector<T> b)
        {
            IVector<T> values = b.Clone();
            for (int i = 0; i < b.Length; i++)
            {
                values[i].FromDouble(a.AsDouble() - b[i].AsDouble());
            }
            return values;
        }

        public static IVector<T> operator *(IVector<T> a, IVector<T> b)
        {
            IVector<T> values = a.Clone();
            for (int i = 0; i < a.Length; i++)
            {
                double value = a[i].AsDouble();
                if (i < b.Length)
                {
                    value *= b[i].AsDouble();
                }
                values[i].FromDouble(value);
            }
            return values;
        }

        public static IVector<T> operator *(IVector<T> a, IScalar<T> b)
        {
            IVector<T> values = a.Clone();
            for (int i = 0; i < a.Length; i++)
            {
                values[i].FromDouble(a[i].AsDouble() * b.AsDouble());
            }
            return values;
        }

        public static IVector<T> operator *(IScalar<T> a, IVector<T> b)
        {
            return b * a;
        }

        public static IVector<T> operator /(IVector<T> a, IVector<T> b)
        {
            IVector<T> values = a.Clone();
            for (int i = 0; i < a.Length; i++)
            {
                double value = a[i].AsDouble() / b[i].AsDouble();
                if (i < b.Length)
                {
                    value /= b[i].AsDouble();
                }
                values[i].FromDouble(value);
            }
            return values;
        }

        public static IVector<T> operator /(IVector<T> a, IScalar<T> b)
        {
            IVector<T> values = a.Clone();
            for (int i = 0; i < a.Length; i++)
            {
                values[i].FromDouble(a[i].AsDouble() / b.AsDouble());
            }
            return values;
        }

        public static IVector<T> operator /(IScalar<T> a, IVector<T> b)
        {
            IVector<T> values = b.Clone();
            for (int i = 0; i < b.Length; i++)
            {
                values[i].FromDouble(a.AsDouble() / b[i].AsDouble());
            }
            return values;
        }

        #endregion
    }

    public struct Vector2Wrapper : IVector<float>
    {
        FloatWrapper[] value;
        public readonly Vector2 Value
        {
            get => new(value[0].As(), value[1].As());
            set
            {
                this.value[0].From(value.x);
                this.value[1].From(value.y);
            }
        }

        public readonly int Length => 2;
        public IScalar<float> this[int index]
        {
            get
            {
                value ??= new FloatWrapper[2];
                return value[index];
            }
            set
            {
                this.value ??= new FloatWrapper[2];
                this.value[index].From(value.As());
            }
        }
        IScalar IVector.this[int index]
        {
            get => this[index];
            set => this[index].FromDouble(value.AsDouble());
        }

        public IVector<float> Clone() => new Vector2Wrapper() { value = value != null ? (FloatWrapper[])value.Clone() : null };
        IVector IVector.Clone() => Clone();
    }

    public struct Vector3Wrapper : IVector<float>
    {
        FloatWrapper[] value;
        public readonly Vector3 Value
        {
            get => new(value[0].As(), value[1].As(), value[2].As());
            set
            {
                this.value[0].From(value.x);
                this.value[1].From(value.y);
                this.value[2].From(value.z);
            }
        }

        public readonly int Length => 3;
        public IScalar<float> this[int index]
        {
            get
            {
                value ??= new FloatWrapper[3];
                return value[index];
            }
            set
            {
                this.value ??= new FloatWrapper[3];
                this.value[index].From(value.As());
            }
        }
        IScalar IVector.this[int index]
        {
            get => this[index];
            set => this[index].FromDouble(value.AsDouble());
        }

        public IVector<float> Clone() => new Vector3Wrapper() { value = value != null ? (FloatWrapper[])value.Clone() : null };
        IVector IVector.Clone() => Clone();
    }
}