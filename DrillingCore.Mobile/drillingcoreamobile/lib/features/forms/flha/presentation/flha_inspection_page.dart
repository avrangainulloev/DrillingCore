import 'dart:convert';
import 'dart:io';
import 'dart:typed_data';

import 'package:drillingcoreamobile/core/constants/constants.dart';
import 'package:flutter/material.dart';
import 'package:flutter_image_compress/flutter_image_compress.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:image_picker/image_picker.dart';
import 'package:path_provider/path_provider.dart';

import '../../common/widgets/signaturemodal.dart';
import '../../inspection_forms/models/drill_inspection_form_model.dart';
import '../models/flha_inspection_form_model.dart';
import '../viewmodel/flha_inspection_view_model.dart';
import 'package:path/path.dart' as path;
class FLHAInspectionPage extends ConsumerStatefulWidget {
  final int formId;
  final int formTypeId;
  final int projectId;

  const FLHAInspectionPage({super.key, required this.formId, required this.formTypeId, required this.projectId});

  @override
  ConsumerState<FLHAInspectionPage> createState() => _FLHAInspectionPageState();
}

class _FLHAInspectionPageState extends ConsumerState<FLHAInspectionPage> {
  late final FLHAInspectionParams _params;
  final _crewNameController = TextEditingController();
  final _taskDescriptionController = TextEditingController();
  final _otherCommentsController = TextEditingController();
  final ImagePicker _picker = ImagePicker();
  final _participantsScrollController = ScrollController();

  bool _isSaving = false;
  String participantSearch = '';

  @override
  void initState() {
    super.initState();
    _params = FLHAInspectionParams(formId: widget.formId, formTypeId: widget.formTypeId, projectId: widget.projectId);
    Future.microtask(() => ref.read(flhaInspectionViewModelProvider(_params).notifier).initialize(_params));
  }

