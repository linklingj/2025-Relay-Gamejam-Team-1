using JetBrains.Annotations;

namespace SeolMJ
{
    public interface IScalar
    {
        public double AsDouble();
        public void FromDouble(double value);

        public IScalar Clone();

        #region Calculations

        public static IScalar operator +(IScalar a)
        {
            return a.Clone();
        }

        public static IScalar operator -(IScalar a)
        {
            var value = a.Clone();
            value.FromDouble(-a.AsDouble());
            return value;
        }

        public static IScalar operator +(IScalar a, IScalar b)
        {
            var value = a.Clone();
            value.FromDouble(a.AsDouble() + b.AsDouble());
            return value;
        }

        public static IScalar operator +(IScalar a, double b)
        {
            var value = a.Clone();
            value.FromDouble(a.AsDouble() + b);
            return value;
        }

        public static IScalar operator +(double a, IScalar b)
        {
            return b + a;
        }

        public static IScalar operator -(IScalar a, IScalar b)
        {
            var value = a.Clone();
            value.FromDouble(a.AsDouble() - b.AsDouble());
            return value;
        }

        public static IScalar operator -(IScalar a, double b)
        {
            var value = a.Clone();
            value.FromDouble(a.AsDouble() - b);
            return value;
        }

        public static IScalar operator -(double a, IScalar b)
        {
            var value = b.Clone();
            value.FromDouble(a - b.AsDouble());
            return value;
        }

        public static IScalar operator *(IScalar a, IScalar b)
        {
            var value = a.Clone();
            value.FromDouble(a.AsDouble() * b.AsDouble());
            return value;
        }

        public static IScalar operator *(IScalar a, double b)
        {
            var value = a.Clone();
            value.FromDouble(a.AsDouble() * b);
            return value;
        }

        public static IScalar operator *(double a, IScalar b)
        {
            return b * a;
        }

        public static IScalar operator /(IScalar a, IScalar b)
        {
            var value = a.Clone();
            value.FromDouble(a.AsDouble() / b.AsDouble());
            return value;
        }

        public static IScalar operator /(IScalar a, double b)
        {
            var value = a.Clone();
            value.FromDouble(a.AsDouble() / b);
            return value;
        }

        public static IScalar operator /(double a, IScalar b)
        {
            var value = b.Clone();
            value.FromDouble(a / b.AsDouble());
            return value;
        }

        public static bool OO(IScalar a, IScalar b)
        {
            return a.AsDouble() == b.AsDouble();
        }

        public static bool OO(IScalar a, double b)
        {
            return a.AsDouble() == b;
        }

        public static bool OO(double a, IScalar b)
        {
            return OO(b, a);
        }

        public static bool XX(IScalar a, IScalar b)
        {
            return a.AsDouble() != b.AsDouble();
        }

        public static bool XX(IScalar a, double b)
        {
            return a.AsDouble() != b;
        }

        public static bool XX(double a, IScalar b)
        {
            return XX(b, a);
        }

        public static bool oO(IScalar a, IScalar b)
        {
            return a.AsDouble() < b.AsDouble();
        }

        public static bool oO(IScalar a, double b)
        {
            return a.AsDouble() < b;
        }

        public static bool oO(double a, IScalar b)
        {
            return Oo(b, a);
        }

        public static bool o_O(IScalar a, IScalar b)
        {
            return a.AsDouble() <= b.AsDouble();
        }

        public static bool o_O(IScalar a, double b)
        {
            return a.AsDouble() <= b;
        }

        public static bool o_O(double a, IScalar b)
        {
            return O_o(b, a);
        }

        public static bool Oo(IScalar a, IScalar b)
        {
            return a.AsDouble() > b.AsDouble();
        }

        public static bool Oo(IScalar a, double b)
        {
            return a.AsDouble() > b;
        }

        public static bool Oo(double a, IScalar b)
        {
            return oO(b, a);
        }

        public static bool O_o(IScalar a, IScalar b)
        {
            return a.AsDouble() >= b.AsDouble();
        }

        public static bool O_o(IScalar a, double b)
        {
            return a.AsDouble() >= b;
        }

        public static bool O_o(double a, IScalar b)
        {
            return o_O(b, a);
        }

        #endregion

        public static IScalar<double> NewD(double value) => new DoubleWrapper() { Value = value };
    }

    public interface IScalar<T> : IScalar
    {
        public T As();
        public void From(T value);

        public new IScalar<T> Clone();

