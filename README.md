# HIT Schedule Master Core

![GitHub Workflow Status](https://img.shields.io/github/workflow/status/HCGStudio/HIT-Schedule-Master-Core/publish%20to%20nuget?label=publish) ![GitHub Workflow Status](https://img.shields.io/github/workflow/status/HCGStudio/HIT-Schedule-Master-Core/test) ![GitHub issues](https://img.shields.io/github/issues/HCGStudio/HIT-Schedule-Master-Core) ![GitHub tag (latest by date)](https://img.shields.io/github/v/tag/HCGStudio/HIT-Schedule-Master-Core) ![GitHub](https://img.shields.io/github/license/HCGStudio/HIT-Schedule-Master-Core) ![Nuget](https://img.shields.io/nuget/dt/HCGStudio.HITScheduleMasterCore) ![Codecov](https://img.shields.io/codecov/c/gh/HCGStudio/HIT-Schedule-Master-Core)

HIT课表大师的核心库，帮助转化XLS或者CSV格式的课表！目前版本已经可以较稳定使用！下载请到nuget，附带XML注释。

快速开始：

``` C#
using var fs = File.OpenRead(path);
var schedule = Schedule.LoadFromXlsStream(fs);
var cal = Schedule.GetCalendar();
var str = new CalendarSerializer().SerializeToString(cal);
```

对于导入的课表，本库也提供了编辑的方法。

[View at Nuget.org](https://www.nuget.org/packages/HCGStudio.HITScheduleMasterCore/)
