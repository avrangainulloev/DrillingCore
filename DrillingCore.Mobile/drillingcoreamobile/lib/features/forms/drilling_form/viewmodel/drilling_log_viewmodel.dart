// 📄 drilling_log_viewmodel.dart
import 'dart:convert';
import 'dart:io';
import 'dart:typed_data';

import 'package:drillingcoreamobile/core/services/user_session.dart';
import 'package:drillingcoreamobile/features/forms/data/drilling_log_form_service.dart';
import 'package:drillingcoreamobile/features/forms/data/form_service.dart';
import 'package:drillingcoreamobile/features/forms/data/dtos/form_participant_dto.dart';
import 'package:drillingcoreamobile/features/forms/drilling_form/models/drilling_log_form_save_dto.dart';
import 'package:drillingcoreamobile/features/forms/inspection_forms/models/drill_inspection_form_model.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../models/drilling_log_form_model.dart';

final drillingLogViewModelProvider = StateNotifierProvider.autoDispose
    .family<DrillingLogViewModel, DrillingLogFormModel, DrillingLogParams>(
  (ref, params) {
    final formService = ref.read(formServiceProvider);
    final drillingLogFormService = ref.read(drillingLogFormServiceProvider);
    return DrillingLogViewModel(formService, drillingLogFormService)
      ..initialize(params);
  },
);

class DrillingLogParams {
  final int formId;
  final int formTypeId;
  final int projectId;

  DrillingLogParams({
    required this.formId,
    required this.formTypeId,
    required this.projectId,
  });

  DrillingLogParams copyWith({int? formId, int? formTypeId, int? projectId}) {
    return DrillingLogParams(
      formId: formId ?? this.formId,
      formTypeId: formTypeId ?? this.formTypeId,
      projectId: projectId ?? this.projectId,
    );
  }
}

class DrillingLogViewModel extends StateNotifier<DrillingLogFormModel> {
  final FormService _formService;
  final DrillingLogFormService _drillingLogFormService;
  late DrillingLogParams _params;
  int userId = 0;

  DrillingLogViewModel(this._formService, this._drillingLogFormService) : super(DrillingLogFormModel.initial());

  Future<void> initialize(DrillingLogParams params) async {
    _params = params;
    final session = await UserSession().getSession();
    if (session == null) return;
    userId = session['userId'] as int;
    final participants = await _formService.getProjectParticipants(params.projectId, userId);

    if (params.formId == 0) {
      String? crewName;
      List<int> selectedParticipantIds = [];

      final current = participants.firstWhere(
        (p) => p.userId == userId,
        orElse: () => participants.isNotEmpty
            ? participants.first
            : ProjectParticipant(
                participantId: 0, userId: 0, fullName: '', groupName: '', endDate: null),
      );

      if (current.groupName != null && current.groupName!.isNotEmpty) {
        final group = participants.where((p) => p.groupName == current.groupName).toList();
        selectedParticipantIds = group.map((p) => p.participantId).toList();
        crewName = current.groupName;
      }

      state = state.copyWith(
        formId: 0,
        projectId: params.projectId,
        formTypeId: params.formTypeId,
        crewName: crewName ?? '',
        dateFilled: DateTime.now().toIso8601String().split('T').first,
        totalWells: 0,
        totalMeters: 0,
        allParticipants: participants,
        selectedParticipantIds: selectedParticipantIds,
        signatures: {},
        originalSignatures: {},
        photos: [],
      );
    } else {
      final filled = await _drillingLogFormService.getDrillingLogFormById(params.formId);

      // ✅ Просто копируем уже base64-подписи
      state = filled.copyWith(
        allParticipants: participants,
        selectedParticipantIds: filled.selectedParticipantIds,
        photos: filled.photos,
        signatures: filled.signatures,
        originalSignatures: Map<int, String>.from(filled.signatures),
      );
    }
  }

  void updateCrewName(String value) => state = state.copyWith(crewName: value);
  void updateDateFilled(String value) => state = state.copyWith(dateFilled: value);
  void updateOtherComments(String value) => state = state.copyWith(otherComments: value);
  void updateTotalWells(int value) => state = state.copyWith(totalWells: value);
  void updateTotalMeters(double value) => state = state.copyWith(totalMeters: value);

  void toggleParticipant(int participantId, bool isChecked) {
    final updated = [...state.selectedParticipantIds];
    if (isChecked) {
      if (!updated.contains(participantId)) updated.add(participantId);
    } else {
      updated.remove(participantId);
    }
    state = state.copyWith(selectedParticipantIds: updated);
  }

  void addPhoto(File file) {
    final newPhoto = DrillingLogPhoto(
      preview: file.path,
      name: file.path.split('/').last,
      file: file,
    );
    state = state.copyWith(photos: [...state.photos, newPhoto]);
  }

  void removePhoto(String previewPath) {
    final updated = state.photos.where((p) => p.preview != previewPath).toList();
    state = state.copyWith(photos: updated);
  }

  void addSignature(int participantId, Uint8List imageBytes) {
    final base64String = base64Encode(imageBytes);
    final dataUrl = 'data:image/png;base64,$base64String';
    state = state.copyWith(signatures: {...state.signatures, participantId: dataUrl});
  }

  Future<void> saveAsync() async {
  final dto = DrillingLogFormSaveDto(
    formId: _params.formId == 0 ? null : _params.formId,
    projectId: _params.projectId,
    dateFilled: state.dateFilled,
    crewName: state.crewName,
    totalWells: state.totalWells,
    totalMeters: state.totalMeters,
    participants: state.selectedParticipantIds
        .map((id) => FormParticipantDto(participantId: id))
        .toList(),
    creatorId: userId,
  );

  int? formId;
  if (_params.formId == 0) {
    formId = await _drillingLogFormService.saveDrillingLogForm(dto);
    if (formId == null) throw Exception('Save failed');
  } else {
    formId = _params.formId;
    await _drillingLogFormService.updateDrillingLogForm(formId, dto);
  }

  if (state.formId == 0) {
    _params = _params.copyWith(formId: formId);
    state = state.copyWith(formId: formId);
  }

  for (final photo in state.photos) {
    if (photo.file != null) {
      await _formService.uploadFormPhoto(formId, photo.file);
    }
  }

  for (final entry in state.signatures.entries) {
    final participantId = entry.key;
    final dataUrl = entry.value;
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
