import 'package:dio/dio.dart';
import '../models/project_model.dart';
import '../../../core/services/api_client.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

final projectServiceProvider = Provider<ProjectService>((ref) {
  return ProjectService(ApiClient().dio);
});

class ProjectService {
  final Dio _dio;

  ProjectService(this._dio);

  Future<List<ProjectDto>> getProjects() async {
  try {
    final response = await _dio.get('Projects', queryParameters: {
      'limit': 30,
    });
    final List data = response.data;
    return data.map((json) => ProjectDto.fromJson(json)).toList();
  } catch (e) {
    print('Failed to load projects: $e');
    return [];
  }
}

}
