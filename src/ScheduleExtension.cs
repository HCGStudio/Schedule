using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Text;

namespace HCGStudio.HITScheduleMasterCore
{
    internal static class ScheduleExtension
    {
        public static string ToCultureString(this CourseTime courseTime,CultureInfo cultureInfo)
        {
            var res = new ResourceManager(typeof(ScheduleMasterString));
            return courseTime switch
            {
                CourseTime.Noon => res.GetString("中午", cultureInfo),
                CourseTime.C12 => res.GetString("一二节", cultureInfo),
                CourseTime.C34 => res.GetString("三四节", cultureInfo),
                CourseTime.C56 => res.GetString("五六节", cultureInfo),
                CourseTime.C78 => res.GetString("七八节", cultureInfo),
                CourseTime.C9A => res.GetString("晚上", cultureInfo),
                _ => throw new ArgumentOutOfRangeException(nameof(courseTime), courseTime, null)
            };
        }
    }
}
