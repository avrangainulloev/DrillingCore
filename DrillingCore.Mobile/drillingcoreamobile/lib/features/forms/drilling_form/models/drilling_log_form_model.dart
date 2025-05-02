import 'dart:typed_data';
import '../../inspection_forms/models/drill_inspection_form_model.dart';

class DrillingLogFormModel {
  final int formId;
  final int projectId;
  final int formTypeId;
  final String crewName;
  final String dateFilled;
  final int totalWells;
  final double totalMeters;
  final String? otherComments;
  final List<ProjectParticipant> allParticipants;
  final List<int> selectedParticipantIds;
  final Map<int, String> signatures;
   final Map<int, String> originalSignatures;
  final List<DrillingLogPhoto> photos;

  DrillingLogFormModel({
    required this.formId,
    required this.projectId,
    required this.formTypeId,
    required this.crewName,
    required this.dateFilled,
    required this.totalWells,
    required this.totalMeters,
    required this.otherComments,
    required this.allParticipants,
    required this.selectedParticipantIds,
    required this.signatures,
    required this.originalSignatures,
    required this.photos,
  });

  factory DrillingLogFormModel.initial() {
    return DrillingLogFormModel(
      formId: 0,
      projectId: 0,
      formTypeId: 0,
      crewName: '',
      dateFilled: DateTime.now().toIso8601String().split('T')[0],
      totalWells: 0,
      totalMeters: 0,
      otherComments: '',
      allParticipants: [],
      selectedParticipantIds: [],
      signatures: {},
       originalSignatures: {},
      photos: [],
    );
  }

  DrillingLogFormModel copyWith({
    int? formId,
    int? projectId,
    int? formTypeId,
    String? crewName,
    String? dateFilled,
    int? totalWells,
    double? totalMeters,
    String? otherComments,
    List<ProjectParticipant>? allParticipants,
    List<int>? selectedParticipantIds,
    Map<int, String>? signatures,
    Map<int, String>? originalSignatures,
    List<DrillingLogPhoto>? photos,
  }) {
    return DrillingLogFormModel(
      formId: formId ?? this.formId,
      projectId: projectId ?? this.projectId,
      formTypeId: formTypeId ?? this.formTypeId,
      crewName: crewName ?? this.crewName,
      dateFilled: dateFilled ?? this.dateFilled,
      totalWells: totalWells ?? this.totalWells,
      totalMeters: (totalMeters ?? this.totalMeters).toDouble(),
      otherComments: this.otherComments,
      allParticipants: allParticipants ?? this.allParticipants,
      selectedParticipantIds: selectedParticipantIds ?? this.selectedParticipantIds,
      signatures: signatures ?? this.signatures,
      originalSignatures: originalSignatures ?? this.originalSignatures,
      photos: photos ?? this.photos,
    );
  }

  factory DrillingLogFormModel.fromJson(Map<String, dynamic> json) {
    final participantList = json['participants'] as List<dynamic>? ?? [];
    final photoUrls = json['photoUrls'] as List<dynamic>? ?? [];

  final signatureList = json['signatures'] as List<dynamic>? ?? [];

final Map<int, String> signatures = {
  for (final sig in signatureList)
    if (sig['signatureUrl'] != null && (sig['signatureUrl'] as String).isNotEmpty)
      sig['participantId'] as int: sig['signatureUrl'] as String
};
    return DrillingLogFormModel(
      formId: json['id'] ?? 0,
      projectId: json['projectId'] ?? 0,
      formTypeId: json['formTypeId'] ?? 0,
      crewName: json['crewName'] ?? '',
      dateFilled: json['dateFilled'] ?? '',
      totalWells: json['totalWells'] ?? 0,
      totalMeters: (json['totalMeters'] as num).toDouble(),
      otherComments: json['otherComments'],
      allParticipants: [], // загружаются отдельно
      selectedParticipantIds: participantList.map<int>((p) => p['participantId'] as int).toList(),
      signatures: signatures,
       originalSignatures: signatures,
      photos: photoUrls
          .map<DrillingLogPhoto>((url) => DrillingLogPhoto(
                preview: url,
                name: '',
                file: null,
              ))
          .toList(),
    );
  }
}

class DrillingLogPhoto {
  final String preview;
  final String name;
  final dynamic file;

  DrillingLogPhoto({
    required this.preview,
    required this.name,
    this.file,
  });
}