using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Newtonsoft_X.Json.Utilities
{
    internal static class CollectionUtils
    {
        /// <summary>
        /// Determines whether the collection is <c>null</c> or empty.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <returns>
        /// 	<c>true</c> if the collection is <c>null</c> or empty; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty<T>(ICollection<T> collection)
        {
            return collection == null || collection.Count == 0;
        }

        /// <summary>
        /// Adds the elements of the specified collection to the specified generic <see cref="T:System.Collections.Generic.IList`1" />.
        /// </summary>
        /// <param name="initial">The list to add to.</param>
        /// <param name="collection">The collection of elements to add.</param>
        public static void AddRange<T>(this IList<T> initial, IEnumerable<T> collection)
        {
            if (initial == null)
            {
                throw new ArgumentNullException("initial");
            }
            if (collection == null)
            {
                return;
            }
            foreach (T item in collection)
            {
                initial.Add(item);
            }
        }

        public static void AddRange<T>(this IList<T> initial, IEnumerable collection)
        {
            ValidationUtils.ArgumentNotNull(initial, "initial");
            initial.AddRange(collection.Cast<T>());
        }

        public static bool IsDictionaryType(Type type)
        {
            ValidationUtils.ArgumentNotNull(type, "type");
            return typeof(IDictionary).IsAssignableFrom(type) || ReflectionUtils.ImplementsGenericDefinition(type, typeof(IDictionary<,>));
        }

        public static ConstructorInfo ResolveEnumerableCollectionConstructor(Type collectionType, Type collectionItemType)
        {
            Type constructorArgumentType = typeof(IList<>).MakeGenericType(new Type[]
            {
                collectionItemType
            });
            return CollectionUtils.ResolveEnumerableCollectionConstructor(collectionType, collectionItemType, constructorArgumentType);
        }

        public static ConstructorInfo ResolveEnumerableCollectionConstructor(Type collectionType, Type collectionItemType, Type constructorArgumentType)
        {
            Type type = typeof(IEnumerable<>).MakeGenericType(new Type[]
            {
                collectionItemType
            });
            ConstructorInfo constructorInfo = null;
            foreach (ConstructorInfo constructorInfo2 in collectionType.GetConstructors(BindingFlags.Instance | BindingFlags.Public))
            {
                IList<ParameterInfo> parameters = constructorInfo2.GetParameters();
                if (parameters.Count == 1)
                {
                    Type parameterType = parameters[0].ParameterType;
                    if (type == parameterType)
                    {
                        constructorInfo = constructorInfo2;
                        break;
                    }
                    if (constructorInfo == null && parameterType.IsAssignableFrom(constructorArgumentType))
                    {
                        constructorInfo = constructorInfo2;
                    }
                }
            }
            return constructorInfo;
        }

        public static bool AddDistinct<T>(this IList<T> list, T value)
        {
            return list.AddDistinct(value, EqualityComparer<T>.Default);
        }

        public static bool AddDistinct<T>(this IList<T> list, T value, IEqualityComparer<T> comparer)
        {
            if (list.ContainsValue(value, comparer))
            {
                return false;
            }
            list.Add(value);
            return true;
        }

        public static bool ContainsValue<TSource>(this IEnumerable<TSource> source, TSource value, IEqualityComparer<TSource> comparer)
        {
            if (comparer == null)
            {
                comparer = EqualityComparer<TSource>.Default;
            }
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            foreach (TSource x in source)
            {
                if (comparer.Equals(x, value))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool AddRangeDistinct<T>(this IList<T> list, IEnumerable<T> values, IEqualityComparer<T> comparer)
        {
            bool result = true;
            foreach (T value in values)
            {
                if (!list.AddDistinct(value, comparer))
                {
                    result = false;
                }
            }
            return result;
        }

        public static int IndexOf<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
        {
            int num = 0;
            foreach (T arg in collection)
            {
                if (predicate(arg))
                {
                    return num;
                }
                num++;
            }
            return -1;
        }

        public static bool Contains<T>(this List<T> list, T value, IEqualityComparer comparer)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (comparer.Equals(value, list[i]))
                {
                    return true;
                }
            }
            return false;
        }

        public static int IndexOfReference<T>(this List<T> list, T item)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (item.Equals(list[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        private static IList<int> GetDimensions(IList values, int dimensionsCount)
        {
            IList<int> list = new List<int>();
            IList list2 = values;
            for (; ; )
            {
                list.Add(list2.Count);
                if (list.Count == dimensionsCount || list2.Count == 0)
                {
                    break;
                }
                object obj = list2[0];
                if (!(obj is IList))
                {
                    break;
                }
                list2 = (IList)obj;
            }
            return list;
        }

        private static void CopyFromJaggedToMultidimensionalArray(IList values, Array multidimensionalArray, int[] indices)
        {
            int num = indices.Length;
            if (num == multidimensionalArray.Rank)
            {
                multidimensionalArray.SetValue(CollectionUtils.JaggedArrayGetValue(values, indices), indices);
                return;
            }
            int length = multidimensionalArray.GetLength(num);
            if (((IList)CollectionUtils.JaggedArrayGetValue(values, indices)).Count != length)
            {
                throw new Exception("Cannot deserialize non-cubical array as multidimensional array.");
            }
            int[] array = new int[num + 1];
            for (int i = 0; i < num; i++)
            {
                array[i] = indices[i];
            }
            for (int j = 0; j < multidimensionalArray.GetLength(num); j++)
            {
                array[num] = j;
                CollectionUtils.CopyFromJaggedToMultidimensionalArray(values, multidimensionalArray, array);
            }
        }

        private static object JaggedArrayGetValue(IList values, int[] indices)
        {
            IList list = values;
            for (int i = 0; i < indices.Length; i++)
            {
                int index = indices[i];
                if (i == indices.Length - 1)
                {
                    return list[index];
                }
                list = (IList)list[index];
            }
            return list;
        }

        public static Array ToMultidimensionalArray(IList values, Type type, int rank)
        {
            IList<int> dimensions = CollectionUtils.GetDimensions(values, rank);
            while (dimensions.Count < rank)
            {
                dimensions.Add(0);
            }
            Array array = Array.CreateInstance(type, dimensions.ToArray<int>());
            CollectionUtils.CopyFromJaggedToMultidimensionalArray(values, array, new int[0]);
            return array;
        }
    }
}
