using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameDetection.ViewModel.Messages
{
    internal class CameraSelectionChangedMessage
    {
        public int Selection { get; }

        public CameraSelectionChangedMessage(int selection)
        {
            Selection = selection;
        }
    }
}
