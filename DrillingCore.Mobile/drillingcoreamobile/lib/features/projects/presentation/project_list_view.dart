import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../viewmodel/project_view_model.dart';
import '../models/project_model.dart';
import 'package:intl/intl.dart';
class ProjectListView extends ConsumerWidget {
  const ProjectListView({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final projects = ref.watch(projectViewModelProvider);
    final viewModel = ref.read(projectViewModelProvider.notifier);

    return RefreshIndicator(
      onRefresh: viewModel.refresh,
      child: ListView.builder(
        padding: const EdgeInsets.all(12),
        itemCount: projects.length,
        itemBuilder: (context, index) {
          final project = projects[index];
          return _ProjectCard(project: project);
        },
      ),
    );
  }
}

class _ProjectCard extends StatelessWidget {
  final ProjectDto project;

  const _ProjectCard({required this.project});

  @override
  Widget build(BuildContext context) {
    final dateFormat = DateFormat('yyyy-MM-dd');
    final startDate = dateFormat.format(project.startDate);
    final endDate = project.endDate != null ? dateFormat.format(project.endDate!) : '—';

    return InkWell(
  onTap: () {
    GoRouter.of(context).push('/form-types?projectId=${project.id}');
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
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            project.name,
            style: const TextStyle(
              fontSize: 18,
              fontWeight: FontWeight.w600,
              color: Colors.teal,
            ),
          ),
          const SizedBox(height: 8),
          _buildRow('📍 Location', project.location),
          _buildRow('📅 Start', startDate),
          _buildRow('🏁 End', endDate),
          _buildRow('🏢 Client', project.client),
          _buildRow('📌 Status', project.status),
          if (project.hasCampOrHotel)
            const Padding(
              padding: EdgeInsets.only(top: 6),
              child: Text(
                '🏕️ Includes Camp or Hotel',
                style: TextStyle(fontSize: 13, fontStyle: FontStyle.italic),
              ),
            ),
        ],
      ),
    ),
  ),
);
  }

  Widget _buildRow(String label, String value) {
    return Padding(
      padding: const EdgeInsets.only(top: 4),
      child: Row(
        children: [
          SizedBox(width: 90, child: Text(label, style: const TextStyle(fontSize: 14, fontWeight: FontWeight.w500))),
          Expanded(
            child: Text(
              value,
              style: const TextStyle(fontSize: 14),
              overflow: TextOverflow.ellipsis,
            ),
          ),
        ],
      ),
    );
  }
}
