import 'package:drillingcoreamobile/features/forms/data/dtos/checklist_response_dto.dart';
import 'package:drillingcoreamobile/features/forms/data/dtos/form_participant_dto.dart';

class DrillInspectionFormSaveDto {
  final int? formId;
  final int projectId;
  final int formTypeId;
  final String dateFilled;
  final String crewName;
  final String unitNumber;
  final List<FormParticipantDto> participants;
  final List<ChecklistResponseDto> checklistResponses;
  final String otherComments;
  final int creatorId;

  DrillInspectionFormSaveDto({
    this.formId,
    required this.projectId,
    required this.formTypeId,
    required this.dateFilled,
    required this.crewName,
    required this.unitNumber,
    required this.participants,
    required this.checklistResponses,
    required this.otherComments,
    required this.creatorId
  });

  Map<String, dynamic> toJson() => {
        'formId': formId,
        'projectId': projectId,
        'formTypeId': formTypeId,
        'dateFilled': dateFilled,
        'crewName': crewName,
        'unitNumber': unitNumber,
        'participants': participants.map((p) => p.toJson()).toList(),
        'checklistResponses': checklistResponses.map((c) => c.toJson()).toList(),
        'otherComments': otherComments,
        'creatorId': creatorId,
      };
}

// class FormParticipantDto {
//   final int participantId;

//   FormParticipantDto({required this.participantId});

  // Map<String, dynamic> toJson() => {
  //       'participantId': participantId,
  //     };
// }

// class ChecklistResponseDto {
//   final int checklistItemId;
//   final bool response;

//   ChecklistResponseDto({
//     required this.checklistItemId,
//     required this.response,
//   });

  // Map<String, dynamic> toJson() => {
  //       'checklistItemId': checklistItemId,
  //       'response': response,
  //     };
// }
