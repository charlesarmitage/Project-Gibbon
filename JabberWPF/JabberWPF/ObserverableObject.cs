using System.ComponentModel;

namespace JabberWPF
{
    public class ObserverableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChangedEvent(string propertyName)
        {
            var callback = PropertyChanged;
            if (callback != null)
            {
                callback(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}