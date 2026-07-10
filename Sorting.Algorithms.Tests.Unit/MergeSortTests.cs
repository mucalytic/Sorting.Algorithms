using FluentAssertions;

namespace Sorting.Algorithms.Tests.Unit;

public class MergeSortTests
{
    public static class Internal
    {
        public static T[] MergeSort<T>(T[] items) where T : IComparable<T>
        {
            if (items.Length <= 1) return items;
            var middle = items.Length / 2;
            var left = MergeSort(items[..middle]);
            var right = MergeSort(items[middle..]);
            return Merge(left, right);
        }

        private static T[] Merge<T>(T[] left, T[] right) where T : IComparable<T>
        {
            var i = 0;
            var j = 0;
            var merged = new T[left.Length + right.Length];
            while (i < left.Length && j < right.Length)
            {
                merged[i + j] = left[i].CompareTo(right[j]) <= 0 ? left[i++] : right[j++];
            }
            while (i < left.Length)
            {
                merged[i + j] = left[i++];
            }
            while (j < right.Length)
            {
                merged[i + j] = right[j++];
            }
            return merged;
        }
    }

    [Fact]
    public void MergeSort_SortsAnUnsortedArray()
    {
        var items = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        var sorted = Internal.MergeSort(items);

        sorted.Should().Equal(1, 2, 3, 4, 5, 6, 7, 8, 9);
    }

    [Fact]
    public void MergeSort_ReturnsAnEmptyArrayUnchanged()
    {
        var items = Array.Empty<int>();

        var sorted = Internal.MergeSort(items);

        sorted.Should().BeEmpty();
    }

    [Fact]
    public void MergeSort_ReturnsASingleElementArrayUnchanged()
    {
        var items = new[] { 42 };

        var sorted = Internal.MergeSort(items);

        sorted.Should().Equal(42);
    }

    [Fact]
    public void MergeSort_LeavesAnAlreadySortedArraySorted()
    {
        var items = new[] { 1, 2, 3, 4, 5 };

        var sorted = Internal.MergeSort(items);

        sorted.Should().Equal(1, 2, 3, 4, 5);
    }

    [Fact]
    public void MergeSort_SortsAReverseSortedArray()
    {
        var items = new[] { 5, 4, 3, 2, 1 };

        var sorted = Internal.MergeSort(items);

        sorted.Should().Equal(1, 2, 3, 4, 5);
    }

    [Fact]
    public void MergeSort_HandlesDuplicateElements()
    {
        var items = new[] { 3, 1, 3, 2, 1, 2, 3 };

        var sorted = Internal.MergeSort(items);

        sorted.Should().Equal(1, 1, 2, 2, 3, 3, 3);
    }

    [Fact]
    public void MergeSort_HandlesNegativeNumbers()
    {
        var items = new[] { 3, -1, 0, -5, 2 };

        var sorted = Internal.MergeSort(items);

        sorted.Should().Equal(-5, -1, 0, 2, 3);
    }

    [Fact]
    public void MergeSort_SortsStrings()
    {
        var items = new[] { "banana", "apple", "cherry" };

        var sorted = Internal.MergeSort(items);

        sorted.Should().Equal("apple", "banana", "cherry");
    }

    [Fact]
    public void MergeSort_DoesNotMutateTheInputArray()
    {
        var items = new[] { 3, 1, 2 };

        Internal.MergeSort(items);

        items.Should().Equal(3, 1, 2);
    }

    [Fact]
    public void MergeSort_SortsALargeRandomArray()
    {
        var random = new Random(1234);
        var items = Enumerable.Range(0, 10_000).Select(_ => random.Next()).ToArray();

        var sorted = Internal.MergeSort(items);

        sorted.Should().Equal(items.OrderBy(x => x));
    }
}
