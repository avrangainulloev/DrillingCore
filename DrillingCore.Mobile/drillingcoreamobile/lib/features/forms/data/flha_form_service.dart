 

import 'package:dio/dio.dart';
 
import 'package:drillingcoreamobile/features/forms/flha/models/flha_form_save_dto.dart';
import 'package:drillingcoreamobile/features/forms/flha/models/flha_inspection_form_model.dart';
import 'package:drillingcoreamobile/features/forms/flha/models/hazard_item_model.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../core/services/api_client.dart';
 

final flhaFormServiceProvider = Provider<FLHAFormService>((ref) {
  return FLHAFormService(ApiClient().dio);
});

class FLHAFormService {
  final Dio _dio;

  FLHAFormService(this._dio);

  Future<List<HazardItem>> getHazardTemplates(int formTypeId) async {
    final response = await _dio.get('flha/hazards/2');
    final list = List<Map<String, dynamic>>.from(response.data);
    return list.map((e) => HazardItem.fromJson(e)).toList();
  }

 Future<int?> saveFLHAForm(FLHAFormSaveDto dto) async {
  final response = await _dio.post('flha', data: dto.toJson());

  if (response.statusCode == 200 && response.data is Map<String, dynamic>) {
    return response.data['formId'] as int?;
  }

  return null; // или throw если ты хочешь падать при ошибке
}

Future<void> updateFLHAForm(int formId, FLHAFormSaveDto dto) async {
  await _dio.put(
    'flha/$formId',
    data: dto.toJson(),
  );
}

 Future<FLHAInspectionFormModel> getFilledFLHAForm(int formId) async {
  final response = await _dio.get('flha/$formId');
  return FLHAInspectionFormModel.fromJson(response.data);
}
}
