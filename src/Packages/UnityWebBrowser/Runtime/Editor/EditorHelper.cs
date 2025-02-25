using System;
using System.Collections.Generic;
using UnityEditor;
using UnityWebBrowser.Shared.Core;

#if UNITY_EDITOR

namespace UnityWebBrowser.Editor
{
    public static class EditorHelper
    {
        public static List<T> FindAssetsByType<T>() where T : UnityEngine.Object
        {
            List<T> assets = new List<T>();
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T)}");
            for( int i = 0; i < guids.Length; i++ )
            {
                string assetPath = AssetDatabase.GUIDToAssetPath( guids[i] );
                T asset = AssetDatabase.LoadAssetAtPath<T>( assetPath );
                if( asset != null )
                {
                    assets.Add(asset);
                }
            }
            return assets;
        }

        public static Platform UnityBuildTargetToPlatform(this BuildTarget buildTarget)
        {
            switch (buildTarget)
            {
                case BuildTarget.StandaloneLinux64:
                    return Platform.Linux64;
                case BuildTarget.StandaloneWindows64:
                    return Platform.Windows64;
                case BuildTarget.StandaloneOSX:
                    return Platform.MacOS;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}

#endif