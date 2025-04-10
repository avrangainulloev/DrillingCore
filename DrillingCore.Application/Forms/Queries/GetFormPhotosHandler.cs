using DrillingCore.Application.DTOs;
using DrillingCore.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.Forms.Queries
{
    public class GetFormPhotosHandler : IRequestHandler<GetFormPhotosQuery, List<FormPhotoDto>>
    {
        private readonly IFormRepository _repository;

        public GetFormPhotosHandler(IFormRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<FormPhotoDto>> Handle(GetFormPhotosQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetFormPhotosAsync(request.FormId);
        }
    }
}
