using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ExcelDataReader;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;

namespace HCGStudio.HITScheduleMasterCore
{
    /// <summary>
    ///     课表中的学期
    /// </summary>
    public enum Semester
    {
        /// <summary>
        /// 春季学期
        /// </summary>
        Spring = 0,

        /// <summary>
        /// 秋季学期
        /// </summary>
        Autumn = 2,

        /// <summary>
        /// 夏季学期
        /// </summary>
        Summer = 1
    }

    /// <summary>
    ///     课表实例
    /// </summary>
    public class Schedule
    {
        private readonly List<ScheduleEntry> _entries = new List<ScheduleEntry>();


        /// <summary>
        ///     指定年份和学期创建空的课表
        /// </summary>
        /// <param name="year">要创建课表的年份</param>
        /// <param name="semester">要创建课表的学期</param>
        public Schedule(int year, Semester semester)
        {
            Year = year;
            Semester = semester;
        }

        /// <summary>
        ///     创建空的课表，请不要单独使用这个API，会导致年份与学期无法更改
        /// </summary>
        public Schedule()
        {
        }

        private static DateTime[] SemesterStarts => new[]
        {
            new DateTime(2020, 02, 24),
            new DateTime(2020, 06, 29),
            new DateTime(2020, 08, 31),
            new DateTime(2021, 02, 22),
            new DateTime(2021, 06, 28)
        };

        /// <summary>
        ///     获取指定的课表条目
        /// </summary>
        /// <param name="index">课表条目的索引</param>
        /// <returns>实例中储存的课表条目实例</returns>
        public ScheduleEntry this[int index] => _entries[index];

        /// <summary>
        ///     当前课表中所有的条目
        /// </summary>
        public List<ScheduleEntry> Entries => new List<ScheduleEntry>(_entries);

        /// <summary>
        ///     当前课表中条目的数量
        /// </summary>
        public int Count => _entries.Count;

        /// <summary>
        ///     课表的年份
        /// </summary>
        public int Year { get; }

        /// <summary>
        ///     课表学期开始的时间
        /// </summary>
        public DateTime SemesterStart => SemesterStarts[Year - 2020 + (int) Semester];

        /// <summary>
        ///     课表的学期
        /// </summary>
        public Semester Semester { get; }

        /// <summary>
        ///     从已经打打开的流中读取并创建课表，将会在下个版本移除，请使用<see cref="LoadFromXlsStream"/>。
        /// </summary>
        /// 
        /// <param name="inputStream">输入的流</param>
        [Obsolete]
        public static Schedule LoadFromStream(Stream inputStream)
        {
            //Fix codepage
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            //I want to say f-word here, but no idea to microsoft, mono or ExcelDataReader
            var reader = ExcelReaderFactory.CreateReader(inputStream);
            var table = reader.AsDataSet().Tables[0];
            if (!(table.Rows[0][0] is string tableHead)) throw new ArgumentException("错误的文件格式。");

            var schedule = new Schedule(int.Parse(tableHead[..4]), tableHead[4] switch
            {
                '春' => Semester.Spring,
                '夏' => Semester.Summer,
                _ => Semester.Autumn
            });

            for (var i = 0; i < 7; i++) //列
            for (var j = 0; j < 5; j++) //行
            {
                var current = table.Rows[j + 2][i + 2] as string;
                if (string.IsNullOrWhiteSpace(current))
                    continue;
                var next = table.Rows[j + 3][i + 2] as string;
                var currentCourses = current.Replace("周\n", "周").Split('\n');
                if (currentCourses.Length % 2 != 0) throw new Exception("课表格式错误。");
                for (var c = 0; c < currentCourses.Length; c += 2)
                    schedule._entries.Add(new ScheduleEntry((DayOfWeek) ((i + 1) % 7),
                        (CourseTime) (j + 1),
                        currentCourses[c], currentCourses[c + 1],
                        current == next));

                if (current == next) j++;
            }

            return schedule;
        }


