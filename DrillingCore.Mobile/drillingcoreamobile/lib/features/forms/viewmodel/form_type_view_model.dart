import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../data/form_service.dart';
import '../data/dtos/form_type_dto.dart';

final formTypeViewModelProvider =
    StateNotifierProvider<FormTypeViewModel, List<FormTypeDto>>(
  (ref) => FormTypeViewModel(ref.read(formServiceProvider)),
);

class FormTypeViewModel extends StateNotifier<List<FormTypeDto>> {
  final FormService _service;

  FormTypeViewModel(this._service) : super([]) {
    loadFormTypes();
  }

  Future<void> loadFormTypes() async {
    final types = await _service.getFormTypes();
    state = types;
  }

   Future<void> refresh() async {
    await loadFormTypes();
  }
}
