using DrillingCore.Application.Interfaces;
using DrillingCore.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Application.Forms.Commands
{
    public class UploadFormPhotoCommandHandler : IRequestHandler<UploadFormPhotoCommand, int>
    {
        private readonly IFormRepository _repository;
        private readonly IFileStorageService _fileStorage;

        public UploadFormPhotoCommandHandler(IFormRepository repository, IFileStorageService fileStorage)
        {
            _repository = repository;
            _fileStorage = fileStorage;
        }

        public async Task<int> Handle(UploadFormPhotoCommand request, CancellationToken cancellationToken)
        {
            var fileUrl = await _fileStorage.SaveFileAsync(request.Photo, "photos");

            var photo = new FormPhoto
            {
                ProjectFormId = request.ProjectFormId,
                PhotoUrl = fileUrl,
                CreatedDate = DateTime.UtcNow

            };

            await _repository.SavePhotoAsync(photo);
            return photo.Id;
        }
    }
}
