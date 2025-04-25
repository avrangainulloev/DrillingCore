import 'dart:io';
import 'dart:typed_data';

import 'package:drillingcoreamobile/features/forms/data/dtos/filled_form_dto.dart';
import 'package:drillingcoreamobile/features/forms/data/dtos/filled_forms_filter.dart';
import 'package:drillingcoreamobile/features/forms/data/dtos/form_type_dto.dart';
import 'package:drillingcoreamobile/features/forms/inspection_forms/models/drill_inspection_dto.dart';

import 'package:drillingcoreamobile/features/forms/inspection_forms/models/rill_inspection_form_save_dto.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../core/services/api_client.dart';
import '../inspection_forms/models/drill_inspection_form_model.dart';
import 'package:dio/dio.dart';
import 'package:http_parser/http_parser.dart';
final formServiceProvider = Provider<FormService>((ref) {
  return FormService(ApiClient().dio);
});

class FormService {
    final Dio _dio;

  FormService(this._dio);

  
Future<List<ProjectParticipant>> getProjectParticipants(int projectId, int userId) async {
  final nowRes = await _dio.get('common/server-date');
  final serverNow = DateTime.parse(nowRes.data['now']);

  final groupRes = await _dio.get('projects/$projectId/groups');
  final groups = List<Map<String, dynamic>>.from(groupRes.data);

  // Собираем всех участников с groupName
  final flat = <Map<String, dynamic>>[];

  for (final group in groups) {
    final groupName = group['groupName'];
    final participants = List<Map<String, dynamic>>.from(group['participants']);

    for (final p in participants) {
      flat.add({
        ...p,
        'groupName': groupName,
      });
    }
  }

  // Группировка по userId
  final Map<int, List<Map<String, dynamic>>> grouped = {};
  for (final p in flat) {
    final uid = p['userId'] as int;
    grouped.putIfAbsent(uid, () => []);
    grouped[uid]!.add(p);
  }

  final List<ProjectParticipant> result = [];

  for (final entry in grouped.entries) {
    final records = entry.value;

    // Оставляем только активных
    final valid = records.where((r) {
      final endDateStr = r['endDate'];
      if (endDateStr == null) return true;
      final endDate = DateTime.tryParse(endDateStr);
      return endDate == null || endDate.isAfter(serverNow);
    }).toList();

    if (valid.isNotEmpty) {
      // Сортировка по ближайшему endDate
      valid.sort((a, b) {
        final aDate = a['endDate'] != null ? DateTime.parse(a['endDate']) : DateTime(9999);
        final bDate = b['endDate'] != null ? DateTime.parse(b['endDate']) : DateTime(9999);
        return aDate.compareTo(bDate);
      });

      final selected = valid.first;
      result.add(ProjectParticipant.fromJson(selected, selected['groupName']));
    }
  }

  return result;
}

// Future<List<ProjectParticipant>> getProjectParticipants(int projectId) async {
//   final response = await _dio.get('projects/$projectId/groups');
//   final groups = List<Map<String, dynamic>>.from(response.data);

//   final List<ProjectParticipant> all = [];

//   for (final group in groups) {
//     final groupName = group['groupName'] as String;
//     final participants = List<Map<String, dynamic>>.from(group['participants']);

//     for (final json in participants) {
//       // фильтруем по endDate (оставляем только активных)
//       if (json['endDate'] == null) {
//         all.add(ProjectParticipant.fromJson(json, groupName));
//       }
//     }
//   }

//   return all;
// }

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

Future<List<FormTypeDto>> getFormTypes() async {
  final response = await _dio.get('Forms/form-types');
  final List<dynamic> data = response.data;
  return data.map((json) => FormTypeDto.fromJson(json)).toList();
}


Future<List<FilledFormDto>> getFilledForms(FilledFormsFilter filter) async {

  final response = await _dio.get(
    'Forms/project/${filter.projectId}/list',
    queryParameters: filter.toQueryParams(),
  );
  final List<dynamic> items = response.data['items'];
  return items.map((json) => FilledFormDto.fromJson(json)).toList();
}


Future<String> getUnitNumber({
  required int formTypeId,
  required int projectId,
  required int participantId,
}) async {
  try {
    final res = await _dio.get(
      'forms/equipment',
      queryParameters: {
        'formTypeId': formTypeId,
        'participantId': participantId,
        'projectId': projectId,
      },
    );

    if (res.statusCode == 200) {
      final data = res.data;
      return data['registrationNumber'] ?? '';
    }
  } catch (e) {
    print('Failed to load unit number: $e');
  }

  return '';
}
}




// вспомогательная моделька для получения формы





