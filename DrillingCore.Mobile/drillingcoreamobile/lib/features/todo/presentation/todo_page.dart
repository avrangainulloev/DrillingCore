import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import '../viewmodel/todo_view_model.dart';
import '../models/unsigned_form_dto.dart';

class ToDoPage extends ConsumerStatefulWidget {
  const ToDoPage({super.key});

  @override
  ConsumerState<ToDoPage> createState() => _ToDoPageState();
}

class _ToDoPageState extends ConsumerState<ToDoPage> {
  @override
  void initState() {
    super.initState();
    Future.microtask(() => ref.read(todoViewModelProvider.notifier).loadAsync());
  }

  @override
Widget build(BuildContext context) {
  final forms = ref.watch(todoViewModelProvider);

  return Scaffold(
    appBar: AppBar(
      title: const Text('To Do'),
      backgroundColor: Colors.teal.shade700,
      foregroundColor: Colors.white,
    ),
    body: RefreshIndicator(
      onRefresh: () async {
        await ref.read(todoViewModelProvider.notifier).refreshAsync();
      },
      child: forms.isEmpty
          ? ListView(
              physics: const AlwaysScrollableScrollPhysics(),
              children: const [
                SizedBox(height: 200),
                Center(child: Text('No forms to sign 🎉')),
              ],
            )
          : ListView.builder(
              physics: const AlwaysScrollableScrollPhysics(),
              itemCount: forms.length,
              itemBuilder: (_, index) {
                final form = forms[index];
                return _FormListTile(form: form);
              },
            ),
    ),
  );
}

}

class _FormListTile extends StatelessWidget {
  final UnsignedFormDto form;

  const _FormListTile({required this.form});

  String formatDateTime(String isoString) {
    try {
      final dt = DateTime.parse(isoString);
      return '${dt.year}-${dt.month.toString().padLeft(2, '0')}-${dt.day.toString().padLeft(2, '0')} ${dt.hour.toString().padLeft(2, '0')}:${dt.minute.toString().padLeft(2, '0')}';
    } catch (_) {
      return isoString;
    }
  }

  String _getEmoji(int typeId) {
    switch (typeId) {
      case 1:
        return '🚛';
      case 2:
        return '🔧';
      case 3:
        return '🛡️';
      case 4:
        return '🏍️';
      case 5:
        return '🚰';
      default:
        return '📄';
    }
  }

  Color _getTextColor(int typeId) {
    switch (typeId) {
      case 1:
        return const Color(0xFF1E88E5);
      case 2:
        return const Color(0xFFD81B60);
      case 3:
        return const Color(0xFF43A047);
      case 4:
        return const Color(0xFFFB8C00);
      case 5:
        return const Color(0xFF8E24AA);
      default:
        return Colors.teal;
    }
  }

  @override
  Widget build(BuildContext context) {
    final emoji = _getEmoji(form.formTypeId);
    final color = _getTextColor(form.formTypeId);

    return InkWell(
     onTap: () async {
  final result = await context.push(
    '/form?formId=${form.formId}&formTypeId=${form.formTypeId}&projectId=${form.projectId}',
  );

  // Вызов обновления, если форма была подписана
  if (result == true) {
    // Найдём nearest ConsumerState для вызова refresh
    final state = context.findAncestorStateOfType<_ToDoPageState>();
    state?.ref.read(todoViewModelProvider.notifier).refreshAsync();
  }
},
      child: Container(
        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
        child: Stack(
          children: [
            Positioned(
              right: 0,
              top: 0,
              child: Container(
                padding:
                    const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
                decoration: BoxDecoration(
                  color: const Color(0xFFfdecea),
                  borderRadius: BorderRadius.circular(12),
                ),
                child: const Text(
                  'Required to sign',
                  style: TextStyle(
                    color: Color(0xFFd32f2f),
                    fontSize: 12,
                    fontWeight: FontWeight.w600,
                  ),
                ),
              ),
            ),
            Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const SizedBox(height: 4),
                Text(
                  '$emoji ${form.formTypeName}',
                  style: TextStyle(
                    fontSize: 16,
                    fontWeight: FontWeight.bold,
                    color: color,
                  ),
                ),
                const SizedBox(height: 4),
                Text(
                  form.projectName,
                  style: const TextStyle(
                    fontSize: 14,
                    color: Colors.black87,
                  ),
                ),
                const SizedBox(height: 10),
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Text(
                      '👤 ${form.creatorName ?? ''}',
                      style: TextStyle(
                        fontSize: 13,
                        color: Colors.grey.shade700,
                      ),
                    ),
                    Text(
                      '🕒 ${formatDateTime(form.updatedAt ?? '')}',
                      style: TextStyle(
                        fontSize: 12,
                        color: Colors.grey.shade500,
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 8),
                const Divider(height: 1, thickness: 1),
              ],
            ),
          ],
        ),
      ),
    );
  }
}
