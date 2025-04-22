class UnsignedFormDto {
  final int formId;
  final int formTypeId;
  final int projectId;
  final String formTypeName;
  final String projectName;
  final String dateFilled;
  final String? creatorName;
  final String? updatedAt;
  
  UnsignedFormDto({
    required this.formId,
    required this.formTypeId,
    required this.projectId,
    required this.formTypeName,
    required this.projectName,
    required this.dateFilled,
    required this.creatorName,
    required this.updatedAt,
  });

  factory UnsignedFormDto.fromJson(Map<String, dynamic> json) {
    return UnsignedFormDto(
     formId: json['formId'] ?? 0,
    formTypeId: json['formTypeId'] ?? 0,
    projectId: json['projectId'] ?? 0,
    formTypeName: json['formTypeName'] as String? ?? 'Unknown',
    projectName: json['projectName'] as String? ?? 'Unknown',
    dateFilled: json['dateFilled'] as String? ?? '',
    creatorName: json['creatorName'] as String? ?? '',
updatedAt: json['updatedAt'] as String? ?? '',
     

    );
  }
}
