using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class ReadOnlyList<T> : ReadOnlyCollection<T>
{
    private readonly List<T> TempList;

    public ReadOnlyList(List<T> list) : base(list)
    {
        TempList = list;
    }

    public int Capacity
    {
        get
        {
            return TempList.Capacity;
        }
    }

    public int BinarySearch(T item)
    {
        return TempList.BinarySearch(item);
    }

    public int BinarySearch(T item, IComparer<T> comparer)
    {
        return TempList.BinarySearch(item, comparer);
    }

    public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
    {
        return TempList.BinarySearch(index, count, item, comparer);
    }

    public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
    {
        return TempList.ConvertAll(converter);
    }

    public void CopyTo(T[] array)
    {
        TempList.CopyTo(array);
    }

    public void CopyTo(int index, T[] array, int arrayIndex, int count)
    {
        TempList.CopyTo(index, array, arrayIndex, count);
    }

    public bool Exists(Predicate<T> match)
    {
        return TempList.Exists(match);
    }

    public T Find(Predicate<T> match)
    {
        return TempList.Find(match);
    }

    public List<T> FindAll(Predicate<T> match)
    {
        return TempList.FindAll(match);
    }

    public int FindIndex(Predicate<T> match)
    {
        return TempList.FindIndex(match);
    }

    public int FindIndex(int startIndex, Predicate<T> match)
    {
        return TempList.FindIndex(startIndex, match);
    }

    public int FindIndex(int startIndex, int count, Predicate<T> match)
    {
        return TempList.FindIndex(startIndex, count, match);
    }

    public T FindLast(Predicate<T> match)
    {
        return TempList.FindLast(match);
    }

    public int FindLastIndex(Predicate<T> match)
    {
        return TempList.FindLastIndex(match);
    }

    public int FindLastIndex(int startIndex, Predicate<T> match)
    {
        return TempList.FindLastIndex(startIndex, match);
    }

    public int FindLastIndex(int startIndex, int count, Predicate<T> match)
    {
        return TempList.FindLastIndex(startIndex, count, match);
    }

    public void ForEach(Action<T> action)
    {
        TempList.ForEach(action);
    }

    public List<T> GetRange(int index, int count)
    {
        return TempList.GetRange(index, count);
    }

    public int IndexOf(T item, int index)
    {
        return TempList.IndexOf(item, index);
    }

    public int IndexOf(T item, int index, int count)
    {
        return TempList.IndexOf(item, index, count);
    }

    public int LastIndexOf(T item)
    {
        return TempList.LastIndexOf(item);
    }

    public int LastIndexOf(T item, int index)
    {
        return TempList.LastIndexOf(item, index);
    }

    public int LastIndexOf(T item, int index, int count)
    {
        return TempList.LastIndexOf(item, index, count);
    }

    public T[] ToArray()
    {
        return TempList.ToArray();
    }

    public bool TrueForAll(Predicate<T> match)
    {
        return TempList.TrueForAll(match);
    }
}
