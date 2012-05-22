using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace DeepDive
{
    [TestFixture]
    public abstract class BaseMergeSequenceTest
    {
        public abstract IEnumerable<int> Merge(IEnumerable<int> leftSequence, IEnumerable<int> rightSequence);

        [Test]
        public void should_merge_empty_sequences_properly()
        {
            Merge(Enumerable.Empty<int>(),
                  Enumerable.Empty<int>()).Should().BeEmpty();
        }

        [Test]
        public void should_merge_not_empty_left_sequence_properly()
        {
            Merge(new[] {1}, Enumerable.Empty<int>()).Should().ContainInOrder(new[]{1});
        }

        [Test]
        public void should_merge_not_empty_right_sequence_properly()
        {
            Merge(Enumerable.Empty<int>(), new[] {2}).Should().ContainInOrder(new[] {2});
        }

        [Test]
        public void should_merge_single_element_sequences_properly()
        {
            Merge(new[] {2}, new[] {1}).Should().ContainInOrder(new[] {1, 2});
        }

        [Test]
        public void should_properly_merge_long_left_sequence()
        {
            Merge(new[] {1, 3, 5}, new[] {2}).Should().ContainInOrder(new[] {1, 2, 3, 5});
        }

        [Test]
        public void should_properly_merge_long_right_sequence()
        {
            Merge(new[] {3}, new[] {2, 4, 6}).Should().ContainInOrder(new[] {2, 3, 4, 6});
        }

        [Test]
        public void should_merge_sequences_with_equal_values_properly()
        {
            Merge(new[] {2, 2, 2, 2}, new[] {2, 2}).Should().ContainInOrder(new[] {2, 2, 2, 2, 2});
        }

        [Test]
        public void should_merge_revrsed_sequences_properly()
        {
            Merge(new[] {4, 5, 6}, new[] {1, 2, 3}).Should().ContainInOrder(new[] {1, 2, 3, 4, 5, 6});
        }
    }
}