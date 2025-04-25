class FormTypeDto {
  final int id;
  final String name;
  final String? description;

  FormTypeDto({
    required this.id,
    required this.name,
    this.description,
  });

  factory FormTypeDto.fromJson(Map<String, dynamic> json) {
    return FormTypeDto(
      id: json['id'],
      name: json['name'],
      description: json['description'],
    );
  }
}
