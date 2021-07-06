using Newtonsoft_X.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace Newtonsoft_X.Json.Linq
{
    /// <summary>
    /// Represents a token that can contain other tokens.
    /// </summary>
    public abstract class JContainer : JToken, IList<JToken>, ICollection<JToken>, IEnumerable<JToken>, IEnumerable, IList, ICollection
    {
        /// <summary>
        /// Gets the container's children tokens.
        /// </summary>
        /// <value>The container's children tokens.</value>
        protected abstract IList<JToken> ChildrenTokens { get; }

        internal JContainer()
        {
        }

        internal JContainer(JContainer other) : this()
        {
            ValidationUtils.ArgumentNotNull(other, "other");
            int num = 0;
            foreach (JToken content in other)
            {
                AddInternal(num, content, false);
                num++;
            }
        }

        internal void CheckReentrancy()
        {
            if (_busy)
            {
                throw new InvalidOperationException("Cannot change {0} during a collection change event.".FormatWith(CultureInfo.InvariantCulture, base.GetType()));
            }
        }

        internal virtual IList<JToken> CreateChildrenCollection()
        {
            return new List<JToken>();
        }

        /// <summary>
        /// Gets a value indicating whether this token has child tokens.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this token has child values; otherwise, <c>false</c>.
        /// </value>
        public override bool HasValues
        {
            get
            {
                return ChildrenTokens.Count > 0;
            }
        }

        internal bool ContentsEqual(JContainer container)
        {
            if (container == this)
            {
                return true;
            }
            IList<JToken> childrenTokens = ChildrenTokens;
            IList<JToken> childrenTokens2 = container.ChildrenTokens;
            if (childrenTokens.Count != childrenTokens2.Count)
            {
                return false;
            }
            for (int i = 0; i < childrenTokens.Count; i++)
            {
                if (!childrenTokens[i].DeepEquals(childrenTokens2[i]))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Get the first child token of this token.
        /// </summary>
        /// <value>
        /// A <see cref="T:Newtonsoft.Json.Linq.JToken" /> containing the first child token of the <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </value>
        public override JToken First
        {
            get
            {
                IList<JToken> childrenTokens = ChildrenTokens;
                if (childrenTokens.Count <= 0)
                {
                    return null;
                }
                return childrenTokens[0];
            }
        }

        /// <summary>
        /// Get the last child token of this token.
        /// </summary>
        /// <value>
        /// A <see cref="T:Newtonsoft.Json.Linq.JToken" /> containing the last child token of the <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </value>
        public override JToken Last
        {
            get
            {
                IList<JToken> childrenTokens = ChildrenTokens;
                int count = childrenTokens.Count;
                if (count <= 0)
                {
                    return null;
                }
                return childrenTokens[count - 1];
            }
        }

        /// <summary>
        /// Returns a collection of the child tokens of this token, in document order.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:Newtonsoft.Json.Linq.JToken" /> containing the child tokens of this <see cref="T:Newtonsoft.Json.Linq.JToken" />, in document order.
        /// </returns>
        public override JEnumerable<JToken> Children()
        {
            return new JEnumerable<JToken>(ChildrenTokens);
        }

        /// <summary>
        /// Returns a collection of the child values of this token, in document order.
        /// </summary>
        /// <typeparam name="T">The type to convert the values to.</typeparam>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerable`1" /> containing the child values of this <see cref="T:Newtonsoft.Json.Linq.JToken" />, in document order.
        /// </returns>
        public override IEnumerable<T> Values<T>()
        {
            return ChildrenTokens.Convert<JToken, T>();
        }

        /// <summary>
        /// Returns a collection of the descendant tokens for this token in document order.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:Newtonsoft.Json.Linq.JToken" /> containing the descendant tokens of the <see cref="T:Newtonsoft.Json.Linq.JToken" />.</returns>
        public IEnumerable<JToken> Descendants()
        {
            return GetDescendants(false);
        }

        /// <summary>
        /// Returns a collection of the tokens that contain this token, and all descendant tokens of this token, in document order.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:Newtonsoft.Json.Linq.JToken" /> containing this token, and all the descendant tokens of the <see cref="T:Newtonsoft.Json.Linq.JToken" />.</returns>
        public IEnumerable<JToken> DescendantsAndSelf()
        {
            return GetDescendants(true);
        }

        internal IEnumerable<JToken> GetDescendants(bool self)
        {
            if (self)
            {
                yield return this;
            }
            foreach (JToken o in ChildrenTokens)
            {
                yield return o;
                if (o is JContainer jcontainer)
                {
                    foreach (JToken jtoken in jcontainer.Descendants())
                    {
                        yield return jtoken;
                    }
                }
            }
            yield break;
        }

        internal bool IsMultiContent(object content)
        {
            return content is IEnumerable && !(content is string) && !(content is JToken) && !(content is byte[]);
        }

        internal JToken EnsureParentToken(JToken item, bool skipParentCheck)
        {
            if (item == null)
            {
                return JValue.CreateNull();
            }
            if (skipParentCheck)
            {
                return item;
            }
            if (item.Parent != null || item == this || (item.HasValues && base.Root == item))
            {
                item = item.CloneToken();
            }
            return item;
        }

        internal abstract int IndexOfItem(JToken item);

        internal virtual void InsertItem(int index, JToken item, bool skipParentCheck)
        {
            IList<JToken> childrenTokens = ChildrenTokens;
            if (index > childrenTokens.Count)
            {
                throw new ArgumentOutOfRangeException("index", "Index must be within the bounds of the List.");
            }
            CheckReentrancy();
            item = EnsureParentToken(item, skipParentCheck);
            JToken jtoken = (index == 0) ? null : childrenTokens[index - 1];
            JToken jtoken2 = (index == childrenTokens.Count) ? null : childrenTokens[index];
            ValidateToken(item, null);
            item.Parent = this;
            item.Previous = jtoken;
            if (jtoken != null)
            {
                jtoken.Next = item;
            }
            item.Next = jtoken2;
            if (jtoken2 != null)
            {
                jtoken2.Previous = item;
            }
            childrenTokens.Insert(index, item);
        }

        internal virtual void RemoveItemAt(int index)
        {
            IList<JToken> childrenTokens = ChildrenTokens;
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index", "Index is less than 0.");
            }
            if (index >= childrenTokens.Count)
            {
                throw new ArgumentOutOfRangeException("index", "Index is equal to or greater than Count.");
            }
            CheckReentrancy();
            JToken jtoken = childrenTokens[index];
            JToken jtoken2 = (index == 0) ? null : childrenTokens[index - 1];
            JToken jtoken3 = (index == childrenTokens.Count - 1) ? null : childrenTokens[index + 1];
            if (jtoken2 != null)
            {
                jtoken2.Next = jtoken3;
            }
            if (jtoken3 != null)
            {
                jtoken3.Previous = jtoken2;
            }
            jtoken.Parent = null;
            jtoken.Previous = null;
            jtoken.Next = null;
            childrenTokens.RemoveAt(index);
        }

        internal virtual bool RemoveItem(JToken item)
        {
            int num = IndexOfItem(item);
            if (num >= 0)
            {
                RemoveItemAt(num);
                return true;
            }
            return false;
        }

        internal virtual JToken GetItem(int index)
        {
            return ChildrenTokens[index];
        }

        internal virtual void SetItem(int index, JToken item)
        {
            IList<JToken> childrenTokens = ChildrenTokens;
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index", "Index is less than 0.");
            }
            if (index >= childrenTokens.Count)
            {
                throw new ArgumentOutOfRangeException("index", "Index is equal to or greater than Count.");
            }
            JToken jtoken = childrenTokens[index];
            if (JContainer.IsTokenUnchanged(jtoken, item))
            {
                return;
            }
            CheckReentrancy();
            item = EnsureParentToken(item, false);
            ValidateToken(item, jtoken);
            JToken jtoken2 = (index == 0) ? null : childrenTokens[index - 1];
            JToken jtoken3 = (index == childrenTokens.Count - 1) ? null : childrenTokens[index + 1];
            item.Parent = this;
            item.Previous = jtoken2;
            if (jtoken2 != null)
            {
                jtoken2.Next = item;
            }
            item.Next = jtoken3;
            if (jtoken3 != null)
            {
                jtoken3.Previous = item;
            }
            childrenTokens[index] = item;
            jtoken.Parent = null;
            jtoken.Previous = null;
            jtoken.Next = null;
        }

        internal virtual void ClearItems()
        {
            CheckReentrancy();
            IList<JToken> childrenTokens = ChildrenTokens;
            foreach (JToken jtoken in childrenTokens)
            {
                jtoken.Parent = null;
                jtoken.Previous = null;
                jtoken.Next = null;
            }
            childrenTokens.Clear();
        }

        internal virtual void ReplaceItem(JToken existing, JToken replacement)
        {
            if (existing == null || existing.Parent != this)
            {
                return;
            }
            int index = IndexOfItem(existing);
            SetItem(index, replacement);
        }

        internal virtual bool ContainsItem(JToken item)
        {
            return IndexOfItem(item) != -1;
        }

        internal virtual void CopyItemsTo(Array array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException("arrayIndex", "arrayIndex is less than 0.");
            }
            if (arrayIndex >= array.Length && arrayIndex != 0)
            {
                throw new ArgumentException("arrayIndex is equal to or greater than the length of array.");
            }
            if (Count > array.Length - arrayIndex)
            {
                throw new ArgumentException("The number of elements in the source JObject is greater than the available space from arrayIndex to the end of the destination array.");
            }
            int num = 0;
            foreach (JToken value in ChildrenTokens)
            {
                array.SetValue(value, arrayIndex + num);
                num++;
            }
        }

        internal static bool IsTokenUnchanged(JToken currentValue, JToken newValue)
        {
            JValue jvalue = currentValue as JValue;
            return jvalue != null && ((jvalue.Type == JTokenType.Null && newValue == null) || jvalue.Equals(newValue));
        }

        internal virtual void ValidateToken(JToken o, JToken existing)
        {
            ValidationUtils.ArgumentNotNull(o, "o");
            if (o.Type == JTokenType.Property)
            {
                throw new ArgumentException("Can not add {0} to {1}.".FormatWith(CultureInfo.InvariantCulture, o.GetType(), base.GetType()));
            }
        }

        /// <summary>
        /// Adds the specified content as children of this <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <param name="content">The content to be added.</param>
        public virtual void Add(object content)
        {
            AddInternal(ChildrenTokens.Count, content, false);
        }

        internal void AddAndSkipParentCheck(JToken token)
        {
            AddInternal(ChildrenTokens.Count, token, true);
        }

        /// <summary>
        /// Adds the specified content as the first children of this <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <param name="content">The content to be added.</param>
        public void AddFirst(object content)
        {
            AddInternal(0, content, false);
        }

        internal void AddInternal(int index, object content, bool skipParentCheck)
        {
            if (IsMultiContent(content))
            {
                IEnumerable enumerable = (IEnumerable)content;
                int num = index;
                IEnumerator enumerator = enumerable.GetEnumerator();
                {
                    while (enumerator.MoveNext())
                    {
                        object content2 = enumerator.Current;
                        AddInternal(num, content2, skipParentCheck);
                        num++;
                    }
                    return;
                }
            }
            JToken item = JContainer.CreateFromContent(content);
            InsertItem(index, item, skipParentCheck);
        }

        internal static JToken CreateFromContent(object content)
        {
            JToken jtoken = content as JToken;
            if (jtoken != null)
            {
                return jtoken;
            }
            return new JValue(content);
        }

        /// <summary>
        /// Creates a <see cref="T:Newtonsoft.Json.JsonWriter" /> that can be used to add tokens to the <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <returns>A <see cref="T:Newtonsoft.Json.JsonWriter" /> that is ready to have content written to it.</returns>
        public JsonWriter CreateWriter()
        {
            return new JTokenWriter(this);
        }

        /// <summary>
        /// Replaces the child nodes of this token with the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        public void ReplaceAll(object content)
        {
            ClearItems();
            Add(content);
        }

        /// <summary>
        /// Removes the child nodes from this token.
        /// </summary>
        public void RemoveAll()
        {
            ClearItems();
        }

        internal abstract void MergeItem(object content, JsonMergeSettings settings);

        /// <summary>
        /// Merge the specified content into this <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <param name="content">The content to be merged.</param>
        public void Merge(object content)
        {
            MergeItem(content, new JsonMergeSettings());
        }

        /// <summary>
        /// Merge the specified content into this <see cref="T:Newtonsoft.Json.Linq.JToken" /> using <see cref="T:Newtonsoft.Json.Linq.JsonMergeSettings" />.
        /// </summary>
        /// <param name="content">The content to be merged.</param>
        /// <param name="settings">The <see cref="T:Newtonsoft.Json.Linq.JsonMergeSettings" /> used to merge the content.</param>
        public void Merge(object content, JsonMergeSettings settings)
        {
            MergeItem(content, settings);
        }

        internal void ReadTokenFrom(JsonReader reader, JsonLoadSettings options)
        {
            int depth = reader.Depth;
            if (!reader.Read())
            {
                throw JsonReaderException.Create(reader, "Error reading {0} from JsonReader.".FormatWith(CultureInfo.InvariantCulture, base.GetType().Name));
            }
            ReadContentFrom(reader, options);
            if (reader.Depth > depth)
            {
                throw JsonReaderException.Create(reader, "Unexpected end of content while loading {0}.".FormatWith(CultureInfo.InvariantCulture, base.GetType().Name));
            }
        }

        internal void ReadContentFrom(JsonReader r, JsonLoadSettings settings)
        {
            ValidationUtils.ArgumentNotNull(r, "r");
            IJsonLineInfo lineInfo = r as IJsonLineInfo;
            JContainer jcontainer = this;
            for (; ; )
            {
                JProperty jproperty = jcontainer as JProperty;
                if (((jproperty != null) ? jproperty.Value : null) != null)
                {
                    if (jcontainer == this)
                    {
                        break;
                    }
                    jcontainer = jcontainer.Parent;
                }
                switch (r.TokenType)
                {
                    case JsonToken.None:
                        goto IL_226;
                    case JsonToken.StartObject:
                        {
                            JObject jobject = new JObject();
                            jobject.SetLineInfo(lineInfo, settings);
                            jcontainer.Add(jobject);
                            jcontainer = jobject;
                            goto IL_226;
                        }
                    case JsonToken.StartArray:
                        {
                            JArray jarray = new JArray();
                            jarray.SetLineInfo(lineInfo, settings);
                            jcontainer.Add(jarray);
                            jcontainer = jarray;
                            goto IL_226;
                        }
                    case JsonToken.StartConstructor:
                        {
                            JConstructor jconstructor = new JConstructor(r.Value.ToString());
                            jconstructor.SetLineInfo(lineInfo, settings);
                            jcontainer.Add(jconstructor);
                            jcontainer = jconstructor;
                            goto IL_226;
                        }
                    case JsonToken.PropertyName:
                        {
                            string name = r.Value.ToString();
                            JProperty jproperty2 = new JProperty(name);
                            jproperty2.SetLineInfo(lineInfo, settings);
                            JProperty jproperty3 = ((JObject)jcontainer).Property(name);
                            if (jproperty3 == null)
                            {
                                jcontainer.Add(jproperty2);
                            }
                            else
                            {
                                jproperty3.Replace(jproperty2);
                            }
                            jcontainer = jproperty2;
                            goto IL_226;
                        }
                    case JsonToken.Comment:
                        if (settings != null && settings.CommentHandling == CommentHandling.Load)
                        {
                            JValue jvalue = JValue.CreateComment(r.Value.ToString());
                            jvalue.SetLineInfo(lineInfo, settings);
                            jcontainer.Add(jvalue);
                            goto IL_226;
                        }
                        goto IL_226;
                    case JsonToken.Integer:
                    case JsonToken.Float:
                    case JsonToken.String:
                    case JsonToken.Boolean:
                    case JsonToken.Date:
                    case JsonToken.Bytes:
                        {
                            JValue jvalue = new JValue(r.Value);
                            jvalue.SetLineInfo(lineInfo, settings);
                            jcontainer.Add(jvalue);
                            goto IL_226;
                        }
                    case JsonToken.Null:
                        {
                            JValue jvalue = JValue.CreateNull();
                            jvalue.SetLineInfo(lineInfo, settings);
                            jcontainer.Add(jvalue);
                            goto IL_226;
                        }
                    case JsonToken.Undefined:
                        {
                            JValue jvalue = JValue.CreateUndefined();
                            jvalue.SetLineInfo(lineInfo, settings);
                            jcontainer.Add(jvalue);
                            goto IL_226;
                        }
                    case JsonToken.EndObject:
                        if (jcontainer == this)
                        {
                            return;
                        }
                        jcontainer = jcontainer.Parent;
                        goto IL_226;
                    case JsonToken.EndArray:
                        if (jcontainer == this)
                        {
                            return;
                        }
                        jcontainer = jcontainer.Parent;
                        goto IL_226;
                    case JsonToken.EndConstructor:
                        if (jcontainer == this)
                        {
                            return;
                        }
                        jcontainer = jcontainer.Parent;
                        goto IL_226;
                }
                goto Block_4;
            IL_226:
                if (!r.Read())
                {
                    return;
                }
            }
            return;
        Block_4:
            throw new InvalidOperationException("The JsonReader should not be on a token of type {0}.".FormatWith(CultureInfo.InvariantCulture, r.TokenType));
        }

        internal int ContentsHashCode()
        {
            int num = 0;
            foreach (JToken jtoken in ChildrenTokens)
            {
                num ^= jtoken.GetDeepHashCode();
            }
            return num;
        }

        int IList<JToken>.IndexOf(JToken item)
        {
            return IndexOfItem(item);
        }

        void IList<JToken>.Insert(int index, JToken item)
        {
            InsertItem(index, item, false);
        }

        void IList<JToken>.RemoveAt(int index)
        {
            RemoveItemAt(index);
        }

        JToken IList<JToken>.this[int index]
        {
            get
            {
                return GetItem(index);
            }
            set
            {
                SetItem(index, value);
            }
        }

        void ICollection<JToken>.Add(JToken item)
        {
            Add(item);
        }

        void ICollection<JToken>.Clear()
        {
            ClearItems();
        }

        bool ICollection<JToken>.Contains(JToken item)
        {
            return ContainsItem(item);
        }

        void ICollection<JToken>.CopyTo(JToken[] array, int arrayIndex)
        {
            CopyItemsTo(array, arrayIndex);
        }

        bool ICollection<JToken>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        bool ICollection<JToken>.Remove(JToken item)
        {
            return RemoveItem(item);
        }

        private JToken EnsureValue(object value)
        {
            if (value == null)
            {
                return null;
            }
            JToken jtoken = value as JToken;
            if (jtoken != null)
            {
                return jtoken;
            }
            throw new ArgumentException("Argument is not a JToken.");
        }

        int IList.Add(object value)
        {
            Add(EnsureValue(value));
            return Count - 1;
        }

        void IList.Clear()
        {
            ClearItems();
        }

        bool IList.Contains(object value)
        {
            return ContainsItem(EnsureValue(value));
        }

        int IList.IndexOf(object value)
        {
            return IndexOfItem(EnsureValue(value));
        }

        void IList.Insert(int index, object value)
        {
            InsertItem(index, EnsureValue(value), false);
        }

        bool IList.IsFixedSize
        {
            get
            {
                return false;
            }
        }

        bool IList.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        void IList.Remove(object value)
        {
            RemoveItem(EnsureValue(value));
        }

        void IList.RemoveAt(int index)
        {
            RemoveItemAt(index);
        }

        object IList.this[int index]
        {
            get
            {
                return GetItem(index);
            }
            set
            {
                SetItem(index, EnsureValue(value));
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            CopyItemsTo(array, index);
        }

        /// <summary>
        /// Gets the count of child JSON tokens.
        /// </summary>
        /// <value>The count of child JSON tokens.</value>
        public int Count
        {
            get
            {
                return ChildrenTokens.Count;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return false;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                {
                    Interlocked.CompareExchange(ref _syncRoot, new object(), null);
                }
                return _syncRoot;
            }
        }

        internal static void MergeEnumerableContent(JContainer target, IEnumerable content, JsonMergeSettings settings)
        {
            switch (settings.MergeArrayHandling)
            {
                case MergeArrayHandling.Concat:

                    {
                        IEnumerator enumerator = content.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            object obj = enumerator.Current;
                            JToken content2 = (JToken)obj;
                            target.Add(content2);
                        }
                        return;
                    }
                case MergeArrayHandling.Union:
                    break;
                case MergeArrayHandling.Replace:
                    goto IL_B6;
                case MergeArrayHandling.Merge:
                    goto IL_FB;
                default:
                    goto IL_18C;
            }
            HashSet<JToken> hashSet = new HashSet<JToken>(target, JToken.EqualityComparer);

            {
                IEnumerator enumerator = content.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    object obj2 = enumerator.Current;
                    JToken jtoken = (JToken)obj2;
                    if (hashSet.Add(jtoken))
                    {
                        target.Add(jtoken);
                    }
                }
                return;
            }
        IL_B6:
            target.ClearItems();

            {
                IEnumerator enumerator = content.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    object obj3 = enumerator.Current;
                    JToken content3 = (JToken)obj3;
                    target.Add(content3);
                }
                return;
            }
        IL_FB:
            int num = 0;

            {
                IEnumerator enumerator = content.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    object obj4 = enumerator.Current;
                    if (num < target.Count)
                    {
                        JContainer jcontainer = target[num] as JContainer;
                        if (jcontainer != null)
                        {
                            jcontainer.Merge(obj4, settings);
                        }
                        else if (obj4 != null)
                        {
                            JToken jtoken2 = JContainer.CreateFromContent(obj4);
                            if (jtoken2.Type != JTokenType.Null)
                            {
                                target[num] = jtoken2;
                            }
                        }
                    }
                    else
                    {
                        target.Add(obj4);
                    }
                    num++;
                }
                return;
            }
        IL_18C:
            throw new ArgumentOutOfRangeException("settings", "Unexpected merge array handling when merging JSON.");
        }

        private object _syncRoot;

        private bool _busy;
    }
}
