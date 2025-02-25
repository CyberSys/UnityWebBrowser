using System.Collections.Generic;
using NUnit.Framework;
using UnityWebBrowser.Editor;

namespace UnityWebBrowser.Tests.Editor
{
    public class EditorHelperTests
    {
        [Test]
        public void FindAssetsByTypeTest()
        {
            List<EditorHelperFindAssetTestAsset> assets = EditorHelper.FindAssetsByType<EditorHelperFindAssetTestAsset>();
            Assert.AreEqual(1, assets.Count);
            
            Assert.AreEqual(69420, assets[0].value);
        }
    }
}