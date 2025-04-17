using DrillingCore.Application.DTOs;
using DrillingCore.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.Report.Queries
{
    public class GetTimesheetPreviewQueryHandler : IRequestHandler<GetTimesheetPreviewQuery, List<TimesheetDayDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IReportRepository _reportRepository;

        public GetTimesheetPreviewQueryHandler(IUserRepository userRepository, IReportRepository reportRepository)
        {
            _userRepository = userRepository;
            _reportRepository = reportRepository;
        }

        public async Task<List<TimesheetDayDto>> Handle(GetTimesheetPreviewQuery request, CancellationToken cancellationToken)
        {
            var signedForms = await _reportRepository.GetSignedFormsForUserAsync(request.UserId, request.FromDate, request.ToDate, cancellationToken);
            var drillingForms = await _reportRepository.GetDrillingFormsWithUserAsync(request.UserId, request.FromDate, request.ToDate, cancellationToken);

            var dailyData = new List<TimesheetDayDto>();

            for (var date = request.FromDate; date <= request.ToDate; date = date.AddDays(1))
            {
                var isSignedOnDate = signedForms.Any(f => f.DateFilled == date);
                var drillingMeters = drillingForms
                    .Where(d => d.ProjectForm.DateFilled == date)
                    .Sum(d => d.TotalMeters);

                var participant = await _reportRepository.GetActiveParticipantForUserAsync(request.UserId, date, cancellationToken);

                var dayRate = participant?.DailyRate ?? 0;
                var meterRate = participant?.MeterRate ?? 0;

                dailyData.Add(new TimesheetDayDto
                {
                    Date = date,
                    Worked = isSignedOnDate,
                    DayRate = isSignedOnDate ? dayRate : 0,
                    MeterRate = isSignedOnDate ? meterRate : 0,
                    Meters = isSignedOnDate ? drillingMeters : 0,
                    Narrative = signedForms.FirstOrDefault(f => f.DateFilled == date)?.OtherComments ?? ""
                });
            }

            return dailyData;
        }
    }
}
