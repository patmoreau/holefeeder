import 'package:flutter/widgets.dart';

class BaseViewModel extends ChangeNotifier {
  final BuildContext _context;

  BaseViewModel({required BuildContext context}) : _context = context;

  BuildContext get context => _context;
}
