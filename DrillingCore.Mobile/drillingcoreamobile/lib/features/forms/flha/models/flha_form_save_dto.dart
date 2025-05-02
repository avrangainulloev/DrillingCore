class FLHAFormSaveDto {
  final String crewName;
  final int projectId;
  final int formTypeId;
  final String dateFilled;
  final String taskDescription;
  final String? otherComments;
  final int? creatorId;
  final List<FLHAHazardEntryDto> hazards;
  final List<ParticipantSignatureDto> participants;

  FLHAFormSaveDto({
    required this.crewName,
    required this.projectId,
    required this.formTypeId,
    required this.dateFilled,
    required this.taskDescription,
    this.otherComments,
    required this.creatorId,
    required this.hazards,
    required this.participants,
  });

  Map<String, dynamic> toJson() => {
        'crewName': crewName,
        'projectId': projectId,
        'formTypeId': formTypeId,
        'dateFilled': dateFilled,
        'taskDescription': taskDescription,
        'otherComments': otherComments,
        'creatorId': creatorId,
        'hazards': hazards.map((h) => h.toJson()).toList(),
        'participants': participants.map((p) => p.toJson()).toList(),
      };
}

class FLHAHazardEntryDto {
  final String hazardText;
  final String controlMeasures;
  final int? hazardTemplateId;

  FLHAHazardEntryDto({
    required this.hazardText,
    required this.controlMeasures,
    this.hazardTemplateId,
  });

  Map<String, dynamic> toJson() => {
        'hazardText': hazardText,
        'controlMeasures': controlMeasures,
        'hazardTemplateId': hazardTemplateId,
      };
}

class ParticipantSignatureDto {
  final int participantId;
  final String? signatureBase64;

  ParticipantSignatureDto({
    required this.participantId,
    this.signatureBase64,
  });

  Map<String, dynamic> toJson() => {
        'participantId': participantId,
        'signatureBase64': signatureBase64,
      };
}
