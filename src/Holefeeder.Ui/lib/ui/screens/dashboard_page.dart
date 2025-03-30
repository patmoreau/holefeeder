import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

class DashboardPage extends StatelessWidget {
  const DashboardPage({super.key});

  @override
  Widget build(BuildContext context) {
    return Center(
      child: ElevatedButton(
        onPressed: () {
          GoRouter.of(context).push('/purchase');
        },
        child: const Text('Test flutter'),
      ),
    );
  }
}
