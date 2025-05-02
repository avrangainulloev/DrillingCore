class HazardItem {
  final int? id;
  final String hazardText;
  final String controlMeasures;
  final int? hazardTemplateId;
  HazardItem({
    this.id,
    required this.hazardText,
    required this.controlMeasures,
    required this.hazardTemplateId
  });

  factory HazardItem.fromJson(Map<String, dynamic> json) {
    return HazardItem(
      id: json['id'] as int?,
      hazardText: json['label'] ?? '',
      controlMeasures: json['controlSuggestion'] ?? '',
       hazardTemplateId:  json['hazardTemplateId'] as int?,
    );
  }


    HazardItem copyWith({
    int? id,
    String? hazardText,
    String? controlMeasures,
    int? hazardTemplateId,
  }) {
    return HazardItem(
      id: id ?? this.id,
      hazardText: hazardText ?? this.hazardText,
      controlMeasures: controlMeasures ?? this.controlMeasures,
      hazardTemplateId: hazardTemplateId ?? this.hazardTemplateId,
    );
  }
}