        /// <summary>
        ///     从已经打打开的XLS流中读取并创建课表
        /// </summary>
        /// <param name="inputStream">输入的流</param>
        public static Schedule LoadFromXlsStream(Stream inputStream)
        {
            //Fix codepage
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            //I want to say f-word here, but no idea to microsoft, mono or ExcelDataReader
            var reader = ExcelReaderFactory.CreateReader(inputStream);
            var table = reader.AsDataSet().Tables[0];
            if (!(table.Rows[0][0] is string tableHead)) throw new ArgumentException("错误的文件格式。");

            var schedule = new Schedule(int.Parse(tableHead[..4]), tableHead[4] switch
            {
                '春' => Semester.Spring,
                '夏' => Semester.Summer,
                _ => Semester.Autumn
            });

            for (var i = 0; i < 7; i++) //列
            for (var j = 0; j < 5; j++) //行
            {
                var current = table.Rows[j + 2][i + 2] as string;
                if (string.IsNullOrWhiteSpace(current))
                    continue;
                var next = table.Rows[j + 3][i + 2] as string;
                var currentCourses = current.Replace("周\n", "周").Split('\n');
                if (currentCourses.Length % 2 != 0) throw new Exception("课表格式错误。");
                for (var c = 0; c < currentCourses.Length; c += 2)
                    schedule._entries.Add(new ScheduleEntry((DayOfWeek) ((i + 1) % 7),
                        (CourseTime) (j + 1),
                        currentCourses[c], currentCourses[c + 1],
                        current == next));

                if (current == next) j++;
            }

            return schedule;
        }

        /// <summary>
        ///     从已经打打开的CSV流中读取并创建课表
        /// </summary>
        /// <param name="inputStream">输入的流</param>
        public static Schedule LoadFromCsvStream(Stream inputStream)
        {
            //Fix codepage
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            //I want to say f-word here, but no idea to microsoft, mono or ExcelDataReader

            var reader = ExcelReaderFactory.CreateCsvReader(inputStream);
            var table = reader.AsDataSet().Tables[0];
            if (!(table.Rows[0][0] is string tableHead)) throw new ArgumentException("错误的文件格式。");

            var schedule = new Schedule(int.Parse(tableHead[..4]), tableHead[4] switch
            {
                '春' => Semester.Spring,
                '夏' => Semester.Summer,
                _ => Semester.Autumn
            });

            for (var i = 0; i < 7; i++) //列
            for (var j = 0; j < 5; j++) //行
            {
                var current = table.Rows[j + 2][i + 2] as string;
                if (string.IsNullOrWhiteSpace(current))
                    continue;
                var next = table.Rows[j + 3][i + 2] as string;
                var currentCourses = current.Replace("周\n", "周").Split('\n');
                if (currentCourses.Length % 2 != 0) throw new Exception("课表格式错误。");
                for (var c = 0; c < currentCourses.Length; c += 2)
                    schedule._entries.Add(new ScheduleEntry((DayOfWeek) ((i + 1) % 7),
                        (CourseTime) (j + 1),
                        currentCourses[c], currentCourses[c + 1],
                        current == next));

                if (current == next) j++;
            }

            return schedule;
        }

        /// <summary>
        ///     向课表中添加条目
        /// </summary>
        /// <param name="scheduleEntry">要添加的条目</param>
        public void AddScheduleEntry(ScheduleEntry scheduleEntry)
        {
            _entries.Add(scheduleEntry);
        }

        /// <summary>
        ///     移除指定的条目
        /// </summary>
        /// <param name="index">条目的索引</param>
        public void RemoveAt(int index)
        {
            _entries.RemoveAt(index);
        }

        /// <summary>
        ///     将当前课表实例转化为日历
        /// </summary>
        /// <returns>表示当前课表的日历实例</returns>
        public Calendar GetCalendar()
        {
            var calendar = new Calendar();
            calendar.AddTimeZone(new VTimeZone("Asia/Shanghai"));
            foreach (var entry in _entries)
            {
                var i = 0;
                var dayOfWeek = entry.DayOfWeek == DayOfWeek.Sunday ? 6 : (int) entry.DayOfWeek - 1;
                for (var w = entry.Week >> 1; w != 0; w >>= 1, i++)
                {
                    if ((w & 1) != 1) continue;
                    var cEvent = new CalendarEvent
                    {
                        Location = entry.Location,
                        Start = new CalDateTime(
                            SemesterStart.AddDays(i * 7 + dayOfWeek) + entry.StartTime),
                        Duration = entry.Length,
                        Summary = $"{entry.CourseName} by {entry.Teacher} at {entry.Location}"
                    };
                    cEvent.Alarms.Add(new Alarm
                    {
                        Summary = $"您在{entry.Location}有一节{entry.CourseName}课程即将开始。",
                        Action = AlarmAction.Display,
                        Trigger = new Trigger(TimeSpan.FromMinutes(-25))
                    });
                    calendar.Events.Add(cEvent);
                }
            }

            //var sem = Semester switch
            //{
            //    Semester.Autumn => "秋",
            //    Semester.Summer => "夏",
            //    _ => "春"
            //};
            //calendar.Name = $"{Year}{sem}课程表";
            return calendar;
        }
    }
}