import 'package:flutter/material.dart';
import 'package:hive_flutter/hive_flutter.dart';
import 'package:spotfinder_app/core/services/notification_service.dart';
import 'app.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  await Hive.initFlutter();
  await Hive.openBox('auth_box');

  // Firebase & push notifications (try-catch: simulator'da çalışmayabilir)
  try {
    await NotificationService().initialize();
  } catch (_) {}

  runApp(const SpotFinderApp());
}
