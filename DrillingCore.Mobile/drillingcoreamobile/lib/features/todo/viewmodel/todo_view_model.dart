import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../core/services/user_session.dart';
import '../data/todo_service.dart';
import '../models/unsigned_form_dto.dart';

final todoViewModelProvider =
    StateNotifierProvider<ToDoViewModel, List<UnsignedFormDto>>(
  (ref) => ToDoViewModel(ref.read(todoServiceProvider)),
);

class ToDoViewModel extends StateNotifier<List<UnsignedFormDto>> {
  final ToDoService _service;
  final _session = UserSession();

  ToDoViewModel(this._service) : super([]) {
    _loadForms();
  }

  Future<void> _loadForms() async {
    final session = await _session.getSession();
    if (session == null) return;

    final userId = session['userId'] as int;
    final forms = await _service.getUnsignedForms(userId);
    state = forms;
  }

  Future<void> refreshAsync() async {
    final session = await _session.getSession();
    if (session == null) return;
    final userId = session['userId'] as int;
  final forms = await _service.getUnsignedForms(userId);
  state = forms;
}
}
