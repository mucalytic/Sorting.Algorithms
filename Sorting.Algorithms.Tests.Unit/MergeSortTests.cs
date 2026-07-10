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

        public static T[] MergeSortOptimized<T>(T[] items) where T : IComparable<T>
        {
            var result = (T[])items.Clone();
            if (items.Length <= 1) return result;
            var scratch = (T[])items.Clone();
            SortRange(scratch, result, 0, items.Length);
            return result;
        }

        // Sorts the range [lo, hi) into destination, using source as scratch space.
        // Both arrays must hold the same values in [lo, hi) on entry; their roles
        // swap at each level of recursion so no per-level copying is needed.
        private static void SortRange<T>(T[] source, T[] destination, int lo, int hi) where T : IComparable<T>
        {
            if (hi - lo <= 1) return;
            var middle = (lo + hi) / 2;
            SortRange(destination, source, lo, middle);
            SortRange(destination, source, middle, hi);
            var i = lo;
            var j = middle;
            for (var k = lo; k < hi; k++)
            {
                destination[k] = j >= hi || (i < middle && source[i].CompareTo(source[j]) <= 0)
                    ? source[i++]
                    : source[j++];
            }
        }
    }

    public enum Variant
    {
        Simple,
        Optimized
    }

    private static T[] Sort<T>(Variant variant, T[] items) where T : IComparable<T> => variant switch
    {
        Variant.Simple => Internal.MergeSort(items),
        Variant.Optimized => Internal.MergeSortOptimized(items),
        _ => throw new ArgumentOutOfRangeException(nameof(variant))
    };

    [Theory]
    [InlineData(Variant.Simple)]
    [InlineData(Variant.Optimized)]
    public void MergeSort_SortsAnUnsortedArray(Variant variant)
    {
        var items = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        var sorted = Sort(variant, items);

        sorted.Should().Equal(1, 2, 3, 4, 5, 6, 7, 8, 9);
    }

    [Theory]
    [InlineData(Variant.Simple)]
    [InlineData(Variant.Optimized)]
    public void MergeSort_ReturnsAnEmptyArrayUnchanged(Variant variant)
    {
        var items = Array.Empty<int>();

        var sorted = Sort(variant, items);

        sorted.Should().BeEmpty();
    }

    [Theory]
    [InlineData(Variant.Simple)]
    [InlineData(Variant.Optimized)]
    public void MergeSort_ReturnsASingleElementArrayUnchanged(Variant variant)
    {
        var items = new[] { 42 };

        var sorted = Sort(variant, items);

        sorted.Should().Equal(42);
    }

    [Theory]
    [InlineData(Variant.Simple)]
    [InlineData(Variant.Optimized)]
    public void MergeSort_LeavesAnAlreadySortedArraySorted(Variant variant)
    {
        var items = new[] { 1, 2, 3, 4, 5 };

        var sorted = Sort(variant, items);

        sorted.Should().Equal(1, 2, 3, 4, 5);
    }

    [Theory]
    [InlineData(Variant.Simple)]
    [InlineData(Variant.Optimized)]
    public void MergeSort_SortsAReverseSortedArray(Variant variant)
    {
        var items = new[] { 5, 4, 3, 2, 1 };

        var sorted = Sort(variant, items);

        sorted.Should().Equal(1, 2, 3, 4, 5);
    }

    [Theory]
    [InlineData(Variant.Simple)]
    [InlineData(Variant.Optimized)]
    public void MergeSort_HandlesDuplicateElements(Variant variant)
    {
        var items = new[] { 3, 1, 3, 2, 1, 2, 3 };

        var sorted = Sort(variant, items);

        sorted.Should().Equal(1, 1, 2, 2, 3, 3, 3);
    }

    [Theory]
    [InlineData(Variant.Simple)]
    [InlineData(Variant.Optimized)]
    public void MergeSort_HandlesNegativeNumbers(Variant variant)
    {
        var items = new[] { 3, -1, 0, -5, 2 };

        var sorted = Sort(variant, items);

        sorted.Should().Equal(-5, -1, 0, 2, 3);
    }

    [Theory]
    [InlineData(Variant.Simple)]
    [InlineData(Variant.Optimized)]
    public void MergeSort_SortsStrings(Variant variant)
    {
        var items = new[] { "banana", "apple", "cherry" };

        var sorted = Sort(variant, items);

        sorted.Should().Equal("apple", "banana", "cherry");
    }

    [Theory]
    [InlineData(Variant.Simple)]
    [InlineData(Variant.Optimized)]
    public void MergeSort_DoesNotMutateTheInputArray(Variant variant)
    {
        var items = new[] { 3, 1, 2 };

        Sort(variant, items);

        items.Should().Equal(3, 1, 2);
    }

    // Compares by Key only, so Tag reveals whether equal elements kept their original order.
    private sealed record Keyed(int Key, string Tag) : IComparable<Keyed>
    {
        public int CompareTo(Keyed? other) => Key.CompareTo(other!.Key);
    }

    [Theory]
    [InlineData(Variant.Simple)]
    [InlineData(Variant.Optimized)]
    public void MergeSort_IsStable(Variant variant)
    {
        var items = new[] { new Keyed(2, "a"), new Keyed(1, "b"), new Keyed(2, "c"), new Keyed(1, "d") };

        var sorted = Sort(variant, items);

        sorted.Select(k => k.Tag).Should().Equal("b", "d", "a", "c");
    }

    [Theory]
    [InlineData(Variant.Simple)]
    [InlineData(Variant.Optimized)]
    public void MergeSort_SortsALargeRandomArray(Variant variant)
    {
        var random = new Random(1234);
        var items = Enumerable.Range(0, 10_000).Select(_ => random.Next()).ToArray();

        var sorted = Sort(variant, items);

        sorted.Should().Equal(items.OrderBy(x => x));
    }

    [Theory]
    [InlineData(Variant.Simple)]
    [InlineData(Variant.Optimized)]
    public void MergeSort_BothVariantsProduceIdenticalOutput(Variant variant)
    {
        var random = new Random(5678);
        var items = Enumerable.Range(0, 1_000).Select(_ => random.Next(0, 100)).ToArray();

        var sorted = Sort(variant, items);

        sorted.Should().Equal(Internal.MergeSort(items));
    }
}
