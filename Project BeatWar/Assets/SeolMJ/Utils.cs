using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SeolMJ
{
    public static class Utils
    {
        #region Constants

        public const float PI = Mathf.PI;
        public const float TAU = 2f * Mathf.PI;

        #endregion

        #region Vectors
        public static float Vec2Deg(Vector2 vector)
            => Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;

        public static bool IsZero(this Vector3 vector)
            => vector.x == 0 && vector.y == 0 && vector.z == 0;

        public static bool IsZero(this Vector2 vector)
            => vector.x == 0 && vector.y == 0;

        public static bool IsZero(this Vector2Int vector)
            => vector.x == 0 && vector.y == 0;

        public static bool IsApproximatelyZero(this Vector2 vector) =>
            Mathf.Approximately(vector.x, 0) && Mathf.Approximately(vector.y, 0);

        public static bool IsAlmostZero(this Vector2 vector, float limit) =>
            Mathf.Abs(vector.x) < limit && Mathf.Abs(vector.y) < limit;

        public static float AbsDeltaAngle(float one, float two)
            => Mathf.Abs(Mathf.DeltaAngle(one, two));

        public static bool IsInt(this Vector2 vector)
            => vector == Vector2Int.RoundToInt(vector);

        public static Vector2 Clamp(this Vector2 vector, float min = 0, float max = 1)
            => vector.normalized * Mathf.Clamp(vector.magnitude, min, max);

        public static Vector3 Clamp(this Vector3 vector, float min = 0, float max = 1)
            => vector.normalized * Mathf.Clamp(vector.magnitude, min, max);

        public static Vector2 NemoNemoBeam(this Vector2 vector)
            => vector * Mathf.Min(Mathf.Abs(1f / vector.x), Mathf.Abs(1f / vector.y));

        public static Vector2 Sign(this Vector2 vector)
            => new Vector2(Mathf.Sign(vector.x), Mathf.Sign(vector.y));

        public static Vector2Int SignToInt(this Vector2 vector)
            => new Vector2Int(Math.Sign(vector.x), Math.Sign(vector.y));

        public static Vector2 SpringDamp(Vector2 current, Vector2 target, ref Vector2 velocity, ref Vector2 velvel, float speed, float damp)
            => SpringDamp(current, target, ref velocity, ref velvel, speed, damp, Time.deltaTime);

        public static Vector2 SpringDamp(Vector2 current, Vector2 target, ref Vector2 velocity, ref Vector2 velvel, float speed, float damp, float deltaTime)
        {
            velocity += speed * deltaTime * (target - current);
            velocity = Vector2.SmoothDamp(velocity, Vector2.zero, ref velvel, damp, Mathf.Infinity, deltaTime);
            return current + velocity * deltaTime;
        }

        public static Vector2 SpringDampLimited(Vector2 current, Vector2 target, ref Vector2 velocity, ref Vector2 velvel, float speed, float damp, float limit)
            => SpringDampLimited(current, target, ref velocity, ref velvel, speed, damp, limit, Time.deltaTime);

        public static Vector2 SpringDampLimited(Vector2 current, Vector2 target, ref Vector2 velocity, ref Vector2 velvel, float speed, float damp, float limit, float deltaTime)
        {
            velocity += speed * deltaTime * (target - current);
            velocity = Vector2.SmoothDamp(velocity, Vector2.zero, ref velvel, damp, Mathf.Infinity, deltaTime);
            velocity = velocity.normalized * Mathf.Clamp(velocity.magnitude, 0, limit);
            return current + velocity * deltaTime;
        }

        public static Vector3 SpringDamp(Vector3 current, Vector3 target, ref Vector3 velocity, ref Vector3 velvel, float speed, float damp)
            => SpringDamp(current, target, ref velocity, ref velvel, speed, damp, Time.deltaTime);

        public static Vector3 SpringDamp(Vector3 current, Vector3 target, ref Vector3 velocity, ref Vector3 velvel, float speed, float damp, float deltaTime)
        {
            velocity += speed * deltaTime * (target - current);
            velocity = Vector3.SmoothDamp(velocity, Vector3.zero, ref velvel, damp, Mathf.Infinity, deltaTime);
            return current + velocity * deltaTime;
        }

        public static Vector3 ToVector3(float scale) =>
            new Vector3(scale, scale, scale);

        public static Vector3 ToVector3(float scale, float z) =>
            new Vector3(scale, scale, z);

        public static Vector3 ToVector3(Vector2 vector, float z = 0f) =>
            new Vector3(vector.x, vector.y, z);

        public static Vector2 Slerp(Vector2 a, Vector2 b, float t)
        {
            return Vector3.Slerp(a, b, t);
        }
        #endregion

        #region Ints

        public static int SignToInt(float input)
        {
            if (input > 0) return 1;
            if (input < 0) return -1;
            return 0;
        }

        #endregion

        #region Floats
        public static float SpringDamp(float current, float target, ref float velocity, ref float velvel, float speed, float damp)
            => SpringDamp(current, target, ref velocity, ref velvel, speed, damp, Time.deltaTime);

        public static float SpringDamp(float current, float target, ref float velocity, ref float velvel, float speed, float damp, float deltaTime)
        {
            velocity += speed * deltaTime * (target - current);
            velocity = Mathf.SmoothDamp(velocity, 0, ref velvel, damp, Mathf.Infinity, deltaTime);
            return current + velocity * deltaTime;
        }

        public static float SpringDamp(float current, float target, ref float velocity, ref float velvel, float speed, float damp, float deltaTime, float limit)
        {
            velocity += speed * deltaTime * (target - current);
            velocity = Mathf.SmoothDamp(velocity, 0, ref velvel, damp, Mathf.Infinity, deltaTime);
            velocity = Mathf.Clamp(velocity, -limit, limit);
            return current + velocity * deltaTime;
        }
        #endregion

        #region Colors
        public static void EditAlpha(this Image image, float alpha)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
        }

        public static Color OnlyAlpha(Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }

        public static Vector3 RGB2OKLAB(Color color)
        {
            var l = 0.4122214708 * color.r + 0.5363325363 * color.g + 0.0514459929 * color.b;
            var m = 0.2119034982 * color.r + 0.6806995451 * color.g + 0.1073969566 * color.b;
            var s = 0.0883024619 * color.r + 0.2817188376 * color.g + 0.6299787005 * color.b;
            l = Math.Cbrt(l);
            m = Math.Cbrt(m);
            s = Math.Cbrt(s);
            return new Vector3
            (
                (float)(l * +0.2104542553 + m * +0.7936177850 + s * -0.0040720468),
                (float)(l * +1.9779984951 + m * -2.4285922050 + s * +0.4505937099),
                (float)(l * +0.0259040371 + m * +0.7827717662 + s * -0.8086757660)
            );
        }

        public static Color OKLAB2RGB(Vector3 color)
        {
            var l = color.x + color.y * +0.3963377774 + color.z * +0.2158037573;
            var m = color.x + color.y * -0.1055613458 + color.z * -0.0638541728;
            var s = color.x + color.y * -0.0894841775 + color.z * -1.2914855480;
            l *= l * l;
            m *= m * m;
            s *= s * s;
            return new Color
            (
                (float)(l * +4.0767416621 + m * -3.3077115913 + s * +0.2309699292),
                (float)(l * -1.2684380046 + m * +2.6097574011 + s * -0.3413193965),
                (float)(l * -0.0041960863 + m * -0.7034186147 + s * +1.7076147010),
                1f
            );
        }

        public static Vector3 OKLCH2OKLAB(Vector3 oklch)
        {
            var h = oklch.z * Math.PI * 2;
            return new Vector3
            (
                oklch.x,
                (float)(oklch.y * Math.Cos(h)),
                (float)(oklch.y * Math.Sin(h))
            );
        }

        public static Vector3 OKLAB2OKLCH(Vector3 oklab)
        {
            var h = oklab.z * Math.PI * 2;
            return new Vector3
            (
                oklab.x,
                (float)Math.Sqrt(oklab.y * oklab.y + oklab.z * oklab.z),
                (float)(Math.Atan2(oklab.z, oklab.y) / (Math.PI * 2))
            );
        }
        #endregion

        #region Threading
        public static Task waitOneMiliSec = Task.Delay(1);

        public static async void TimeOut(Action action, TimeSpan timeout, Action onTimeOut = null, Action onSuccess = null)
        {
            Task task = Task.Run(action);
            using (var cancel = new CancellationTokenSource())
            {
                Task completed = await Task.WhenAny(task, Task.Delay(timeout, cancel.Token));
                if (completed == task)
                {
                    if (completed.IsFaulted) throw completed.Exception;
                    cancel.Cancel();
                    onSuccess?.Invoke();
                }
                else
                {
                    onTimeOut?.Invoke();
                }
            }
        }
        #endregion

        #region SmoothDamp
        public static void SmoothDamp(ref this Vector3 current, Vector3 to, ref Vector3 velocity, float time)
        {
            current = Vector3.SmoothDamp(current, to, ref velocity, time);
        }

        public static void SmoothDamp(ref this Vector2 current, Vector2 to, ref Vector2 velocity, float time)
        {
            current = Vector2.SmoothDamp(current, to, ref velocity, time);
        }

        public static void SmoothDamp(this RectTransform rect, Vector2 target, ref Vector2 velocity, float speed, float deltaTime, float maxSpeed = float.PositiveInfinity)
        {
            rect.anchoredPosition = Vector2.SmoothDamp(rect.anchoredPosition, target, ref velocity, speed, maxSpeed, deltaTime);
        }

        public static void SmoothDamp(ref this float current, float target, ref float velocity, float time)
        {
            current = Mathf.SmoothDamp(current, target, ref velocity, time);
        }

        public static Color SmoothDamp(Color current, Color target, ref Color velocity, float time)
        {
            return new Color(
                Mathf.SmoothDamp(current.r, target.r, ref velocity.r, time),
                Mathf.SmoothDamp(current.g, target.g, ref velocity.g, time),
                Mathf.SmoothDamp(current.b, target.b, ref velocity.b, time),
                Mathf.SmoothDamp(current.a, target.a, ref velocity.a, time)
                );
        }
        #endregion

        #region Components
        public static T Copy<T>(this Component comp, T other) where T : Component
        {
            Type type = comp.GetType();
            if (type != other.GetType()) return null; // type mis-match
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
            PropertyInfo[] pinfos = type.GetProperties(flags);
            foreach (var pinfo in pinfos)
            {
                if (pinfo.CanWrite)
                {
                    try
                    {
                        pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                    }
                    catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
                }
            }
            FieldInfo[] finfos = type.GetFields(flags);
            foreach (var finfo in finfos)
            {
                finfo.SetValue(comp, finfo.GetValue(other));
            }
            return comp as T;
        }

        public static T AddComponent<T>(this GameObject go, T toAdd) where T : Component
        {
            return go.AddComponent<T>().Copy(toAdd);
        }
        #endregion

        #region GameObjects
        public static void DeCloneName(GameObject thisObject)
        {
            if (thisObject.name.Contains("(Clone)")) thisObject.name = thisObject.name.Replace("(Clone)", "");
        }
        #endregion

        #region Coroutines
        private static WaitForEndOfFrame waitNextFrame;
        public static WaitForEndOfFrame WaitNextFrame()
        {
            if (waitNextFrame == null) waitNextFrame = new WaitForEndOfFrame();
            return waitNextFrame;
        }
        #endregion

        #region Array
        public static void Shuffle<T>(this T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                int k = UnityEngine.Random.Range(0, n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }

        public static void Shuffle<T>(this List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                int k = UnityEngine.Random.Range(0, n--);
                T temp = list[n];
                list[n] = list[k];
                list[k] = temp;
            }
        }

        public static void Shuffle<T, D>(this Dictionary<T, D> dict)
        {
            System.Random rand = new();
            dict = dict.OrderBy(x => rand.Next())
              .ToDictionary(item => item.Key, item => item.Value);

        }

        public static bool Contains<T>(this T[] array, T item)
        {
            int count = array.Length;
            for (int i = 0; i < count; i++)
            {
                if (array[i].Equals(item)) return true;
            }
            return false;
        }
        #endregion

        #region Compute

        public static async Task<long> ComputeAsync(string exp)
        {
            return await Task.Run(() => Compute(ref exp));
        }

        public static long Compute(ref string exp)
        {
            bool started = false;
            long result = 0;
            long numTemp = 0;
            List<ComputeParen> parenStack = new();
            short opTemp = 0;
            bool prevNum = false;
            for (int i = 0; i < exp.Length; i++)
            {
                char c = exp[i];

                if ('0' <= c && c <= '9')
                {
                    numTemp *= 10;
                    numTemp += (c - '0');
                    prevNum = true;
                    continue;
                }
                else if (prevNum)
                {
                    if (opTemp != 0)
                    {
                        if (parenStack.Count == 0)
                        {
                            switch (opTemp)
                            {
                                case 1: result += numTemp; break;
                                case -1: result -= numTemp; break;
                                case 2: result *= numTemp; break;
                                case -2: result *= -numTemp; break;
                                case 3: result /= numTemp; break;
                                case -3: result /= -numTemp; break;
                            }
                        }
                        else
                        {
                            switch (opTemp)
                            {
                                case 1: parenStack[^1] = new(parenStack[^1].op, parenStack[^1].num + numTemp, true); break;
                                case -1: parenStack[^1] = new(parenStack[^1].op, parenStack[^1].num - numTemp, true); break;
                                case 2: parenStack[^1] = new(parenStack[^1].op, parenStack[^1].num * numTemp, true); break;
                                case -2: parenStack[^1] = new(parenStack[^1].op, parenStack[^1].num * -numTemp, true); break;
                                case 3: parenStack[^1] = new(parenStack[^1].op, parenStack[^1].num / numTemp, true); break;
                                case -3: parenStack[^1] = new(parenStack[^1].op, parenStack[^1].num / -numTemp, true); break;
                            }
                        }
                    }
                    else if (parenStack.Count > 0)
                    {
                        if (parenStack[^1].started) throw new InvalidOperationException("Invalid Expression"); // no operator between numbers
                        parenStack[^1] = new(parenStack[^1].op, numTemp, true);
                    }
                    else
                    {
                        if (started) throw new InvalidOperationException("Invalid Expression"); // no operator between numbers
                        result += numTemp;
                        started = true;
                    }
                    numTemp = 0;
                    opTemp = 0;
                    prevNum = false;
                }

                if (c == '(')
                {
                    parenStack.Add(new(opTemp));
                    opTemp = 0;
                    continue;
                }

                if (c == ')')
                {
                    if (opTemp != 0) throw new InvalidOperationException("Unfinished Expression"); // no numbers after operator
                    if (parenStack.Count == 1)
                    {
                        switch (parenStack[^1].op)
                        {
                            case 0:
                                if (started) throw new InvalidOperationException("Invalid Expression"); // no operator between numbers and paren
                                result = parenStack[^1].num;
                                break;
                            case 1: result += parenStack[^1].num; break;
                            case -1: result -= parenStack[^1].num; break;
                            case 2: result *= parenStack[^1].num; break;
                            case -2: result *= -parenStack[^1].num; break;
                            case 3: result /= parenStack[^1].num; break;
                            case -3: result /= -parenStack[^1].num; break;
                        }
                    }
                    else if (parenStack.Count > 1)
                    {
                        switch (parenStack[^1].op)
                        {
                            case 0:
                                if (parenStack[^2].started) throw new InvalidOperationException("Invalid Expression"); // no operator between numbers and paren
                                parenStack[^2] = new(parenStack[^2].op, parenStack[^1].num, true);
                                break;
                            case 1: parenStack[^2] = new(parenStack[^2].op, parenStack[^2].num + parenStack[^1].num, true); break;
                            case -1: parenStack[^2] = new(parenStack[^2].op, parenStack[^2].num - parenStack[^1].num, true); break;
                            case 2: parenStack[^2] = new(parenStack[^2].op, parenStack[^2].num * parenStack[^1].num, true); break;
                            case -2: parenStack[^2] = new(parenStack[^2].op, parenStack[^2].num * -parenStack[^1].num, true); break;
                            case 3: parenStack[^2] = new(parenStack[^2].op, parenStack[^2].num / parenStack[^1].num, true); break;
                            case -3: parenStack[^2] = new(parenStack[^2].op, parenStack[^2].num / -parenStack[^1].num, true); break;
                        }
                    }
                    else throw new InvalidOperationException("Invalid Expression"); // ')' operator before placing '('
                    parenStack.RemoveAt(parenStack.Count - 1);
                    opTemp = 0;
                    continue;
                }

                if (c == '+')
                {
                    if (opTemp == 0)
                    {
                        opTemp = 1;
                    }
                    continue;
                }

                if (c == '-')
                {
                    if (opTemp == 0)
                    {
                        opTemp = -1;
                        continue;
                    }
                    opTemp *= -1;
                    continue;
                }

                if (c == '*')
                {
                    if (opTemp == 1 || opTemp == -1) throw new InvalidOperationException("Invalid Expression"); // '*' operator after '+' or '-'
                    opTemp = 2;
                    continue;
                }

                if (c == '/')
                {
                    if (opTemp == 1 || opTemp == -1) throw new InvalidOperationException("Invalid Expression"); // '/' operator after '+' or '-'
                    opTemp = 3;
                    continue;
                }
            }

            if (parenStack.Count != 0) throw new InvalidOperationException("Unfinished Expression"); // paren not ended

            if (prevNum)
            {
                switch (opTemp)
                {
                    case 0:
                        if (started) throw new InvalidOperationException("Invalid Expression");  // no operator between numbers
                        else result += numTemp;
                        break;
                    case 1: result += numTemp; break;
                    case -1: result -= numTemp; break;
                    case 2: result *= numTemp; break;
                    case -2: result *= -numTemp; break;
                    case 3: result /= numTemp; break;
                    case -3: result /= -numTemp; break;
                }
            }
            else if (opTemp != 0) throw new InvalidOperationException("Unfinished Expression"); // no numbers after operator

            return result;
        }

        #region Lukince's Method
        /*
        public static async Task<long> Execute(string exp)
        {
            return await Task.Run(() =>
            {
                List<char> list = new();

                if (!GetPostFix(in exp, ref list)) throw new InvalidOperationException("Invalid Expression");

                int tmp = 0;
                Stack<long> stack = new();

                foreach (char c in list)
                {
                    if ('0' <= c && c <= '9' || c < 0)
                    {
                        if (tmp == 0 && c == '0') throw new InvalidOperationException("Starting With '0' is Not Allowed");
                        tmp *= 10;
                        if (c > 0) tmp += c - '0';
                        else tmp -= Mathf.Abs(c) - '0';
                        continue;
                    }

                    if (tmp != 0)
                    {
                        stack.Push(tmp);
                        tmp = 0;
                    }

                    if (c == ' ') continue;

                    if (stack.Count() < 2)
                    {
                        throw new InvalidOperationException("Unfinished Expression");
                    }

                    long a = stack.Pop(), b = stack.Pop();

                    switch (c)
                    {
                        case '+': stack.Push(b + a); break;
                        case '-': stack.Push(b - a); break;
                        case '*': stack.Push(b * a); break;
                        case '/': stack.Push(b / a); break;
                        default: throw new InvalidOperationException($"Invalid Operator: '{c}'");
                    }
                }

                if (stack.Count() == 0) return long.Parse(exp);

                return stack.Peek();
            });
        }

        public static bool GetPostFix(in string exp, ref List<char> list)
        {
            Stack<char> stack = new();
            int numc = 0, opc, i = 0;
            bool flag = false;

            if (exp[0] == '+') i = 1;
            else if (exp[0] == '-') { i = 1; flag = true; }
            else if (('0' > exp[0] || exp[0] > '9') && exp[0] != '(') return false;

            if (('0' > exp[0] || exp[0] > '9') && ('0' > exp[1] || exp[1] > '9'))
            {
                int res = 1;
                int j = 0;
                for (; ; j++)
                {
                    if (exp[j] == '-') res *= -1;
                    else if (exp[j] != '+') break;
                }
                if (j > 1)
                {
                    exp.Remove(0, j);
                    exp.Insert(0, res == 1 ? "+" : "-");
                }
                else return false;
            }

            for (int size = exp.Length; i < size; i++)
            {
                char c = exp[i];
                if ('0' <= c && c <= '9')
                {
                    if (numc > 15) return false;

                    numc++;
                    if (flag)
                    {
                        list.Add((char)-(long)char.GetNumericValue(c));
                        flag = false;
                    }
                    else list.Add(c);
                    continue;
                }

                list.Add(' ');
                if (i > 0 && (exp[i - 1] == '+' || exp[i - 1] == '-'))
                {
                    if (i > 1 && ('0' > exp[i - 2] || exp[i - 2] > '9'))
                    {
                        int res = 1;
                        int j = -1;
                        for (; ; j++)
                        {
                            if (exp[j] == '-') res *= -1;
                            else if (exp[j] != '+') break;
                        }
                        if (j > 0)
                        {
                            exp.Remove(i - 1, j);
                            exp.Insert(i - 1, res == 1 ? "+" : "-");
                        }
                        else return false;
                    }
                    flag = true;
                    continue;
                }
                numc = 0;

                if (c == ')')
                {
                    opc = 0;

                    if (stack.Count() == 0) return false;

                    while (stack.Peek() != '(')
                    {
                        if ('0' > stack.Peek() || stack.Peek() > '9') opc++;
                        list.Add(stack.Pop());
                    }

                    if (opc == 0) return false;

                    stack.Pop();
                    continue;
                }

                while (stack.Count != 0 && GetPriority(c) <= GetPriority(stack.Peek(), true)) list.Add(stack.Pop());

                stack.Push(c);
            }

            while (stack.Count != 0) list.Add(stack.Pop());

            return true;
        }

        public static int GetPriority(char token, bool stack = false) =>
            token switch
            {
                '(' => stack ? 0 : 3,
                '*' => 2,
                '/' => 2,
                '+' => 1,
                '-' => 1,
                _ => -1
            };
        */
        #endregion

        public struct ComputeParen
        {
            public long num;
            public short op;
            public bool started;

            public ComputeParen(short op, long num = 0, bool started = false)
            {
                this.num = num;
                this.op = op;
                this.started = started;
            }

        }

        #endregion

        #region Types

        public static Type ClassOf(MethodBase method)
        {
            var type = method.DeclaringType;
            if (type == null) return null;

            // If it looks like a compiler-generated async/iterator state machine
            if (type.IsNested && type.Name.Contains("<") && type.Name.Contains(">"))
            {
                return type.DeclaringType; // outer/original class
            }

            return type; // normal case
        }

        public static Type RootOf(Type type)
        {
            while (type.DeclaringType != null)
            {
                type = type.DeclaringType;
            }
            return type;
        }

        #endregion
    }

    #region Enums

    public enum Direction
    {
        Up, Down, Left, Right
    }

    #endregion

    #region Structs
    public struct Optional<T> where T : struct
    {
        public bool HasValue;
        public T Value;

        public Optional<T> Empty => new() { HasValue = false, Value = Value };

        public static implicit operator Optional<T>(T value)
        {
            return new Optional<T>() { HasValue = true, Value = value };
        }
    }
    #endregion

    #region Else

    public class CountDictionary<TKey> : IDictionary<TKey, int> where TKey : notnull // by Lukince
    {
        public CountDictionary()
        {
            Collections = new();
        }
        public CountDictionary(IEnumerable<KeyValuePair<TKey, int>> pair)
        {
            Collections = new(pair);
        }
        private Dictionary<TKey, int> Collections;
        public int this[TKey key] { get => Collections[key]; set => Collections[key] = value; }
        public ICollection<TKey> Keys => Collections.Keys;
        public ICollection<int> Values => Collections.Values;
        public int Count => Keys.Count;
        public bool IsReadOnly => false;
        public void Add(TKey key, int value)
        {
            if (Collections.ContainsKey(key))
                Collections[key] += value;
            else
                Collections.Add(key, value);
        }
        public void Add(TKey key)
            => Add(key, 1);
        public void Minus(TKey key, int value)
        {
            if (Collections.ContainsKey(key))
                Collections[key] -= value;
            else
                throw new KeyNotFoundException();
        }
        public void Minus(TKey key)
            => Minus(key, 1);
        public void Add(KeyValuePair<TKey, int> item)
            => Add(item.Key, item.Value);
        public void Clear()
            => Collections.Clear();
        public bool Contains(KeyValuePair<TKey, int> item)
            => Collections.Contains(item);
        public bool ContainsKey(TKey key)
            => Collections.ContainsKey(key);
        public void CopyTo(KeyValuePair<TKey, int>[] array, int arrayIndex)
            => new NotImplementedException();
        public IEnumerator<KeyValuePair<TKey, int>> GetEnumerator()
            => Collections.GetEnumerator();
        public bool Remove(TKey key)
            => Collections.Remove(key);
        public bool Remove(KeyValuePair<TKey, int> item)
            => Collections.Remove(item.Key);
        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out int value)
        {
            if (Collections.TryGetValue(key, out int values))
            {
                value = values;
                return true;
            }
            value = default;
            return false;
        }
        IEnumerator IEnumerable.GetEnumerator()
            => Collections.GetEnumerator();
    }

    #endregion

    #region Interpolation

    public interface IDynamicsPreset<T>
    {
        public IScalar<T> GetSpeed();
        public IScalar<T> GetDamp();
        public IScalar<T> GetInitial();
    }

    [Serializable]
    public struct DynamicsPreset : IDynamicsPreset<float>
    {
        public float Speed, Damp, Initial;
        public readonly IScalar<float> GetSpeed() => (FloatWrapper)Speed;
        public readonly IScalar<float> GetDamp() => (FloatWrapper)Damp;
        public readonly IScalar<float> GetInitial() => (FloatWrapper)Initial;
    }

    [Serializable]
    public struct DynamicsPreset<T> : IDynamicsPreset<T>
    {
        public IScalar<T> Speed, Damp, Initial;
        public readonly IScalar<T> GetSpeed() => Speed;
        public readonly IScalar<T> GetDamp() => Damp;
        public readonly IScalar<T> GetInitial() => Initial;
    }

    public class Dynamics<T>
    {
        private double W, Z, D, k1, k2, k3;

        public IVector<T> Position;
        public IVector<T> Velocity;

        /// <summary>
        /// Setup the (duration, vibration, initial) or re-assign
        /// </summary>
        /// <param name="speed">duration (not exact)</param>
        /// <param name="damp">the scale of the vibration</param>
        /// <param name="initial">0~1: normal, 1~: EaseInBounce, ~0: EaseInBack</param>
        public Dynamics<T> Setup(IScalar<T> speed, IScalar<T> damp, IScalar<T> initial)
        {
            var speedD = speed.AsDouble();
            var dampD = damp.AsDouble();
            var initialD = initial.AsDouble();
            W = Utils.TAU * speedD;
            Z = dampD;
            D = W * Math.Sqrt(Math.Abs(dampD * dampD - 1));
            k1 = dampD / (Utils.PI * speedD);
            k2 = 1 / W;
            k3 = initialD * dampD * k2;
            k2 *= k2;
            return this;
        }

        public Dynamics<T> Setup(IDynamicsPreset<T> preset)
        {
            return Setup(preset.GetSpeed(), preset.GetDamp(), preset.GetInitial());
        }

        public Dynamics<T> Reset(IVector<T> position = default, IVector<T> velocity = default)
        {
            Position = position.Clone();
            Velocity = velocity.Clone();
            return this;
        }

        public IVector<T> Update(IScalar<T> timeStep, IVector<T> target)
        {
            IVector<T> velocity = (target - Position) / timeStep;
            return Update(timeStep, target, velocity);
        }

        public IVector<T> Update(IScalar<T> timeStep, IVector<T> target, IVector<T> velocity)
        {
            double timeStepD = timeStep.AsDouble();
            double k1_stable, k2_stable;
            if (W * timeStepD < Z)
            {
                k1_stable = k1;
                k2_stable = Math.Max(Math.Max(k2, timeStepD * 0.5f * (timeStepD + k1)), timeStepD * k1);
            }
            else
            {
                double t1 = Math.Exp(-Z * W * timeStepD);
                double alpha = 2f * t1 * (Z <= 1f ? Math.Cos(timeStepD * D) : Math.Cosh(timeStepD * D));
                double beta = t1 * t1;
                double t2 = timeStepD / (1f + beta - alpha);
                k1_stable = (1f - beta) * t2;
                k2_stable = timeStepD * t2;
            }
            Position += timeStep * Velocity;
            Velocity += timeStep * (target + IScalar<T>.NewD(k3) * velocity - Position - IScalar<T>.NewD(k1_stable) * Velocity) / IScalar<T>.NewD(k2_stable);
            return Position;
        }
    }

    #endregion
}