using DrillingCore.Application.DTOs;
using DrillingCore.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.Interfaces
{
    public interface IExcelReportBuilder
    {
        byte[] BuildTimesheetExcel(User user, DateOnly fromDate, DateOnly toDate, List<TimesheetDayDto> data);
    }
}
