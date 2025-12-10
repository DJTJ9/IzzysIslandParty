
public class Heap<T> where T : IHeapItem<T>
{
    private T[] items;
    private int currentItemCount;


    public int Count => currentItemCount;

    public Heap(int _maxHeapSize)
    {
        items = new T[_maxHeapSize];
    }

    public void Add(T _item)
    {
        _item.HeapIndex = currentItemCount;
        items[currentItemCount] = _item;

        SortUp(_item);

        currentItemCount++;
    }

    public T RemoveFirst()
    {
        T firstItem = items[0];
        currentItemCount--;

        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;

        SortDown(items[0]);

        return firstItem;
    }

    public void Clear(int _maxHeapSize)
    {
        currentItemCount = 0;
        // Why do I need to do this?? It makes no sense
        // items = new T[_maxHeapSize];s
    }

    public bool Contains(T _item)
    {
        if (_item.HeapIndex < currentItemCount)
            return Equals(items[_item.HeapIndex], _item);

        return false;
    }

    public void UpdateItem(T _item)
    {
        SortUp(_item);
    }


    private void SortUp(T _itemToSort)
    {
        int parentIndex = (_itemToSort.HeapIndex - 1) / 2;

        while (true)
        {
            T parentItem = items[parentIndex];

            if (_itemToSort.CompareTo(parentItem) > 0)
                Swap(_itemToSort, parentItem);
            else
                break;

            parentIndex = (_itemToSort.HeapIndex - 1) / 2;
        }
    }

    private void SortDown(T _itemToSort)
    {
        while (true)
        {
            int childIndexLeft = (_itemToSort.HeapIndex * 2) + 1;
            int childIndexRight = (_itemToSort.HeapIndex * 2) + 2;

            int swapIndex = 0;

            if (childIndexLeft < currentItemCount)
            {
                swapIndex = childIndexLeft;

                if (childIndexRight < currentItemCount)
                {
                    if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                        swapIndex = childIndexRight;
                }

                if (_itemToSort.CompareTo(items[swapIndex]) < 0)
                    Swap(_itemToSort, items[swapIndex]);
                else
                    return;
            }
            else
                return;
        }
    }

    private void Swap(T _itemA, T _itemB)
    {
        items[_itemA.HeapIndex] = _itemB;
        items[_itemB.HeapIndex] = _itemA;

        int itemAIndex = _itemA.HeapIndex;

        _itemA.HeapIndex = _itemB.HeapIndex;
        _itemB.HeapIndex = itemAIndex;
    }
}
