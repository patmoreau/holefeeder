import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:holefeeder/ui/screens/purchase_form.dart';
import 'package:universal_platform/universal_platform.dart';

import '../../core/view_models/screens/purchase_screen_view_model.dart';
import '../shared/view_model_provider.dart';

class PurchaseScreen extends StatelessWidget {
  const PurchaseScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return ViewModelProvider<PurchaseViewModel>(
      model: PurchaseViewModel(context: context),
      builder:
          (model) =>
              UniversalPlatform.isApple
                  ? _buildCupertinoScaffold(model)
                  : _buildMaterialScaffold(model),
    );
  }

  Widget _buildCupertinoScaffold(PurchaseViewModel model) {
    return CupertinoPageScaffold(
      navigationBar: const CupertinoNavigationBar(middle: Text('Purchase')),
      child: SafeArea(child: PurchaseForm(model: model)),
    );
  }

  Widget _buildMaterialScaffold(PurchaseViewModel model) {
    return Scaffold(
      appBar: AppBar(title: const Text('Purchase')),
      body: PurchaseForm(model: model),
    );
  }
}