  @override
  void dispose() {
    _crewNameController.dispose();
    _taskDescriptionController.dispose();
    _otherCommentsController.dispose();
    _participantsScrollController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final formState = ref.watch(flhaInspectionViewModelProvider(_params));
    final notifier = ref.read(flhaInspectionViewModelProvider(_params).notifier);

    _crewNameController.text = formState.crewName;
    _taskDescriptionController.text = formState.taskDescription;
    _otherCommentsController.text = formState.otherComments;

    return Scaffold(
      backgroundColor: const Color(0xFFF8FAFC),
      appBar: AppBar(
        title: const Text('📝 FLHA Inspection'),
        backgroundColor: Colors.indigo.shade800,
        foregroundColor: Colors.white,
      ),
      body: Padding(
        padding: const EdgeInsets.all(16),
        child: ListView(
          children: [
            _buildCrewNameAndTaskDescription(notifier),
            const SizedBox(height: 20),
            _buildParticipantsList(formState, notifier),
            const SizedBox(height: 20),
            _buildHazardGroups(formState, notifier),
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

Widget _buildCrewNameAndTaskDescription(FLHAInspectionViewModel notifier) {
  final formState = ref.watch(flhaInspectionViewModelProvider(_params)); // добавлено здесь!

  return Column(
    crossAxisAlignment: CrossAxisAlignment.start,
    children: [
      _sectionLabel('👷 Crew Name'),
      const SizedBox(height: 4),
      _readonlyInput(_crewNameController, notifier.updateCrewName),
      const SizedBox(height: 10),

      _sectionLabel('📅 Date Filled'),
      const SizedBox(height: 4),
      TextField(
        readOnly: true,
        controller: TextEditingController(text: formState.dateFilled),
        decoration: InputDecoration(
          filled: true,
          fillColor: const Color(0xFFE3F2FD),
          border: OutlineInputBorder(borderRadius: BorderRadius.circular(10)),
          suffixIcon: const Icon(Icons.calendar_today),
        ),
        onTap: () async {
          final selected = await showDatePicker(
            context: context,
            initialDate: DateTime.tryParse(formState.dateFilled) ?? DateTime.now(),
            firstDate: DateTime(2000),
            lastDate: DateTime(2100),
          );
          if (selected != null) {
            notifier.updateDateFilled(selected.toIso8601String().split('T').first);
          }
        },
      ),

      const SizedBox(height: 10),
      _sectionLabel('📝 Task Description'),
      const SizedBox(height: 4),
      _readonlyInput(_taskDescriptionController, notifier.updateTaskDescription, maxLines: 2),
    ],
  );
}

  Widget _buildParticipantsList(FLHAInspectionFormModel formState, FLHAInspectionViewModel notifier) {
    final filteredParticipants = [...formState.allParticipants]
      ..retainWhere((p) => p.fullName.toLowerCase().contains(participantSearch.toLowerCase()));
 


    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
       _sectionLabel('🧑‍🤝‍🧑 Participants'),
            const SizedBox(height: 4),
            _searchBar(),
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

  Widget _buildHazardGroups(FLHAInspectionFormModel formState, FLHAInspectionViewModel notifier) {
  final themeBorder = OutlineInputBorder(
    borderRadius: BorderRadius.circular(8),
    borderSide: BorderSide(color: Colors.blue.shade300),
  );

  return Column(
    crossAxisAlignment: CrossAxisAlignment.start,
    children: [
      _sectionLabel('⚠️ Hazards Templates'),
const SizedBox(height: 8),
...formState.hazardTemplates.map((hazard) {
  final selected = formState.selectedHazardIds.contains(hazard.id);
  return Column(
    crossAxisAlignment: CrossAxisAlignment.start,
    children: [
      CheckboxListTile(
        title: Text(hazard.hazardText),
        value: selected,
        onChanged: (v) => notifier.toggleHazard(hazard.id!, v ?? false),
      ),
      if (selected)
        Padding(
          padding: const EdgeInsets.only(bottom: 12),
          child: TextFormField(
            initialValue: hazard.controlMeasures,
            onChanged: (value) => notifier.updateHazardControlMeasure(hazard.id!, value),
            maxLines: 2,
            decoration: InputDecoration(
              hintText: 'Control Measures...',
              filled: true,
              fillColor: const Color(0xFFE3F2FD),
              border: themeBorder,
            ),
          ),
        ),
    ],
  );
}),


     const SizedBox(height: 24),
_sectionLabel('🛠 Custom Hazards'),
const SizedBox(height: 8),
...formState.customHazards.map((hazard) {
  return Container(
    margin: const EdgeInsets.only(bottom: 12),
    padding: const EdgeInsets.all(12),
    decoration: BoxDecoration(
      color: Colors.blue.shade50,
      border: Border.all(color: Colors.blue.shade300, width: 1.5),
      borderRadius: BorderRadius.circular(10),
    ),
    child: Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        TextFormField(
          initialValue: hazard.hazardText,
          maxLines: 2,
          onChanged: (v) => notifier.updateCustomHazardText(hazard, v),
          decoration: InputDecoration(
            hintText: 'Hazard Description...',
            filled: true,
            fillColor: Colors.white,
            border: themeBorder,
          ),
        ),
        const SizedBox(height: 8),
        TextFormField(
          initialValue: hazard.controlMeasures,
          maxLines: 2,
          onChanged: (v) => notifier.updateCustomControlMeasure(hazard, v),
          decoration: InputDecoration(
            hintText: 'Control Measures...',
            filled: true,
            fillColor: Colors.white,
            border: themeBorder,
          ),
        ),
        const SizedBox(height: 8),
        ElevatedButton(
          onPressed: () => notifier.removeCustomHazard(hazard),
          style: ElevatedButton.styleFrom(
            backgroundColor: Colors.red.shade600,
            padding: const EdgeInsets.symmetric(horizontal: 24, vertical: 10),
            foregroundColor: Colors.white,
            shape: RoundedRectangleBorder(
              borderRadius: BorderRadius.circular(8),
            ),
          ),
          child: const Text('Remove'),
        ),
      ],
    ),
  );
}),
const SizedBox(height: 8),
ElevatedButton.icon(
  onPressed: () => notifier.addEmptyCustomHazard(),
  icon: const Icon(Icons.add),
  label: const Text('Add Hazard'),
  style: ElevatedButton.styleFrom(
    backgroundColor: Colors.blueGrey.shade700,
    padding: const EdgeInsets.symmetric(horizontal: 24, vertical: 12),
    shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(8)),
    foregroundColor: Colors.white,
  ),
),     
    ],
  );
}


Widget _buildOtherComments(FLHAInspectionViewModel notifier) {
  return Column(
    crossAxisAlignment: CrossAxisAlignment.start,
    children: [
      _sectionLabel('📝 Other Comments'),
      TextFormField(
        initialValue: ref.read(flhaInspectionViewModelProvider(_params)).otherComments,
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

  Widget _buildPhotos(FLHAInspectionFormModel formState, FLHAInspectionViewModel notifier) {
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
            final networkUrl = p.preview.startsWith('http')
                ? p.preview
                : '$kApiBaseUrl${p.preview.startsWith('/') ? p.preview.substring(1) : p.preview}';

            return Stack(
              children: [
                GestureDetector(
                  onTap: () {
                    showDialog(
                      context: context,
                      builder: (_) => Dialog(
                        backgroundColor: Colors.transparent,
                        child: isLocal
                            ? Image.file(File(p.preview), fit: BoxFit.contain)
                            : Image.network(
                                networkUrl,
                                fit: BoxFit.contain,
                                errorBuilder: (_, __, ___) => const Icon(Icons.broken_image, size: 60, color: Colors.grey),
                              ),
                      ),
                    );
                  },
                  child: ClipRRect(
                    borderRadius: BorderRadius.circular(8),
                    child: isLocal
                        ? Image.file(File(p.preview), width: 100, height: 100, fit: BoxFit.cover)
                        : Image.network(
                            networkUrl,
                            width: 100,
                            height: 100,
                            fit: BoxFit.cover,
                            errorBuilder: (_, __, ___) => Container(
                              width: 100,
                              height: 100,
                              color: Colors.grey.shade200,
                              child: const Icon(Icons.broken_image, color: Colors.grey),
                            ),
                          ),
                  ),
                ),
                Positioned(
                  top: 0,
                  right: 0,
                  child: GestureDetector(
                    onTap: () => notifier.removePhoto(p.preview),
                    child: Container(
                      decoration: const BoxDecoration(
                        color: Colors.black54,
                        shape: BoxShape.circle,
                      ),
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

Widget _buildSignatures(FLHAInspectionFormModel formState, FLHAInspectionViewModel notifier) {
  return Column(
    crossAxisAlignment: CrossAxisAlignment.start,
    children: [
      _sectionLabel('✍️ Signatures'),
      const SizedBox(height: 6),
      ...formState.selectedParticipantIds.map((id) {
        final participant = formState.allParticipants.firstWhere(
          (p) => p.participantId == id,
          orElse: () => ProjectParticipant(
            participantId: id,
            userId: 0,
            fullName: 'Unknown',
            groupName: null,
            endDate: null,
          ),
        );

        final signatureDataUrl = formState.signatures[id];
        Uint8List? signatureBytes;

        if (signatureDataUrl != null && signatureDataUrl.startsWith('data:image')) {
          try {
            final base64Str = signatureDataUrl.split(',').last;
            signatureBytes = base64Decode(base64Str);
          } catch (e) {
            print('❌ Failed to decode signature for participant $id: $e');
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
              Expanded(
                child: Text(
                  participant.fullName,
                  style: const TextStyle(
                    fontWeight: FontWeight.w600,
                    fontSize: 15,
                  ),
                ),
              ),
              if (signatureBytes != null)
                Padding(
                  padding: const EdgeInsets.only(right: 12),
                  child: Image.memory(
                    signatureBytes,
                    width: 120,
                    height: 50,
                    fit: BoxFit.contain,
                  ),
                ),
              ElevatedButton(
                onPressed: () async {
                  final data = await showDialog<Uint8List>(
                    context: context,
                    builder: (_) => SignatureModal(
                      onSave: (data) => Navigator.of(context, rootNavigator: true).pop(data),
                    ),
                  );
                  if (data != null) {
                    notifier.addSignature(participant.participantId, data);
                  }
                },
                style: ElevatedButton.styleFrom(
                  backgroundColor: Colors.orange,
                  padding: const EdgeInsets.symmetric(horizontal: 18, vertical: 10),
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(8),
                  ),
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


 Widget _buildSaveAndCloseButtons(FLHAInspectionViewModel notifier) {
  return Row(
    children: [
      Expanded(
        child: ElevatedButton.icon(
          onPressed: _isSaving
              ? null
              : () async {
                  final state = ref.read(flhaInspectionViewModelProvider(_params));

                  if (state.crewName.trim().isEmpty) {
                    _showError('❗ Crew name is required.');
                    return;
                  }

                  if (state.selectedParticipantIds.isEmpty) {
                    _showError('❗ At least one participant must be selected.');
                    return;
                  }

                  final hasTemplateHazard = state.selectedHazardIds.isNotEmpty;
                  final hasCustomHazard = state.customHazards.isNotEmpty;
                  if (!hasTemplateHazard && !hasCustomHazard) {
                    _showError('❗ Please add at least one hazard (template or custom).');
                    return;
                  }

                  setState(() => _isSaving = true);
                  await notifier.saveAsync();
                  if (mounted) context.pop(true);
                },
          icon: _isSaving
              ? const CircularProgressIndicator(color: Colors.white)
              : const Icon(Icons.save),
          label: Text(_isSaving ? 'Saving...' : 'Save'),
          style: ElevatedButton.styleFrom(
            backgroundColor: Colors.green,
            foregroundColor: Colors.white,
          ),
        ),
      ),
      const SizedBox(width: 10),
      Expanded(
        child: ElevatedButton.icon(
          onPressed: () => context.pop(),
          icon: const Icon(Icons.close),
          label: const Text('Close'),
          style: ElevatedButton.styleFrom(
            backgroundColor: Colors.red.shade600,
            foregroundColor: Colors.white,
          ),
        ),
      ),
    ],
  );
}
void _showError(String message) {
  ScaffoldMessenger.of(context).showSnackBar(
    SnackBar(
      content: Text(message),
      behavior: SnackBarBehavior.floating,
      margin: const EdgeInsets.only(bottom: 70, left: 16, right: 16),
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      backgroundColor: Colors.red.shade700,
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

  Widget _sectionLabel(String label) {
    return Text(
      label,
      style: const TextStyle(
        fontSize: 17,
        fontWeight: FontWeight.bold,
        color: Color(0xFF1A237E),
      ),
    );
  }

    Widget _searchBar() {
    return TextField(
      onChanged: (value) => setState(() => participantSearch = value),
      decoration: InputDecoration(
        hintText: '🔍 Search participants...',
        fillColor: const Color(0xFFECEFF1),
        filled: true,
        border: OutlineInputBorder(borderRadius: BorderRadius.circular(8), borderSide: BorderSide.none),
      ),
    );}

  // Future<void> _pickImage(FLHAInspectionViewModel notifier) async {
  //   final picked = await _picker.pickImage(source: ImageSource.gallery);
  //   if (picked != null) notifier.addPhoto(File(picked.path));
  // }

Future<void> _pickImage(FLHAInspectionViewModel notifier) async {
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
        final processed = await compressAndConvertToJpeg(File(file.path));
        if (processed != null) notifier.addPhoto(processed);
      }
    } else {
      final pickedFile = await _picker.pickImage(source: source);
      if (pickedFile == null) return;

      final processed = await compressAndConvertToJpeg(File(pickedFile.path));
      if (processed != null) notifier.addPhoto(processed);
    }
  } catch (e) {
    print('❌ Ошибка при выборе файла: $e');
  }
}

Future<File?> compressAndConvertToJpeg(File file) async {
  try {
    final dir = await getTemporaryDirectory();
    final targetPath = path.join(dir.path, '${DateTime.now().millisecondsSinceEpoch}.jpg');

    final xfile = await FlutterImageCompress.compressAndGetFile(
      file.absolute.path,
      targetPath,
      format: CompressFormat.jpeg,
      quality: 85,
    );

    if (xfile != null) {
      print('✅ Конвертировано в JPEG: ${xfile.path}');
      return File(xfile.path); // ✅ конвертация XFile → File
    } else {
      print('❌ Конвертация вернула null');
      return null;
    }
  } catch (e) {
    print('❌ Ошибка при конвертации в JPEG: $e');
    return null;
  }
}
  // Future<void> _showAddCustomHazardDialog(FLHAInspectionViewModel notifier) async {
  //   final hazardController = TextEditingController();
  //   final controlController = TextEditingController();

  //   await showDialog(
  //     context: context,
  //     builder: (_) => AlertDialog(
  //       title: const Text('Add Custom Hazard'),
  //       content: Column(
  //         mainAxisSize: MainAxisSize.min,
  //         children: [
  //           TextField(controller: hazardController, decoration: const InputDecoration(labelText: 'Hazard Text')),
  //           TextField(controller: controlController, decoration: const InputDecoration(labelText: 'Control Measures')),
  //         ],
  //       ),
  //       actions: [
  //         TextButton(
  //           onPressed: () {
  //             if (hazardController.text.trim().isNotEmpty) {
  //               notifier.addCustomHazard(hazardController.text.trim(), controlController.text.trim());
  //               Navigator.pop(context);
  //             }
  //           },
  //           child: const Text('Add'),
  //         ),
  //       ],
  //     ),
  //   );
  // }
}
