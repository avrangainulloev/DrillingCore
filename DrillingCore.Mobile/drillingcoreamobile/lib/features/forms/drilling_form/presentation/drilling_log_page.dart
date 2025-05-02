// 📄 drilling_log_page.dart
import 'dart:convert';
import 'dart:io';
import 'dart:typed_data';

import 'package:drillingcoreamobile/core/constants/constants.dart';
import 'package:drillingcoreamobile/features/forms/inspection_forms/models/drill_inspection_form_model.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:image_picker/image_picker.dart';
import 'package:path_provider/path_provider.dart';
import 'package:flutter_image_compress/flutter_image_compress.dart';
import 'package:path/path.dart' as path;

import '../../common/widgets/signaturemodal.dart';
import '../models/drilling_log_form_model.dart';
import '../viewmodel/drilling_log_viewmodel.dart';

class DrillingLogPage extends ConsumerStatefulWidget {
  final int formId;
  final int formTypeId;
  final int projectId;

  const DrillingLogPage({super.key, required this.formId, required this.formTypeId, required this.projectId});

  @override
  ConsumerState<DrillingLogPage> createState() => _DrillingLogPageState();
}

class _DrillingLogPageState extends ConsumerState<DrillingLogPage> {
  late final DrillingLogParams _params;
  final _crewNameController = TextEditingController();
  final _wellsController = TextEditingController();
  final _metersController = TextEditingController();
  final _otherCommentsController = TextEditingController();
    final _participantsScrollController = ScrollController();
  final ImagePicker _picker = ImagePicker();

  bool _isSaving = false;
  String participantSearch = '';
  @override
  void initState() {
    super.initState();
    _params = DrillingLogParams(
      formId: widget.formId,
      formTypeId: widget.formTypeId,
      projectId: widget.projectId,
    );
    Future.microtask(() => ref.read(drillingLogViewModelProvider(_params).notifier).initialize(_params));
  }

  @override
  void dispose() {
    _crewNameController.dispose();
    _wellsController.dispose();
    _metersController.dispose();
    _otherCommentsController.dispose();
     _participantsScrollController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final formState = ref.watch(drillingLogViewModelProvider(_params));
    final notifier = ref.read(drillingLogViewModelProvider(_params).notifier);

    _crewNameController.text = formState.crewName;
    _wellsController.text = formState.totalWells.toString();
    _metersController.text = formState.totalMeters.toString();

    return Scaffold(
      backgroundColor: const Color(0xFFF8FAFC),
      appBar: AppBar(
        title: const Text('🛢 Drilling Log'),
        backgroundColor: Colors.indigo.shade800,
        foregroundColor: Colors.white,
      ),
      body: Padding(
        padding: const EdgeInsets.all(16),
        child: ListView(
          children: [
            _sectionLabel('👷 Crew Name'),
            _readonlyInput(_crewNameController, notifier.updateCrewName),
               // 🧑‍🤝‍🧑 Добавь участников здесь
           _buildParticipantsList(formState, notifier),
            const SizedBox(height: 20),
            const SizedBox(height: 20),
            _sectionLabel('🕳 Total Wells'),
            _readonlyInput(_wellsController, (v) => notifier.updateTotalWells(int.tryParse(v) ?? 0)),
            const SizedBox(height: 20),
            _sectionLabel('📏 Total Meters'),
            _readonlyInput(_metersController, (v) => notifier.updateTotalMeters(double.tryParse(v) ?? 0.0)),
            const SizedBox(height: 20),
            _buildOtherComments(notifier),
            const SizedBox(height: 20),
            _buildPhotos(formState, notifier),
            const SizedBox(height: 20),
            _buildSignatures(formState, notifier),
            const SizedBox(height: 30),
            _buildSaveAndCloseButtons(notifier),
          ],
        ),
      ),
    );
  }

  Widget _readonlyInput(TextEditingController controller, void Function(String) onChanged, {int maxLines = 1}) {
    return TextField(
      controller: controller,
      maxLines: maxLines,
      onChanged: onChanged,
      decoration: InputDecoration(
        filled: true,
        fillColor: const Color(0xFFE3F2FD),
        border: OutlineInputBorder(borderRadius: BorderRadius.circular(10)),
      ),
    );
  }

  Widget _sectionLabel(String label) => Text(
    label,
    style: const TextStyle(fontSize: 17, fontWeight: FontWeight.bold, color: Color(0xFF1A237E)),
  );

