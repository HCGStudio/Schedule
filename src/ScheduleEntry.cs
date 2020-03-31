using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace HCGStudio.HITScheduleMasterCore
{
    /// <summary>
    ///     课程的开始时间
    /// </summary>
    public enum CourseTime
    {
        Noon = 0,
        C12 = 1,
        C34 = 2,
        C56 = 3,
        C78 = 4,
        C9A = 5
    }

    /// <summary>
    ///     课表条目
    /// </summary>
    public class ScheduleEntry
    {
        /// <summary>
        ///     创建一个空的课表条目实例
        /// </summary>
        public ScheduleEntry()
        {
        }

        /// <summary>
        ///     创造一个课表条目实例
        /// </summary>
        /// <param name="dayOfWeek">课程在一周中所在的日子</param>
        /// <param name="courseTime">课程开始的时间</param>
        /// <param name="courseName">课程的名称</param>
        /// <param name="scheduleExpression">课程的描述</param>
        /// <param name="isLongCourse">是否是长课</param>
        public ScheduleEntry(DayOfWeek dayOfWeek, CourseTime courseTime, string courseName, string scheduleExpression,
            bool isLongCourse = false)
        {
            CourseName = courseName;
            Teacher = scheduleExpression[..scheduleExpression.IndexOf('[')];
            Week = ParseWeek(
                scheduleExpression[(1 + scheduleExpression.IndexOf('['))..scheduleExpression.LastIndexOf(']')]
            );
            var location = scheduleExpression[scheduleExpression.LastIndexOf('周')..];
            Location = location.Length == 1 ? "待定地点" : location[1..];
            CourseTime = courseTime;
            DayOfWeek = dayOfWeek;
            IsLongCourse = isLongCourse;
            Length = !IsLongCourse
                ? new TimeSpan(1, 45, 00)
                : new TimeSpan(3, 30, 00);

            StartTime = StartTimes[(int) CourseTime];
            CourseTimeName = CourseTime switch
            {
                CourseTime.C12 => "一二节",
                CourseTime.C34 => "三四节",
                CourseTime.C56 => "五六节",
                CourseTime.C78 => "七八节",
                CourseTime.C9A => "晚上",
                _ => "中午"
            };
        }

        /// <summary>
        ///     周几的数值记录
        /// </summary>
        public DayOfWeek DayOfWeek { get; set; }

        /// <summary>
        ///     周几
        /// </summary>
        public string DayOfWeekName => DayOfWeek switch
        {
            DayOfWeek.Monday => "周一",
            DayOfWeek.Tuesday => "周二",
            DayOfWeek.Wednesday => "周三",
            DayOfWeek.Thursday => "周四",
            DayOfWeek.Friday => "周五",
            DayOfWeek.Saturday => "周六",
            _ => "周日"
        };

        /// <summary>
        ///     是否是三节课长度的课
        /// </summary>
        public bool IsLongCourse { get; }

        /// <summary>
        ///     课程时间
        /// </summary>
        public CourseTime CourseTime { get; }

        /// <summary>
        ///     授课教师
        /// </summary>
        public string Teacher { get; set; }

        /// <summary>
        ///     课程详细描述
        /// </summary>
        public string WeekExpression { get; private set; }

        /// <summary>
        /// 课程名称
        /// </summary>
        public string CourseName { get; set; }

        /// <summary>
        ///     上课的位置
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        ///     最大周数
        /// </summary>
        public int MaxWeek { get; set; }

        /// <summary>
        ///     状压储存的周数。i位为1表示此周有课。
        /// </summary>
        public uint Week { get; set; }

        /// <summary>
        ///     课程的长度
        /// </summary>
        public TimeSpan Length { get; set; }

        private static TimeSpan[] StartTimes => new[]
        {
            new TimeSpan(12, 30, 00),
            new TimeSpan(08, 00, 00),
            new TimeSpan(10, 00, 00),
            new TimeSpan(13, 45, 00),
            new TimeSpan(15, 45, 00),
            new TimeSpan(18, 30, 00)
        };

        /// <summary>
        ///     课程开始的时间距离0点的时长
        /// </summary>
        public TimeSpan StartTime { get; set; }

        /// <summary>
        ///     时间段的汉字名称
        /// </summary>
        public string CourseTimeName { get; }

        /// <summary>
        ///     从周数的表达式中更改周数
        /// </summary>
        /// <param name="weekExpression">周数的表达式</param>
        public void ChangeWeek(string weekExpression)
        {
            Week = ParseWeek(weekExpression);
        }

        /// <summary>
        ///     从周数的表达式中获取周数
        /// </summary>
        /// <param name="weekExpression">周数的表达式</param>
        /// <returns>周数</returns>
        private uint ParseWeek(string weekExpression)
        {
            var week = 0u;
            WeekExpression = weekExpression
                .Replace(", ", "|") //英文逗号+空格
                .Replace("，", "|") //中文逗号
                .Replace(" ", "|"); //手动输入的空格
            var subWeekExpression = WeekExpression.Split("周|[".ToCharArray());
            foreach (var s in subWeekExpression)
            {
                var singleWeek = !s.Contains("双");
                var doubleWeek = !s.Contains("单");
                var expressions = s.Split('|');

                foreach (var expression in expressions)
                {
                    var weekRange = (
                        from Match w in Regex.Matches(expression, @"\d+")
                        select int.Parse(w.Value)
                    ).ToList();
                    if (weekRange.Count == 0) continue;
                    if (weekRange.Count == 1)
                        week |= 1u << weekRange[0];
                    else
                        for (var i = weekRange[0]; i <= weekRange[1]; i++)
                            if ((i & 1) == 1)
                            {
                                if (singleWeek) week |= 1u << i;
                            }
                            else
                            {
                                if (doubleWeek) week |= 1u << i;
                            }
                }
            }

            var maxWeek = 0;
            for (var weekCpy = week; weekCpy != 0; maxWeek++, weekCpy >>= 1)
            {
            }

            maxWeek--;
            if (maxWeek > MaxWeek) MaxWeek = maxWeek;

            Week = week;
            return week;
        }
    }
}