using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MagicBits.Minigame_2_x.Scripts;
using UnityEngine;
using UnityEngine.Events;

namespace MagicBits_OSS.Shared.Scripts
{
    public static class Utilities
    {
        // https://stackoverflow.com/a/13543600/8903027
        public const int FLOAT_MAX_PRECISION_DIGITS = 7;
        public const int DOUBLE_MAX_PRECISION_DIGITS = 15;
        
        #region Event
        
        /// <summary>
        /// <b>TESTE!</b><br/>
        /// Esse não recebe componente.
        /// </summary>
        public static Task WaitEventTask(UnityEvent unityEvent, Action callback)
        {
            return new Task(WaitEvent(unityEvent, callback));
        }

        /// <summary>
        /// https://stackoverflow.com/a/51372785/8903027
        /// </summary>
        public static IEnumerator WaitEvent(UnityEvent unityEvent, Action callback)
        {
            var trigger = false;
            Action action = () => trigger = true;
            unityEvent.AddListener(action.Invoke);
            yield return new WaitUntil(() => trigger);
            unityEvent.RemoveListener(action.Invoke);
            callback();
        }

        public static IEnumerator SetTimeoutRoutine(Action callback, float waitTime, bool isRealTime)
        {
            if (isRealTime)
                yield return new WaitForSecondsRealtime(waitTime);
            else
                yield return new WaitForSeconds(waitTime);

            callback();
        }
        
        #endregion
        
        #region Math: Base Manipulation

        /// <summary>
        /// https://stackoverflow.com/a/10981113/8903027
        /// </summary>
        public static readonly char[] BaseChars =
            "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray();

        public static readonly Dictionary<char, int> CharValues = BaseChars
            .Select((c, i) => new { Char = c, Index = i })
            .ToDictionary(c => c.Char, c => c.Index);

        /// <summary>
        /// Intervalo: [-int.MaxValue, int.MaxValue]
        /// </summary>
        public static string NumToBase(long num, int toRadix)
        {
            if (toRadix > BaseChars.Length)
            {
                UnityEngine.Debug.LogError($"Error! Não é possível representar a base {toRadix}.");
                return "";
            }

            // Determine exact number of characters to use.
            bool neg = num < 0;
            num = Math.Abs(num);
            double log = Math.Log(num, toRadix);
            int size = (int)Math.Max(Math.Ceiling(log), 1);
            char[] buffer = new char[1 + size]; // +1 para char do sinal
            buffer[0] = neg ? '-' : ' ';

            var i = buffer.Length - 1;
            do
            {
                buffer[i--] = BaseChars[num % toRadix];
                num /= toRadix;
            } while (num > 0);

            // string ans = new string(buffer, i, buffer.Length - i);
            // string ans = new(buffer, 0 + (neg ? 1 : 0), buffer.Length);
            string ans = new(buffer);
            return ans.Trim();
        }

        public static long BaseToNum(string numStr, int fromRadix)
        {
            if (fromRadix > BaseChars.Length)
            {
                UnityEngine.Debug.LogError($"Error! Não é possível representar a base {fromRadix}.");
                return 0;
            }

            bool neg = numStr[0] == '-';
            if (neg) numStr = numStr.Substring(1);
            char[] chrs = numStr.ToCharArray();
            int m = chrs.Length - 1;
            long x, res = 0;
            for (int i = 0; i < chrs.Length; i++)
            {
                x = CharValues[chrs[i]];
                res += x * (long)Math.Pow(fromRadix, m--);
            }

            if (neg) res *= -1;
            return res;
        }

        public static string BaseToBase(string num, int fromRadix, int toRadix)
        {
            long base10 = BaseToNum(num, fromRadix);
            return NumToBase(base10, toRadix);
        }

        public static string ToBaseComplement(string num, int radix)
        {
            /* 
            base3: -21

            222
             21
            201
            202

            */

            string baseNums = "";
            string convertedRadix = NumToBase(radix - 1, radix);
            num = num.Replace("-", "");

            // Debug.Log($"Numero: {num}");
            // Debug.Log($"NumLenght: {num.Length}");
            // Debug.Log($"Radix: {radix}");
            // Debug.Log($"ConvertedRadix: {convertedRadix}");

            for (int i = 0; i < num.Length + 1; i++)
                baseNums = $"{baseNums}{convertedRadix}";

            long decimalBaseNums = BaseToNum(baseNums, radix);
            long decimalNum = BaseToNum(num, radix);
            long result = decimalBaseNums - decimalNum + 1;
            string resultConverted = NumToBase(result, radix);

            // Debug.Log($"baseNums: {baseNums}");
            // Debug.Log($"decimalBaseNums: {decimalBaseNums}");
            // Debug.Log($"decimalNum: {decimalNum}");
            // Debug.Log($"result: {result}");
            // Debug.Log($"resultConverted: {resultConverted}");

            return resultConverted;
        }

        public static float ParseFloatingPointSingle(string fp)
        {
            if (fp.Length != 32)
            {
                UnityEngine.Debug.LogWarning($"ParseFloatingPointSingle: valor informado não está com 32 digitos.");
                return float.MaxValue;
            }

            int signal = fp[0] == '1' ? -1 : 1;
            long exp = BaseToNum(fp[1..9], 2) - 127;
            float frac = (float)(BaseToNum(fp[9..32], 2) / Math.Pow(2, 23));

            return signal * (float)Math.Pow(2, exp) * (1 + frac);
        }

        public static string ToFloatingPointSingle(float value)
        {
            return BitConverter.GetBytes(value)
                .Reverse()
                .Select(x => Convert.ToString(x, 2))
                .Select(x => x.PadLeft(8, '0'))
                .Aggregate("", (a, b) => a + b);
            // .Aggregate("0b", (a, b) => a + "_" + b);
        }

        /// <summary>
        ///  Euclidean algorithm https://stackoverflow.com/a/41766138/8903027
        /// </summary>
        public static uint GCD(uint a, uint b)
        {
            while (a != 0 && b != 0)
            {
                if (a > b)
                    a %= b;
                else
                    b %= a;
            }

            return a | b;
        }

        #endregion

        #region Log

        // TODO: Search for Unity 'Logger'.
        public enum LogLevel
        {
            Info,
            Warn,
            Debug,
            Editor,
        }

        public static void Log(object message, LogLevel level = LogLevel.Debug)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    if (!Application.isEditor && !GameController_2_2_1.hasPrivilegedAccess && !Debug.isDebugBuild) return;
                    break;
                case LogLevel.Editor:
                    if (!Application.isEditor) return;
                    break;
            }

            string lv = "LogLevel";
            string cl = "White";
            switch (level)
            {
                case LogLevel.Info:
                    lv = "Default";
                    break;
                case LogLevel.Warn:
                    lv = "Warn";
                    cl = "Yellow";
                    break;
                case LogLevel.Debug:
                    lv = "Debug";
                    break;
                case LogLevel.Editor:
                    lv = "Editor";
                    break;
            }

            string str = $"{$"[{lv}]".Color(cl)} {message}";
            UnityEngine.Debug.Log(str);
        }

        #endregion
    }
}
