using DrillingCore.Application.DTOs;
using DrillingCore.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.Report.Commands
{
    public class GenerateTimesheetReportCommandHandler : IRequestHandler<GenerateTimesheetReportCommand, byte[]>
    {
        private readonly IUserRepository _userRepository;
        private readonly IReportRepository _reportRepository;
        private readonly IExcelReportBuilder _reportBuilder;

        public GenerateTimesheetReportCommandHandler(
            IUserRepository userRepository,
            IReportRepository reportRepository,
            IExcelReportBuilder reportBuilder)
        {
            _userRepository = userRepository;
            _reportRepository = reportRepository;
            _reportBuilder = reportBuilder;
        }

        public async Task<byte[]> Handle(GenerateTimesheetReportCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null) throw new Exception("User not found");

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

            return _reportBuilder.BuildTimesheetExcel(user, request.FromDate, request.ToDate, dailyData);
        }
    }
}
