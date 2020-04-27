using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HCGStudio.HITScheduleMasterCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HITScheduleMasterCore.Test
{
    [TestClass]
    public class ScheduleTest
    {
        [TestMethod]
        public void TestXlsImport()
        {
            var schedule = Schedule.LoadFromXlsStream(new FileStream("张三课表.xls", FileMode.Open));
            var cal = schedule.GetCalendar();
            _ = cal.ToString();
        }

        [TestMethod]
        public void TestCsvImport()
        {
            var schedule = Schedule.LoadFromCsvStream(new FileStream("张三课表.csv", FileMode.Open));
            var cal = schedule.GetCalendar();
            _ = cal.ToString();
        }
    }
}
