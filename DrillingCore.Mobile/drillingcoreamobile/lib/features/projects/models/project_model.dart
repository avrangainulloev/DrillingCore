class ProjectDto {
  final int id;
  final String name;
  final String location;
  final DateTime startDate;
  final DateTime? endDate;
  final String client;
  final bool hasCampOrHotel;
  final String status;
  final int statusId;

  ProjectDto({
    required this.id,
    required this.name,
    required this.location,
    required this.startDate,
    this.endDate,
    required this.client,
    required this.hasCampOrHotel,
    required this.status,
    required this.statusId,
  });

  factory ProjectDto.fromJson(Map<String, dynamic> json) {
    return ProjectDto(
      id: json['id'],
      name: json['name'],
      location: json['location'],
      startDate: DateTime.parse(json['startDate']),
      endDate: json['endDate'] != null ? DateTime.tryParse(json['endDate']) : null,
      client: json['client'],
      hasCampOrHotel: json['hasCampOrHotel'],
      status: json['status'],
      statusId: json['statusId'],
    );
  }
}
