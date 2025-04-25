import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../data/form_service.dart';
import '../data/dtos/filled_form_dto.dart';
import '../data/dtos/filled_forms_filter.dart';


final filledFormsProvider = StateNotifierProvider.autoDispose
    .family<FilledFormsViewModel, List<FilledFormDto>, FilledFormsFilter>(
  (ref, filter) => FilledFormsViewModel(ref.read(formServiceProvider), filter),
);

class FilledFormsViewModel extends StateNotifier<List<FilledFormDto>> {
  final FormService _service;
  final FilledFormsFilter filter;

  FilledFormsViewModel(this._service, this.filter) : super([]) {
    loadForms();
  }

 Future<void> loadForms() async {
    state = await _service.getFilledForms(filter);
  }
}