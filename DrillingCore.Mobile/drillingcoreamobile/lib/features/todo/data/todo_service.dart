import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:dio/dio.dart';
import '../../../core/services/api_client.dart';
import '../models/unsigned_form_dto.dart';

final todoServiceProvider = Provider<ToDoService>((ref) {
  return ToDoService(ApiClient().dio);
});

class ToDoService {
  final Dio _dio;

  ToDoService(this._dio);

  Future<List<UnsignedFormDto>> getUnsignedForms(int userId) async {
    try {
      final response = await _dio.get('forms/unsigned/$userId');
      final data = List<Map<String, dynamic>>.from(response.data);
      return data.map((e) => UnsignedFormDto.fromJson(e)).toList();
      
    } catch (e) {
      print('Error loading forms: $e');
      return [];
    }
  }
}
