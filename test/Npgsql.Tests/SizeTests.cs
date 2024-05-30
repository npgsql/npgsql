using System;
using NUnit.Framework;
using Npgsql.Internal;

namespace Npgsql.Tests;

public class SizeTests
{
    [Test]
    public void UnknownKind() => Assert.That(Size.Unknown.Kind, Is.EqualTo(SizeKind.Unknown));

    [Test]
    public void UnknownThrowsOnValue() => Assert.Throws<InvalidOperationException>(() => _ = Size.Unknown.Value);

    [Test]
    public void Exact()
    {
        Assert.That(Size.Create(1).Value, Is.EqualTo(1));
        Assert.That(Size.Create(1).Kind, Is.EqualTo(SizeKind.Exact));
    }

    [Test]
    public void ZeroIsExactKind() => Assert.That(Size.Zero.Kind, Is.EqualTo(SizeKind.Exact));

    [Test]
    public void UpperBound()
    {
        Assert.That(Size.CreateUpperBound(1).Value, Is.EqualTo(1));
        Assert.That(Size.CreateUpperBound(1).Kind, Is.EqualTo(SizeKind.UpperBound));
    }

    [Test]
    public void CombineThrowsOnOverflow() => Assert.Throws<OverflowException>(() => Size.Create(1).Combine(int.MaxValue));

    [Test]
    public void CombineExactWorks() => Assert.That(Size.Create(1).Combine(1), Is.EqualTo(Size.Create(2)));

    [Test]
    public void CombineUpperBoundWorks() => Assert.That(Size.CreateUpperBound(1).Combine(1), Is.EqualTo(Size.CreateUpperBound(2)));

    [Test]
    public void CombineUnknownWithAnyGivesUnknown()
    {
        Assert.That(Size.Unknown.Combine(Size.Unknown), Is.EqualTo(Size.Unknown));

        Assert.That(Size.Create(1).Combine(Size.Unknown), Is.EqualTo(Size.Unknown));
        Assert.That(Size.Unknown.Combine(Size.Create(1)), Is.EqualTo(Size.Unknown));

        Assert.That(Size.Unknown.Combine(Size.CreateUpperBound(1)), Is.EqualTo(Size.Unknown));
        Assert.That(Size.CreateUpperBound(1).Combine(Size.Unknown), Is.EqualTo(Size.Unknown));
    }

    [Test]
    public void CombineUpperBoundWithExactGivesUpperBound()
    {
        Assert.That(Size.Create(1).Combine(Size.CreateUpperBound(1)), Is.EqualTo(Size.CreateUpperBound(2)));
        Assert.That(Size.CreateUpperBound(1).Combine(Size.Create(1)), Is.EqualTo(Size.CreateUpperBound(2)));
    }
}
