import 'package:flutter/material.dart';

const int itemCount = 20;

class CashflowPage extends StatelessWidget {
  const CashflowPage({super.key});

  @override
  Widget build(BuildContext context) {
    return ListView.builder(
      itemCount: itemCount,
      itemBuilder: (context, index) {
        return ListTile(
          title: Text('Item ${index + 1}'),
          leading: Icon(Icons.person),
          trailing: Icon(Icons.delete),
          onTap: () {
            debugPrint('Item ${index + 1} tapped');
          },
        );
      },
    );
  }
}
