import 'dart:convert';
import 'dart:io';
import 'dart:typed_data';

import 'package:drillingcoreamobile/core/constants/constants.dart';
import 'package:drillingcoreamobile/core/services/user_session.dart';
import 'package:drillingcoreamobile/features/forms/data/flha_form_service.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
 
import '../models/flha_inspection_form_model.dart';
import '../models/hazard_item_model.dart';
import '../models/flha_form_save_dto.dart';
import '../../data/form_service.dart';
 

class FLHAInspectionParams {
  final int formId;
  final int formTypeId;
  final int projectId;

  FLHAInspectionParams({
    required this.formId,
    required this.formTypeId,
    required this.projectId,
  });
}

final flhaInspectionViewModelProvider =
    StateNotifierProvider.autoDispose.family<FLHAInspectionViewModel, FLHAInspectionFormModel, FLHAInspectionParams>(
  (ref, params) {
    final formService = ref.read(formServiceProvider);
    final flhaService = ref.read(flhaFormServiceProvider);
    return FLHAInspectionViewModel(formService, flhaService)..initialize(params);
  },
);

class FLHAInspectionViewModel extends StateNotifier<FLHAInspectionFormModel> {
  final FormService _formService;
  final FLHAFormService _flhaService;

  FLHAInspectionViewModel(this._formService, this._flhaService)
      : super(FLHAInspectionFormModel.empty());

Future<void> initialize(FLHAInspectionParams params) async {
  state = state.copyWith(
    formId: params.formId,
    formTypeId: params.formTypeId,
    projectId: params.projectId,
  );

  final hazards = await _flhaService.getHazardTemplates(params.formTypeId);
  final participants = await _formService.getProjectParticipants(params.projectId, 0);

  if (params.formId > 0) {
    final filledForm = await _flhaService.getFilledFLHAForm(params.formId);

    final updatedHazardTemplates = hazards.map((template) {
  final match = filledForm.hazardTemplates.firstWhere(
    (h) => h.id == template.id,
    orElse: () => template,
  );
  return template.copyWith(controlMeasures: match.controlMeasures);
}).toList(); 
    // Первичное состояние
    state = filledForm.copyWith(
      formId: params.formId,
      formTypeId: params.formTypeId,
      projectId: params.projectId,
      hazardTemplates: updatedHazardTemplates,
      allParticipants: participants,
      
    );

    // 🔁 Загрузка подписей по URL и преобразование в data:image/png;base64,...
    for (final entry in filledForm.signatures.entries) {
      final id = entry.key;
      final sigUrl = entry.value;

      try {
        final fullUrl = sigUrl.startsWith('http')
            ? sigUrl
            : '$kApiBaseUrl/${sigUrl.replaceFirst(RegExp(r'^/+'), '')}';

        final uri = Uri.parse(fullUrl);
        final request = await HttpClient().getUrl(uri);
        final response = await request.close();

        if (response.statusCode == 200) {
          final bytes = await consolidateHttpClientResponseBytes(response);
          final base64 = base64Encode(bytes);
          final dataUrl = 'data:image/png;base64,$base64';

          state = state.copyWith(signatures: {
            ...state.signatures,
            id: dataUrl,
          });
          state = state.copyWith(
            originalSignatures: Map<int, String>.from(state.signatures),
          );
        }
      } catch (e) {
        print('❌ Error loading signature for participant $id: $e');
      }
    }
  } else {
    state = state.copyWith(
      hazardTemplates: hazards,
      allParticipants: participants,
    );
  }
}


  void updateCrewName(String value) => state = state.copyWith(crewName: value);

  void updateDateFilled(String value) => state = state.copyWith(dateFilled: value);

  void updateTaskDescription(String value) => state = state.copyWith(taskDescription: value);

  void updateOtherComments(String value) => state = state.copyWith(otherComments: value);

  void toggleParticipant(int participantId, bool selected) {
    final updated = [...state.selectedParticipantIds];
    if (selected) {
      updated.add(participantId);
    } else {
      updated.remove(participantId);
    }
    state = state.copyWith(selectedParticipantIds: updated);
  }

  void toggleHazard(int hazardId, bool selected) {
    final updated = [...state.selectedHazardIds];
    if (selected) {
      updated.add(hazardId);
    } else {
      updated.remove(hazardId);
    }
    state = state.copyWith(selectedHazardIds: updated);
  }

  void updateHazardControlMeasure(int hazardId, String value) {
    final updated = state.hazardTemplates.map((h) {
      if (h.id == hazardId) {
        return HazardItem(id: h.id, hazardText: h.hazardText, controlMeasures: value, hazardTemplateId: null);
      }
      return h;
    }).toList();
    state = state.copyWith(hazardTemplates: updated);
  }