        #region Calculations

        public static IScalar<T> operator +(IScalar<T> a)
        {
            return a.Clone();
        }

        public static IScalar<T> operator -(IScalar<T> a)
        {
            var value = a.Clone();
            value.FromDouble(-a.AsDouble());
            return value;
        }

        public static IScalar<T> operator +(IScalar<T> a, IScalar<T> b)
        {
            var value = a.Clone();
            value.FromDouble(a.AsDouble() + b.AsDouble());
            return value;
        }

        public static IScalar<T> operator +(IScalar<T> a, double b)
        {
            var value = a.Clone();
            value.FromDouble(a.AsDouble() + b);
            return value;
        }

        public static IScalar<T> operator +(double a, IScalar<T> b)
        {
            return b + a;
        }

        public static IScalar<T> operator -(IScalar<T> a, IScalar<T> b)
        {
            var value = a.Clone();
            value.FromDouble(a.AsDouble() - b.AsDouble());
            return value;
        }

        public static IScalar<T> operator -(IScalar<T> a, double b)
        {
            var value = a.Clone();
            value.FromDouble(a.AsDouble() - b);
            return value;
        }

        public static IScalar<T> operator -(double a, IScalar<T> b)
        {
            var value = b.Clone();
            value.FromDouble(a - b.AsDouble());
            return value;
        }

        public static IScalar<T> operator *(IScalar<T> a, IScalar<T> b)
        {
            var value = a.Clone();
            value.FromDouble(a.AsDouble() * b.AsDouble());
            return value;
        }

        public static IScalar<T> operator *(IScalar<T> a, double b)
        {
            var value = a.Clone();
            value.FromDouble(a.AsDouble() * b);
            return value;
        }

        public static IScalar<T> operator *(double a, IScalar<T> b)
        {
            return b * a;
        }

        public static IScalar<T> operator /(IScalar<T> a, IScalar<T> b)
        {
            var value = a.Clone();
            value.FromDouble(a.AsDouble() / b.AsDouble());
            return value;
        }

        public static IScalar<T> operator /(IScalar<T> a, double b)
        {
            var value = a.Clone();
            value.FromDouble(a.AsDouble() / b);
            return value;
        }

        public static IScalar<T> operator /(double a, IScalar<T> b)
        {
            var value = b.Clone();
            value.FromDouble(a / b.AsDouble());
            return value;
        }

        public static bool OO(IScalar<T> a, IScalar<T> b)
        {
            return a.AsDouble() == b.AsDouble();
        }

        public static bool OO(IScalar<T> a, double b)
        {
            return a.AsDouble() == b;
        }

        public static bool OO(double a, IScalar<T> b)
        {
            return OO(b, a);
        }

        public static bool XX(IScalar<T> a, IScalar<T> b)
        {
            return a.AsDouble() != b.AsDouble();
        }

        public static bool XX(IScalar<T> a, double b)
        {
            return a.AsDouble() != b;
        }

        public static bool XX(double a, IScalar<T> b)
        {
            return XX(b, a);
        }

        public static bool oO(IScalar<T> a, IScalar<T> b)
        {
            return a.AsDouble() < b.AsDouble();
        }

        public static bool oO(IScalar<T> a, double b)
        {
            return a.AsDouble() < b;
        }

        public static bool oO(double a, IScalar<T> b)
        {
            return Oo(b, a);
        }

        public static bool o_O(IScalar<T> a, IScalar<T> b)
        {
            return a.AsDouble() <= b.AsDouble();
        }

        public static bool o_O(IScalar<T> a, double b)
        {
            return a.AsDouble() <= b;
        }

        public static bool o_O(double a, IScalar<T> b)
        {
            return O_o(b, a);
        }

        public static bool Oo(IScalar<T> a, IScalar<T> b)
        {
            return a.AsDouble() > b.AsDouble();
        }

        public static bool Oo(IScalar<T> a, double b)
        {
            return a.AsDouble() > b;
        }

        public static bool Oo(double a, IScalar<T> b)
        {
            return oO(b, a);
        }

        public static bool O_o(IScalar<T> a, IScalar<T> b)
        {
            return a.AsDouble() >= b.AsDouble();
        }

        public static bool O_o(IScalar<T> a, double b)
        {
            return a.AsDouble() >= b;
        }

        public static bool O_o(double a, IScalar<T> b)
        {
            return o_O(b, a);
        }

        #endregion

