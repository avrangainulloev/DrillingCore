class FormSignatureDto {
  final int participantId;
  final String signatureUrl;

  FormSignatureDto({required this.participantId, required this.signatureUrl});

  factory FormSignatureDto.fromJson(Map<String, dynamic> json) {
    return FormSignatureDto(
      participantId: json['participantId'],
      signatureUrl: json['signatureUrl'],
    );
  }
}