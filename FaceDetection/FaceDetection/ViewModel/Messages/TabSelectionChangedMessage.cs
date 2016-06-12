namespace FaceDetection.ViewModel.Messages
{
    class TabSelectionChangedMessage
    {
        public int Index { get; }
        public TabSelectionChangedMessage(int index)
        {
            Index = index;
        }
    }
}
