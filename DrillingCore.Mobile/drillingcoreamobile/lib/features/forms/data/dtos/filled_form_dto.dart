class FilledFormDto {
  final int id;
  final String formType;
  final String creatorName;
  final String? crewName;
  final DateTime dateFilled;
  final String? status;

  FilledFormDto({
    required this.id,
    required this.formType,
    required this.creatorName,
    required this.crewName,
    required this.dateFilled,
    this.status,
  });

  factory FilledFormDto.fromJson(Map<String, dynamic> json) {
    return FilledFormDto(
      id: json['id'],
      formType: json['formType'],
      creatorName: json['creatorName'],
      crewName: json['crewName'],
      dateFilled: DateTime.parse(json['dateFilled']),
      status: json['status'],
    );
  }
}
