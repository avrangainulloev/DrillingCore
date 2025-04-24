import 'package:drillingcoreamobile/features/projects/models/project_model.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../data/project_service.dart';

final projectViewModelProvider =
    StateNotifierProvider<ProjectViewModel, List<ProjectDto>>(
  (ref) => ProjectViewModel(ref.read(projectServiceProvider)),
);

class ProjectViewModel extends StateNotifier<List<ProjectDto>> {
  final ProjectService _service;

  ProjectViewModel(this._service) : super([]) {
    loadProjects();
  }

  Future<void> loadProjects() async {
    final projects = await _service.getProjects();
    state = projects;
  }

  Future<void> refresh() async => await loadProjects();
}
