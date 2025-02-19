using System;
using System.Collections.Generic;

namespace _Case.Scripts.Extensions
{
    public static class StackExtensions
    {
        public static void Shuffle<T>(this List<T> list)
        {
            int n = list.Count;
            for (int i = n - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        public static void Shuffle<T>(this Stack<T> stack)
        {
            List<T> list = new List<T>(stack);
            list.Shuffle();
            stack.Clear();
            foreach (var item in list)
            {
                stack.Push(item);
            }
        }
    }
    
}