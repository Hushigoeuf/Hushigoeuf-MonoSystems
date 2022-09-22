using System;
using System.Collections;
using System.Collections.Generic;
using Hushigoeuf.MonoSystems;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SystemTests
{
    private Type[] _targetExtensionTypes = new[]
    {
        typeof(TestSystem_1),
        typeof(TestSystem_1_1),
        typeof(TestSystem_1_2),
        typeof(TestSystem_2),
        typeof(TestSystem_3),
        typeof(TestSystemInChildren),
    };

    [UnityTest]
    public IEnumerator WhenGameLoaded_InitExtensions_ThenAllExtensionsRegistered()
    {
        var container = CreateDefaultContainerAndExtensions();

        yield return null;

        foreach (var t in _targetExtensionTypes)
            Assert.IsTrue(container.ContainsSystem(t));
    }

    [UnityTest]
    public IEnumerator WhenGameLoaded_InitExtensions_ThenExtensionsLengthEquals()
    {
        var container = CreateDefaultContainerAndExtensions();

        yield return null;

        Assert.AreEqual(container.SystemList.AuthorizedSystems.Count, _targetExtensionTypes.Length);
    }

    [UnityTest]
    public IEnumerator WhenGameLoaded_InitExtensions_ThenAllExtensionsInitialized()
    {
        var container = CreateDefaultContainerAndExtensions();

        yield return null;

        foreach (var t in _targetExtensionTypes)
            Assert.IsTrue(container.SystemList[t].Initialized);
    }

    [UnityTest]
    public IEnumerator WhenGameLoaded_InitExtensions_ThenPriorityEquals()
    {
        var container = CreateDefaultContainerAndExtensions();

        yield return null;

        var extensionPlaceIndexes = new Dictionary<Type, int>();
        var items = container.SystemList.ActivatedSystems;
        for (var i = 0; i < items.Count; i++)
            extensionPlaceIndexes.Add(items[i].SystemType, i);

        Assert.Greater(extensionPlaceIndexes[typeof(TestSystem_1_1)],
            extensionPlaceIndexes[typeof(TestSystem_1)]);
        Assert.Greater(extensionPlaceIndexes[typeof(TestSystem_1_2)],
            extensionPlaceIndexes[typeof(TestSystem_1_1)]);
    }

    private HGSystemContainer CreateDefaultContainerAndExtensions()
    {
        var container = new GameObject().AddComponent<TestSystemContainer>();
        container.gameObject.AddComponent<TestSystem_1_2>();
        container.gameObject.AddComponent<TestSystem_3>();
        container.gameObject.AddComponent<TestSystem_1>();
        container.gameObject.AddComponent<TestSystem_2>();
        container.gameObject.AddComponent<TestSystem_1_1>();

        var extensionInChildren = new GameObject();
        extensionInChildren.SetActive(false);
        extensionInChildren.AddComponent<TestSystemInChildren>();
        extensionInChildren.transform.SetParent(container.transform);
        extensionInChildren.SetActive(true);

        return container;
    }
}