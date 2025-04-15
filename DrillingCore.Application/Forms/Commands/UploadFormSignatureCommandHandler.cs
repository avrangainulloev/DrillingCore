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
    public class UploadFormSignatureCommandHandler : IRequestHandler<UploadFormSignatureCommand, int>
    {
        private readonly IFormRepository _repository;
        private readonly IFileStorageService _fileStorage;

        public UploadFormSignatureCommandHandler(IFormRepository repository, IFileStorageService fileStorage)
        {
            _repository = repository;
            _fileStorage = fileStorage;
        }

        public async Task<int> Handle(UploadFormSignatureCommand request, CancellationToken cancellationToken)
        {
            var fileUrl = await _fileStorage.SaveFileAsync(request.File, "signatures");

            var signature = new FormSignature
            {
                ProjectFormId = request.ProjectFormId,
                ParticipantId = request.ParticipantId,
                SignatureUrl = fileUrl,
                CreatedDate = DateTime.UtcNow
            };

            await _repository.SaveSignatureAsync(signature,cancellationToken);
            return signature.Id;
        }
    }
}
