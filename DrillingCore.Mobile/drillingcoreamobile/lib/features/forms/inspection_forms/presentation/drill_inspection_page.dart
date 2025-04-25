import 'dart:convert';
import 'dart:io';

import 'dart:typed_data'; 
 
import 'package:drillingcoreamobile/core/constants/constants.dart';
import 'package:drillingcoreamobile/features/forms/common/widgets/signaturemodal.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:image_picker/image_picker.dart';
import '../viewmodel/drill_inspection_view_model.dart';
import '../models/drill_inspection_form_model.dart';
import 'package:flutter_image_compress/flutter_image_compress.dart';
import 'package:path_provider/path_provider.dart';
import 'package:path/path.dart' as path;
 

class DrillInspectionPage extends ConsumerStatefulWidget {
  final int formId;
  final int formTypeId;
  final int projectId;

  const DrillInspectionPage({
    super.key,
    required this.formId,
    required this.formTypeId,
    required this.projectId,
  });

  @override
  ConsumerState<DrillInspectionPage> createState() => _DrillInspectionPageState();
}

class _DrillInspectionPageState extends ConsumerState<DrillInspectionPage> {
  late final DrillInspectionParams _params;
  late final ScrollController _participantsScrollController;
   bool _isSaving = false; 
  String participantSearch = '';
  final ImagePicker _picker = ImagePicker();

  @override
  void initState() {
    super.initState();
    _participantsScrollController = ScrollController();
    _params = DrillInspectionParams(
      formId: widget.formId,
      formTypeId: widget.formTypeId,
      projectId: widget.projectId,
    );
    Future.microtask(() {
      ref.read(drillInspectionViewModelProvider(_params).notifier).initialize(_params);
    });
  }