  void addCustomHazard(String hazardText, String controlMeasures) {
    final newHazard = HazardItem(hazardText: hazardText, controlMeasures: controlMeasures, hazardTemplateId: null);
    state = state.copyWith(customHazards: [...state.customHazards, newHazard]);
  }

  void updateCustomHazard(HazardItem hazard, {String? hazardText, String? controlMeasures}) {
    final updated = state.customHazards.map((h) {
      if (h == hazard) {
        return HazardItem(
          hazardText: hazardText ?? h.hazardText,
          controlMeasures: controlMeasures ?? h.controlMeasures, hazardTemplateId: null,
        );
      }
      return h;
    }).toList();
    state = state.copyWith(customHazards: updated);
  }

  void removeCustomHazard(HazardItem hazard) {
    final updated = [...state.customHazards]..remove(hazard);
    state = state.copyWith(customHazards: updated);
  }

  void addEmptyCustomHazard() {
  state = state.copyWith(
    customHazards: [
      ...state.customHazards,
      HazardItem(hazardText: '', controlMeasures: '',  hazardTemplateId: null),
    ],
  );
}

void updateCustomHazardText(HazardItem hazard, String value) {
  final updated = state.customHazards.map((h) {
    if (h == hazard) {
      return h.copyWith(hazardText: value);
    }
    return h;
  }).toList();

  state = state.copyWith(customHazards: updated);
}

void updateCustomControlMeasure(HazardItem hazard, String value) {
  final updated = state.customHazards.map((h) {
    if (h == hazard) {
      return h.copyWith(controlMeasures: value);
    }
    return h;
  }).toList();

  state = state.copyWith(customHazards: updated);
}

  void addPhoto(File file) {
    final photo = FLHAPhoto(preview: file.path, name: file.path.split('/').last, file: file);
    state = state.copyWith(photos: [...state.photos, photo]);
  }

  void removePhoto(String previewPath) {
    final updated = [...state.photos]..removeWhere((p) => p.preview == previewPath);
    state = state.copyWith(photos: updated);
  }

 void addSignature(int participantId, Uint8List imageBytes) {
  final base64String = base64Encode(imageBytes);
  final dataUrl = 'data:image/png;base64,$base64String';

  state = state.copyWith(
    signatures: {
      ...state.signatures,
      participantId: dataUrl, // меняем с Uint8List на String
    },
  );
}

Future<void> saveAsync() async {
    final session0 = UserSession();
    final userId = await session0.getUserId();
  final dto = FLHAFormSaveDto(
    crewName: state.crewName,
    projectId: state.projectId,
    formTypeId: state.formTypeId,
    dateFilled: state.dateFilled,
    taskDescription: state.taskDescription,
    otherComments: state.otherComments.isNotEmpty ? state.otherComments : null,
    creatorId: userId,
    hazards: [
      ...state.hazardTemplates
          .where((h) => state.selectedHazardIds.contains(h.id))
          .map((h) => FLHAHazardEntryDto(
                hazardText: h.hazardText,
                controlMeasures: h.controlMeasures,
                hazardTemplateId: h.id,
              )),
      ...state.customHazards.map((h) => FLHAHazardEntryDto(
            hazardText: h.hazardText,
            controlMeasures: h.controlMeasures,
          )),
    ],
    participants: state.selectedParticipantIds.map((id) {
      return ParticipantSignatureDto(
        participantId: id,
        signatureBase64: state.formId == 0 ? state.signatures[id] : null,
      );
    }).toList(),
  );

  int formId = state.formId;

  if (formId == 0) {
    final newId = await _flhaService.saveFLHAForm(dto); // POST
    if (newId == null || newId == 0) throw Exception("Save failed: no formId returned");
    formId = newId;
    state = state.copyWith(formId: formId);
  } else {
    await _flhaService.updateFLHAForm(formId, dto); // PUT
  }

  // 📷 Загрузка фото
  for (final photo in state.photos) {
    if (photo.file != null) {
      await _formService.uploadFormPhoto(formId, photo.file);
    }
  }

  // ✍️ Загрузка подписей (всегда отдельно)
 for (final entry in state.signatures.entries) {
  final participantId = entry.key;
  final dataUrl = entry.value;

  // Пропустить, если подпись не новая (совпадает с original)
  final original = state.originalSignatures[participantId];
  if (original != null && original == dataUrl) continue;

  if (dataUrl.startsWith('data:image')) {
    final base64Str = dataUrl.split(',').last;
    final bytes = base64Decode(base64Str);
    await _formService.uploadFormSignature(formId, participantId, bytes);
  }
}
}



}
