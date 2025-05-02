import 'package:dio/dio.dart';
import 'package:drillingcoreamobile/core/services/api_client.dart';
import 'package:drillingcoreamobile/features/forms/drilling_form/models/drilling_log_form_model.dart';
 
import '../drilling_form/models/drilling_log_form_save_dto.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

final drillingLogFormServiceProvider =  Provider<DrillingLogFormService>((ref) {
  return DrillingLogFormService(ApiClient().dio);
});

class DrillingLogFormService {
  final Dio _dio;
  DrillingLogFormService(this._dio);

  Future<int?> saveDrillingLogForm(DrillingLogFormSaveDto dto) async {
    final response = await _dio.post('/forms/drilling', data: dto.toJson());
    return response.data['formId'] as int?;
  }

  Future<void> updateDrillingLogForm(int id, DrillingLogFormSaveDto dto) async {
    await _dio.put('/forms/drilling', data: {
      'formId': id,
      ...dto.toJson(),
    });
  }

  Future<DrillingLogFormModel> getDrillingLogFormById(int id) async {
    final response = await _dio.get('/forms/drilling/$id');
    return DrillingLogFormModel.fromJson(response.data);
  }
}
