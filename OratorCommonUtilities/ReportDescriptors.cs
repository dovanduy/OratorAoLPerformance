using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OratorCommonUtilities
{
    public static class ReportDescriptors
    {
        public static Dictionary<string, ReportItemDescriptor> Descriptors { get; private set; }

        static ReportDescriptors()
        {
            Descriptors = new Dictionary<string, ReportItemDescriptor>();

            Descriptors.Add("FileName", new ReportItemDescriptor() 
            {
                ItemPropertyName = "FileName",
                Description = "Relative .osf file name",
                DisplayName = "File name",
                ValueType = typeof(String),
                Hide = true
            });

            Descriptors.Add("Modified", new ReportItemDescriptor()
            {
                ItemPropertyName = "Modified",
                Description = "File modification date",
                DisplayName = "File modification date",
                ValueType = typeof(DateTime)
            });
            
            Descriptors.Add("Label", new ReportItemDescriptor()
            {
                ItemPropertyName = "Label",
                Description = "User defined label",
                DisplayName = "Label",
                ValueType = typeof(String)
            });
            
            Descriptors.Add("OratorVersion", new ReportItemDescriptor()
            {
                ItemPropertyName = "OratorVersion",
                Description = "Orator version",
                DisplayName = "Orator version",
                ValueType = typeof(String)
            });
            
            Descriptors.Add("MCUSWVersion", new ReportItemDescriptor()
            {
                ItemPropertyName = "MCUSWVersion",
                Description = "MCUSW version",
                DisplayName = "MCUSW version",
                ValueType = typeof(String)
            });
            
            //Descriptors.Add("TestCases", new ReportItemDescriptor()
            //{
            //    ItemPropertyName = "TestCases",
            //    Description = "Test case",
            //    DisplayName = "Test case",
            //    ValueType = typeof(String)
            //});
            
            Descriptors.Add("ResetTerminated", new ReportItemDescriptor()
            {
                ItemPropertyName = "ResetTerminated",
                Description = "Reset terminated",
                DisplayName = "Reset terminated",
                ValueType = typeof(Boolean)
            });
            
            Descriptors.Add("FromDeviceBootup", new ReportItemDescriptor()
            {
                ItemPropertyName = "FromDeviceBootup",
                Description = "From boot",
                DisplayName = "From boot",
                ValueType = typeof(Boolean)
            });
            
            //Descriptors.Add("LogFiles", new ReportItemDescriptor()
            //{
            //    ItemPropertyName = "LogFiles",
            //    Description = "Log file",
            //    DisplayName = "Log file",
            //    ValueType = typeof(String)
            //});

            Descriptors.Add("NOSHeapPeak", new ReportItemDescriptor()
            {
                ItemPropertyName = "NOSHeapPeak",
                Description = "Test description...",
                DisplayName = "NOS Heap peak",
                Unit = "Mb",
                ValueType = typeof(Double)
            });

            Descriptors.Add("NOSHeapFragmentationMinimum", new ReportItemDescriptor()
            {
                ItemPropertyName = "NOSHeapFragmentationMinimum",
                Description = "Test description...",
                DisplayName = "NOS Heap fragmentation minimum",
                Unit = "Mb",
                ValueType = typeof(Double)
            });

            Descriptors.Add("NOSHeapFragmentationEndValue", new ReportItemDescriptor()
            {
                ItemPropertyName = "NOSHeapFragmentationEndValue",
                Description = "Test description...",
                DisplayName = "NOS Heap fragmentation end value",
                Unit = "Mb",
                ValueType = typeof(Double)
            });

            Descriptors.Add("TotalPeak", new ReportItemDescriptor()
            {
                ItemPropertyName = "TotalPeak",
                Description = "Test description...",
                DisplayName = "Total memory usage peak",
                Unit = "Mb",
                ValueType = typeof(Double)
            });

            Descriptors.Add("NOSPoolPeak", new ReportItemDescriptor()
            {
                ItemPropertyName = "NOSPoolPeak",
                Description = "Test description...",
                DisplayName = "NOS Pool peak",
                Unit = "Mb",
                ValueType = typeof(Double)
            });

            Descriptors.Add("YapasHeapPeak", new ReportItemDescriptor()
            {
                ItemPropertyName = "YapasHeapPeak",
                Description = "Test description...",
                DisplayName = "YAPAS Heap peak",
                Unit = "Mb",
                ValueType = typeof(Double)
            });

            Descriptors.Add("GraphicsCacheSizePeak", new ReportItemDescriptor()
            {
                ItemPropertyName = "GraphicsCacheSizePeak",
                Description = "Test description...",
                DisplayName = "Graphics cache peak",
                Unit = "Mb",
                ValueType = typeof(Double)
            });

            Descriptors.Add("SXCellsUsagePeak", new ReportItemDescriptor()
            {
                ItemPropertyName = "SXCellsUsagePeak",
                Description = "Test description...",
                DisplayName = "SX Cell usage peak",
                Unit = "Mb",
                ValueType = typeof(Double)
            });

            Descriptors.Add("SXCellsSizeUsagePeak", new ReportItemDescriptor()
            {
                ItemPropertyName = "SXCellsSizeUsagePeak",
                Description = "Test description...",
                DisplayName = "SX Cells size peak",
                Unit = "Mb",
                ValueType = typeof(Double)
            });

            Descriptors.Add("JavaUsagePeak", new ReportItemDescriptor()
            {
                ItemPropertyName = "JavaUsagePeak",
                Description = "Test description...",
                DisplayName = "Java memory usage peak",
                Unit = "Mb",
                ValueType = typeof(Double)
            });


            Descriptors.Add("VMMUsagePeak", new ReportItemDescriptor()
            {
                ItemPropertyName = "VMMUsagePeak",
                Description = "Test description...",
                DisplayName = "VMM memory usage peak",
                Unit = "Mb",
                ValueType = typeof(Double)
            });

            Descriptors.Add("MemoryLeaks", new ReportItemDescriptor()
            {
                ItemPropertyName = "MemoryLeaks",
                Description = "Test description...",
                DisplayName = "Memory leaks",
                ValueType = typeof(Double)
            });

            
        }
    }
}
