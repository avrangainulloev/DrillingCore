import 'dart:typed_data';
import 'package:flutter/material.dart';
import 'package:signature/signature.dart';

class SignatureModal extends StatefulWidget {
  final Function(Uint8List signatureData) onSave;

  const SignatureModal({super.key, required this.onSave});

  @override
  State<SignatureModal> createState() => _SignatureModalState();
}

class _SignatureModalState extends State<SignatureModal> {
  final SignatureController _controller = SignatureController(
    penStrokeWidth: 3,
    penColor: Colors.black,
    exportBackgroundColor: Colors.transparent,
  );

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

 Future<void> _saveSignature() async {
  if (_controller.isNotEmpty) {
    final data = await _controller.toPngBytes();
    if (data != null) {
      Navigator.of(context).pop(data); // ✅ возвращаем данные — и всё
    }
  }
}

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: const Text('Sign Below'),
      content: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          Container(
            width: 300,
            height: 150,
            decoration: BoxDecoration(
              border: Border.all(color: Colors.grey.shade400),
            ),
            child: Signature(controller: _controller, backgroundColor: Colors.white),
          ),
          const SizedBox(height: 16),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              TextButton(
                onPressed: _controller.clear,
                child: const Text('Clear'),
              ),
              TextButton(
                onPressed: () => Navigator.of(context, rootNavigator: true).pop(),
                child: const Text('Cancel'),
              ),
              ElevatedButton(
                onPressed: _saveSignature,
                child: const Text('Save'),
              ),
            ],
          )
        ],
      ),
    );
  }
}
