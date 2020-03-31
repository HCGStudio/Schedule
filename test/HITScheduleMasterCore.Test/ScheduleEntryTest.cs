using System;
using System.Collections.Generic;
using System.Text;
using HCGStudio.HITScheduleMasterCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HITScheduleMasterCore.Test
{
    [TestClass]
    public class ScheduleEntryTest
    {
        private ScheduleEntry entry;

        [TestInitialize]
        public void TestInit()
        {
            entry = new ScheduleEntry(DayOfWeek.Monday, CourseTime.C12, "测试用课", "张三[8-15]周格物201");
        }

        [TestMethod]
        public void TestDayOfWeek()
        {
            Assert.AreEqual("周一", entry.DayOfWeekName);
            entry.DayOfWeek = DayOfWeek.Tuesday;
            Assert.AreEqual("周二", entry.DayOfWeekName);
            entry.DayOfWeek = DayOfWeek.Wednesday;
            Assert.AreEqual("周三", entry.DayOfWeekName);
            entry.DayOfWeek = DayOfWeek.Thursday;
            Assert.AreEqual("周四", entry.DayOfWeekName);
            entry.DayOfWeek = DayOfWeek.Friday;
            Assert.AreEqual("周五", entry.DayOfWeekName);
            entry.DayOfWeek = DayOfWeek.Saturday;
            Assert.AreEqual("周六", entry.DayOfWeekName);
            entry.DayOfWeek = DayOfWeek.Sunday;
            Assert.AreEqual("周日", entry.DayOfWeekName);
        }

        [TestMethod]
        public void TestWeekExpressionParse()
        {
            Assert.AreEqual("张三", entry.Teacher);
            Assert.AreEqual("格物201", entry.Location);
            Assert.AreEqual(15, entry.MaxWeek);
            Assert.AreEqual((uint)0b11111111 << 8, entry.Week);
            entry.ChangeWeek("1-3，10-13");
            Assert.AreEqual(((uint)0b1111 << 10) + 0b1110, entry.Week);
            entry = new ScheduleEntry(DayOfWeek.Monday, CourseTime.C12, "测试用课", "张三[7，11]单周格物201");
            Assert.AreEqual((uint)0b10001 << 7, entry.Week);
        }
    }
}
