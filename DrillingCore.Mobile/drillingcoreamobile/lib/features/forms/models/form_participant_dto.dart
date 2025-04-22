class FormParticipantDto {
  final int participantId;

  FormParticipantDto({required this.participantId});

  factory FormParticipantDto.fromJson(Map<String, dynamic> json) {
    return FormParticipantDto(
      participantId: json['participantId'],
    );
  }

    Map<String, dynamic> toJson() => {
        'participantId': participantId,
      };
}

