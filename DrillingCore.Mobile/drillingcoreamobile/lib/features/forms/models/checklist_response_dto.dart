class ChecklistResponseDto {
  final int checklistItemId;
  final bool response;

  ChecklistResponseDto({
    required this.checklistItemId,
    required this.response,
  });

  factory ChecklistResponseDto.fromJson(Map<String, dynamic> json) {
    return ChecklistResponseDto(
      checklistItemId: json['checklistItemId'],
      response: json['response'],
    );
  }

    Map<String, dynamic> toJson() => {
        'checklistItemId': checklistItemId,
        'response': response,
      };
}