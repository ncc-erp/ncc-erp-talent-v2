using System;
using System.Collections.Generic;

namespace TalentV2.Utils
{
    public static class TreeUtils
    {
        public static IEnumerable<T> Expand<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> elementSelector)
        {
            var stack = new Stack<IEnumerator<T>>();
            var enumerator = source.GetEnumerator();
            try
            {
                while (true)
                {
                    while (enumerator.MoveNext())
                    {
                        var item = enumerator.Current;
                        yield return item;
                        var elements = elementSelector(item);
                        if (elements == null)
                        {
                            continue;
                        }
                        stack.Push(enumerator);
                        enumerator = elements.GetEnumerator();
                    }
                    if (stack.Count == 0)
                    {
                        break;
                    }
                    enumerator.Dispose();
                    enumerator = stack.Pop();
                }
            }
            finally
            {
                enumerator.Dispose();
                while (stack.Count != 0)
                {
                    stack.Pop().Dispose();
                }
            }
        }
    }
}
