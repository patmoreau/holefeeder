import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:holefeeder/ui/screens/purchase_screen.dart';
import 'package:holefeeder/ui/shared/widgets.dart';
import 'package:universal_platform/universal_platform.dart';

class DashboardScreen extends StatelessWidget {
  const DashboardScreen({super.key});

  @override
  Widget build(BuildContext context) =>
      UniversalPlatform.isApple
          ? _buildForCupertino(context)
          : _buildForMaterial(context);

  Widget _buildForCupertino(BuildContext context) => CupertinoPageScaffold(
    navigationBar: CupertinoNavigationBar(
      middle: const Text('Dashboard'),
      trailing: IconButton(
        onPressed: () {
          context.push('/purchase');
        },
        icon: const Icon(CupertinoIcons.purchased),
      ),
    ),
    child: _buildScreen(context),
  );

  Widget _buildForMaterial(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Dashboard')),
      body: _buildScreen(context),
    );
  }

  Widget _buildScreen(BuildContext context) => Center(
    child: Column(
      mainAxisAlignment: MainAxisAlignment.center,
      children: [
        const Text('Dashboard Page'),
        HolefeederWidgets.button(
          onPressed: () => context.go('/settings'),
          child: const Text('Go to Settings'),
        ),
      ],
    ),
  );
}
