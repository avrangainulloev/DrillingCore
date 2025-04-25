import 'dart:io';

class PhotoDto {
  final String preview;
  final File? file;

  PhotoDto({required this.preview, this.file});
}