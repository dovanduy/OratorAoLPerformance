using OratorCommonUtilities;
using OratorCommonUtilities.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TestControls
{
    public class TestLogDataViewModel : ViewModelBase
    {

        private RequestCommand _testDataGridCommand;

        public RequestCommand TestDataGridCommand
        {
            get { return _testListViewCommand ?? new RequestCommand(param => this.StartTestDataGrid()); }
        }


        private RequestCommand _testListBoxCommand;

        public RequestCommand TestListBoxCommand
        {
            get { return _testListViewCommand ?? new RequestCommand(param => this.StartTestListBox()); }
        }

        private RequestCommand _testListViewCommand;

        public RequestCommand TestListViewCommand
        {
            get { return _testListViewCommand ?? new RequestCommand(param => this.StartTestListView()); }
        }


        private RequestCommand _stopCommand;

        public RequestCommand StopCommand
        {
            get { return _stopCommand ?? new RequestCommand(param => this.Stop()); }
        }

        private RequestCommand _setVerticalValueCommand;

        public RequestCommand SetVerticalValueCommand
        {
            get { return _setVerticalValueCommand ?? new RequestCommand(param => this.SetVerticalValue()); }
        }

        private RequestCommand _weakReferenceCommand;
        public ICommand WeakReferenceCommand
        {
            get { return _weakReferenceCommand ?? new RequestCommand(param => this.TestWeakReference()); }
        }

        public string DisplayName { get; set; }

        private ObservableCollection<TestLogData> _logCollectionForDataGrid;
        public ObservableCollection<TestLogData> LogCollectionForDataGrid
        {
            get { return _logCollectionForDataGrid; }
            private set
            {
                _logCollectionForDataGrid = value;
                OnPropertyChanged("LogCollectionForDataGrid");
            }
        }

        private ObservableCollection<TestLogData> _logCollectionForListBox;
        public ObservableCollection<TestLogData> LogCollectionForListBox
        {
            get { return _logCollectionForListBox; }
            private set
            {
                _logCollectionForListBox = value;
                OnPropertyChanged("LogCollectionForListBox");
            }
        }

        private ObservableCollection<TestLogData> _logCollectionForListView;
        public ObservableCollection<TestLogData> LogCollectionForListView
        {
            get { return _logCollectionForListView; }
            private set
            {
                _logCollectionForListView = value;
                OnPropertyChanged("LogCollectionForListView");
            }
        }


        private TestLogData _selectedItemForDataGrid;

        public TestLogData SelectedItemForDataGrid
        {
            get { return _selectedItemForDataGrid; }
            set
            {
                _selectedItemForDataGrid = value;
                OnPropertyChanged("SelectedItemForDataGrid");
            }
        }

        private TestLogData _selectedItemForListBox;

        public TestLogData SelectedItemForListBox
        {
            get { return _selectedItemForListBox; }
            set
            {
                _selectedItemForListBox = value;
                OnPropertyChanged("SelectedItemForListBox");
            }
        }

        private TestLogData _selectedItemForListView;

        public TestLogData SelectedItemForListView
        {
            get { return _selectedItemForListView; }
            set
            {
                _selectedItemForListView = value;
                OnPropertyChanged("SelectedItemForListView");
            }
        }
        private double _verticalViewportSize;

        public double VerticalViewportSize
        {
            get { return _verticalViewportSize; }
            set
            {
                _verticalViewportSize = value;
                OnPropertyChanged("VerticalViewportSize");
            }
        }
        private double _verticalValue;
        public double VerticalValue
        {
            get { return _verticalValue; }
            set
            {
                _verticalValue = value;
                OnPropertyChanged("VerticalValue");
            }
        }

        public TestLogDataViewModel()
        {
            DisplayName = "Logger";
            LogCollectionForDataGrid = new ObservableCollection<TestLogData>();
            LogCollectionForListBox = new ObservableCollection<TestLogData>();
            LogCollectionForListView = new ObservableCollection<TestLogData>();
        }

        private bool IsTestDataGrid { get; set; }
        private void StartTestDataGrid()
        {
            IsTestDataGrid = false;
            Random ran = new Random();
            int[] seeds = new int[] { 1, 2, 4, 8, 16 };
            Thread ts = new Thread(new ThreadStart(() =>
            {
                while (!IsTestDataGrid)
                {
                    TestLogData l = new TestLogData()
                    {
                        Data = "12345567890",
                        Tag = "Testing",
                        LocalTimestamp = DateTime.Now,
                        SourceBuffer = (LogBuffer)seeds[ran.Next(4)]
                    };

                    ReceivedLogDataForDataGrid(l);
                    Thread.Sleep(100);
                }
            }));
            ts.Start();
        }


        private void StartTestListBox()
        {
            Random ran = new Random();
            int[] seeds = new int[] { 1, 2, 4, 8, 16 };
            Thread ts = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    TestLogData l = new TestLogData()
                    {
                        Data = "12345567890",
                        Tag = "Testing",
                        LocalTimestamp = DateTime.Now,
                        SourceBuffer = (LogBuffer)seeds[ran.Next(4)]
                    };

                    ReceivedLogDataForListBox(l);
                    Thread.Sleep(100);
                }
            }));
            ts.Start();
        }

        private void StartTestListView()
        {
            Random ran = new Random();
            int[] seeds = new int[] { 1,2,4,8,16};
            Thread ts = new Thread(new ThreadStart(() => {
                while (true)
                {
                    TestLogData l = new TestLogData()
                    { 
                       Data = "12345567890",
                       Tag = "Testing",
                       LocalTimestamp = DateTime.Now,
                       SourceBuffer = (LogBuffer)seeds[ran.Next(4)]
                    };

                    ReceivedLogDataForListView(l);
                    Thread.Sleep(100);
                }
            }));
            ts.Start();
        }

        private void Stop()
        {
            IsTestDataGrid = true;
        }

        public void ReceivedLogDataForDataGrid(TestLogData log)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                LogCollectionForDataGrid.Add(log);
                SelectedItemForDataGrid = LogCollectionForDataGrid[LogCollectionForDataGrid.Count - 1];
            });
        }

        public void ReceivedLogDataForListBox(TestLogData log)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                LogCollectionForListBox.Add(log);
                SelectedItemForListBox = LogCollectionForListBox[LogCollectionForListBox.Count - 1];
            });
        }

        private void SetVerticalValue()
        {
            VerticalValue = 20;
        }


        public void ReceivedLogDataForListView(TestLogData log)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                LogCollectionForListView.Add(log);
                SelectedItemForListView = LogCollectionForListView[LogCollectionForListView.Count - 1];
            });
        }

        public void ClearLogData()
        {
            if (LogCollectionForDataGrid != null)
            {
                LogCollectionForDataGrid.Clear();
            }
        }

        public class TestWeakReferenceCache
        {
            public int Count{get;set;}
        }

        private void TestWeakReference()
        {
            TestWeakReferenceCache abc = new TestWeakReferenceCache();
            WeakReference testWI = new WeakReference(abc);
            string res = string.Empty;
            int count = 0;

           

            while (testWI.IsAlive)
            {
                TestWeakReferenceCache abo = testWI.Target as TestWeakReferenceCache;
                if (abo == null)
                {
                    Console.WriteLine("reference is null");
                }
                else
                {
                    count++;
                    if (count % 10 == 0)
                    {
                        GC.Collect();
                        Console.WriteLine("GC invokes");
                    }
                }
            }
            Console.WriteLine("Test done");
        }
    }
}
