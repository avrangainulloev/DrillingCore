import 'dart:io';
import 'package:drillingcoreamobile/features/forms/models/checklist_response_dto.dart';
import 'package:drillingcoreamobile/features/forms/models/form_participant_dto.dart';
import 'package:drillingcoreamobile/features/forms/models/rill_inspection_form_save_dto.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../models/drill_inspection_form_model.dart';
import '../data/form_service.dart';
import 'dart:typed_data';
import 'dart:convert';

final drillInspectionViewModelProvider = StateNotifierProvider.autoDispose
    .family<DrillInspectionViewModel, DrillInspectionFormModel, DrillInspectionParams>(
  (ref, params) {
    return DrillInspectionViewModel(ref.read(formServiceProvider))
      ..initialize(params);
  },
);

class DrillInspectionParams {
  final int formId;
  final int formTypeId;
  final int projectId;

  DrillInspectionParams({
    required this.formId,
    required this.formTypeId,
    required this.projectId,
  });

  DrillInspectionParams copyWith({
    int? formId,
    int? formTypeId,
    int? projectId,
  }) {
    return DrillInspectionParams(
      formId: formId ?? this.formId,
      formTypeId: formTypeId ?? this.formTypeId,
      projectId: projectId ?? this.projectId,
    );
  }
}

class DrillInspectionViewModel extends StateNotifier<DrillInspectionFormModel> {
  final FormService _formService;
  late DrillInspectionParams _params;

  DrillInspectionViewModel(this._formService)
      : super(DrillInspectionFormModel.initial());

  Future<void> initialize(DrillInspectionParams params) async {
    _params = params;
    final participants = await _formService.getProjectParticipants(params.projectId);
    final checklist = await _formService.getChecklistItems(params.formTypeId);
    final form = await _formService.getDrillInspectionById(params.formId);

    state = state.copyWith(
      crewName: form.crewName,
      unitNumber: form.unitNumber,
      dateFilled: form.dateFilled,
      otherComments: form.otherComments ?? '',
      allParticipants: participants,
      checklistItems: checklist,
      selectedParticipantIds:
          form.participants.map((p) => p.participantId).toList(),
      checkedItemIds: form.checklistResponses
          .where((r) => r.response)
          .map((r) => r.checklistItemId)
          .toList(),
      signatures: {
        for (final sig in form.signatures) sig.participantId: sig.signatureUrl,
      },
      photos: form.photoUrls.map((url) => DrillInspectionPhoto(preview: url, file: null)).toList(),
    );
  }

  void toggleChecklistItem(int itemId, bool isChecked) {
    final updated = [...state.checkedItemIds];
    if (isChecked) {
      if (!updated.contains(itemId)) {
        updated.add(itemId);
      }
    } else {
      updated.remove(itemId);
    }

    state = state.copyWith(checkedItemIds: updated);
  }

  void toggleParticipant(int participantId, bool isChecked) {
    final updated = [...state.selectedParticipantIds];
    if (isChecked) {
      if (!updated.contains(participantId)) {
        updated.add(participantId);
      }
    } else {
      updated.remove(participantId);
    }

    state = state.copyWith(selectedParticipantIds: updated);
  }

  void addPhoto(File file) {
    final newPhoto = DrillInspectionPhoto(
      preview: file.path,
      file: file,
    );
    state = state.copyWith(photos: [...state.photos, newPhoto]);
  }

  void removePhoto(String previewPath) {
    final updatedPhotos = state.photos.where((p) => p.preview != previewPath).toList();
    state = state.copyWith(photos: updatedPhotos);
  }

  void addSignature(int participantId, Uint8List imageBytes) {
    final base64String = base64Encode(imageBytes);
    final dataUrl = 'data:image/png;base64,$base64String';

    state = state.copyWith(
      signatures: {
        ...state.signatures,
        participantId: dataUrl,
      },
    );
  }

  String imageBytesToDataUrl(Uint8List bytes) {
    final base64 = base64Encode(bytes);
    return 'data:image/png;base64,$base64';
  }

  // ✅ Метод сохранения формы
 Future<void> saveAsync() async {
  final dto = DrillInspectionFormSaveDto(
    formId: _params.formId == 0 ? null : _params.formId,
    projectId: _params.projectId,
    formTypeId: _params.formTypeId,
    dateFilled: state.dateFilled,
    crewName: state.crewName,
    unitNumber: state.unitNumber,
    participants: state.selectedParticipantIds
        .map((id) => FormParticipantDto(participantId: id))
        .toList(),
    checklistResponses: state.checkedItemIds
        .map((id) => ChecklistResponseDto(
              checklistItemId: id,
              response: true,
            ))
        .toList(),
    otherComments: state.otherComments,
  );

  final savedFormId = await _formService.saveDrillInspectionForm(dto);

  // ✅ Если PUT вернул 204, используем исходный ID
  final formId = savedFormId ?? dto.formId;
  if (formId == null) throw Exception("Save failed: no formId returned");

  // 🔁 Обновляем локально, если это была новая форма
  if (state.formId == 0) {
    _params = _params.copyWith(formId: formId);
    state = state.copyWith(formId: formId);
  }

  // 📸 Фото
  for (final photo in state.photos) {
    if (photo.file != null) {
      await _formService.uploadFormPhoto(formId, photo.file);
    }
  }

  // ✍️ Подписи
  for (final entry in state.signatures.entries) {
    final participantId = entry.key;
    final signatureDataUrl = entry.value;

    if (signatureDataUrl.startsWith('data:image')) {
      final base64Str = signatureDataUrl.split(',').last;
      final bytes = base64Decode(base64Str);
      await _formService.uploadFormSignature(formId, participantId, bytes);
    }
  }
}
}