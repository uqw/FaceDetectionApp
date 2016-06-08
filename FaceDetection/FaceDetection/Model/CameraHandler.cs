﻿using System.Collections.Generic;
using System.Windows.Documents;
using DirectShowLib;
using FilterCategory = AForge.Video.DirectShow.FilterCategory;

namespace FrameDetection.Model
{
    public class CameraHandler
    {
        public List<Camera> GetAllCameras()
        {
            var cameras = new List<Camera>();

            var systemCameras = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

            var i = 0;
            foreach (var camera in systemCameras)
            {
                cameras.Add(new Camera(i, camera.Name));
                i++;
            }

            return cameras;
        }
    }
}