  @override
  void dispose() {
    _participantsScrollController.dispose();
    super.dispose();
  }

Future<void> _pickImage(DrillInspectionViewModel notifier) async {
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
 
  @override
  Widget build(BuildContext context) {
    final formState = ref.watch(drillInspectionViewModelProvider(_params));
    final notifier = ref.read(drillInspectionViewModelProvider(_params).notifier);

    final filteredParticipants = [...formState.allParticipants]
      ..sort((a, b) {
        final aSelected = formState.selectedParticipantIds.contains(a.participantId);
        final bSelected = formState.selectedParticipantIds.contains(b.participantId);
        if (aSelected && !bSelected) return -1;
        if (!aSelected && bSelected) return 1;
        return 0;
      })
      ..retainWhere((p) => p.fullName.toLowerCase().contains(participantSearch.toLowerCase()));

    return Scaffold(
      backgroundColor: const Color(0xFFF8FAFC),
      appBar: AppBar(
        title: const Text('🛠 Drill Inspection'),
        backgroundColor: Colors.indigo.shade800,
        foregroundColor: Colors.white,
      ),
      body: Padding(
        padding: const EdgeInsets.all(16),
        child: ListView(
          children: [
            _readonlyField('👷 Crew Name', formState.crewName),
            const SizedBox(height: 10),
            _readonlyField('📅 Date Filled', formState.dateFilled),
            const SizedBox(height: 10),
            _readonlyField('🚛 Unit Number', formState.unitNumber),
            const SizedBox(height: 20),
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
            const SizedBox(height: 20),
            _sectionLabel('📋 Checklist'),
            ..._buildChecklistGroups(formState, notifier),
            const SizedBox(height: 20),
            _sectionLabel('📝 Other Comments'),
            TextFormField(
              initialValue: formState.otherComments,
              maxLines: 3,
              readOnly: true,
              decoration: InputDecoration(
                hintText: 'Add any notes...',
                filled: true,
                fillColor: const Color(0xFFE3F2FD),
                border: OutlineInputBorder(borderRadius: BorderRadius.circular(10)),
              ),
            ),
            const SizedBox(height: 20),
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
              ));
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
                decoration: BoxDecoration(
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

            const SizedBox(height: 20),
_sectionLabel('✍️ Signatures'),
...formState.selectedParticipantIds.map((id) {
  final participant = formState.allParticipants.firstWhere(
    (p) => p.participantId == id,
    orElse: () => ProjectParticipant(
      participantId: id,
      userId: 0,
      fullName: 'Unknown',
       groupName: null, // или '', если хочешь пустую строку
  endDate: null,
    ),
  );
  final signature = formState.signatures[id];

  Widget? signatureWidget;
  if (signature != null) {
    if (signature.startsWith('data:image')) {
      try {
        final base64Str = signature.split(',').last;
        final bytes = base64Decode(base64Str);
        signatureWidget = Image.memory(
          bytes,
          width: 120,
          height: 50,
          fit: BoxFit.contain,
        );
      } catch (_) {
        signatureWidget = const Icon(Icons.error_outline);
      }
    } else {
    // 🔧 Добавляем базовый URL, если это относительный путь
    final fullUrl = signature.startsWith('http')
        ? signature
        : '$kApiBaseUrl$signature';
   
    signatureWidget = Image.network(
      fullUrl,
      width: 120,
      height: 50,
      fit: BoxFit.contain,
      errorBuilder: (_, __, ___) => const Icon(Icons.error_outline),
    );
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
            style: const TextStyle(fontWeight: FontWeight.w600, fontSize: 15),
          ),
        ),
        if (signatureWidget != null)
          Padding(
            padding: const EdgeInsets.only(right: 12),
            child: signatureWidget,
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
              // Преобразуем в base64 для корректного отображения
              //final base64Str = base64Encode(data);
              //final base64DataUri = 'data:image/png;base64,$base64Str';
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
Row(
  children: [
   Expanded(
  child: ElevatedButton.icon(
    onPressed: _isSaving
        ? null
        : () async {
            setState(() => _isSaving = true);
            try {
              await notifier.saveAsync();
              if (context.mounted) {
                ScaffoldMessenger.of(context).showSnackBar(
                  const SnackBar(content: Text('✅ Drill Inspection saved')),
                );
              }
            } catch (e) {
              print(e);
              if (context.mounted) {
                ScaffoldMessenger.of(context).showSnackBar(
                  SnackBar(content: Text('❌ Save failed: $e')),
                );
              }
            } finally {
              if (mounted) setState(() => _isSaving = false);
            }
          },
    icon: _isSaving
        ? const SizedBox(
            width: 20,
            height: 20,
            child: CircularProgressIndicator(
              strokeWidth: 2,
              color: Colors.white,
            ),
          )
        : const Icon(Icons.save),
    label: Text(_isSaving ? 'Saving...' : 'Save'),
    style: ElevatedButton.styleFrom(
      backgroundColor: Colors.green,
      foregroundColor: Colors.white,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(10)),
      padding: const EdgeInsets.symmetric(vertical: 12),
    ),
  ),
),
    const SizedBox(width: 12), // отступ между кнопками
    Expanded(
      child: ElevatedButton.icon(
       onPressed: () {
  if (Navigator.of(context).canPop()) {
    context.pop();
  } else {
    context.go('/home');
  }
},
        icon: const Icon(Icons.close),
        label: const Text('Close'),
        style: ElevatedButton.styleFrom(
          backgroundColor: Colors.red.shade600,
          foregroundColor: Colors.white,
          shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(10)),
          padding: const EdgeInsets.symmetric(vertical: 12),
        ),
      ),
    ),
  ],
),
          ],
        ),
      ),
    );
  }

  Widget _readonlyField(String label, String value) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(label, style: const TextStyle(fontWeight: FontWeight.bold)),
        const SizedBox(height: 4),
        TextFormField(
          initialValue: value,
          readOnly: true,
          decoration: InputDecoration(
            filled: true,
            fillColor: const Color(0xFFECEFF1),
            border: OutlineInputBorder(borderRadius: BorderRadius.circular(10)),
          ),
        ),
      ],
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
    );
  }

  List<Widget> _buildChecklistGroups(
    DrillInspectionFormModel state,
    DrillInspectionViewModel notifier,
  ) {
    final grouped = <String, List<ChecklistItem>>{};
    for (var item in state.checklistItems) {
      grouped.putIfAbsent(item.groupName, () => []).add(item);
    }

    final widgets = <Widget>[];
    grouped.forEach((groupName, items) {
      widgets.add(Container(
        margin: const EdgeInsets.only(top: 12),
        padding: const EdgeInsets.all(8),
        decoration: BoxDecoration(
          color: const Color(0xFFE3F2FD),
          border: Border.all(color: Colors.blue.shade300),
          borderRadius: BorderRadius.circular(10),
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(groupName,
                style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 15, color: Colors.black87)),
            const SizedBox(height: 6),
            ...items.map((item) {
              final checked = state.checkedItemIds.contains(item.id);
              return Row(
                children: [
                  Checkbox(
                    value: checked,
                    onChanged: (v) => notifier.toggleChecklistItem(item.id, v ?? false),
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
                    child: Text(item.label, style: const TextStyle(fontSize: 14)),
                  ),
                ],
              );
            }),
          ],
        ),
      ));
    });

    return widgets;
  }
}