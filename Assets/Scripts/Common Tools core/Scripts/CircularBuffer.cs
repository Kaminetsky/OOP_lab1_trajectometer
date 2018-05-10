
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;


public class CircularBuffer<T> : ICollection<T>, IEnumerable<T>, ICollection, IEnumerable
	{
	private int m_capacity;
	private int m_size;
	private int m_head;
	private int m_tail;
	private T[] m_buffer;

	[NonSerialized()]
	private object syncRoot;


	public CircularBuffer (int m_capacity) : this(m_capacity, true)
		{
		}


	public CircularBuffer (int capacity, bool allowOverflow)
		{
		if (m_capacity < 0)
			throw new ArgumentException("Capacity must be non-negative");

		m_capacity = capacity;
		m_size = 0;
		m_head = 0;
		m_tail = 0;
		m_buffer = new T[m_capacity];
		this.allowOverflow = allowOverflow;
		}


	public bool allowOverflow
		{
		get;
		set;
		}


	public int capacity
		{
		get { return m_capacity; }

		set
			{
			if (value == m_capacity)
				return;

			if (value < m_size)
				throw new ArgumentOutOfRangeException("Capacity too small (" + value + "). Buffer contains " + m_size + " items.");

			var dst = new T[value];
			if (m_size > 0) CopyTo(dst);
			m_buffer = dst;

			m_head = 0;
			m_tail = m_size;
			m_capacity = value;
			}
		}


	public int size
		{
		get { return m_size; }
		}


	// Returns index of the first occurrence of the given item.
	// -1 if the item doesn't exist.

	public int IndexOf (T item)
		{
		int bufferIndex = m_head;
		var comparer = EqualityComparer<T>.Default;

		for (int i = 0; i < m_size; i++, bufferIndex++)
			{
			if (bufferIndex == m_capacity) bufferIndex = 0;

			if (item == null && m_buffer[bufferIndex] == null)
				return i;
			else
				if ((m_buffer[bufferIndex] != null) && comparer.Equals(m_buffer[bufferIndex], item))
					return i;
			}

		return -1;
		}


	public void RemoveAt (int index)
		{
		if (index < 0 || index >= m_size)
			throw new ArgumentOutOfRangeException("Index out of range");

		int bufferIndex = m_head + index;
		if (bufferIndex >= m_capacity)
			bufferIndex -= m_capacity;

		int toMove = m_size - index - 1;

		for (int i = 0; i < toMove; i++, bufferIndex++)
			{
			if (bufferIndex == m_capacity) bufferIndex = 0;
			int bufferNext = bufferIndex + 1;
			if (bufferNext == m_capacity) bufferNext = 0;

			m_buffer[bufferIndex] = m_buffer[bufferNext];
			}

		m_size--;
		m_tail--;
		if (m_tail < 0) m_tail = m_capacity-1;
		}


	public bool Contains (T item)
		{
		return IndexOf(item) >= 0;
		}


	public void Clear ()
		{
		m_size = 0;
		m_head = 0;
		m_tail = 0;
		}


	public int Put (T[] src)
		{
		return Put (src, 0, src.Length);
		}


	public int Put (T[] src, int srcOffset, int count)
		{
		if (!allowOverflow && count > m_capacity - m_size)
			throw new InvalidOperationException("This operation would cause buffer overflow");

		int srcIndex = srcOffset;
		for (int i = 0; i < count; i++, srcIndex++)
			{
			if (m_tail == m_capacity) m_tail = 0;
			m_buffer[m_tail++] = src[srcIndex];

			// Head is moved when the buffer capacity is reached

			if (m_size == m_capacity)
				{
				if (m_head == m_capacity) m_head = 0;
				m_head++;
				}
			else
				m_size++;
			}

		return count;
		}


	public void Put (T item)
		{
		if (!allowOverflow && m_size == m_capacity)
			throw new InvalidOperationException("This operation would cause buffer overflow");

		if (m_tail == m_capacity) m_tail = 0;
		m_buffer[m_tail++] = item;

		// Head is moved when the buffer capacity is reached

		if (m_size == m_capacity)
			{
			if (m_head == m_capacity) m_head = 0;
			m_head++;
			}
		else
			m_size++;
		}


	public void Skip (int count)
		{
		if (count > m_size) count = m_size;

		m_head += count;
		if (m_head > m_capacity)
			m_head -= m_capacity;
		m_size -= count;
		}


	public T[] Get (int count)
		{
		if (count > m_size) count = m_size;

		var dst = new T[count];
		Get(dst);
		return dst;
		}


	public int Get (T[] dst)
		{
		return Get(dst, 0, dst.Length);
		}


	public int Get (T[] dst, int dstOffset, int count)
		{
		if (count > m_size) count = m_size;

		int dstIndex = dstOffset;
		for (int i = 0; i < count; i++, dstIndex++)
			{
			if (m_head == m_capacity) m_head = 0;
			dst[dstIndex] = m_buffer[m_head++];
			}

		m_size -= count;
		return count;
		}


	public T Get ()
		{
		if (m_size == 0)
			throw new InvalidOperationException("Buffer is empty");

		if (m_head == m_capacity) m_head = 0;
		var item = m_buffer[m_head++];
		m_size--;
		return item;
		}


	public T Peek (int offset)
		{
		if (offset >= m_size)
			throw new InvalidOperationException("Offset overflow");

		int bufferIndex = m_head + offset;
		if (bufferIndex >= m_capacity)
			bufferIndex -= m_capacity;

		return m_buffer[bufferIndex];
		}


	public T Head ()
		{
		if (m_size == 0)
			throw new InvalidOperationException("Buffer is empty");

		int bufferIndex = m_head;
		if (bufferIndex == m_capacity) bufferIndex = 0;
		return m_buffer[bufferIndex];
		}


	public T Tail ()
		{
		if (m_size == 0)
			throw new InvalidOperationException("Buffer is empty");

		int bufferIndex = m_tail-1;
		if (bufferIndex < 0) bufferIndex = m_capacity-1;
		return m_buffer[bufferIndex];
		}


	public void CopyTo (T[] array)
		{
		CopyTo(array, 0);
		}


	public void CopyTo (T[] array, int arrayIndex)
		{
		CopyTo(0, array, arrayIndex, m_size);
		}


	public void CopyTo (int index, T[] array, int arrayIndex, int count)
		{
		if (count > m_size)
			throw new ArgumentOutOfRangeException("Count too large");

		int bufferIndex = m_head;
		for (int i = 0; i < count; i++, bufferIndex++, arrayIndex++)
			{
			if (bufferIndex == m_capacity) bufferIndex = 0;
			array[arrayIndex] = m_buffer[bufferIndex];
			}
		}


	public IEnumerator<T> GetEnumerator ()
		{
		int bufferIndex = m_head;
		for (int i = 0; i < m_size; i++, bufferIndex++)
			{
			if (bufferIndex == m_capacity) bufferIndex = 0;
			yield return m_buffer[bufferIndex];
			}
		}


	public T[] GetBuffer ()
		{
		return m_buffer;
		}


	public T[] ToArray ()
		{
		var dst = new T[m_size];
		CopyTo(dst);
		return dst;
		}


	#region ICollection<T> Members

	int ICollection<T>.Count
		{
		get { return size; }
		}

	bool ICollection<T>.IsReadOnly
		{
		get { return false; }
		}

	void ICollection<T>.Add (T item)
		{
		Put(item);
		}

	bool ICollection<T>.Remove (T item)
		{
		int index = IndexOf(item);
		if (index >= 0)
			{
			RemoveAt(index);
			return true;
			}

		return false;
		}

	#endregion


	#region IEnumerable<T> Members

	IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
		return GetEnumerator();
		}

	#endregion


	#region ICollection Members

	int ICollection.Count
		{
		get { return size; }
		}

	bool ICollection.IsSynchronized
		{
		get { return false; }
		}

	object ICollection.SyncRoot
		{
		get
			{
			if (syncRoot == null)
				Interlocked.CompareExchange(ref syncRoot, new object(), null);
			return syncRoot;
			}
		}

	void ICollection.CopyTo(Array array, int arrayIndex)
		{
		CopyTo((T[])array, arrayIndex);
		}

	#endregion


	#region IEnumerable Members

	IEnumerator IEnumerable.GetEnumerator()
		{
		return (IEnumerator)GetEnumerator();
		}

	#endregion

	// Debug only

	public int headIdx { get { return m_head; } }
	public int tailIdx { get { return m_tail; } }
	}
