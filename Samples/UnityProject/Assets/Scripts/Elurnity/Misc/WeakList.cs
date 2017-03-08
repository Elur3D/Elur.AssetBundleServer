﻿using System;
using System.Linq;
using System.Collections.Generic;

public class WeakList<T> : IList<T>
{
    private List<WeakReference<T>> _innerList = new List<WeakReference<T>>();

    #region IList<T> Members

    public int IndexOf(T item)
    {
        for (int i = 0; i < _innerList.Count; i++)
        {
            if (EqualityComparer<T>.Default.Equals(_innerList[i].Target, item))
            {
                return i;
            }
        }
        return -1;
    }

    public void Insert(int index, T item)
    {
        _innerList.Insert(index, new WeakReference<T>(item));
    }

    public void RemoveAt(int index)
    {
        _innerList.RemoveAt(index);
    }

    public T this[int index]
    {
        get
        {
            return _innerList[index].Target;
        }

        set
        {
            _innerList[index] = new WeakReference<T>(value);
        }
    }

    #endregion

    #region ICollection<T> Members

    public void Add(T item)
    {
        _innerList.Add(new WeakReference<T>(item));
    }

    public void Clear()
    {
        _innerList.Clear();
    }

    public bool Contains(T item)
    {
        for (int i = 0; i < _innerList.Count; i++)
        {
            if (EqualityComparer<T>.Default.Equals(_innerList[i].Target, item))
            {
                return true;
            }
        }
        return false;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        throw new NotImplementedException();
        //_innerList.Select(wr => wr.Target).CopyTo(array, arrayIndex);
    }

    public int Count
    {
        get { return _innerList.Count; }
    }

    public bool IsReadOnly
    {
        get { return false; }
    }

    public bool Remove(T item)
    {
        for (int i = 0; i < _innerList.Count; i++)
        {
            if (EqualityComparer<T>.Default.Equals(_innerList[i].Target, item))
            {
                RemoveAt(i);
                return true;
            }
        }
        return false;
    }

    #endregion

    #region IEnumerable<T> Members

    public IEnumerator<T> GetEnumerator()
    {
        return _innerList.Select(x => x.Target).GetEnumerator();
    }

    #endregion

    #region IEnumerable Members

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    #endregion

    public void Purge()
    {
        _innerList.RemoveAll(wr => !wr.IsAlive);
    }
}