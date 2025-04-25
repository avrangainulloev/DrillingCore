class FilledFormsFilter {
  final int projectId;
  final int formTypeId;
  final bool onlyMine;
  final String? status; // опционально
  final int page;
  final int limit;

  const FilledFormsFilter({
    required this.projectId,
    required this.formTypeId,
    this.onlyMine = false,
    this.status,
    this.page = 1,
    this.limit = 30,
  });

  Map<String, String> toQueryParams({int? userId}) {
    final Map<String, String> params = {
      'formTypeId': formTypeId.toString(),
      'page': page.toString(),
      'limit': limit.toString(),
    };

    if (status != null) params['status'] = status!;
    if (onlyMine) params['onlyMine'] = onlyMine.toString();

    return params;
  }

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      other is FilledFormsFilter &&
          runtimeType == other.runtimeType &&
          projectId == other.projectId &&
          formTypeId == other.formTypeId &&
          onlyMine == other.onlyMine &&
          status == other.status &&
          page == other.page &&
          limit == other.limit;

  @override
  int get hashCode =>
      projectId.hashCode ^
      formTypeId.hashCode ^
      onlyMine.hashCode ^
      status.hashCode ^
      page.hashCode ^
      limit.hashCode;
}
