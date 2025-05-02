import 'package:drillingcoreamobile/features/forms/inspection_forms/models/drill_inspection_form_model.dart';
import 'hazard_item_model.dart';

class FLHAInspectionFormModel {
  final int formId;
  final int projectId;
  final int formTypeId;

  final String crewName;
  final String dateFilled;
  final String taskDescription;
  final String otherComments;

  final List<ProjectParticipant> allParticipants;
  final List<int> selectedParticipantIds;
  final Map<int, String> signatures; // data:image/png;base64,...
  final Map<int, String> originalSignatures;
  final List<HazardItem> hazardTemplates;
  final List<int> selectedHazardIds;
  final List<HazardItem> customHazards;

  final List<FLHAPhoto> photos;

  FLHAInspectionFormModel({
    required this.formId,
    required this.projectId,
    required this.formTypeId,
    required this.crewName,
    required this.dateFilled,
    required this.taskDescription,
    required this.otherComments,
    required this.allParticipants,
    required this.selectedParticipantIds,
    required this.signatures,
    required this.originalSignatures,
    required this.hazardTemplates,
    required this.selectedHazardIds,
    required this.customHazards,
    required this.photos,
  });

  factory FLHAInspectionFormModel.empty() {
    return FLHAInspectionFormModel(
      formId: 0,
      projectId: 0,
      formTypeId: 3,
      crewName: '',
      dateFilled: DateTime.now().toIso8601String().split('T')[0],
      taskDescription: '',
      otherComments: '',
      allParticipants: [],
      selectedParticipantIds: [],
      signatures: {},
      originalSignatures: {},
      hazardTemplates: [],
      selectedHazardIds: [],
      customHazards: [],
      photos: [],
    );
  }

  FLHAInspectionFormModel copyWith({
    int? formId,
    int? projectId,
    int? formTypeId,
    String? crewName,
    String? dateFilled,
    String? taskDescription,
    String? otherComments,
    List<ProjectParticipant>? allParticipants,
    List<int>? selectedParticipantIds,
    Map<int, String>? signatures,
    Map<int, String>? originalSignatures,
    List<HazardItem>? hazardTemplates,
    List<int>? selectedHazardIds,
    List<HazardItem>? customHazards,
    List<FLHAPhoto>? photos,
  }) {
    return FLHAInspectionFormModel(
      formId: formId ?? this.formId,
      projectId: projectId ?? this.projectId,
      formTypeId: formTypeId ?? this.formTypeId,
      crewName: crewName ?? this.crewName,
      dateFilled: dateFilled ?? this.dateFilled,
      taskDescription: taskDescription ?? this.taskDescription,
      otherComments: otherComments ?? this.otherComments,
      allParticipants: allParticipants ?? this.allParticipants,
      selectedParticipantIds: selectedParticipantIds ?? this.selectedParticipantIds,
      signatures: signatures ?? this.signatures,
      originalSignatures: originalSignatures ?? this.originalSignatures,
      hazardTemplates: hazardTemplates ?? this.hazardTemplates,
      selectedHazardIds: selectedHazardIds ?? this.selectedHazardIds,
      customHazards: customHazards ?? this.customHazards,
      photos: photos ?? this.photos,
    );
  }

  factory FLHAInspectionFormModel.fromJson(Map<String, dynamic> json) {
    final hazardList = json['hazards'] as List<dynamic>? ?? [];
    final participantList = json['participants'] as List<dynamic>? ?? [];
    final photoUrls = json['photoUrls'] as List<dynamic>? ?? [];

    final Map<int, String> initialSignatures = {
      for (final p in participantList)
        if (p['signatureUrl'] != null && (p['signatureUrl'] as String).isNotEmpty)
          p['participantId'] as int: p['signatureUrl'] as String
    };

    final List<HazardItem> allHazards = hazardList.map<HazardItem>((h) {
      return HazardItem(
        id: h['hazardTemplateId'] as int?,
        hazardText: h['hazardText'] ?? '',
        controlMeasures: h['controlMeasures'] ?? '',
        hazardTemplateId: h['hazardTemplateId'],
      );
    }).toList();

    return FLHAInspectionFormModel(
      formId: json['id'] ?? 0,
      projectId: json['projectId'] ?? 0,
      formTypeId: 3,
      crewName: json['crewName'] ?? '',
      dateFilled: json['dateFilled'] ?? '',
      taskDescription: json['taskDescription'] ?? '',
      otherComments: json['otherComments'] ?? '',
      allParticipants: [],
      selectedParticipantIds: participantList.map<int>((p) => p['participantId'] as int).toList(),
      signatures: initialSignatures,
      originalSignatures: initialSignatures,
      hazardTemplates: allHazards.where((h) => h.hazardTemplateId != null).toList(),
      selectedHazardIds: allHazards.where((h) => h.hazardTemplateId != null).map((h) => h.hazardTemplateId!).toList(),
      customHazards: allHazards.where((h) => h.hazardTemplateId == null).toList(),
      photos: photoUrls.map<FLHAPhoto>((url) => FLHAPhoto(preview: url, name: '', file: null)).toList(),
    );
  }
}

class FLHAPhoto {
  final String preview;
  final String name;
  final dynamic file;

  FLHAPhoto({
    required this.preview,
    required this.name,
    this.file,
  });
}
