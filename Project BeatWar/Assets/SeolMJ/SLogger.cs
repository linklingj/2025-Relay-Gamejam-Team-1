using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SeolMJ.Pool;
using UnityEngine;

namespace SeolMJ
{
    public static class SLogger
    {
        public static readonly Stack<LogScope> Scopes = new();

        [HideInCallstack]
        public static LogScope Scope(string open = null, string close = null, int unwrap = 1)
        {
#if UNITY_EDITOR
            var stackTrace = new StackTrace();
            var frame = stackTrace.GetFrame(unwrap);
            var method = frame.GetMethod();
            var type = Utils.RootOf(method.DeclaringType);
            var scope = new LogScope(type, open, close);
            return scope;
#else
            return default;
#endif
        }

        [HideInCallstack]
        internal static void EnterScope(LogScope scope)
        {
#if UNITY_EDITOR
            Scopes.Push(scope);
            if (scope.Open != null) Log(scope.Open, point: LogPoint.Open, unwrap: 4);
#endif
        }

        [HideInCallstack]
        internal static void ExitScope()
        {
#if UNITY_EDITOR
            var scope = Scopes.Peek();
            if (scope.Close != null) Log(scope.Close, point: LogPoint.Close, unwrap: 3);
            Scopes.Pop();
#endif
        }

        public const float COLOR_C = 0.1f;
        public const float COLOR_L = 0.9f;

        [HideInCallstack]
        public static void Log(string title, string content = null, LogPoint point = LogPoint.None, Color32? colorOverride = null, int unwrap = 1)
        {
#if UNITY_EDITOR
            var builder = StringBuilderPool.Get();
            string name;
            Color color;
            foreach (var scope in Scopes.Reverse().SkipLast(1))
            {
                color = GetColor(scope.Type);
                builder.Append(ColorEnter(ToHex(new Color(color.r, color.g, color.b, color.a / 2f))));
                builder.Append(Deco(LogDeco.Continue));
                builder.Append(ColorExit());
            }
            if (Scopes.TryPeek(out var topScope))
            {
                color = GetColor(topScope.Type);
                builder.Append(ColorEnter(ToHex(new Color(color.r, color.g, color.b, color.a / 2f))));
                builder.Append(Deco(point == LogPoint.None ? LogDeco.Continue : point switch
                {
                    LogPoint.Open => LogDeco.Open,
                    LogPoint.Peek => LogDeco.Peek,
                    LogPoint.Close => LogDeco.Close,
                    _ => LogDeco.None,
                }));
                builder.Append(ColorExit());
            }
            var stackTrace = new StackTrace();
            var frame = stackTrace.GetFrame(unwrap);
            var method = frame.GetMethod();
            var type = Utils.RootOf(method.DeclaringType);
            name = type.Name;
            color = GetColor(type);
            if (colorOverride != null)
            {
                color = colorOverride.Value;
            }
            builder.Append(ColorEnter(ToHex(new Color(color.r, color.g, color.b, color.a / 2f))));
            builder.Append(' ');
            builder.Append(name);
            builder.Append(": ");
            builder.Append(ColorExit());
            builder.Append(ColorEnter(ToHex(color)));
            builder.Append(title);
            builder.Append(ColorExit());
            if (content != null)
            {
                builder.Append('\n');
                builder.Append(content);
            }
            string result = builder.ToString();
            StringBuilderPool.Release(builder);
            UnityEngine.Debug.Log(result);
#endif
        }

        [HideInCallstack]
        public static void LogError(string title, string content = null, int unwrap = 1)
        {
#if UNITY_EDITOR
            Log(title, content, colorOverride: new Color32(193, 57, 43, 255), unwrap: unwrap);
#endif
        }

        [HideInCallstack]
        public static void LogError(Exception exception, int unwrap = 1)
        {
#if UNITY_EDITOR
            var builder = StringBuilderPool.Get();
            if (exception.Data != null && exception.Data.Count != 0)
            {
                builder.Append("<b>Data ---</b>\n");
                foreach (DictionaryEntry entry in exception.Data)
                {
                    builder.Append("    ");
                    builder.Append(entry.Key);
                    builder.Append(": ");
                    builder.Append(entry.Value);
                    builder.Append('\n');
                }
            }
            builder.Append("<b>StackTrace ---</b>\n");
            builder.Append(exception.StackTrace);
            var result = builder.ToString();
            StringBuilderPool.Release(builder);
            Log($"[{exception.GetType().Name}] {exception.Message}", result, colorOverride: new Color32(255, 64, 48, 255), unwrap: unwrap);
#endif
        }

#if UNITY_EDITOR
        static readonly Dictionary<Type, Color32> colors = new();
#endif

        public static Color32 GetColor(Type type)
        {
#if UNITY_EDITOR
            if (!colors.TryGetValue(type, out var color))
            {
                var random = new System.Random(type.Name.GetHashCode());
                color = Utils.OKLAB2RGB(Utils.OKLCH2OKLAB(new(COLOR_L, COLOR_C, (float)random.NextDouble())));
                colors.Add(type, color);
            }
            return color;
#else
            return default;
#endif
        }

        public static string ToHex(Color32 color)
        {
            return string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", color.r, color.g, color.b, color.a);
        }

        public static string ColorEnter(string hex)
        {
            return $"<color=#{hex}>";
        }

        public static string ColorExit()
        {
            return "</color>";
        }

        public static string Deco(LogDeco deco)
        {
            return deco switch
            {
                LogDeco.Open => "¦£",
                LogDeco.Continue => "¦¢",
                LogDeco.Close => "¦¦",
                LogDeco.Peek => "¦§",
                _ => ""
            };
        }
    }

    public enum LogPoint
    {
        None,
        Open,
        Peek,
        Close,
    }

    public class LogScope : IDisposable
    {
        public readonly Type Type;
        public readonly string Open;
        public readonly string Close;

        public LogScope(Type type, string open, string close)
        {
            Type = type;
            Open = open;
            Close = close;
            SLogger.EnterScope(this);
        }

        public void Dispose()
        {
            SLogger.ExitScope();
            GC.SuppressFinalize(this);
        }

        ~LogScope()
        {
            SLogger.ExitScope();
        }
    }

    public enum LogDeco
    {
        None,
        Open,
        Continue,
        Peek,
        Close,
    }
}
