import 'package:flutter/material.dart';

import '../../core/models/category.dart';
import '../../core/view_models/screens/categories_view_model.dart';

class CategoriesPage extends StatefulWidget {
  final CategoriesViewModel viewModel;

  const CategoriesPage(this.viewModel, {super.key});

  @override
  State<CategoriesPage> createState() => _CategoriesPageState();
}

class _CategoriesPageState extends State<CategoriesPage> {
  @override
  void initState() {
    super.initState();
    widget.viewModel.loadInitialCategories();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: FutureBuilder<List<Category>>(
        future: widget.viewModel.categoriesFuture,
        builder: (_, AsyncSnapshot<List<Category>> snapshot) {
          switch (snapshot.connectionState) {
            case ConnectionState.done:
              if (snapshot.hasError) {
                return Padding(
                  padding: const EdgeInsets.all(16),
                  child: Center(
                    child: Column(
                      children: <Widget>[
                        const Text('Oops we had trouble loading your wishlist'),
                        const SizedBox(height: 32),
                        ElevatedButton(
                          onPressed: () async {
                            await widget.viewModel.refreshCategories();
                          },
                          child: const Text('Retry'),
                        ),
                      ],
                    ),
                  ),
                );
              }
              final List<Category> items = snapshot.data ?? <Category>[];
              if (items.isEmpty) {
                return const Center(child: Text('Your wishlist is empty. Why not add some items'));
              }
              return ListView.builder(
                itemCount: items.length,
                itemBuilder: (_, int index) {
                  final Category item = items[index];
                  return Card(
                    child: Padding(
                      padding: const EdgeInsets.all(8),
                      child: Row(
                        children: <Widget>[
                          Expanded(
                            child: Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: <Widget>[Text(item.name), Text(item.color)],
                            ),
                          ),
                          IconButton(onPressed: () async {}, icon: const Icon(Icons.edit)),
                          IconButton(onPressed: () async {}, icon: const Icon(Icons.delete)),
                        ],
                      ),
                    ),
                  );
                },
              );
            default:
              return Padding(
                padding: const EdgeInsets.all(16),
                child: Center(
                  child: Column(
                    children: const <Widget>[
                      Text('Loading your wishlist'),
                      SizedBox(height: 32),
                      CircularProgressIndicator(),
                    ],
                  ),
                ),
              );
          }
        },
      ),
    );
  }
}