        public static IScalar<T> New(T value)
        {
            if (value is double d)
            {
                return (IScalar<T>)(IScalar)new DoubleWrapper() { Value = d };
            }
            if (value is float f)
            {
                return (IScalar<T>)(IScalar)new FloatWrapper() { Value = f };
            }
            if (value is long l)
            {
                return (IScalar<T>)(IScalar)new LongWrapper() { Value = l };
            }
            if (value is int i)
            {
                return (IScalar<T>)(IScalar)new IntWrapper() { Value = i };
            }
            if (value is short s)
            {
                return (IScalar<T>)(IScalar)new ShortWrapper() { Value = s };
            }
            if (value is bool b)
            {
                return (IScalar<T>)(IScalar)new BoolWrapper() { Value = b };
            }
            return null;
        }

        public static new IScalar<T> NewD(double value)
        {
            var scalar = New(default);
            scalar.FromDouble(value);
            return scalar;
        }
    }

    public interface IWhole
    {
        public long AsLong();
        public void FromLong(long value);
    }

    public struct DoubleWrapper : IScalar<double>
    {
        public double Value;

        public readonly double As() => Value;
        public void From(double value) => Value = value;

        public readonly double AsDouble() => Value;
        public void FromDouble(double value) => Value = value;

        public readonly IScalar<double> Clone() => this;
        readonly IScalar IScalar.Clone() => this;

        public static implicit operator DoubleWrapper(double value) => new() { Value = value };
        public static implicit operator double(DoubleWrapper value) => value.Value;
    }

    public struct FloatWrapper : IScalar<float>
    {
        public float Value;

        public readonly float As() => Value;
        public void From(float value) => Value = value;

        public readonly double AsDouble() => Value;
        public void FromDouble(double value) => Value = (float)value;

        public readonly IScalar<float> Clone() => this;
        readonly IScalar IScalar.Clone() => this;

        public static implicit operator FloatWrapper(float value) => new() { Value = value };
        public static implicit operator float(FloatWrapper value) => value.Value;
    }

    public struct LongWrapper : IScalar<long>, IWhole
    {
        public long Value;

        public readonly long As() => Value;
        public void From(long value) => Value = value;

        public readonly double AsDouble() => Value;
        public void FromDouble(double value) => Value = (long)value;

        public readonly long AsLong() => Value;
        public void FromLong(long value) => Value = value;

        public readonly IScalar<long> Clone() => this;
        readonly IScalar IScalar.Clone() => this;

        public static implicit operator LongWrapper(long value) => new() { Value = value };
        public static implicit operator long(LongWrapper value) => value.Value;
    }

    public struct IntWrapper : IScalar<int>, IWhole
    {
        public int Value;

        public readonly int As() => Value;
        public void From(int value) => Value = value;

        public readonly double AsDouble() => Value;
        public void FromDouble(double value) => Value = (int)value;

        public readonly long AsLong() => Value;
        public void FromLong(long value) => Value = (int)value;

        public readonly IScalar<int> Clone() => this;
        readonly IScalar IScalar.Clone() => this;

        public static implicit operator IntWrapper(int value) => new() { Value = value };
        public static implicit operator int(IntWrapper value) => value.Value;
    }

    public struct ShortWrapper : IScalar<short>, IWhole
    {
        public short Value;

        public readonly short As() => Value;
        public void From(short value) => Value = value;

        public readonly double AsDouble() => Value;
        public void FromDouble(double value) => Value = (short)value;

        public readonly long AsLong() => Value;
        public void FromLong(long value) => Value = (short)value;

        public readonly IScalar<short> Clone() => this;
        readonly IScalar IScalar.Clone() => this;

        public static implicit operator ShortWrapper(short value) => new() { Value = value };
        public static implicit operator short(ShortWrapper value) => value.Value;
    }

    public struct BoolWrapper : IScalar<bool>, IWhole
    {
        public bool Value;

        public readonly bool As() => Value;
        public void From(bool value) => Value = value;

        public readonly double AsDouble() => Value ? 1 : 0;
        public void FromDouble(double value) => Value = value != 0;

        public readonly long AsLong() => Value ? 1 : 0;
        public void FromLong(long value) => Value = value != 0;

        public readonly IScalar<bool> Clone() => this;
        readonly IScalar IScalar.Clone() => this;

        public static implicit operator BoolWrapper(bool value) => new() { Value = value };
        public static implicit operator bool(BoolWrapper value) => value.Value;
    }
}