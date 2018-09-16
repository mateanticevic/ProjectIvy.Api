﻿using ProjectIvy.Model.Binding.Web;
using ProjectIvy.Model.View.Web;
using ProjectIvy.Model.View;
using System.Collections.Generic;

namespace ProjectIvy.BL.Handlers.Web
{
    public interface IWebHandler
    {
        IEnumerable<WebTime> GetTimeSummed(WebTimeGetPagedBinding binding);

        int GetTimeSum(WebTimeGetBinding binding);

        IEnumerable<TimeByDay> GetTimeTotalByDay(WebTimeGetBinding binding);

        IEnumerable<GroupedByMonth<int>> GetTimeTotalByMonth(WebTimeGetBinding binding);

        IEnumerable<GroupedByYear<int>> GetTimeTotalByYear(WebTimeGetBinding binding);
    }
}
