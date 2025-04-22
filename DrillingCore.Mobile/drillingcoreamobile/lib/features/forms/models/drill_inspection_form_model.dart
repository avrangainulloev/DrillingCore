class DrillInspectionFormModel {
  final int formId;
  final String crewName;
  final String unitNumber;
  final String dateFilled;
  final String otherComments;
  final List<ProjectParticipant> allParticipants;
  final List<ChecklistItem> checklistItems;
  final List<int> selectedParticipantIds;
  final List<int> checkedItemIds;
  final Map<int, String> signatures;
  final List<DrillInspectionPhoto> photos;

  DrillInspectionFormModel({
    required this.formId,
    required this.crewName,
    required this.unitNumber,
    required this.dateFilled,
    required this.otherComments,
    required this.allParticipants,
    required this.checklistItems,
    required this.selectedParticipantIds,
    required this.checkedItemIds,
    required this.signatures,
    required this.photos,
  });

  factory DrillInspectionFormModel.initial() {
    return DrillInspectionFormModel(
      formId: 0,
      crewName: '',
      unitNumber: '',
      dateFilled: '',
      otherComments: '',
      allParticipants: [],
      checklistItems: [],
      selectedParticipantIds: [],
      checkedItemIds: [],
      signatures: {},
      photos: [],
    );
  }

  DrillInspectionFormModel copyWith({
    int? formId,
    String? crewName,
    String? unitNumber,
    String? dateFilled,
    String? otherComments,
    List<ProjectParticipant>? allParticipants,
    List<ChecklistItem>? checklistItems,
    List<int>? selectedParticipantIds,
    List<int>? checkedItemIds,
    Map<int, String>? signatures,
    List<DrillInspectionPhoto>? photos,
  }) {
    return DrillInspectionFormModel(
      formId: formId ?? this.formId,
      crewName: crewName ?? this.crewName,
      unitNumber: unitNumber ?? this.unitNumber,
      dateFilled: dateFilled ?? this.dateFilled,
      otherComments: otherComments ?? this.otherComments,
      allParticipants: allParticipants ?? this.allParticipants,
      checklistItems: checklistItems ?? this.checklistItems,
      selectedParticipantIds: selectedParticipantIds ?? this.selectedParticipantIds,
      checkedItemIds: checkedItemIds ?? this.checkedItemIds,
      signatures: signatures ?? this.signatures,
      photos: photos ?? this.photos,
    );
  }
}

class ProjectParticipant {
  final int participantId;
  final int userId;
  final String fullName;

  ProjectParticipant({
    required this.participantId,
    required this.userId,
    required this.fullName,
  });

  factory ProjectParticipant.fromJson(Map<String, dynamic> json) {
    return ProjectParticipant(
      participantId: json['id'],
      userId: json['userId'],
      fullName: json['fullName'],
    );
  }
}

class ChecklistItem {
  final int id;
  final String label;
  final String groupName;

  ChecklistItem({
    required this.id,
    required this.label,
    required this.groupName,
  });

  factory ChecklistItem.fromJson(Map<String, dynamic> json) {
    return ChecklistItem(
      id: json['id'],
      label: json['label'],
      groupName: json['groupName'],
    );
  }
}

class DrillInspectionPhoto {
  final String preview;
  final dynamic file; // если загружено новое фото, сюда File

  DrillInspectionPhoto({required this.preview, this.file});
}
