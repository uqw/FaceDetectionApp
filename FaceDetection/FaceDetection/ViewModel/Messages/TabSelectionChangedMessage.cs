namespace FaceDetection.ViewModel.Messages
{
    internal class TabSelectionChangedMessage
    {
        public int Index { get; }
        public TabSelectionChangedMessage(int index)
        {
            Index = index;
        }
    }
}
