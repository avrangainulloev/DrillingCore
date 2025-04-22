import 'package:drillingcoreamobile/features/forms/models/form_signature_dto.dart';
import 'package:drillingcoreamobile/features/forms/models/checklist_response_dto.dart';
import 'package:drillingcoreamobile/features/forms/models/form_participant_dto.dart';


class DrillInspectionDto {
  final int formId;
  final String crewName;
  final String unitNumber;
  final String dateFilled;
  final String? otherComments;
  final int projectId;
  final List<ChecklistResponseDto> checklistResponses;
  final List<FormParticipantDto> participants;
  final List<String> photoUrls;
  final List<FormSignatureDto> signatures;

  DrillInspectionDto({
    required this.formId,
    required this.crewName,
    required this.unitNumber,
    required this.dateFilled,
    required this.otherComments,
    required this.projectId,
    required this.checklistResponses,
    required this.participants,
    required this.photoUrls,
    required this.signatures,
  });

  factory DrillInspectionDto.fromJson(Map<String, dynamic> json) {
    return DrillInspectionDto(
      formId: json['id'],
      crewName: json['crewName'],
      unitNumber: json['unitNumber'],
      dateFilled: json['dateFilled'],
      otherComments: json['otherComments'],
      projectId: json['projectId'],
      checklistResponses: (json['checklistResponses'] as List)
          .map((e) => ChecklistResponseDto.fromJson(e))
          .toList(),
      participants: (json['participants'] as List)
          .map((e) => FormParticipantDto.fromJson(e))
          .toList(),
      photoUrls: List<String>.from(json['photoUrls']),
      signatures: (json['signatures'] as List)
          .map((e) => FormSignatureDto.fromJson(e))
          .toList(),
    );
  }
}