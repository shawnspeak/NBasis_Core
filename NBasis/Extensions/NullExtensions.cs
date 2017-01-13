using System;
using System.Collections.Generic;
using System.Linq;

namespace NBasis
{
    public static class NullExtensions
    {
        /// <summary>
        /// Returns the original value of a nullable type if it is not null, or the alternate if it is.
        /// </summary>
        /// <typeparam name="T">Type of the value to compare.</typeparam>
        /// <param name="original">Original value.</param>
        /// <param name="alternate">Alternate value.</param>
        /// <returns>Original value if it is not null, or the alternate value otherwise.</returns>
        public static T Or<T>(this T? original, T alternate) where T : struct
        {
            return original == null ? alternate : original.Value;
        }

        /// <summary>
        /// Returns the original value if it is not the default of the type or an empty string, or the alternate if it is.
        /// </summary>
        /// <param name="original">Original value.</param>
        /// <param name="alternate">Alternate value.</param>
        /// <returns>Original value if it is not empty or null, or the alternate value otherwise.</returns>
        public static T Or<T>(this T original, T alternate)
        {
            return null == original || original.Equals(default(T)) || (original is String && String.IsNullOrWhiteSpace(original.ToString()))
                ? alternate
                : original;
        }

        /// <summary>
        /// Returns a given value if a given condition is true. Seems useless on first glance but
        /// enables fluent language like (null != null).GetValue(object).Or(otherObject) which is sometimes
        /// easier to read in view markup. object.GetValue(fn) is typically more useful, however.
        /// </summary>
        /// <typeparam name="T">Type of the value to return.</typeparam>
        /// <param name="condition">
        /// Condition indicating whether or not to return the given value or
        /// a default of its type.
        /// </param>
        /// <param name="value">Value to return if the condition is true.</param>
        /// <returns>The given value if the condition is true, or its default if not.</returns>
        public static T Value<T>(this bool condition, T value)
        {
            return condition ? value : default(T);
        }

        /// <summary>
        /// Gets a value in the context of a given object, or a default value of the indicated
        /// output type if the context object is null.
        /// </summary>
        /// <typeparam name="TObject">Type of the object to use for context.</typeparam>
        /// <typeparam name="TValue">Type of the value to return based on the context object.</typeparam>
        /// <param name="context">Context object to check for null and to pass into the value function.</param>
        /// <param name="fn">Function to invoke to get the value to return based on the object context.</param>
        /// <returns>
        /// Value result from the given function or a default of the value type if the context
        /// object is null.
        /// </returns>
        public static TValue Value<TObject, TValue>(this TObject context, Func<TObject, TValue> fn)
        {
            return !ReferenceEquals(context, null) ? fn(context) : default(TValue);
        }

        /// <summary>
        /// Returns either the given list or an empty list of the same type.
        /// </summary>
        /// <typeparam name="T">Type of the object to use for context.</typeparam>
        /// <param name="list">List to work over.</param>
        /// <returns>Original list or a new empty list.</returns>
        public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T> list)
        {
            return list ?? Enumerable.Empty<T>();
        }

        /// <summary>
        /// Safe count any IEnumerable even null lists
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="nullCountAs"></param>
        /// <returns></returns>
        public static int SafeCount<T>(this IEnumerable<T> list, int nullCountAs = 0)
        {
            if (list == null) return nullCountAs;
            return list.Count();
        }
    }
}
