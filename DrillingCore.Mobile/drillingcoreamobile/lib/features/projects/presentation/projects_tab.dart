import 'package:flutter/material.dart';
import 'project_list_view.dart';

class ProjectsTab extends StatelessWidget {
  const ProjectsTab({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Projects'),
        backgroundColor: Colors.teal,
        foregroundColor: Colors.white,
      ),
      body: const ProjectListView(),
    );
  }
}
