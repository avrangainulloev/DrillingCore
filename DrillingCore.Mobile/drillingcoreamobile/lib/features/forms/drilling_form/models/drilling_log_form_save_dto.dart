import 'package:drillingcoreamobile/features/forms/data/dtos/form_participant_dto.dart';

class DrillingLogFormSaveDto {
  final int? formId;
  final int projectId;
  final int creatorId;
  final String dateFilled;
  final String? unitNumber;
  final String? crewName;
  final int totalWells;
  final double totalMeters;
  final String? otherComments;
  final List<FormParticipantDto> participants;

  DrillingLogFormSaveDto({
    this.formId,
    required this.projectId,
    required this.creatorId,
    required this.dateFilled,
    this.unitNumber,
    this.crewName,
    required this.totalWells,
    required this.totalMeters,
    this.otherComments,
    required this.participants,
  });

  Map<String, dynamic> toJson() => {
        if (formId != null) 'formId': formId,
        'projectId': projectId,
        'creatorId': creatorId,
        'dateFilled': dateFilled,
        'unitNumber': unitNumber,
        'crewName': crewName,
        'totalWells': totalWells,
        'totalMeters': totalMeters,
        'otherComments': otherComments,
        'participants': participants.map((p) => p.toJson()).toList(),
      };
}
