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
            var schedule = Schedule.LoadFromStream(new FileStream("张三课表.xls", FileMode.Open));
            var cal = schedule.GetCalendar();
        }
    }
}
