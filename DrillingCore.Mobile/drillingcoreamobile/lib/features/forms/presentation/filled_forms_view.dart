
import 'package:drillingcoreamobile/features/forms/data/dtos/filled_forms_filter.dart';
import 'package:drillingcoreamobile/features/forms/viewmodel/filled_forms_view_model.dart';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:intl/intl.dart';

class FilledFormsView extends ConsumerStatefulWidget {
  final int projectId;
  final int formTypeId;

  const FilledFormsView({
    super.key,
    required this.projectId,
    required this.formTypeId,
  });

  @override
  ConsumerState<FilledFormsView> createState() => _FilledFormsViewState();
}

class _FilledFormsViewState extends ConsumerState<FilledFormsView> {
  bool showOnlyMine = false;

  @override
  Widget build(BuildContext context) {
    final filter = FilledFormsFilter(
      projectId: widget.projectId,
      formTypeId: widget.formTypeId,
      onlyMine: showOnlyMine,
    );

    final forms = ref.watch(filledFormsProvider(filter));

    return Scaffold(
      appBar: AppBar(
        title: const Text('Filled Forms'),
        leading: IconButton(
          icon: const Icon(Icons.arrow_back),
          onPressed: () => context.pop(),
        ),
        backgroundColor: Colors.teal,
        foregroundColor: Colors.white,
        actions: [
          Row(
            children: [
              const Text("Only mine", style: TextStyle(fontSize: 12)),
              Switch(
                value: showOnlyMine,
                onChanged: (val) => setState(() => showOnlyMine = val),
              ),
            ],
          ),
        ],
      ),
      body: forms.isEmpty
          ? const Center(child: CircularProgressIndicator())
          : ListView.separated(
              padding: const EdgeInsets.only(top: 12, bottom: 80),
              itemCount: forms.length,
              separatorBuilder: (_, __) => const Divider(height: 1),
              itemBuilder: (_, index) {
                final form = forms[index];

                return ListTile(
                  contentPadding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
                  title: (form.crewName ?? '').trim().isNotEmpty
                      ? Text(
                          form.crewName!,
                          style: const TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
                        )
                      : null,
                  subtitle: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Row(
                        children: [
                          const Text('👤 ', style: TextStyle(fontSize: 18)),
                          Text(form.creatorName),
                        ],
                      ),
                      const SizedBox(height: 4),
                      Row(
                        children: [
                          const Text('🕒 ', style: TextStyle(fontSize: 18)),
                          Text(DateFormat.yMd().add_Hm().format(form.dateFilled)),
                        ],
                      ),
                    ],
                  ),
                  trailing: Text(
                    form.status ?? '',
                    style: TextStyle(
                      color: _getStatusColor(form.status),
                      fontWeight: FontWeight.w500,
                    ),
                  ),
                  onTap: () {
                    if ([1, 2, 4].contains(widget.formTypeId)) {
                      // context.push(
                      //   '/form',
                      //   extra: {
                      //     'formId': form.id,
                      //     'formTypeId': widget.formTypeId,
                      //     'projectId': widget.projectId,
                       context.push('/form?formId=${form.id}&formTypeId=${widget.formTypeId}&projectId=${widget.projectId}');
                    } else {
                      ScaffoldMessenger.of(context).showSnackBar(
                        const SnackBar(content: Text('This form type is not yet supported')),
                      );
                    }
                  },
                );
              },
            ),
      floatingActionButton: FloatingActionButton(
        onPressed: () {
          if ([1, 2, 4].contains(widget.formTypeId)) {
           context.push('/form?formId=0&formTypeId=${widget.formTypeId}&projectId=${widget.projectId}');
          }
        },
        backgroundColor: Colors.teal,
        child: const Icon(Icons.add),
      ),
    );
  }

  Color _getStatusColor(String? status) {
    switch (status?.toLowerCase()) {
      case 'completed':
        return Colors.green;
      case 'pending':
        return Colors.orange;
      case 'draft':
        return Colors.grey;
      default:
        return Colors.black54;
    }
  }
}
