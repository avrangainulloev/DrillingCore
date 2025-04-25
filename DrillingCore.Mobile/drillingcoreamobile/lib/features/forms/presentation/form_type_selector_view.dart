import 'package:drillingcoreamobile/features/forms/data/dtos/form_type_dto.dart';
import 'package:drillingcoreamobile/features/forms/viewmodel/form_type_view_model.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

 

class FormTypeSelectorView extends ConsumerWidget {
  final int projectId;

  const FormTypeSelectorView({super.key, required this.projectId});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final formTypes = ref.watch(formTypeViewModelProvider);
    final viewModel = ref.read(formTypeViewModelProvider.notifier);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Select Form Type'),
        backgroundColor: Colors.teal,
        foregroundColor: Colors.white,
        leading: IconButton(
          icon: const Icon(Icons.arrow_back),
          onPressed: () => context.pop(),
        ),
      ),
      body: RefreshIndicator(
        onRefresh: viewModel.refresh,
        child: ListView.builder(
          padding: const EdgeInsets.all(12),
          itemCount: formTypes.length,
          itemBuilder: (context, index) {
            final type = formTypes[index];
            return _FormTypeCard(type: type, projectId: projectId);
          },
        ),
      ),
    );
  }
}

class _FormTypeCard extends StatelessWidget {
  final FormTypeDto type;
  final int projectId;

  const _FormTypeCard({required this.type, required this.projectId});

  @override
  Widget build(BuildContext context) {
    return InkWell(
      onTap: () {
        GoRouter.of(context).push('/forms?projectId=$projectId&formTypeId=${type.id}');
      },
      child: Card(
        elevation: 2,
        margin: const EdgeInsets.only(bottom: 12),
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(12),
          side: BorderSide(color: Colors.grey.shade300),
        ),
        child: Padding(
          padding: const EdgeInsets.all(16),
          child: Row(
            children: [
              const Icon(Icons.description, color: Colors.teal),
              const SizedBox(width: 12),
              Expanded(
                child: Text(
                  type.name,
                  style: const TextStyle(fontSize: 16, fontWeight: FontWeight.w500),
                ),
              ),
              const Icon(Icons.arrow_forward_ios, size: 16, color: Colors.grey),
            ],
          ),
        ),
      ),
    );
  }
}
