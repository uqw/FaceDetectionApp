using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FaceDetection.Model.Recognition;

namespace FaceDetection.ViewModel.Messages
{
    class FaceAddedMessage
    {
        public Face Face { get; }

        public FaceAddedMessage(Face face)
        {
            Face = face;
        }
    }
}
