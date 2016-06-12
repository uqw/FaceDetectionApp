using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;

namespace FaceDetection.ViewModel.Helpers
{
    /// <summary>
    /// Represents a dynamic data collection that provides notifications when items get added, removed or when the whole list is refreshed. 
    /// Unlike the normal <see cref="ObservableCollection{T}"/> the <see cref="AsyncObservableCollection{T}"/> also supports asynchronous access to the list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="System.Collections.ObjectModel.ObservableCollection{T}" />
    public class AsyncObservableCollection<T>: ObservableCollection<T>
    {
        private SynchronizationContext _synchronizationContext;

        private SynchronizationContext SynchronizationContext
        {
            get
            {
                return _synchronizationContext ?? SynchronizationContext.Current;
            }

            set
            {
                if(value != null)
                    _synchronizationContext = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncObservableCollection{T}"/> class.
        /// </summary>
        public AsyncObservableCollection()
        {
            SynchronizationContext = SynchronizationContext.Current;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncObservableCollection{T}"/> class.
        /// </summary>
        /// <param name="list">The list.</param>
        public AsyncObservableCollection(IEnumerable<T> list): base(list)
        {
            SynchronizationContext = SynchronizationContext.Current;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Collections.ObjectModel.ObservableCollection`1.CollectionChanged" /> event with the provided arguments.
        /// </summary>
        /// <param name="e">Arguments of the event being raised.</param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if(SynchronizationContext.Current == SynchronizationContext)
            {
                RaiseCollectionChanged(e);
            }
            else
            {
                SynchronizationContext.Send(RaiseCollectionChanged, e);
            }
        }

        private void RaiseCollectionChanged(object param)
        {
            try
            {
                base.OnCollectionChanged((NotifyCollectionChangedEventArgs)param);
            }
            catch(NotSupportedException)
            {
                System.Windows.Application.Current.Dispatcher.BeginInvoke(
                    System.Windows.Threading.DispatcherPriority.Background,
                    new Action(() =>
                    {
                        try
                        {
                            base.OnCollectionChanged((NotifyCollectionChangedEventArgs)param);
                        }
                        catch
                        {
                            // Ignored
                        }                        
                    }
                    )
                );       
            }            
            catch(InvalidOperationException)
            {
                // Ignored
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Collections.ObjectModel.ObservableCollection`1.PropertyChanged" /> event with the provided arguments.
        /// </summary>
        /// <param name="e">Arguments of the event being raised.</param>
        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if(SynchronizationContext.Current == SynchronizationContext)
            {
                RaisePropertyChanged(e);
            }
            else
            {
                SynchronizationContext.Send(RaisePropertyChanged, e);
            }
        }

        private void RaisePropertyChanged(object param)
        {
            base.OnPropertyChanged((PropertyChangedEventArgs) param);
        }

        private void RaisePropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
        }

        /// <summary>
        /// Combines list1 with list2 and returns the modified list1
        /// </summary>
        /// <param name="list1">The first list</param>
        /// <param name="list2">The second list</param>
        /// <returns>The modified list1 instance</returns>
        public static AsyncObservableCollection<T> operator +(AsyncObservableCollection<T> list1, IList<T> list2)
        {
            foreach(var item in list2)
            {
                list1.Add(item);
            }

            return list1;
        }
    }
}
