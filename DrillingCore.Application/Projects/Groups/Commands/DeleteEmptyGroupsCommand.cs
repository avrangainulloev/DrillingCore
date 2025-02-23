using MediatR;
using System.Collections.Generic;

namespace DrillingCore.Application.Groups.Commands
{
    public class DeleteEmptyGroupsCommand : IRequest<Unit>
    {
        public int ProjectId { get; set; }
        public List<string> Groups { get; set; } = new List<string>();
    }
}
