using System;
using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using UImGui;
using Unity.Profiling;
using UnityEngine;
using UnityWebBrowser.Core;
using Resolution = UnityWebBrowser.Shared.Resolution;

namespace UnityWebBrowser.Prj
{
    public class UWBPrjDebugUI : MonoBehaviour
    {
        public WebBrowserUIBasic webBrowserUIBasic;
        public Resolution[] resolutions;
        public float refreshRate = 1f;

        private string[] resolutionsText;
        private int selectedIndex;
        private int lastSelectedIndex;

        private ProfilerRecorder getPixelsMarker;
        private ProfilerRecorder applyTextureMarker;

        private float timer;
        private int fps;

        private double getPixelsTime;
        private double applyTextureTime;
    
        private void Start()
        {
            if (webBrowserUIBasic == null)
                throw new ArgumentNullException(nameof(webBrowserUIBasic), "Web browser UI is unassigned!");
        
            UImGuiUtility.Layout += OnImGuiLayout;

            getPixelsMarker = ProfilerRecorder.StartNew(WebBrowserClient.markerGetPixels, 15);
            applyTextureMarker = ProfilerRecorder.StartNew(WebBrowserClient.markerLoadTextureApply, 15);

            resolutionsText = new string[resolutions.Length];
            for (int i = 0; i < resolutions.Length; i++)
            {
                Resolution resolution = resolutions[i];
                resolutionsText[i] = resolution.ToString();

                if (!resolution.Equals(webBrowserUIBasic.browserClient.Resolution)) 
                    continue;
            
                selectedIndex = i;
                lastSelectedIndex = i;
            }
        }

        private void OnDestroy()
        {
            UImGuiUtility.Layout -= OnImGuiLayout;
        
            getPixelsMarker.Dispose();
            applyTextureMarker.Dispose();
        }

        private void Update()
        {
            fps = (int) (1f / Time.unscaledDeltaTime);
        
            if (!(Time.unscaledTime > timer)) return;
        
            getPixelsTime = GetRecorderFrameTimeAverage(getPixelsMarker) * 1e-6f;
            applyTextureTime = GetRecorderFrameTimeAverage(applyTextureMarker) * 1e-6f;
        
            timer = Time.unscaledTime + refreshRate;
        }

        private void OnImGuiLayout(UImGui.UImGui uImGui)
        {
            ImGui.Begin("UWB Debug UI");
            {
                ImGui.Text("UWB Debug UI");
                ImGui.Spacing();
                ImGui.Text($"FPS: {fps}");
                ImGui.Text($"Get Texture Pixels: {getPixelsTime:F1}ms");
                ImGui.Text($"Texture Apply Time: {applyTextureTime:F1}ms");
                ImGui.Spacing();

                ImGui.Text("Resolution:");
                ImGui.PushItemWidth(100);
                ImGui.ListBox("", ref selectedIndex, resolutionsText, resolutionsText.Length);
                ImGui.PopItemWidth();

            }
            ImGui.End();

            if (selectedIndex != lastSelectedIndex)
            {
                webBrowserUIBasic.browserClient.Resolution = resolutions[selectedIndex];
                lastSelectedIndex = selectedIndex;
            }
        }
    
        private double GetRecorderFrameTimeAverage(ProfilerRecorder recorder)
        {
            int samplesCount = recorder.Capacity;
            if (samplesCount == 0)
                return 0;

            List<ProfilerRecorderSample> samples = new (samplesCount);
            recorder.CopyTo(samples);
            double r = samples.Aggregate<ProfilerRecorderSample, double>(0,
                (current, sample) => current + sample.Value);
            r /= samplesCount;

            return r;
        }
    }
}
