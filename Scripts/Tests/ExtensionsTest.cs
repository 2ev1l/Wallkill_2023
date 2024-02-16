using System.Collections;
using System.Collections.Generic;
using Data.Stored;
using Game.DataBase;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Universal;

public class ExtensionsTest
{
    [Test]
    public void ExistTest1()
    {
        List<string> list = new() { "a", "B", "c", "d", "a", "b", "C" };
        bool resultTrue = list.Exists(x => x == "a", out string str1);
        bool resultFalse = list.Exists(x => x.Equals("Z"), out string str2);
        Assert.IsTrue(resultTrue);
        Assert.IsFalse(resultFalse);
        Assert.AreSame(list[0], str1);
        Assert.AreSame(str2, null);
    }
    [Test]
    public void ExistTest2()
    {
        List<Modifier> list = new() { new Modifier(0, 1), new Modifier(1, 0), new Modifier(2, 0), new Modifier(1, 1) };
        bool resultTrue = list.Exists(x => x.Id == 1, out Modifier exist1);
        bool resultFalse = list.Exists(x => x.Rank == 2, out Modifier exist2);

        Assert.IsTrue(resultTrue);
        Assert.IsFalse(resultFalse);
        Assert.AreSame(exist1, list[1]);
        Assert.AreSame(exist2, null);
    }

}