  Widget _buildParticipantsList(DrillingLogFormModel formState, DrillingLogViewModel notifier) {
    final filteredParticipants = [...formState.allParticipants]
      ..retainWhere((p) => p.fullName.toLowerCase().contains(participantSearch.toLowerCase()));

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        _sectionLabel('🧑‍🤝‍🧑 Participants'),
        const SizedBox(height: 4),
        TextField(
          onChanged: (v) => setState(() => participantSearch = v),
          decoration: InputDecoration(
            hintText: '🔍 Search participants...',
            fillColor: const Color(0xFFECEFF1),
            filled: true,
            border: OutlineInputBorder(borderRadius: BorderRadius.circular(8), borderSide: BorderSide.none),
          ),
        ),
        const SizedBox(height: 8),
        Container(
          height: 220,
          padding: const EdgeInsets.symmetric(horizontal: 8),
          decoration: BoxDecoration(
            color: Colors.white,
            border: Border.all(color: Colors.blue.shade400, width: 1.5),
            borderRadius: BorderRadius.circular(12),
          ),
          child: Scrollbar(
            controller: _participantsScrollController,
            thumbVisibility: true,
            child: ListView.builder(
              controller: _participantsScrollController,
              itemCount: filteredParticipants.length,
              itemBuilder: (context, index) {
                final p = filteredParticipants[index];
                final selected = formState.selectedParticipantIds.contains(p.participantId);
                return Row(
                  children: [
                    Checkbox(
                      value: selected,
                      onChanged: (v) => notifier.toggleParticipant(p.participantId, v ?? false),
                      visualDensity: const VisualDensity(horizontal: -4, vertical: -4),
                      materialTapTargetSize: MaterialTapTargetSize.shrinkWrap,
                      checkColor: Colors.white,
                      fillColor: WidgetStateProperty.resolveWith((states) {
                        if (states.contains(WidgetState.selected)) {
                          return Colors.indigo;
                        }
                        return Colors.blue.shade100;
                      }),
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(4),
                      ),
                    ),
                    Expanded(
                      child: Text(p.fullName, style: const TextStyle(fontSize: 14)),
                    ),
                  ],
                );
              },
            ),
          ),
        ),
      ],
    );
  }

  Widget _buildOtherComments(DrillingLogViewModel notifier) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        _sectionLabel('📝 Other Comments'),
        TextFormField(
          initialValue: ref.read(drillingLogViewModelProvider(_params)).otherComments,
          maxLines: 3,
          onChanged: notifier.updateOtherComments,
          decoration: InputDecoration(
            filled: true,
            fillColor: const Color(0xFFE3F2FD),
            border: OutlineInputBorder(borderRadius: BorderRadius.circular(10)),
          ),
        ),
      ],
    );
  }

  Widget _buildPhotos(DrillingLogFormModel formState, DrillingLogViewModel notifier) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        _sectionLabel('📷 Photos'),
        Wrap(
          spacing: 8,
          runSpacing: 8,
          children: [
            ...formState.photos.map((p) {
              final isLocal = p.file != null;
              final url = p.preview.startsWith('http') ? p.preview : '$kApiBaseUrl${p.preview.startsWith('/') ? p.preview.substring(1) : p.preview}';
              return Stack(
                children: [
                  GestureDetector(
                    onTap: () => showDialog(
                      context: context,
                      builder: (_) => Dialog(
                        backgroundColor: Colors.transparent,
                        child: isLocal
                          ? Image.file(File(p.preview), fit: BoxFit.contain)
                          : Image.network(url, fit: BoxFit.contain),
                      ),
                    ),
                    child: ClipRRect(
                      borderRadius: BorderRadius.circular(8),
                      child: isLocal
                        ? Image.file(File(p.preview), width: 100, height: 100, fit: BoxFit.cover)
                        : Image.network(url, width: 100, height: 100, fit: BoxFit.cover),
                    ),
                  ),
                  Positioned(
                    top: 0,
                    right: 0,
                    child: GestureDetector(
                      onTap: () => notifier.removePhoto(p.preview),
                      child: Container(
                        decoration: const BoxDecoration(color: Colors.black54, shape: BoxShape.circle),
                        padding: const EdgeInsets.all(4),
                        child: const Icon(Icons.close, size: 18, color: Colors.white),
                      ),
                    ),
                  ),
                ],
              );
            }),
            GestureDetector(
              onTap: () => _pickImage(notifier),
              child: Container(
                width: 100,
                height: 100,
                decoration: BoxDecoration(
                  border: Border.all(color: Colors.grey.shade400),
                  borderRadius: BorderRadius.circular(8),
                  color: Colors.grey.shade100,
                ),
                child: const Icon(Icons.add_a_photo, color: Colors.black45),
              ),
            ),
          ],
        ),
      ],
    );
  }

  Future<void> _pickImage(DrillingLogViewModel notifier) async {
    final source = await showDialog<ImageSource>(
      context: context,
      builder: (_) => AlertDialog(
        title: const Text('Add Photo'),
        content: const Text('Choose image source:'),
        actions: [
          TextButton(onPressed: () => Navigator.pop(context, ImageSource.camera), child: const Text('Camera')),
          TextButton(onPressed: () => Navigator.pop(context, ImageSource.gallery), child: const Text('Gallery')),
        ],
      ),
    );
    if (source == null) return;

    try {
      if (source == ImageSource.gallery) {
        final pickedFiles = await _picker.pickMultiImage();
        if (pickedFiles.isEmpty) return;
        for (final file in pickedFiles) {
          final processed = await _compressAndConvertToJpeg(File(file.path));
          if (processed != null) notifier.addPhoto(processed);
        }
      } else {
        final pickedFile = await _picker.pickImage(source: source);
        if (pickedFile == null) return;
        final processed = await _compressAndConvertToJpeg(File(pickedFile.path));
        if (processed != null) notifier.addPhoto(processed);
      }
    } catch (e) {
      print('❌ Error picking image: $e');
    }
  }

  Future<File?> _compressAndConvertToJpeg(File file) async {
    try {
      final dir = await getTemporaryDirectory();
      final targetPath = path.join(dir.path, '${DateTime.now().millisecondsSinceEpoch}.jpg');
      final xfile = await FlutterImageCompress.compressAndGetFile(
        file.absolute.path,
        targetPath,
        format: CompressFormat.jpeg,
        quality: 85,
      );
      return xfile != null ? File(xfile.path) : null;
    } catch (e) {
      print('❌ Compression error: $e');
      return null;
    }
  }

  Widget _buildSignatures(DrillingLogFormModel formState, DrillingLogViewModel notifier) {
  return Column(
    crossAxisAlignment: CrossAxisAlignment.start,
    children: [
      _sectionLabel('✍️ Signatures'),
      const SizedBox(height: 6),
      ...formState.selectedParticipantIds.map((id) {
        final participant = formState.allParticipants.firstWhere(
          (p) => p.participantId == id,
          orElse: () => ProjectParticipant(participantId: id, userId: 0, fullName: 'Unknown', groupName: null, endDate: null),
        );

        final dataUrl = formState.signatures[id];
        Widget? signatureWidget;

        if (dataUrl != null && dataUrl.isNotEmpty) {
          if (dataUrl.startsWith('data:image')) {
            // base64 подпись (локально)
            try {
              final bytes = base64Decode(dataUrl.split(',').last);
              signatureWidget = Image.memory(bytes, width: 120, height: 50, fit: BoxFit.contain);
            } catch (_) {}
          } else {
            // загруженная подпись
            final fullUrl = '$kApiBaseUrl${dataUrl.startsWith('/') ? dataUrl.substring(1) : dataUrl}';
            signatureWidget = Image.network(fullUrl, width: 120, height: 50, fit: BoxFit.contain);
          }
        }

        return Container(
          margin: const EdgeInsets.only(bottom: 12),
          padding: const EdgeInsets.all(12),
          decoration: BoxDecoration(
            border: Border.all(color: Colors.indigo.shade300, width: 1.5),
            borderRadius: BorderRadius.circular(10),
            color: Colors.blue.shade50,
          ),
          child: Row(
            children: [
              Expanded(child: Text(participant.fullName, style: const TextStyle(fontWeight: FontWeight.w600, fontSize: 15))),
              if (signatureWidget != null)
                Padding(
                  padding: const EdgeInsets.only(right: 12),
                  child: signatureWidget,
                ),
              ElevatedButton(
                onPressed: () async {
                  final data = await showDialog<Uint8List>(
                    context: context,
                    builder: (_) => SignatureModal(onSave: (data) => Navigator.of(context, rootNavigator: true).pop(data)),
                  );
                  if (data != null) notifier.addSignature(participant.participantId, data);
                },
                style: ElevatedButton.styleFrom(
                  backgroundColor: Colors.orange,
                  padding: const EdgeInsets.symmetric(horizontal: 18, vertical: 10),
                  shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(8)),
                ),
                child: const Text('Sign'),
              ),
            ],
          ),
        );
      }),
    ],
  );
}

  Widget _buildSaveAndCloseButtons(DrillingLogViewModel notifier) {
    return Row(
      children: [
        Expanded(
          child: ElevatedButton.icon(
            onPressed: _isSaving ? null : () async {
              setState(() => _isSaving = true);
              await notifier.saveAsync();
              if (mounted) context.pop(true);
            },
            icon: _isSaving ? const CircularProgressIndicator(color: Colors.white) : const Icon(Icons.save),
            label: Text(_isSaving ? 'Saving...' : 'Save'),
            style: ElevatedButton.styleFrom(backgroundColor: Colors.green, foregroundColor: Colors.white),
          ),
        ),
        const SizedBox(width: 10),
        Expanded(
          child: ElevatedButton.icon(
            onPressed: () => context.pop(),
            icon: const Icon(Icons.close),
            label: const Text('Close'),
            style: ElevatedButton.styleFrom(backgroundColor: Colors.red.shade600, foregroundColor: Colors.white),
          ),
        ),
      ],
    );
  }
}
