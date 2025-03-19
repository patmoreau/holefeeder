import 'package:flutter/material.dart';
import 'package:holefeeder/core/view_models/base_view_model.dart';
import 'package:provider/provider.dart';

class ViewModelProvider<T extends BaseViewModel> extends StatelessWidget {
  final T model;
  final Widget Function(T model) builder;

  const ViewModelProvider({super.key, required this.builder, required this.model});

  @override
  Widget build(BuildContext context) {
    return ChangeNotifierProvider<T>(
      create: (BuildContext context2) => model,
      child: Consumer<T>(builder: (context, T value, child) => builder(value)),
    );
  }
}
