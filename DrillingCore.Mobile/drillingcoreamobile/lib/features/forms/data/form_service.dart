import 'dart:io';
import 'dart:typed_data';

import 'package:drillingcoreamobile/features/forms/models/drill_inspection_dto.dart';

import 'package:drillingcoreamobile/features/forms/models/rill_inspection_form_save_dto.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../core/services/api_client.dart';
import '../models/drill_inspection_form_model.dart';
import 'package:dio/dio.dart';
import 'package:http_parser/http_parser.dart';
final formServiceProvider = Provider<FormService>((ref) {
  return FormService(ApiClient().dio);
});

class FormService {
    final Dio _dio;

  FormService(this._dio);

  Future<List<ProjectParticipant>> getProjectParticipants(int projectId) async {
    final response = await _dio.get('projects/$projectId/groups');
    final groups = List<Map<String, dynamic>>.from(response.data);
    final flat = groups.expand((g) => g['participants']).toList();
    return flat.map((e) => ProjectParticipant.fromJson(e)).toList();
  }

  Future<List<ChecklistItem>> getChecklistItems(int formTypeId) async {
    final response = await _dio.get('checklist/by-form-type/$formTypeId');
    final list = List<Map<String, dynamic>>.from(response.data);
    return list.map((e) => ChecklistItem.fromJson(e)).toList();
  }

  Future<DrillInspectionDto> getDrillInspectionById(int formId) async {
    final response = await _dio.get('forms/drill-inspection/$formId');
    return DrillInspectionDto.fromJson(response.data);
  }

Future<int?> saveDrillInspectionForm(DrillInspectionFormSaveDto dto) async {
  final data = dto.toJson();

  final isNew = dto.formId == null;

  final response = await _dio.request(
    'forms/drill-inspection',
    data: data,
    options: Options(
      method: isNew ? 'POST' : 'PUT',
      contentType: Headers.jsonContentType,
    ),
  );

    if (response.statusCode == 200) {
    // Вернулось тело с formId — только для POST
    return response.data['formId'];
  } else if (response.statusCode == 204) {
    // Успешное обновление — ничего не возвращается
    return dto.formId;
  } else {
    throw Exception('Failed to save form: ${response.statusCode}');
  }
}

Future<void> uploadFormPhoto(int formId, File file) async {
  final formData = FormData.fromMap({
    'formId': formId,
    'file': await MultipartFile.fromFile(file.path),
  });

  await _dio.post('forms/$formId/photos', data: formData);
}

Future<void> uploadFormSignature(int formId, int participantId, Uint8List imageBytes) async {
  final formData = FormData.fromMap({
    'formId': formId,
    'participantId': participantId,
    'file': MultipartFile.fromBytes(
      imageBytes,
      filename: 'signature.png',
      contentType: MediaType('image', 'png'),
    ),
  });

  await _dio.post('forms/$formId/signatures', data: formData);
}

}




// вспомогательная моделька для получения формы





