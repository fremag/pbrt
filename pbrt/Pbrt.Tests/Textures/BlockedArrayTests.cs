using System.Linq;
using NFluent;
using NUnit.Framework;
using pbrt.Textures;

namespace Pbrt.Tests.Textures;

[TestFixture]
public class BlockedArrayTests
{
    readonly int[] data =
    {
        0, 1, 2, 3, 4, 5, 6, 7, 8, 9,
        10, 11, 12, 13, 14, 15, 16, 17, 18, 19,
        20, 21, 22, 23, 24, 25, 26, 27, 28, 29,
        30, 31
    };

    private BlockedArray<int> blockedArray;

    [SetUp]
    public void Setup()
    {
        blockedArray = new BlockedArray<int>(4, 8, data);
    }
    
    [Test]
    public void BasicTest()
    {
        Check.That(blockedArray.LogBlockSize).IsEqualTo(2);
        Check.That(blockedArray.UBlocks).IsEqualTo(1);
        Check.That(blockedArray.URes).IsEqualTo(4);
        Check.That(blockedArray.VRes).IsEqualTo(8);
        Check.That(blockedArray.USize()).IsEqualTo(4);
        Check.That(blockedArray.VSize()).IsEqualTo(8);
        Check.That(blockedArray.BlockSize()).IsEqualTo(4);
        Check.That(blockedArray.Data).ContainsExactly(data);
        
        var anotherBlockedArray = new BlockedArray<int>(4, 8);
        Check.That(anotherBlockedArray.Data.All(i => i == 0)).IsTrue();
    }

    [Test]
    public void IndexGetTest()
    {
        Check.That(blockedArray[0,0]).IsEqualTo(0);
        Check.That(blockedArray[1,0]).IsEqualTo(1);
        Check.That(blockedArray[2,0]).IsEqualTo(2);
        Check.That(blockedArray[3,0]).IsEqualTo(3);
        Check.That(blockedArray[4,0]).IsEqualTo(16);
        Check.That(blockedArray[5,0]).IsEqualTo(17);
        Check.That(blockedArray[6,0]).IsEqualTo(18);
        Check.That(blockedArray[7,0]).IsEqualTo(19);

        Check.That(blockedArray[0,1]).IsEqualTo(4);
        Check.That(blockedArray[1,1]).IsEqualTo(5);
        Check.That(blockedArray[2,1]).IsEqualTo(6);
        Check.That(blockedArray[3,1]).IsEqualTo(7);
        Check.That(blockedArray[4,1]).IsEqualTo(20);
        Check.That(blockedArray[5,1]).IsEqualTo(21);
        Check.That(blockedArray[6,1]).IsEqualTo(22);
        Check.That(blockedArray[7,1]).IsEqualTo(23);

        Check.That(blockedArray[0,2]).IsEqualTo(8);
        Check.That(blockedArray[1,2]).IsEqualTo(9);
        Check.That(blockedArray[2,2]).IsEqualTo(10);
        Check.That(blockedArray[3,2]).IsEqualTo(11);
        Check.That(blockedArray[4,2]).IsEqualTo(24);
        Check.That(blockedArray[5,2]).IsEqualTo(25);
        Check.That(blockedArray[6,2]).IsEqualTo(26);
        Check.That(blockedArray[7,2]).IsEqualTo(27);

        Check.That(blockedArray[0,3]).IsEqualTo(12);
        Check.That(blockedArray[1,3]).IsEqualTo(13);
        Check.That(blockedArray[2,3]).IsEqualTo(14);
        Check.That(blockedArray[3,3]).IsEqualTo(15);
        Check.That(blockedArray[4,3]).IsEqualTo(28);
        Check.That(blockedArray[5,3]).IsEqualTo(29);
        Check.That(blockedArray[6,3]).IsEqualTo(30);
        Check.That(blockedArray[7,3]).IsEqualTo(31);
    }

    [Test]
    public void RoundUpTest()
    {
        Check.That(blockedArray.RoundUp(0)).IsEqualTo(0);
        Check.That(blockedArray.RoundUp(1)).IsEqualTo(4);
        Check.That(blockedArray.RoundUp(2)).IsEqualTo(4);
        Check.That(blockedArray.RoundUp(3)).IsEqualTo(4);
        Check.That(blockedArray.RoundUp(4)).IsEqualTo(4);
        Check.That(blockedArray.RoundUp(5)).IsEqualTo(8);
        Check.That(blockedArray.RoundUp(6)).IsEqualTo(8);
        Check.That(blockedArray.RoundUp(7)).IsEqualTo(8);
        Check.That(blockedArray.RoundUp(8)).IsEqualTo(8);
        Check.That(blockedArray.RoundUp(9)).IsEqualTo(12);
    }
    
    [Test]
    public void BlockTest()
    {
        Check.That(blockedArray.Block(0)).IsEqualTo(0);
        Check.That(blockedArray.Block(1)).IsEqualTo(0);
        Check.That(blockedArray.Block(2)).IsEqualTo(0);
        Check.That(blockedArray.Block(3)).IsEqualTo(0);
        Check.That(blockedArray.Block(4)).IsEqualTo(1);
        Check.That(blockedArray.Block(5)).IsEqualTo(1);
        Check.That(blockedArray.Block(6)).IsEqualTo(1);
        Check.That(blockedArray.Block(7)).IsEqualTo(1);
        Check.That(blockedArray.Block(8)).IsEqualTo(2);
        Check.That(blockedArray.Block(9)).IsEqualTo(2);
    }
    
    [Test]
    public void OffsetTest()
    {
        Check.That(blockedArray.Offset(0)).IsEqualTo(0);
        Check.That(blockedArray.Offset(1)).IsEqualTo(1);
        Check.That(blockedArray.Offset(2)).IsEqualTo(2);
        Check.That(blockedArray.Offset(3)).IsEqualTo(3);
        Check.That(blockedArray.Offset(4)).IsEqualTo(0);
        Check.That(blockedArray.Offset(5)).IsEqualTo(1);
        Check.That(blockedArray.Offset(6)).IsEqualTo(2);
        Check.That(blockedArray.Offset(7)).IsEqualTo(3);
        Check.That(blockedArray.Offset(8)).IsEqualTo(0);
        Check.That(blockedArray.Offset(9)).IsEqualTo(1);
    }

    [Test]
    public void GetLinearArrayTest()
    {
        var d = new int[data.Length];
        blockedArray.GetLinearArray(d);

        Check.That(d).ContainsExactly(3,2,1,0,7,6,5,4,11,10,9,8,15,14,13,12,19,18,17,16,23,22,21,20, 27,26,25,24, 31, 30, 29, 28);
    }
}